using NAudio.Wave;
using NAudio.WindowsMediaFormat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavToWma.WindowsMediaFormat
{
    internal static class Operations
    {
        internal static void JoinManyFiles(Func<FileStream, WaveFormat, WmaWriter> buildWriter, string[] sourceFiles, string outputFile)
        {
            // Ref. http://stackoverflow.com/questions/6777340/how-to-join-2-or-more-wav-files-together-programatically
            // Applied to writing WMA instead of WAV

            byte[] buffer = new byte[1024];
            WaveFormat firstWaveFormat = null;
            WmaWriter wmaWriter = null;
            FileStream wmaFileStream = null;

            try
            {
                foreach (string sourceFile in sourceFiles)
                {
                    using (WaveFileReader waveReader = new WaveFileReader(sourceFile))
                    {
                        if (wmaWriter == null)
                        {
                            // first time in save input wave format
                            firstWaveFormat = waveReader.WaveFormat;

                            // first time in create new Writer
                            wmaFileStream = new FileStream(outputFile, FileMode.Create);

                            wmaWriter = buildWriter(wmaFileStream, firstWaveFormat);
                        }
                        else
                        {
                            if (!waveReader.WaveFormat.Equals(firstWaveFormat))
                            {
                                throw new InvalidOperationException("Can't concatenate WAV Files having different formats: first file has: " +
                                    firstWaveFormat + ", another has: " + waveReader.WaveFormat);
                            }
                        }

                        int read;

                        while ((read = waveReader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            wmaWriter.Write(buffer, 0, read);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (wmaWriter != null)
                {
                    wmaWriter.Dispose();
                }

                if (wmaFileStream != null)
                {
                    wmaFileStream.Dispose();
                }
            }
        }
    }
}
