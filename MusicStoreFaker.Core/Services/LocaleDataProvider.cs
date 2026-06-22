

using MusicStoreFaker.Data;

namespace MusicStoreFaker.Core.Services
{
    public class LocaleDataProvider : ILocaleDataProvider
    {
        private readonly Dictionary<string, List<string>> _firstNames = new();
        private readonly Dictionary<string, List<string>> _lastNames = new();
        private readonly Dictionary<string, Dictionary<string, List<string>>> _titleWords = new();
        private readonly Dictionary<string, List<string>> _albums = new();
        private readonly Dictionary<string, List<string>> _genres = new();
        private readonly Dictionary<string, List<string>> _bandWords = new();
        private List<string> _activeLocales = new();
        private bool _initialized;

        public async Task InitializeAsync()
        {
            if (_initialized) return;

            await Task.CompletedTask; 
        }

        public void LoadData(LocaleDbContext context)
        {
            _firstNames.Clear();
            _lastNames.Clear();
            _titleWords.Clear();
            _albums.Clear();
            _genres.Clear();
            _bandWords.Clear();
            _activeLocales.Clear();

            _activeLocales = context.Locales
                .Where(l => l.IsActive)
                .Select(l => l.Code)
                .ToList();

            foreach (var locale in _activeLocales)
            {
                _firstNames[locale] = context.FirstNames
                    .Where(f => f.Locale == locale)
                    .Select(f => f.Value)
                    .ToList();

                _lastNames[locale] = context.LastNames
                    .Where(l => l.Locale == locale)
                    .Select(l => l.Value)
                    .ToList();

                _titleWords[locale] = new Dictionary<string, List<string>>
                {
                    ["Noun"] = context.SongTitleWords
                        .Where(w => w.Locale == locale && w.WordType == "Noun")
                        .Select(w => w.Value)
                        .ToList(),
                    ["Adjective"] = context.SongTitleWords
                        .Where(w => w.Locale == locale && w.WordType == "Adjective")
                        .Select(w => w.Value)
                        .ToList()
                };

                _albums[locale] = context.AlbumTitles
                    .Where(a => a.Locale == locale)
                    .Select(a => a.Value)
                    .ToList();

                _genres[locale] = context.Genres
                    .Where(g => g.Locale == locale)
                    .Select(g => g.Value)
                    .ToList();

                _bandWords[locale] = context.BandWords
                    .Where(b => b.Locale == locale)
                    .Select(b => b.Value)
                    .ToList();
            }
            _initialized = true;
        }

        public IReadOnlyList<string> GetFirstNames(string locale) =>
            _firstNames.TryGetValue(locale, out var list) ? list : _firstNames["en-US"];

        public IReadOnlyList<string> GetLastNames(string locale) =>
            _lastNames.TryGetValue(locale, out var list) ? list : _lastNames["en-US"];

        public IReadOnlyList<string> GetSongTitleWords(string locale, string wordType) =>
            _titleWords.TryGetValue(locale, out var dict) && dict.TryGetValue(wordType, out var list)
                ? list
                : _titleWords["en-US"][wordType];

        public IReadOnlyList<string> GetAlbumTitles(string locale) =>
            _albums.TryGetValue(locale, out var list) ? list : _albums["en-US"];

        public IReadOnlyList<string> GetGenres(string locale) =>
            _genres.TryGetValue(locale, out var list) ? list : _genres["en-US"];

        public IReadOnlyList<string> GetBandWords(string locale) =>
            _bandWords.TryGetValue(locale, out var list) ? list : _bandWords["en-US"];

        public IReadOnlyList<string> GetActiveLocales() => _activeLocales.AsReadOnly();
    }
}