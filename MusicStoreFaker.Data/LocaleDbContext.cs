using Microsoft.EntityFrameworkCore;
using MusicStoreFaker.Data.Entities;

namespace MusicStoreFaker.Data
{
    public class LocaleDbContext : DbContext
    {
        public DbSet<LocaleInfo> Locales { get; set; }
        public DbSet<FirstName> FirstNames { get; set; }
        public DbSet<LastName> LastNames { get; set; }
        public DbSet<SongTitleWord> SongTitleWords { get; set; }
        public DbSet<AlbumTitle> AlbumTitles { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<BandWord> BandWords { get; set; }

        public LocaleDbContext(DbContextOptions<LocaleDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LocaleInfo>(entity =>
            {
                entity.ToTable("Locales");
                entity.HasIndex(e => e.Code).IsUnique();
            });

            modelBuilder.Entity<FirstName>(entity =>
            {
                entity.ToTable("FirstNames");
                entity.HasIndex(e => new { e.Locale, e.Gender });
            });

            modelBuilder.Entity<LastName>(entity =>
            {
                entity.ToTable("LastNames");
                entity.HasIndex(e => e.Locale);
            });

            modelBuilder.Entity<SongTitleWord>(entity =>
            {
                entity.ToTable("SongTitleWords");
                entity.HasIndex(e => new { e.Locale, e.WordType });
            });

            modelBuilder.Entity<AlbumTitle>(entity =>
            {
                entity.ToTable("AlbumTitles");
                entity.HasIndex(e => e.Locale);
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.ToTable("Genres");
                entity.HasIndex(e => e.Locale);
            });

            modelBuilder.Entity<BandWord>(entity =>
            {
                entity.ToTable("BandWords");
                entity.HasIndex(e => e.Locale);
            });
        }
    }
}