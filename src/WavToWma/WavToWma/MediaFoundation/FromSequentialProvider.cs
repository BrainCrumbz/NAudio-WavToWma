using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavToWma.MediaFoundation
{
    internal static class FromSequentialProvider
    {
        internal static IWaveProvider BuildJoinedProvider(IEnumerable<MediaFoundationReader> waveFileReaders)
        {
            SequentialWaveProvider joiningWaveProvider = new SequentialWaveProvider(waveFileReaders);

            return joiningWaveProvider;
        }

        private class SequentialWaveProvider : BaseJoinWaveProvider
        {
            public SequentialWaveProvider(IEnumerable<MediaFoundationReader> inputFileReaders) 
                : base(inputFileReaders)
            {
                _readIndex = 0;
            }

            /// <summary>
            /// Reads data from this WaveProvider
            /// </summary>
            /// <param name="buffer">Buffer to be filled with sample data</param>
            /// <param name="offset">Offset to write to within buffer, usually 0</param>
            /// <param name="count">Number of bytes required</param>
            /// <returns>Number of bytes read</returns>
            public override int Read(byte[] buffer, int offset, int count)
            {
                Debug.WriteLine("Read, offset: " + offset + ", count: " + count);

                int startIndex = _readIndex;
                int currentOffset = offset;
                int totalRead = 0;
                int remaining = count;

                for (_readIndex = startIndex; _readIndex < _inputFileReaders.Count && remaining > 0; _readIndex++)
                {
                    MediaFoundationReader currentReader = _inputFileReaders[_readIndex];

                    Debug.WriteLine("  - readIndex: " + _readIndex + ", totalTime: " + currentReader.TotalTime.TotalMilliseconds
                        + ", currentOffset: " + currentOffset + ", totalRead: " + totalRead + ", remaining: " + remaining);
                    int readNow = currentReader.Read(buffer, currentOffset, remaining);

                    currentOffset += readNow;
                    totalRead += readNow;
                    remaining -= readNow;
                }

                Debug.WriteLine("  - currentOffset: " + currentOffset + ", totalRead: " + totalRead + ", remaining: " + remaining);
                return totalRead;
            }

            private int _readIndex;
        }
    }
}
