using NAudio.MediaFoundation;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavToWma.MediaFoundation
{
    internal static class Operations
    {
        internal static void Initialize()
        {
            MediaFoundationApi.Startup();
        }

        internal static void ConvertSingleFile(string sourceFile, string outputFile, bool explicitEncoder)
        {
            using (var wavReader = new MediaFoundationReader(sourceFile))
            {
                SaveToWma(wavReader, outputFile, explicitEncoder);
            }
        }

        internal static void JoinManyFiles(Func<IEnumerable<MediaFoundationReader>, IWaveProvider> buildProvider, string[] sourceFiles, string outputFile, bool explicitEncoder)
        {
            WaveFormat firstWaveFormat = null;
            List<MediaFoundationReader> waveFileReaders = new List<MediaFoundationReader>(sourceFiles.Length);

            try
            {
                foreach (string sourceFile in sourceFiles)
                {
                    MediaFoundationReader waveReader = new MediaFoundationReader(sourceFile);

                    if (firstWaveFormat == null)
                    {
                        // first time in save input wave format
                        firstWaveFormat = waveReader.WaveFormat;
                    }
                    else
                    {
                        if (!waveReader.WaveFormat.Equals(firstWaveFormat))
                        {
                            throw new InvalidOperationException("Can't concatenate WAV Files having different formats: first file has: " +
                                firstWaveFormat + ", another has: " + waveReader.WaveFormat);
                        }
                    }

                    waveFileReaders.Add(waveReader);
                }

                Debug.WriteLine("Common format: " + firstWaveFormat);

                IWaveProvider joinedWaveProvider = buildProvider(waveFileReaders);

                SaveToWma(joinedWaveProvider, outputFile, explicitEncoder);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
            finally
            {
                waveFileReaders.ForEach(reader => reader.Dispose());
            }
        }

        private static void SaveToWma(IWaveProvider waveProvider, string outputFile, bool explicitEncoder)
        {
            const int sampleRateInHz = 44100;
            const int nrOfChannels = 1;
            const int bitRateInKbps = 16;
            const int bitRateInBps = bitRateInKbps * 1000;

            if (explicitEncoder)
            {
                // Explicit encoder, get a media type

                Debug.WriteLine("Saving to WMA with explicit encoder");

                MediaType wmaMediaType = MediaFoundationEncoder.SelectMediaType(
                    AudioSubtypes.MFAudioFormat_WMAudioV8,
                    new WaveFormat(sampleRateInHz, nrOfChannels),
                    bitRateInBps);

                if (wmaMediaType == null)
                {
                    throw new ApplicationException("Could not find a suitable WMA Audio codec installed on this machine");
                }

                using (MediaFoundationEncoder wmaEncoder = new MediaFoundationEncoder(wmaMediaType))
                {
                    wmaEncoder.Encode(outputFile, waveProvider);
                }
            }
            else
            {
                // Single line encoding, just set some bitrate

                Debug.WriteLine("Saving to WMA with implicit encoder");

                MediaFoundationEncoder.EncodeToWma(waveProvider, outputFile, bitRateInBps);
            }
        }
    }
}
