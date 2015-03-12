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
    internal static class FromProfileGuid
    {
        internal static WmaWriter BuildWmaWriter(FileStream wmaFileStream, WaveFormat waveFormat)
        {
            // Get system profile GUID, then load profile

            Guid WMProfile_V80_288MonoAudio = new Guid("{7EA3126D-E1BA-4716-89AF-F65CEE0C0C67}");

            Guid wmaSystemProfileGuid = WMProfile_V80_288MonoAudio;

            IWMProfile wmaProfile = null;

            WM.ProfileManager.LoadProfileByID(ref wmaSystemProfileGuid, out wmaProfile);

            try
            {
                WmaWriter wmaWriter = new WmaWriter(wmaFileStream, waveFormat, wmaProfile);

                return wmaWriter;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
