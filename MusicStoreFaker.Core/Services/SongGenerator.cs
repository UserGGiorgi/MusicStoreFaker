using MusicStoreFaker.Core.Models;
using System;

namespace MusicStoreFaker.Core.Services
{
    public class SongGenerator : ISongGenerator
    {
        private readonly ILocaleDataProvider _localeData;

        public SongGenerator(ILocaleDataProvider localeData)
        {
            _localeData = localeData;
        }

        public SongData GenerateSong(int sequenceIndex, string locale, long seed, double avgLikes)
        {
            long effectiveSeed = CombineSeed(seed, sequenceIndex);
            var random = new Random((int)(effectiveSeed ^ (effectiveSeed >> 32)));

            string title = GenerateTitle(random, locale);
            string artist = GenerateArtist(random, locale);
            (string album, bool isSingle) = GenerateAlbum(random, locale);
            string genre = PickRandom(_localeData.GetGenres(locale), random);

            int likes = GenerateLikes(seed, sequenceIndex, avgLikes);

            return new SongData
            {
                SequenceIndex = sequenceIndex,
                Title = title,
                Artist = artist,
                Album = album,
                Genre = genre,
                Likes = likes,
                IsSingle = isSingle
            };
        }

        public SongDetail GenerateDetail(SongData song, string locale, long seed)
        {
            return new SongDetail
            {
                SequenceIndex = song.SequenceIndex,
                Title = song.Title,
                Artist = song.Artist,
                Album = song.Album,
                Genre = song.Genre,
                Likes = song.Likes,
                IsSingle = song.IsSingle,
                CoverUrl = $"/api/songs/{song.SequenceIndex}/cover?region={locale}&seed={seed}",
                AudioUrl = $"/api/songs/{song.SequenceIndex}/audio?seed={seed}",
                Review = GenerateReview(seed, song.SequenceIndex, locale)
            };
        }

        public PaginatedSongs GeneratePage(int page, int pageSize, string locale, long seed, double avgLikes)
        {
            int totalCount = 1_000_000;
            int startIndex = (page - 1) * pageSize + 1;
            var songs = new List<SongData>();
            for (int i = 0; i < pageSize; i++)
            {
                int index = startIndex + i;
                songs.Add(GenerateSong(index, locale, seed, avgLikes));
            }
            return new PaginatedSongs
            {
                Songs = songs,
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize
            };
        }

        public GalleryBatch GenerateBatch(int startIndex, int count, string locale, long seed, double avgLikes)
        {
            var songs = new List<SongData>();
            for (int i = 0; i < count; i++)
            {
                int index = startIndex + i;
                songs.Add(GenerateSong(index, locale, seed, avgLikes));
            }
            return new GalleryBatch
            {
                Songs = songs,
                NextStartIndex = startIndex + count,
                HasMore = true 
            };
        }


        private static long CombineSeed(long seed, int index)
        {
            const long Magic = unchecked((long)0x9E3779B97F4A7C15UL);
            return unchecked(seed ^ (index * Magic));
        }

        private string GenerateTitle(Random random, string locale)
        {
            var adjectives = _localeData.GetSongTitleWords(locale, "Adjective");
            var nouns = _localeData.GetSongTitleWords(locale, "Noun");

            string adj = PickRandom(adjectives, random);
            string noun = PickRandom(nouns, random);
            int choice = random.Next(3);
            return choice switch
            {
                0 => $"{adj} {noun}",
                1 => $"{noun} {adj}",
                2 => $"The {adj} {noun}",
                _ => $"{adj} {noun}"
            };
        }

        private string GenerateArtist(Random random, string locale)
        {
            var firstNames = _localeData.GetFirstNames(locale);
            var lastNames = _localeData.GetLastNames(locale);
            var bandWords = _localeData.GetBandWords(locale);

            if (random.Next(2) == 0)
            {
                string firstName = PickRandom(firstNames, random);
                string lastName = PickRandom(lastNames, random);
                return $"{firstName} {lastName}";
            }
            else
            {
                string word1 = PickRandom(bandWords, random);
                string word2 = PickRandom(bandWords, random);
                if (word1 == word2) word2 = PickRandom(bandWords, random);
                return random.Next(3) switch
                {
                    0 => $"{word1} {word2}",
                    1 => $"The {word1}s",
                    2 => $"{word1} & The {word2}s",
                    _ => $"{word1} {word2}"
                };
            }
        }

        private (string album, bool isSingle) GenerateAlbum(Random random, string locale)
        {
            if (random.Next(5) == 0)
                return ("Single", true);

            var albumTitles = _localeData.GetAlbumTitles(locale);
            string album = PickRandom(albumTitles, random);
            return (album, false);
        }

        private string GenerateReview(long seed, int index, string locale)
        {
            return $"A captivating {PickRandom(_localeData.GetGenres(locale), new Random((int)(seed ^ index)))} experience.";
        }

        private int GenerateLikes(long seed, int index, double avgLikes)
        {
            var rand = new Random((int)(seed ^ (index * 0xDEADBEEF) ^ 0x100000));
            int whole = (int)Math.Floor(avgLikes);
            double fraction = avgLikes - whole;
            int likes = whole + (rand.NextDouble() < fraction ? 1 : 0);
            return Math.Min(10, likes);
        }

        private static T PickRandom<T>(IReadOnlyList<T> list, Random random)
        {
            if (list.Count == 0) return default!;
            return list[random.Next(list.Count)];
        }
    }
}