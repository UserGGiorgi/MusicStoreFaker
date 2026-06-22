using Microsoft.AspNetCore.Mvc;
using MusicStoreFaker.Core.Models;
using MusicStoreFaker.Core.Services;
using System;
using System.Collections.Generic;

namespace MusicStoreFaker.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongsController : ControllerBase
    {
        private readonly ISongGenerator _songGenerator;
        private readonly ICoverGenerator _coverGenerator;
        private readonly IAudioGenerator _audioGenerator;

        public SongsController(
            ISongGenerator songGenerator,
            ICoverGenerator coverGenerator,
            IAudioGenerator audioGenerator)
        {
            _songGenerator = songGenerator;
            _coverGenerator = coverGenerator;
            _audioGenerator = audioGenerator;
        }

        [HttpGet]
        public ActionResult<PaginatedSongs> GetSongs(
            [FromQuery] string region = "en-US",
            [FromQuery] long seed = 0,
            [FromQuery] double likes = 5.0,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = _songGenerator.GeneratePage(page, pageSize, region, seed, likes);
            return Ok(result);
        }

        [HttpGet("gallery")]
        public ActionResult<GalleryBatch> GetGallery(
            [FromQuery] string region = "en-US",
            [FromQuery] long seed = 0,
            [FromQuery] double likes = 5.0,
            [FromQuery] int startIndex = 1,
            [FromQuery] int count = 10)
        {
            var result = _songGenerator.GenerateBatch(startIndex, count, region, seed, likes);
            return Ok(result);
        }

        [HttpGet("{index:int}")]
        public ActionResult<SongDetail> GetSongDetail(
            int index,
            [FromQuery] string region = "en-US",
            [FromQuery] long seed = 0)
        {
            var song = _songGenerator.GenerateSong(index, region, seed, 0);
            var detail = _songGenerator.GenerateDetail(song, region, seed);
            return Ok(detail);
        }

        [HttpGet("{index:int}/cover")]
        public IActionResult GetCover(
            int index,
            [FromQuery] string region = "en-US",
            [FromQuery] long seed = 0)
        {
            var song = _songGenerator.GenerateSong(index, region, seed, 0);
            var coverBytes = _coverGenerator.GenerateCover(
                index, song.Title, song.Artist, song.Album, song.IsSingle, region, seed);
            return File(coverBytes, "image/png");
        }

        [HttpGet("{index:int}/audio")]
        public IActionResult GetAudio(int index, [FromQuery] long seed = 0)
        {
            var audioBytes = _audioGenerator.GeneratePreviewWav(index, seed);
            return File(audioBytes, "audio/wav");
        }

        [HttpGet("{index:int}/lyrics")]
        public ActionResult<List<LyricLine>> GetLyrics(
            int index,
            [FromQuery] long seed = 0)
        {
            var lyrics = GenerateLyrics(index, seed);
            return Ok(lyrics);
        }

        private List<LyricLine> GenerateLyrics(int index, long seed)
        {
            long effectiveSeed = CombineSeed(seed, index);
            var rng = new Random((int)(effectiveSeed ^ (effectiveSeed >> 32)));

            var templates = new[]
            {
                "The {0} is {1} tonight",
                "{2} and {3} in the {4}",
                "I'm {5} through the {6}",
                "{7} every {8} I {9}",
            };
            var verbs = new[] { "falling", "dancing", "running", "dreaming", "flying", "singing" };
            var nouns = new[] { "light", "rain", "shadow", "fire", "wind", "storm" };

            var lines = new List<LyricLine>();
            int timeMs = 2000;

            for (int i = 0; i < 12; i++)
            {
                string line = string.Format(
                    templates[rng.Next(templates.Length)],
                    nouns[rng.Next(nouns.Length)],
                    verbs[rng.Next(verbs.Length)],
                    nouns[rng.Next(nouns.Length)],
                    verbs[rng.Next(verbs.Length)],
                    nouns[rng.Next(nouns.Length)],
                    nouns[rng.Next(nouns.Length)],
                    nouns[rng.Next(nouns.Length)],
                    verbs[rng.Next(verbs.Length)],
                    nouns[rng.Next(nouns.Length)],
                    verbs[rng.Next(verbs.Length)]
                );
                lines.Add(new LyricLine { TimeMs = timeMs, Text = line });
                timeMs += rng.Next(2500, 4500);
            }
            return lines;
        }

        private static long CombineSeed(long seed, int index)
        {
            const long Magic = unchecked((long)0x9E3779B97F4A7C15UL);
            return unchecked(seed ^ (index * Magic));
        }
    }

    public class LyricLine
    {
        public int TimeMs { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}