using Microsoft.EntityFrameworkCore;
using MusicStoreFaker.Data.Entities;

namespace MusicStoreFaker.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(LocaleDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            if (!await context.Locales.AnyAsync())
            {
                context.Locales.AddRange(
                    new LocaleInfo { Code = "en-US", Name = "English (USA)", IsActive = true },
                    new LocaleInfo { Code = "de-DE", Name = "German (Germany)", IsActive = true }
                );

                SeedEnglishData(context);

                SeedGermanData(context);

                await context.SaveChangesAsync();
            }
        }

        private static void SeedEnglishData(LocaleDbContext context)
        {
            var locale = "en-US";

            var firstNames = new[]
            {
                "James", "Mary", "Robert", "Patricia", "John", "Jennifer", "Michael", "Linda",
                "David", "Barbara", "William", "Elizabeth", "Richard", "Susan", "Joseph", "Jessica",
                "Thomas", "Sarah", "Charles", "Karen", "Christopher", "Lisa", "Daniel", "Nancy",
                "Matthew", "Betty", "Anthony", "Margaret", "Mark", "Sandra", "Donald", "Ashley",
                "Steven", "Dorothy", "Paul", "Kimberly", "Andrew", "Emily", "Joshua", "Donna",
                "Kenneth", "Michelle", "Kevin", "Carol", "Brian", "Amanda", "George", "Melissa"
            };

            foreach (var name in firstNames)
                context.FirstNames.Add(new FirstName { Locale = locale, Value = name, Gender = name.EndsWith("a") || name.EndsWith("y") ? "F" : "M" });

            var lastNames = new[]
            {
                "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis",
                "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson",
                "Thomas", "Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez", "Thompson",
                "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson"
            };

            foreach (var name in lastNames)
                context.LastNames.Add(new LastName { Locale = locale, Value = name });

            var nouns = new[]
            {
                "Heart", "Dream", "Fire", "Rain", "Star", "Moon", "Shadow", "Storm",
                "River", "Mountain", "Ocean", "Thunder", "Lightning", "Diamond", "Crystal",
                "Midnight", "Sunset", "Paradise", "Heaven", "Revolution", "Freedom", "Desire",
                "Memory", "Silence", "Echo", "Mystery", "Destiny", "Passion", "Wonder"
            };

            foreach (var noun in nouns)
                context.SongTitleWords.Add(new SongTitleWord { Locale = locale, Value = noun, WordType = "Noun" });

            var adjectives = new[]
            {
                "Broken", "Electric", "Crystal", "Midnight", "Endless", "Burning", "Frozen",
                "Sacred", "Silent", "Fading", "Wild", "Dancing", "Lonely", "Sweet", "Dark",
                "Golden", "Velvet", "Stormy", "Neon", "Cosmic", "Rebel", "Lost", "Shining"
            };

            foreach (var adj in adjectives)
                context.SongTitleWords.Add(new SongTitleWord { Locale = locale, Value = adj, WordType = "Adjective" });

            var albums = new[]
            {
                "Eternal Echoes", "Neon Dreams", "Midnight Symphony", "Crystal Visions",
                "Burning Bridges", "Silent Storm", "Cosmic Journey", "Velvet Horizons",
                "Rebel Heart", "Fading Lights", "Golden Memories", "Sacred Fire",
                "Electric Nights", "Stormy Weather", "Paradise Lost"
            };

            foreach (var album in albums)
                context.AlbumTitles.Add(new AlbumTitle { Locale = locale, Value = album });

            
            var genres = new[]
            {
                "Rock", "Pop", "Jazz", "Electronic", "Hip Hop", "R&B", "Country",
                "Blues", "Classical", "Folk", "Metal", "Punk", "Indie", "Alternative",
                "Soul", "Funk", "Reggae", "Disco", "Techno", "House"
            };

            foreach (var genre in genres)
                context.Genres.Add(new Genre { Locale = locale, Value = genre });

            var bandWords = new[]
            {
                "Electric", "Velvet", "Crystal", "Neon", "Cosmic", "Rebel", "Storm",
                "Shadow", "Phoenix", "Dragon", "Thunder", "Iron", "Steel", "Midnight",
                "Silver", "Golden", "Wild", "Free", "Dark", "Light", "Dream", "Fire"
            };

            foreach (var word in bandWords)
                context.BandWords.Add(new BandWord { Locale = locale, Value = word });
        }

        private static void SeedGermanData(LocaleDbContext context)
        {
            var locale = "de-DE";

            var firstNames = new[]
            {
                "Maximilian", "Sophie", "Alexander", "Maria", "Paul", "Anna", "Leon", "Emma",
                "Lukas", "Hannah", "Felix", "Laura", "Jonas", "Lena", "David", "Lea",
                "Tim", "Sarah", "Tom", "Julia", "Jan", "Lisa", "Niklas", "Katharina",
                "Fabian", "Marie", "Simon", "Sophia", "Moritz", "Mia", "Jakob", "Emilia"
            };

            foreach (var name in firstNames)
                context.FirstNames.Add(new FirstName { Locale = locale, Value = name, Gender = name.EndsWith("e") || name.EndsWith("a") ? "F" : "M" });

            var lastNames = new[]
            {
                "Müller", "Schmidt", "Schneider", "Fischer", "Weber", "Meyer", "Wagner",
                "Becker", "Hoffmann", "Schäfer", "Koch", "Bauer", "Richter", "Klein",
                "Wolf", "Schröder", "Neumann", "Schwarz", "Zimmermann", "Braun"
            };

            foreach (var name in lastNames)
                context.LastNames.Add(new LastName { Locale = locale, Value = name });

            var nouns = new[]
            {
                "Herz", "Traum", "Feuer", "Regen", "Stern", "Mond", "Schatten", "Sturm",
                "Fluss", "Berg", "Meer", "Donner", "Blitz", "Diamant", "Kristall",
                "Mitternacht", "Sonnenuntergang", "Paradies", "Himmel", "Revolution"
            };

            foreach (var noun in nouns)
                context.SongTitleWords.Add(new SongTitleWord { Locale = locale, Value = noun, WordType = "Noun" });

            var adjectives = new[]
            {
                "Gebrochen", "Elektrisch", "Kristall", "Mitternacht", "Endlos", "Brennend",
                "Gefroren", "Heilig", "Still", "Verblassend", "Wild", "Tanzend", "Einsam",
                "Süß", "Dunkel", "Golden", "Samt", "Stürmisch", "Neon", "Kosmisch"
            };

            foreach (var adj in adjectives)
                context.SongTitleWords.Add(new SongTitleWord { Locale = locale, Value = adj, WordType = "Adjective" });

            var albums = new[]
            {
                "Ewige Echos", "Neon Träume", "Mitternachts Symphonie", "Kristall Visionen",
                "Brennende Brücken", "Stiller Sturm", "Kosmische Reise", "Samt Horizonte",
                "Rebellen Herz", "Verblassende Lichter", "Goldene Erinnerungen", "Heiliges Feuer"
            };

            foreach (var album in albums)
                context.AlbumTitles.Add(new AlbumTitle { Locale = locale, Value = album });

            var genres = new[]
            {
                "Rock", "Pop", "Jazz", "Elektronisch", "Hip Hop", "R&B", "Country",
                "Blues", "Klassik", "Folk", "Metal", "Punk", "Indie", "Alternative",
                "Soul", "Funk", "Reggae", "Disco", "Techno", "House"
            };

            foreach (var genre in genres)
                context.Genres.Add(new Genre { Locale = locale, Value = genre });

            var bandWords = new[]
            {
                "Elektrisch", "Samt", "Kristall", "Neon", "Kosmisch", "Rebell", "Sturm",
                "Schatten", "Phönix", "Drache", "Donner", "Eisen", "Stahl", "Mitternacht",
                "Silber", "Golden", "Wild", "Frei", "Dunkel", "Licht", "Traum", "Feuer"
            };

            foreach (var word in bandWords)
                context.BandWords.Add(new BandWord { Locale = locale, Value = word });
        }
    }
}