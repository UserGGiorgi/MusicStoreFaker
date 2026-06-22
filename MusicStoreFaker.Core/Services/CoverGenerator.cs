using SkiaSharp;
using System;

namespace MusicStoreFaker.Core.Services
{
    public class CoverGenerator : ICoverGenerator
    {
        private const int Width = 600;
        private const int Height = 600;

        public byte[] GenerateCover(int sequenceIndex, string title, string artist,
                                    string album, bool isSingle, string locale, long seed)
        {
            long effectiveSeed = CombineSeed(seed, sequenceIndex);
            Random rng = new Random((int)(effectiveSeed ^ (effectiveSeed >> 32)));

            using var surface = SKSurface.Create(new SKImageInfo(Width, Height));
            var canvas = surface.Canvas;

            DrawBackground(canvas, rng);

            if (isSingle)
                DrawBadge(canvas, rng);

            DrawText(canvas, title, artist, album, rng);

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();
        }

        private void DrawBackground(SKCanvas canvas, Random rng)
        {
            int style = rng.Next(5);
            switch (style)
            {
                case 0:
                    using (var paint = new SKPaint())
                    {
                        paint.Shader = SKShader.CreateLinearGradient(
                            new SKPoint(0, 0), new SKPoint(Width, Height),
                            new[] { RandomColor(rng), RandomColor(rng) },
                            SKShaderTileMode.Clamp);
                        canvas.DrawRect(0, 0, Width, Height, paint);
                    }
                    break;

                case 1: 
                    canvas.Clear(RandomColor(rng));
                    for (int i = 0; i < 15; i++)
                        DrawRandomShape(canvas, rng);
                    break;

                case 2:
                    canvas.Clear(RandomColor(rng));
                    int stripeCount = rng.Next(5, 20);
                    float stripeWidth = Width / (float)stripeCount;
                    using (var stripePaint = new SKPaint())
                    {
                        for (int i = 0; i < stripeCount; i++)
                        {
                            stripePaint.Color = RandomColor(rng);
                            canvas.DrawRect(i * stripeWidth, 0, stripeWidth, Height, stripePaint);
                        }
                    }
                    break;

                case 3: 
                    canvas.Clear(RandomColor(rng));
                    using (var circlePaint = new SKPaint())
                    {
                        for (int i = 0; i < 40; i++)
                        {
                            circlePaint.Color = RandomColor(rng, 100);
                            float x = (float)(rng.NextDouble() * Width);
                            float y = (float)(rng.NextDouble() * Height);
                            float r = rng.Next(10, 80);
                            canvas.DrawCircle(x, y, r, circlePaint);
                        }
                    }
                    break;

                case 4: // rays from center
                    canvas.Clear(RandomColor(rng));
                    using (var rayPaint = new SKPaint())
                    {
                        int rays = rng.Next(8, 20);
                        for (int i = 0; i < rays; i++)
                        {
                            float angle = (float)(i * 2 * Math.PI / rays);
                            rayPaint.Color = RandomColor(rng, 180);
                            canvas.DrawLine(Width / 2, Height / 2,
                                Width / 2 + Width * (float)Math.Cos(angle),
                                Height / 2 + Height * (float)Math.Sin(angle), rayPaint);
                        }
                    }
                    break;
            }
        }

        private void DrawRandomShape(SKCanvas canvas, Random rng)
        {
            using var paint = new SKPaint
            {
                Color = RandomColor(rng, 150),
                IsAntialias = true,
                Style = rng.Next(2) == 0 ? SKPaintStyle.Fill : SKPaintStyle.Stroke,
                StrokeWidth = rng.Next(1, 5)
            };

            int x = rng.Next(Width);
            int y = rng.Next(Height);
            int size = rng.Next(20, 120);

            switch (rng.Next(3))
            {
                case 0: canvas.DrawRect(x, y, size, size, paint); break;
                case 1: canvas.DrawCircle(x, y, size / 2, paint); break;
                case 2:
                    using (var path = new SKPath())
                    {
                        path.MoveTo(x, y);
                        path.LineTo(x + size, y);
                        path.LineTo(x + size / 2, y - size);
                        path.Close();
                        canvas.DrawPath(path, paint);
                    }
                    break;
            }
        }

        private void DrawBadge(SKCanvas canvas, Random rng)
        {
            using var badgePaint = new SKPaint
            {
                Color = SKColors.Gold,
                IsAntialias = true
            };
            canvas.DrawRect(Width - 150, 20, 130, 50, badgePaint);

            using var textPaint = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = 28,
                IsAntialias = true,
                TextAlign = SKTextAlign.Center
            };
            canvas.DrawText("SINGLE", Width - 85, 55, textPaint);
        }

        private void DrawText(SKCanvas canvas, string title, string artist, string album, Random rng)
        {
            using var titlePaint = new SKPaint
            {
                Color = SKColors.White,
                TextSize = 44,
                IsAntialias = true,
                TextAlign = SKTextAlign.Center,
                FakeBoldText = true
            };
            titlePaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 3);
            canvas.DrawText(title, Width / 2, Height / 2 + 40, titlePaint);

            using var artistPaint = new SKPaint
            {
                Color = SKColors.LightGray,
                TextSize = 32,
                IsAntialias = true,
                TextAlign = SKTextAlign.Center
            };
            canvas.DrawText(artist, Width / 2, Height / 2 + 100, artistPaint);

            if (!string.IsNullOrEmpty(album) && album != "Single")
            {
                using var albumPaint = new SKPaint
                {
                    Color = SKColors.White.WithAlpha(200),
                    TextSize = 24,
                    IsAntialias = true,
                    TextAlign = SKTextAlign.Center
                };
                canvas.DrawText(album, Width / 2, Height / 2 + 140, albumPaint);
            }
        }

        private SKColor RandomColor(Random rng, byte alpha = 255)
        {
            return new SKColor((byte)rng.Next(256), (byte)rng.Next(256),
                               (byte)rng.Next(256), alpha);
        }

        private static long CombineSeed(long seed, int index)
        {
            const long Magic = unchecked((long)0x9E3779B97F4A7C15UL);
            return unchecked(seed ^ (index * Magic));
        }
    }
}