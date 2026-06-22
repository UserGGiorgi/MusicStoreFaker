using Microsoft.AspNetCore.Mvc;
using MusicStoreFaker.Core.Services;
using System.IO.Compression;

namespace MusicStoreFaker.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExportController : ControllerBase
    {
        private readonly ISongGenerator _songGenerator;
        private readonly IAudioGenerator _audioGenerator;

        public ExportController(ISongGenerator songGenerator, IAudioGenerator audioGenerator)
        {
            _songGenerator = songGenerator;
            _audioGenerator = audioGenerator;
        }

        [HttpGet]
        public IActionResult Export(
            [FromQuery] string region = "en-US",
            [FromQuery] long seed = 0,
            [FromQuery] double likes = 5.0,
            [FromQuery] int count = 20)
        {
            var zipStream = new MemoryStream();
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                for (int i = 1; i <= count; i++)
                {
                    var song = _songGenerator.GenerateSong(i, region, seed, likes);
                    var audioBytes = _audioGenerator.GeneratePreviewWav(i, seed);

                    string fileName = $"{i:000} - {Sanitize(song.Artist)} - {Sanitize(song.Title)}.mp3";
                    var entry = archive.CreateEntry(fileName, CompressionLevel.Fastest);
                    using (var entryStream = entry.Open())
                    {
                        entryStream.Write(audioBytes, 0, audioBytes.Length);
                    }
                }
            }
            zipStream.Position = 0;
            return File(zipStream, "application/zip", $"music_export_{region}_{seed}.zip");
        }

        private static string Sanitize(string name) =>
            string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
    }
}