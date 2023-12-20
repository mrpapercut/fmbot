using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using FMBot.Bot.Configurations;
using FMBot.Bot.Extensions;
using FMBot.Bot.Interfaces;
using FMBot.Bot.Services.Guild;
using FMBot.Bot.Services.ThirdParty;
using FMBot.Bot.Services.WhoKnows;
using FMBot.Domain;
using FMBot.Domain.Models;
using FMBot.Persistence.Domain.Models;
using Hangfire;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Serilog;
using Image = Discord.Image;

namespace FMBot.Bot.Services;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class TimerService
{
    private readonly UserService _userService;
    private readonly IUpdateService _updateService;
    private readonly IIndexService _indexService;
    private readonly GuildService _guildService;
    private readonly DiscordShardedClient _client;
    private readonly WebhookService _webhookService;
    private readonly BotSettings _botSettings;
    private readonly FeaturedService _featuredService;
    private readonly SupporterService _supporterService;
    private readonly IMemoryCache _cache;
    private readonly DiscogsService _discogsService;
    private readonly WhoKnowsFilterService _whoKnowsFilterService;

    public FeaturedLog CurrentFeatured;

    public TimerService(DiscordShardedClient client,
        IUpdateService updateService,
        UserService userService,
        IIndexService indexService,
        GuildService guildService,
        WebhookService webhookService,
        IOptions<BotSettings> botSettings,
        FeaturedService featuredService,
        IMemoryCache cache,
        SupporterService supporterService,
        DiscogsService discogsService,
        WhoKnowsFilterService whoKnowsFilterService)
    {
        this._client = client;
        this._userService = userService;
        this._indexService = indexService;
        this._guildService = guildService;
        this._webhookService = webhookService;
        this._featuredService = featuredService;
        this._cache = cache;
        this._supporterService = supporterService;
        this._discogsService = discogsService;
        this._whoKnowsFilterService = whoKnowsFilterService;
        this._updateService = updateService;
        this._botSettings = botSettings.Value;

        this.CurrentFeatured = this._featuredService.GetFeaturedForDateTime(DateTime.UtcNow).Result;
    }

    public void QueueJobs()
    {
        Log.Information($"RecurringJob: Adding {nameof(CheckForNewFeatured)}");
        RecurringJob.AddOrUpdate(nameof(CheckForNewFeatured), () => CheckForNewFeatured(), "*/2 * * * *");

        Log.Information($"RecurringJob: Adding {nameof(UpdateMetricsAndStatus)}");
        RecurringJob.AddOrUpdate(nameof(UpdateMetricsAndStatus), () => UpdateMetricsAndStatus(), "* * * * *");

        Log.Information($"RecurringJob: Adding {nameof(UpdateHealthCheck)}");
        RecurringJob.AddOrUpdate(nameof(UpdateHealthCheck), () => UpdateHealthCheck(), "*/20 * * * * *");

        Log.Information($"RecurringJob: Adding {nameof(CheckIfShardsNeedReconnect)}");
        RecurringJob.AddOrUpdate(nameof(CheckIfShardsNeedReconnect), () => CheckIfShardsNeedReconnect(), "*/2 * * * *");

        Log.Information($"RecurringJob: Adding {nameof(ClearUserCache)}");
        RecurringJob.AddOrUpdate(nameof(ClearUserCache), () => ClearUserCache(), "30 */2 * * *");

        Log.Information($"RecurringJob: Adding {nameof(ClearInternalLogs)}");
        RecurringJob.AddOrUpdate(nameof(ClearInternalLogs), () => ClearInternalLogs(), "0 8 * * *");

        if (this._botSettings.LastFm.UserIndexFrequencyInDays != null &&
            this._botSettings.LastFm.UserIndexFrequencyInDays != 0 &&
            ConfigData.Data.Shards.MainInstance == true)
        {
            Log.Information($"RecurringJob: Adding {nameof(AddUsersToIndexQueue)}");
            RecurringJob.AddOrUpdate(nameof(AddUsersToIndexQueue), () => AddUsersToIndexQueue(), "0 8 * * *");
        }
        else
        {
            Log.Warning($"No {nameof(this._botSettings.LastFm.UserIndexFrequencyInDays)} set in config, not queuing user index job");
        }

        if (this._botSettings.LastFm.UserUpdateFrequencyInHours != null &&
            this._botSettings.LastFm.UserUpdateFrequencyInHours != 0 &&
            ConfigData.Data.Shards.MainInstance == true)
        {
            Log.Information($"RecurringJob: Adding {nameof(AddUsersToUpdateQueue)}");
            RecurringJob.AddOrUpdate(nameof(AddUsersToUpdateQueue), () => AddUsersToUpdateQueue(), "0 * * * *");
        }
        else
        {
            Log.Warning($"No {nameof(this._botSettings.LastFm.UserUpdateFrequencyInHours)} set in config, not queuing user update job");
        }

        if (this._client.CurrentUser.Id == Constants.BotBetaId && ConfigData.Data.Shards.MainInstance == true)
        {
            Log.Information($"RecurringJob: Adding {nameof(UpdateGlobalWhoKnowsFilters)}");
            RecurringJob.AddOrUpdate(nameof(UpdateGlobalWhoKnowsFilters), () => UpdateGlobalWhoKnowsFilters(), "0 10 * * *");
        }

        var mainGuildConnected = this._client.Guilds.Any(a => a.Id == ConfigData.Data.Bot.BaseServerId);
        if (this._client.CurrentUser.Id == Constants.BotProductionId && mainGuildConnected)
        {
            QueueMasterJobs();
        }
        else
        {
            Log.Warning("Main guild not connected, not queuing master jobs");
            BackgroundJob.Schedule(() => MakeSureMasterJobsAreQueued(), TimeSpan.FromMinutes(2));
        }
    }

    public void MakeSureMasterJobsAreQueued()
    {
        var mainGuildConnected = this._client.Guilds.Any(a => a.Id == ConfigData.Data.Bot.BaseServerId);
        if (this._client.CurrentUser.Id == Constants.BotProductionId && mainGuildConnected)
        {
            QueueMasterJobs();
        }
    }

    public void QueueMasterJobs()
    {
        Log.Warning("Queueing master jobs on instance {instance}", ConfigData.Data.Shards?.InstanceName);

        // Log.Information($"RecurringJob: Adding {nameof(AddLatestDiscordSupporters)}");
        // RecurringJob.AddOrUpdate(nameof(AddLatestDiscordSupporters), () => AddLatestDiscordSupporters(), "* * * * *");

        // Log.Information($"RecurringJob: Adding {nameof(CheckExpiredDiscordSupporters)}");
        // RecurringJob.AddOrUpdate(nameof(CheckExpiredDiscordSupporters), () => CheckExpiredDiscordSupporters(), "0 8,18 * * *");

        Log.Information($"RecurringJob: Adding {nameof(PickNewFeatureds)}");
        RecurringJob.AddOrUpdate(nameof(PickNewFeatureds), () => PickNewFeatureds(), "0 12 * * *");

        // Log.Information($"RecurringJob: Adding {nameof(CheckForNewOcSupporters)}");
        // RecurringJob.AddOrUpdate(nameof(CheckForNewOcSupporters), () => CheckForNewOcSupporters(), "*/3 * * * *");

        // Log.Information($"RecurringJob: Adding {nameof(UpdateExistingOcSupporters)}");
        // RecurringJob.AddOrUpdate(nameof(UpdateExistingOcSupporters), () => UpdateExistingOcSupporters(), "0 * * * *");

        // Log.Information($"RecurringJob: Adding {nameof(CheckDiscordSupportersUserType)}");
        // RecurringJob.AddOrUpdate(nameof(CheckDiscordSupportersUserType), () => CheckDiscordSupportersUserType(), "30 * * * *");

        // Log.Information($"RecurringJob: Adding {nameof(UpdateDiscogsUsers)}");
        // RecurringJob.AddOrUpdate(nameof(UpdateDiscogsUsers), () => UpdateDiscogsUsers(), "0 12 * * *");
    }

    public async Task UpdateMetricsAndStatus()
    {
        Log.Information($"Running {nameof(UpdateMetricsAndStatus)}");

        Statistics.RegisteredUserCount.Set(await this._userService.GetTotalUserCountAsync());
        Statistics.AuthorizedUserCount.Set(await this._userService.GetTotalAuthorizedUserCountAsync());
        Statistics.RegisteredGuildCount.Set(await this._guildService.GetTotalGuildCountAsync());
        Statistics.ActiveSupporterCount.Set(await this._supporterService.GetActiveSupporterCountAsync());

        Statistics.OneDayActiveUserCount.Set(await this._userService.GetTotalActiveUserCountAsync(1));
        Statistics.SevenDayActiveUserCount.Set(await this._userService.GetTotalActiveUserCountAsync(7));
        Statistics.ThirtyDayActiveUserCount.Set(await this._userService.GetTotalActiveUserCountAsync(30));

        try
        {
            if (this._client?.Guilds?.Count == null)
            {
                Log.Information($"Client guild count is null, cancelling {nameof(UpdateMetricsAndStatus)}");
                return;
            }

            Statistics.ConnectedShards.Set(this._client.Shards.Count(c => c.ConnectionState == ConnectionState.Connected));
            Statistics.ConnectedDiscordServerCount.Set(this._client.Guilds.Count);

            var appInfo = await this._client.GetApplicationInfoAsync();
            Statistics.TotalDiscordServerCount.Set(appInfo.ApproximateGuildCount.GetValueOrDefault());

            if (string.IsNullOrEmpty(this._botSettings.Bot.Status))
            {
                if (!PublicProperties.IssuesAtLastFm)
                {
                    await this._client.SetCustomStatusAsync(
                        $"{this._botSettings.Bot.Prefix}fm — fmbot.xyz — {appInfo.ApproximateGuildCount.GetValueOrDefault()} servers");
                }
                else
                {
                    await this._client.SetCustomStatusAsync(
                        $"⚠️ Last.fm is currently experiencing issues -> twitter.com/lastfmstatus");
                }

            }
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(UpdateMetricsAndStatus));
            throw;
        }
    }

    public async Task UpdateHealthCheck()
    {
        Log.Debug($"Running {nameof(UpdateHealthCheck)}");

        try
        {
            var allShardsConnected = this._client.Shards.All(shard => shard.ConnectionState != ConnectionState.Disconnected) &&
                               this._client.Shards.All(shard => shard.ConnectionState != ConnectionState.Disconnecting) &&
                               this._client.Shards.All(shard => shard.ConnectionState != ConnectionState.Connecting);

            if (allShardsConnected)
            {
                const string path = "healthcheck";
                await using var fileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                File.SetLastWriteTimeUtc(path, DateTime.UtcNow);
            }
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(UpdateHealthCheck));
            throw;
        }
    }

    public async Task AddUsersToIndexQueue()
    {
        if (PublicProperties.IssuesAtLastFm)
        {
            Log.Information($"Skipping {nameof(AddUsersToIndexQueue)} - issues at Last.fm");
            return;
        }

        Log.Information("Getting users to index");
        var timeToIndex = DateTime.UtcNow.AddDays(-120);

        var usersToUpdate = (await this._indexService.GetOutdatedUsers(timeToIndex))
            .Take(2000)
            .ToList();

        Log.Information($"Found {usersToUpdate.Count} outdated users, adding them to index queue");

        this._indexService.AddUsersToIndexQueue(usersToUpdate);
    }

    public async Task AddUsersToUpdateQueue()
    {
        Log.Information("Getting users to update");
        var authorizedTimeToUpdate = DateTime.UtcNow.AddHours(-this._botSettings.LastFm.UserUpdateFrequencyInHours.Value);
        var unauthorizedTimeToUpdate = DateTime.UtcNow.AddHours(-(this._botSettings.LastFm.UserUpdateFrequencyInHours.Value + 48));

        var usersToUpdate = await this._updateService.GetOutdatedUsers(authorizedTimeToUpdate, unauthorizedTimeToUpdate);
        Log.Information($"Found {usersToUpdate.Count} outdated users, adding them to update queue");

        this._updateService.AddUsersToUpdateQueue(usersToUpdate);
    }

    public async Task CheckForNewFeatured()
    {
        var newFeatured = await this._featuredService.GetFeaturedForDateTime(DateTime.UtcNow);

        if (newFeatured == null)
        {
            Log.Warning("Featured: No new featured ready");
            return;
        }

        if (newFeatured.DateTime == this.CurrentFeatured?.DateTime && newFeatured.HasFeatured)
        {
            return;
        }

        var cached = (string)this._cache.Get("avatar");

        if ((this._botSettings.Shards == null || this._botSettings.Shards?.MainInstance == true) && cached != newFeatured.ImageUrl)
        {
            try
            {
                await ChangeToNewAvatar(this._client, newFeatured.ImageUrl);
                this._cache.Set("avatar", newFeatured.ImageUrl, TimeSpan.FromMinutes(30));
            }
            catch
            {
                // ignored
            }
        }

        var mainGuildConnected = this._client.Guilds.Any(a => a.Id == ConfigData.Data.Bot.BaseServerId);
        if (this._client.CurrentUser.Id == Constants.BotProductionId && mainGuildConnected && !newFeatured.HasFeatured && newFeatured.NoUpdate != true)
        {
            Log.Information("Featured: Posting new featured to webhooks");

            var botType = BotTypeExtension.GetBotType(this._client.CurrentUser.Id);
            await this._webhookService.PostFeatured(newFeatured, this._client);
            await this._featuredService.SetFeatured(newFeatured);
            await this._webhookService.SendFeaturedWebhooks(botType, newFeatured);

            if (newFeatured.FeaturedMode == FeaturedMode.RecentPlays)
            {
                await this._featuredService.ScrobbleTrack(this._client.CurrentUser.Id, newFeatured);
            }
        }

        this.CurrentFeatured = await this._featuredService.GetFeaturedForDateTime(DateTime.UtcNow);
    }

    public async Task CheckForNewOcSupporters()
    {
        await this._supporterService.CheckForNewOcSupporters();
    }

    public async Task UpdateExistingOcSupporters()
    {
        await this._supporterService.UpdateExistingOpenCollectiveSupporters();
    }

    public async Task AddLatestDiscordSupporters()
    {
        await this._supporterService.AddLatestDiscordSupporters();
    }

    public async Task CheckExpiredDiscordSupporters()
    {
        await this._supporterService.CheckExpiredDiscordSupporters();
    }

    public async Task CheckDiscordSupportersUserType()
    {
        await this._supporterService.CheckIfDiscordSupportersHaveCorrectUserType();
    }

    public async Task UpdateDiscogsUsers()
    {
        var usersToUpdate = await this._discogsService.GetOutdatedDiscogsUsers();
        await this._discogsService.UpdateDiscogsUsers(usersToUpdate);
    }

    public static async Task ChangeToNewAvatar(DiscordShardedClient client, string imageUrl)
    {
        Log.Information($"Updating avatar to {imageUrl}");
        try
        {
            var request = WebRequest.Create(imageUrl);
            var response = await request.GetResponseAsync();
            using (Stream output = File.Create(AppDomain.CurrentDomain.BaseDirectory + "newavatar.png"))
            using (var input = response.GetResponseStream())
            {
                input.CopyTo(output);
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "newavatar.png"))
                {
                    File.SetAttributes(AppDomain.CurrentDomain.BaseDirectory + "newavatar.png", FileAttributes.Normal);
                }

                output.Close();
                Log.Information("New avatar downloaded");
            }

            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "newavatar.png"))
            {
                var fileStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "newavatar.png", FileMode.Open);
                await client.CurrentUser.ModifyAsync(u => u.Avatar = new Image(fileStream));
                fileStream.Close();
                Log.Information("Avatar successfully changed");
            }

            await Task.Delay(5000);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Featured: Error while attempting to change avatar");
            throw;
        }
    }

    public async Task PickNewFeatureds()
    {
        for (var i = 0; i <= 32; i++)
        {
            var dateTime = DateTime.UtcNow.AddHours(i);
            var featuredDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, Constants.FeaturedMinute, 0);
            var hourFeatured = await this._featuredService.GetFeaturedForDateTime(featuredDateTime);

            if (hourFeatured == null)
            {
                var newFeatured = await this._featuredService.NewFeatured(featuredDateTime);

                if (newFeatured == null)
                {
                    Log.Warning("Featured: Could not pick a new one for {dateTime}", featuredDateTime);
                    return;
                }

                await this._featuredService.AddFeatured(newFeatured);
                Log.Information("Featured: Added future feature for {dateTime}", featuredDateTime);

                if (!string.IsNullOrWhiteSpace(this._botSettings.Bot.FeaturedPreviewWebhookUrl))
                {
                    await this._webhookService.SendFeaturedPreview(newFeatured, this._botSettings.Bot.FeaturedPreviewWebhookUrl);
                }
            }
        }
    }

    public void CheckIfShardsNeedReconnect()
    {
        Log.Debug("ShardReconnectTimer: Running shard reconnect timer");

        var currentProcess = Process.GetCurrentProcess();
        var startTime = DateTime.Now - currentProcess.StartTime;

        if (startTime.Minutes <= 15)
        {
            Log.Information($"Skipping {nameof(CheckIfShardsNeedReconnect)} because bot only just started");
            return;
        }

        var shards = this._client.Shards;

        foreach (var shard in shards.Where(w => w.ConnectionState == ConnectionState.Disconnected))
        {
            var cacheKey = $"{shard.ShardId}-shard-disconnected";
            if (this._cache.TryGetValue(cacheKey, out int shardDisconnected))
            {
                if (shardDisconnected > 7 && !this._cache.TryGetValue("shard-connecting", out _))
                {
                    this._cache.Set("shard-connecting", 1, TimeSpan.FromSeconds(15));
                    Log.Information("ShardReconnectTimer: Reconnecting shard #{shardId}", shard.ShardId);
                    _ = shard.StartAsync();
                    this._cache.Remove(cacheKey);
                }
                else
                {
                    shardDisconnected++;
                    this._cache.Set(cacheKey, shardDisconnected, TimeSpan.FromMinutes(25 - shardDisconnected));
                    Log.Information("ShardReconnectTimer: Shard #{shardId} has been disconnected {shardDisconnected} times", shard.ShardId, shardDisconnected);
                }
            }
            else
            {
                Log.Information("ShardReconnectTimer: Shard #{shardId} has been disconnected one time", shard.ShardId);
                this._cache.Set(cacheKey, 1, TimeSpan.FromMinutes(25));
            }
        }
    }

    public async Task UpdateGlobalWhoKnowsFilters()
    {
        var filteredUsers = await this._whoKnowsFilterService.GetNewGlobalFilteredUsers();
        await this._whoKnowsFilterService.AddFilteredUsersToDatabase(filteredUsers);
    }

    public void ClearUserCache()
    {
        var clients = this._client.Shards;
        foreach (var socketClient in clients)
        {
            socketClient.PurgeUserCache();
        }
        Log.Information("Purged discord caches");
    }

    public void ClearInternalLogs()
    {
        PublicProperties.UsedCommandsResponses = new ConcurrentDictionary<ulong, CommandResponse>();
        PublicProperties.UsedCommandsErrorReferences = new ConcurrentDictionary<ulong, string>();
        PublicProperties.UsedCommandsArtists = new ConcurrentDictionary<ulong, string>();
        PublicProperties.UsedCommandsAlbums = new ConcurrentDictionary<ulong, string>();
        PublicProperties.UsedCommandsTracks = new ConcurrentDictionary<ulong, string>();

        Log.Information("Cleared internal logs");
    }
}
