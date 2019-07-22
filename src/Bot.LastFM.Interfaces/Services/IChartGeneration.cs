using System.Collections.Generic;
using Bot.Domain.LastFM;
using System.Threading.Tasks;
using Bot.Domain.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Bot.LastFM.Interfaces.Services
{
    public interface IChartGeneration
    {
        /// <summary>
        /// Get album info
        /// </summary>
        /// <param name="album">The album for which you want to make the chart.</param>
        /// <param name="timespan">The timespan for the chart.</param>
        /// <param name="rows">The amount of rows for the chart.</param>
        /// <param name="columns">The amount of columns in the chart.</param>
        /// <param name="showTitles">The option to disable/enable titles</param>
        Task<Image<Rgba32>> GenerateChartAsync(IReadOnlyList<Album> albums, LastFMTimespan timespan, int rows = 3, int columns = 3, bool showTitles = true);
    }
}
