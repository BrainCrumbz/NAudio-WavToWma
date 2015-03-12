using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavToWma.MediaFoundation
{
    internal abstract class BaseJoinWaveProvider : IWaveProvider
    {
        public BaseJoinWaveProvider(IEnumerable<MediaFoundationReader> inputFileReaders)
        {
            if (!inputFileReaders.Any())
            {
                throw new ArgumentException("At least one input file currentReader is needed");
            }

            WaveFormat firstFormat = inputFileReaders.First().WaveFormat;

            IEnumerable<WaveFormat> mismatchingFormat =
                from reader in inputFileReaders
                let format = reader.WaveFormat
                where !format.Equals(firstFormat)
                select format;

            if (mismatchingFormat.Any())
            {
                throw new ArgumentException("All readers must have the same WAV format: first file has: " +
                    firstFormat + ", others have: " +
                    String.Join(", ", mismatchingFormat.Select(f => f.ToString())));
            }

            WaveFormat = firstFormat;

            _inputFileReaders = inputFileReaders.ToList();

            Debug.WriteLine("Constructed, list count: " + _inputFileReaders.Count);
        }

        protected readonly List<MediaFoundationReader> _inputFileReaders;

        public abstract int Read(byte[] buffer, int offset, int count);

        public virtual WaveFormat WaveFormat { get; protected set; }
    }
}
