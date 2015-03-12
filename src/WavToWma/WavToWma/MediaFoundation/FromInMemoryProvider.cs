using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavToWma.MediaFoundation
{
    internal static class FromInMemoryProvider
    {
        internal static IWaveProvider BuildJoinedProvider(IEnumerable<MediaFoundationReader> waveFileReaders)
        {
            InMemoryWaveProvider joiningWaveProvider = new InMemoryWaveProvider(waveFileReaders);

            return joiningWaveProvider;
        }

        private class InMemoryWaveProvider : BaseJoinWaveProvider, IDisposable
        {
            public InMemoryWaveProvider(IEnumerable<MediaFoundationReader> inputFileReaders)
                : base(inputFileReaders)
            {
                BuildMemoryStream();
            }

            private void BuildMemoryStream()
            {
                _memoryStream = new MemoryStream();

                int oneSecondChunkSize = WaveFormat.AverageBytesPerSecond;

                byte[] buffer = new byte[oneSecondChunkSize];

                using (IgnoreDisposeStream skipDisposeStream = new IgnoreDisposeStream(_memoryStream))
                using (WaveFileWriter wavWriter = new WaveFileWriter(skipDisposeStream, WaveFormat))
                {
                    foreach (MediaFoundationReader reader in _inputFileReaders)
                    {
                        int read = 0;

                        while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            wavWriter.Write(buffer, 0, read);
                        }
                    }
                }

                // NOTE do not (re)set position: it will cause an initial click noise in output file
                //_memoryStream.Position = 0;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int read = 0;

                using(IgnoreDisposeStream skipDisposeStream = new IgnoreDisposeStream(_memoryStream))
                using (RawSourceWaveStream wavReader = new RawSourceWaveStream(skipDisposeStream, WaveFormat))
                {
                    read = wavReader.Read(buffer, offset, count);
                }

                return read;
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
                Dispose(true);
            }

            protected void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (_memoryStream != null)
                    {
                        _memoryStream.Dispose();
                        _memoryStream = null;
                    }
                }
            }

            private MemoryStream _memoryStream;
        }
    }
}
