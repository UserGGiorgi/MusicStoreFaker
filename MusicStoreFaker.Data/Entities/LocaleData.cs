using System.ComponentModel.DataAnnotations;

namespace MusicStoreFaker.Data.Entities
{
    public abstract class LocaleData
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string Locale { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Value { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Category { get; set; }
    }

    public class FirstName : LocaleData
    {
        [MaxLength(10)]
        public string? Gender { get; set; }
    }

    public class LastName : LocaleData { }

    public class SongTitleWord : LocaleData
    {
        [MaxLength(20)]
        public string? WordType { get; set; } 
    }

    public class AlbumTitle : LocaleData { }

    public class Genre : LocaleData { }

    public class BandWord : LocaleData { }

    public class LocaleInfo
    {
        [Key]
        [MaxLength(10)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}