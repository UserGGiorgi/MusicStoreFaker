namespace MusicStoreFaker.Core.Services
{
    public interface IAudioGenerator
    {
        byte[] GeneratePreviewWav(int sequenceIndex, long seed);
    }
}