using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.IO;

namespace XCutter
{
    public static class WaveFileUtils
    {

        public static int TrimAudioFile(string inPath, string outPath, TimeSpan cutFromStart, TimeSpan cutFromEnd)
        {
            TimeSpan fadeSpan = new TimeSpan(0, 0, 0, 2, 470);

            if (Path.GetExtension(inPath) != ".mp3" && Path.GetExtension(inPath) != ".wav")
            {
                return -1;
            }

            WaveStream ws;

            if (Path.GetExtension(inPath) == ".mp3")
            {
                ws = new Mp3FileReader(inPath); 
            }
            else if (Path.GetExtension(inPath) == ".wav")
            {
                ws = new WaveFileReader(inPath);
            }
            else
            {
                return -1;
            }

            try
            {
                //using (AudioFileReader audioReader = new AudioFileReader(inPath))
                using (WaveStream audioReader = ws)
                {

                    string temp_file = Path.GetTempFileName();

                    var sampleProvider = audioReader.ToSampleProvider();

                    var SampleRate = audioReader.WaveFormat.SampleRate;

                    var startPosition = (cutFromStart.TotalMilliseconds * SampleRate) / 1000;

                    var takeSpan = audioReader.TotalTime - cutFromStart - cutFromEnd;

                    var skipBeforeFade = takeSpan - fadeSpan;

                    sampleProvider = sampleProvider.Skip(cutFromStart);

                    sampleProvider = sampleProvider.Take(takeSpan);

                    //var fader = new FadeInOutSampleProvider(sampleProvider);
                    var fader = new DelayFadeOutSampleProvider(sampleProvider);

                    fader.BeginFadeOut(skipBeforeFade.TotalMilliseconds, fadeSpan.TotalMilliseconds);

                    //fader.BeginFadeOut(fadeSpan.TotalMilliseconds);
                    

                    var wave = new SampleToWaveProvider(fader);

                    //WaveFileWriter.CreateWaveFile(outPath, wave);


                    using (WaveFileWriter writer = new WaveFileWriter(temp_file, audioReader.WaveFormat))
                    {
                        int read = 0;
                        float[] fBuffer = new float[64 * 1024];
                        do
                        {
                            read = fader.Read(fBuffer, 0, 64 * 1024);
                            if (read > 0)
                            {
                                writer.WriteSamples(fBuffer, 0, read);
                            }

                            //read = wave.Read(fBuffer, 0, 1024);
                            //if (read > 0)
                            //{
                            //    writer.Write(fBuffer, 0, read);
                            //}
                        } while (read > 0);
                    }

                    Console.Write($"Writing file: {Path.GetFileName(outPath)}.....");
                    File.Copy(temp_file, outPath);
                    Console.WriteLine($"Done.");
                    File.Delete(temp_file);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine($"Error: {e.Message}");
                return -1;
            }

            return 1;
        }



    } 
}