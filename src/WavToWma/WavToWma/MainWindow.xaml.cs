using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WavToWma.MediaFoundation;
using WavToWma.WindowsMediaFormat;

namespace WavToWma
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            MediaFoundation.Operations.Initialize();
        }

        private void JoinFromCodec_Button_Click(object sender, RoutedEventArgs e)
        {
            // This causes exception: Object contains non-primitive or non-blittable data
            WindowsMediaFormat.Operations.JoinManyFiles(FromCodec.BuildWmaWriter, _sourceFiles, _mediaFormatCodecJoinedOutputFile);
        }

        private void JoinFromProfileGuid_Button_Click(object sender, RoutedEventArgs e)
        {
            // This causes exception: Object contains non-primitive or non-blittable data
            WindowsMediaFormat.Operations.JoinManyFiles(FromProfileGuid.BuildWmaWriter, _sourceFiles, _mediaFormatProfileJoinedOutputFile);
        }

        private void SingleFromMediaFoundation_Button_Click(object sender, RoutedEventArgs e)
        {
            bool useExplicitEncoder = DetectEncoderOption(sender);
            string outputFile = _foundationSingleOutputFilePre + (useExplicitEncoder ? "-expl" : "-impl") + ".wma";

            // This runs fine
            MediaFoundation.Operations.ConvertSingleFile(_sourceFiles[0], outputFile, useExplicitEncoder);
        }

        private void JoinFromMediaFoundationMixing_Button_Click(object sender, RoutedEventArgs e)
        {
            bool useExplicitEncoder = DetectEncoderOption(sender);
            string outputFile = _foundationMixingJoinedOutputFilePre + (useExplicitEncoder ? "-expl" : "-impl") + ".wma";

            // This runs fine
            MediaFoundation.Operations.JoinManyFiles(FromMixingProvider.BuildJoinedProvider, _sourceFiles, outputFile, useExplicitEncoder);
        }

        private void JoinFromMediaFoundationSequential_Button_Click(object sender, RoutedEventArgs e)
        {
            bool useExplicitEncoder = DetectEncoderOption(sender);
            string outputFile = _foundationSequentialJoinedOutputFilePre + (useExplicitEncoder ? "-expl" : "-impl") + ".wma";

            // This runs fine
            MediaFoundation.Operations.JoinManyFiles(FromSequentialProvider.BuildJoinedProvider, _sourceFiles, outputFile, useExplicitEncoder);
        }

        private void JoinFromMediaFoundationInMemory_Button_Click(object sender, RoutedEventArgs e)
        {
            bool useExplicitEncoder = DetectEncoderOption(sender);
            string outputFile = _foundationInMemoryJoinedOutputFilePre + (useExplicitEncoder ? "-expl" : "-impl") + ".wma";

            // This runs fine
            MediaFoundation.Operations.JoinManyFiles(FromInMemoryProvider.BuildJoinedProvider, _sourceFiles, outputFile, useExplicitEncoder);
        }

        private static bool DetectEncoderOption(object sender)
        {
            string encoderTag = (string)((Button)sender).Tag;

            bool useExplicitEncoder;

            switch (encoderTag)
            {
                case "Implicit":
                    useExplicitEncoder = false;
                    break;

                case "Explicit":
                    useExplicitEncoder = true;
                    break;

                default:
                    throw new ArgumentException("Unknown value passed as tag: " + encoderTag);
            }

            return useExplicitEncoder;
        }

        // basic example
        /*
        private static string[] _sourceFiles = new [] 
        { 
            @".\Assets\a_pr.wav",
            @".\Assets\b_pr.wav",
        };
        */

        // see what happens with WAV files having same/different formats
        /*
        private static string[] _sourceFiles = new[] 
        { 
            @".\Assets\a_pr.wav",
            // add the following to see what happens with WAV files having different formats
            @".\Assets\1.wav",
            // this one instead has again the same format
            @".\Assets\Numero-1.wav",
            @".\Assets\b_pr.wav",
        };
        */

        // emulate announcement message composition
        private static string[] _sourceFiles = new[] 
        { 
            @".\Assets\Announce\Windows-Sonata-Windows-Logon-Sound.wav",
            @".\Assets\Announce\Lettera-T.wav",
            @".\Assets\Announce\Lettera-E.wav",
            @".\Assets\Announce\Lettera-S.wav",
            @".\Assets\Announce\Lettera-T.wav",
            @".\Assets\Announce\Numero-5.wav",
            @".\Assets\Announce\Windows-Quirky-Windows-Logoff-Sound.wav",
        };

        private static string _mediaFormatCodecJoinedOutputFile = @".\Assets\mediaformat-joined-codec.wma";
        private static string _mediaFormatProfileJoinedOutputFile = @".\Assets\mediaformat-joined-profile.wma";

        private static string _foundationSingleOutputFilePre = @".\Assets\foundation-converted";
        private static string _foundationMixingJoinedOutputFilePre = @".\Assets\foundation-mixing";
        private static string _foundationSequentialJoinedOutputFilePre = @".\Assets\foundation-sequential";
        private static string _foundationInMemoryJoinedOutputFilePre = @".\Assets\foundation-inmemory";
    }
}
