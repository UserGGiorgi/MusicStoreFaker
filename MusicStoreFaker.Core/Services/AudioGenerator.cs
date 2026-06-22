using System;
using System.IO;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.MusicTheory;
using Melanchall.DryWetMidi.Common;
using MeltySynth; 
using NAudio.Wave;
using NAudio.Lame;

using DryWetMidiFile = Melanchall.DryWetMidi.Core.MidiFile;

namespace MusicStoreFaker.Core.Services
{
    public class AudioGenerator : IAudioGenerator
    {
        private readonly string _soundFontPath;
        private const int SampleRate = 44100;
        private const int MaxDurationSeconds = 30;

        public AudioGenerator(string soundFontPath)
        {
            _soundFontPath = soundFontPath;
        }

        public byte[] GeneratePreviewMp3(int sequenceIndex, long seed)
        {
            long effectiveSeed = CombineSeed(seed, sequenceIndex);
            Random rng = new Random((int)(effectiveSeed ^ (effectiveSeed >> 32)));

            byte[] midiBytes = CreateMidi(rng);
            byte[] wavBytes = RenderMidiToWav(midiBytes);
            return ConvertWavToMp3(wavBytes);
        }

        private byte[] CreateMidi(Random rng)
        {
            var midiFile = new DryWetMidiFile();

            var tempoTrack = new TrackChunk();

            int bpm = rng.Next(80, 161);
            int microsecondsPerQuarterNote = (int)(60000000.0 / bpm);

            tempoTrack.Events.Add(
                new SetTempoEvent(microsecondsPerQuarterNote));

            tempoTrack.Events.Add(
                new TimeSignatureEvent(4, 4));

            NoteName[] keys =
            {
        NoteName.C,
        NoteName.G,
        NoteName.D,
        NoteName.A,
        NoteName.E,
        NoteName.F
    };

            NoteName key = keys[rng.Next(keys.Length)];
            bool isMinor = rng.Next(2) == 0;

            int[] scaleNotes = GetScaleNotes(key, isMinor);

            int[][] progressions =
            {
        new[] { 0, 3, 4, 0 },
        new[] { 0, 4, 5, 3 },
        new[] { 0, 2, 3, 0 },
        new[] { 0, 5, 3, 4 },
        new[] { 0, 3, 2, 0 }
    };

            int[] progression =
                progressions[rng.Next(progressions.Length)];

            var melodyTrack = new TrackChunk();
            var bassTrack = new TrackChunk();
            var drumsTrack = new TrackChunk();

            melodyTrack.Events.Add(
                new ProgramChangeEvent(
                    (SevenBitNumber)rng.Next(0, 8))
                {
                    Channel = (FourBitNumber)0
                });

            bassTrack.Events.Add(
                new ProgramChangeEvent(
                    (SevenBitNumber)rng.Next(32, 40))
                {
                    Channel = (FourBitNumber)1
                });

            drumsTrack.Events.Add(
                new ProgramChangeEvent((SevenBitNumber)0)
                {
                    Channel = (FourBitNumber)9
                });

            const int TicksPerBeat = 480;
            const int BarLength = TicksPerBeat * 4;

            int sections = 4;

            for (int s = 0; s < sections; s++)
            {
                for (int c = 0; c < 4; c++)
                {
                    int chordRoot = progression[c];
                    int[] chordNotes =
                        GetChordNotes(scaleNotes, chordRoot);

                    for (int beat = 0; beat < 4; beat++)
                    {
                        int notesInBeat = rng.Next(1, 4);

                        for (int n = 0; n < notesInBeat; n++)
                        {
                            int note =
                                chordNotes[rng.Next(chordNotes.Length)];

                            int velocity = rng.Next(70, 110);
                            int duration = rng.Next(120, 480);

                            melodyTrack.Events.Add(
                                new NoteOnEvent(
                                    (SevenBitNumber)note,
                                    (SevenBitNumber)velocity)
                                {
                                    Channel = (FourBitNumber)0
                                });

                            melodyTrack.Events.Add(
                                new NoteOffEvent(
                                    (SevenBitNumber)note,
                                    (SevenBitNumber)0)
                                {
                                    Channel = (FourBitNumber)0,
                                    DeltaTime = (long)duration
                                });
                        }
                    }

                    int bassNote = chordNotes[0] - 12;

                    bassTrack.Events.Add(
                        new NoteOnEvent(
                            (SevenBitNumber)bassNote,
                            (SevenBitNumber)80)
                        {
                            Channel = (FourBitNumber)1
                        });

                    bassTrack.Events.Add(
                        new NoteOffEvent(
                            (SevenBitNumber)bassNote,
                            (SevenBitNumber)0)
                        {
                            Channel = (FourBitNumber)1,
                            DeltaTime = BarLength
                        });

                    drumsTrack.Events.Add(
                        new NoteOnEvent(
                            (SevenBitNumber)36,
                            (SevenBitNumber)100)
                        {
                            Channel = (FourBitNumber)9
                        });

                    drumsTrack.Events.Add(
                        new NoteOffEvent(
                            (SevenBitNumber)36,
                            (SevenBitNumber)0)
                        {
                            Channel = (FourBitNumber)9,
                            DeltaTime = TicksPerBeat / 2
                        });

                    drumsTrack.Events.Add(
                        new NoteOnEvent(
                            (SevenBitNumber)38,
                            (SevenBitNumber)90)
                        {
                            Channel = (FourBitNumber)9
                        });

                    drumsTrack.Events.Add(
                        new NoteOffEvent(
                            (SevenBitNumber)38,
                            (SevenBitNumber)0)
                        {
                            Channel = (FourBitNumber)9,
                            DeltaTime = TicksPerBeat / 2
                        });
                }
            }

            midiFile.Chunks.Add(tempoTrack);
            midiFile.Chunks.Add(melodyTrack);
            midiFile.Chunks.Add(bassTrack);
            midiFile.Chunks.Add(drumsTrack);

            using var stream = new MemoryStream();
            midiFile.Write(stream);

            return stream.ToArray();
        }
        private byte[] RenderMidiToWav(byte[] midiBytes)
        {
            MeltySynth.MidiFile synthMidi;
            using (var ms = new MemoryStream(midiBytes))
            {
                synthMidi = new MeltySynth.MidiFile(ms);
            }

            var synth = new Synthesizer(_soundFontPath, SampleRate);
            var sequencer = new MidiFileSequencer(synth);
            sequencer.Play(synthMidi, loop: false);

            int maxSamples = SampleRate * MaxDurationSeconds;
            float[] left = new float[maxSamples];
            float[] right = new float[maxSamples];
            sequencer.Render(left, right);

            short[] shortSamples = new short[maxSamples * 2];
            int samplesToUse = Math.Min(maxSamples, left.Length);
            for (int i = 0; i < samplesToUse; i++)
            {
                shortSamples[i * 2] = (short)(left[i] * 32767);
                shortSamples[i * 2 + 1] = (short)(right[i] * 32767);
            }

            using var wavStream = new MemoryStream();
            var format = new WaveFormat(SampleRate, 16, 2);
            using (var writer = new WaveFileWriter(wavStream, format))
            {
                byte[] byteBuffer = new byte[shortSamples.Length * sizeof(short)];
                Buffer.BlockCopy(shortSamples, 0, byteBuffer, 0, byteBuffer.Length);
                writer.Write(byteBuffer, 0, byteBuffer.Length);
            }
            return wavStream.ToArray();
        }

        private byte[] ConvertWavToMp3(byte[] wavBytes)
        {
            using var inputStream = new MemoryStream(wavBytes);
            using var reader = new WaveFileReader(inputStream);
            using var outputStream = new MemoryStream();
            using var mp3Writer = new LameMP3FileWriter(outputStream, reader.WaveFormat, 128);
            reader.CopyTo(mp3Writer);
            return outputStream.ToArray();
        }

        private int[] GetScaleNotes(NoteName root, bool minor)
        {
            var notes = new List<int>();
            int[] intervals = minor
                ? new[] { 2, 1, 2, 2, 1, 2, 2 }
                : new[] { 2, 2, 1, 2, 2, 2, 1 };
            int note = (int)root;
            notes.Add(note);
            foreach (int interval in intervals)
            {
                note += interval;
                notes.Add(note % 12);
            }
            return notes.ConvertAll(n => n + 60).ToArray();
        }

        private int[] GetChordNotes(int[] scale, int degree)
        {
            int root = scale[degree % 7];
            int third = scale[(degree + 2) % 7];
            int fifth = scale[(degree + 4) % 7];
            return new[] { root, third, fifth };
        }

        private static long CombineSeed(long seed, int index)
        {
            const long Magic = unchecked((long)0x9E3779B97F4A7C15UL);
            return unchecked(seed ^ (index * Magic));
        }
    }
}