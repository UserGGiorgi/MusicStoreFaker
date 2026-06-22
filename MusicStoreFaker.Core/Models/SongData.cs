namespace MusicStoreFaker.Core.Models
{
    public class SongData
    {
        public int SequenceIndex { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string Album { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int Likes { get; set; }
        public bool IsSingle { get; set; }
    }

    public class SongDetail : SongData
    {
        public string CoverUrl { get; set; } = string.Empty;
        public string AudioUrl { get; set; } = string.Empty;
        public string Review { get; set; } = string.Empty;
    }

    public class PaginatedSongs
    {
        public List<SongData> Songs { get; set; } = new();
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public bool HasMore => (CurrentPage * PageSize) < TotalCount;
    }

    public class GalleryBatch
    {
        public List<SongData> Songs { get; set; } = new();
        public int NextStartIndex { get; set; }
        public bool HasMore { get; set; }
    }
}