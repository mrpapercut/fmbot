using System.Diagnostics.CodeAnalysis;
using Bot.Persistence.Domain;
using Bot.Persistence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bot.Persistence.EntityFrameWork.Configurations
{

    /// <summary>
    /// This class contains the configurations for the <see cref="User"/> table. 
    /// </summary>
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class UsersConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id").IsRequired();
            builder.Property(x => x.Name).HasColumnName("name").IsRequired();
            builder.Property(x => x.SpamWarning).HasColumnName("spamwarning").IsRequired();
            builder.Property(x => x.TotalTimesTimedOut).HasColumnName("timestimedout").IsRequired();
            builder.Property(x => x.CommandUsed).HasColumnName("commandused").IsRequired();
            builder.Property(x => x.CommandSpam).HasColumnName("commandspam").IsRequired();
            builder.Property(x => x.Featured).HasColumnName("featured");
            builder.Property(x => x.LastFMUserName).HasColumnName("lastfmusername");
            builder.Property(x => x.DefaultTimeSpan).HasColumnName("defaulttimespan").IsRequired();
            builder.Property(x => x.UserType).HasColumnName("usertype").IsRequired();
            builder.Property(x => x.DefaultFMType).HasColumnName("defaultfmtype").IsRequired();
        }
    }
}
