namespace MusicStoreFaker.Core.Services
{
    public interface IAudioGenerator
    {
        byte[] GeneratePreviewMp3(int sequenceIndex, long seed);
    }
}