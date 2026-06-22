namespace MusicStoreFaker.Core.Services
{
    public interface ICoverGenerator
    {
        byte[] GenerateCover(int sequenceIndex, string title, string artist, string album,
                             bool isSingle, string locale, long seed);
    }
}