using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavToWma.MediaFoundation
{
    internal static class FromMixingProvider
    {
        internal static IWaveProvider BuildJoinedProvider(IEnumerable<MediaFoundationReader> waveFileReaders)
        {
            IEnumerable<ISampleProvider> sampleProviders =
                from reader in waveFileReaders
                select reader.ToSampleProvider();

            MixingSampleProvider joinedSampleProvider = new MixingSampleProvider(sampleProviders);

            IWaveProvider joinedWaveProvider = new SampleToWaveProvider(joinedSampleProvider);

            return joinedWaveProvider;
        }
    }
}
