using MusicStoreFaker.Core.Models;

namespace MusicStoreFaker.Core.Services
{
    public interface ISongGenerator
    {
        SongData GenerateSong(int sequenceIndex, string locale, long seed, double avgLikes);
        SongDetail GenerateDetail(SongData song, string locale, long seed);
        PaginatedSongs GeneratePage(int page, int pageSize, string locale, long seed, double avgLikes);
        GalleryBatch GenerateBatch(int startIndex, int count, string locale, long seed, double avgLikes);
    }
}