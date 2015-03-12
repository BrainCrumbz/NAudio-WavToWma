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
    internal static class FromCodec
    {
        internal static WmaWriter BuildWmaWriter(FileStream wmaFileStream, WaveFormat waveFormat)
        {
            // Get codec, then codec format

            Codec[] wmaCodecs = Codec.GetCodecs(NAudio.WindowsMediaFormat.MediaTypes.WMMEDIATYPE_Audio);

            if (wmaCodecs.Length == 0)
            {
                throw new ApplicationException("Could not find any WMA Audio codec installed on this machine");
            }

            CodecFormat[] wmaCodecFormats = wmaCodecs[0].CodecFormats;

            if (wmaCodecFormats.Length == 0)
            {
                throw new ApplicationException("Could not find any WMA Audio codec format installed on this machine");
            }

            CodecFormat selectedWmaCodecFormat = wmaCodecFormats[wmaCodecFormats.Length - 1];

            try
            {
                WmaWriter wmaWriter = new WmaWriter(wmaFileStream, waveFormat, selectedWmaCodecFormat);

                return wmaWriter;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
