using MusicStoreFaker.Core.Models;

namespace MusicStoreFaker.Core.Services
{
    public interface ILocaleDataProvider
    {
        Task InitializeAsync();
        IReadOnlyList<string> GetFirstNames(string locale);
        IReadOnlyList<string> GetLastNames(string locale);
        IReadOnlyList<string> GetSongTitleWords(string locale, string wordType);
        IReadOnlyList<string> GetAlbumTitles(string locale);
        IReadOnlyList<string> GetGenres(string locale);
        IReadOnlyList<string> GetBandWords(string locale);
        IReadOnlyList<string> GetActiveLocales();
    }
}