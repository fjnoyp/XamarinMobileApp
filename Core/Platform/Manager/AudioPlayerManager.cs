
#if __ANDROID__
using Android.Media;
using Android.Net;
#endif

#if __IOS__
using AVFoundation;
using Foundation;
#endif

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace Cards.Core.Platform.Manager
{
    public static class AudioPlayerManager
    {

        public delegate void PlayFinished();
        public static event PlayFinished audioPlayFinished;

#if __IOS__
        private static AVAudioRecorder recorder;
        private static NSError error;
        private static NSUrl url;
        private static NSDictionary settings;

        private static AVAudioPlayer player;

     
#endif

#if __IOS__
        private static void initialize()
        {
            // Initialize audio session 
            var audioSession = AVAudioSession.SharedInstance();
            var err = audioSession.SetCategory(AVAudioSessionCategory.PlayAndRecord);
            if (err != null)
            {
                Console.WriteLine("audioSession: {0}", err);
                //return false;
            }
            err = audioSession.SetActive(true);
            if (err != null)
            {
                Console.WriteLine("audioSession: {0}", err);
                //return false;
            }

            // Prepare settings and recorder 

            // Set up the NSObject Array of values that will be combined with the keys to make the NSDictionary
            NSObject[] values = new NSObject[]
            {
    NSNumber.FromFloat (44100.0f), //Sample Rate
    NSNumber.FromInt32 ((int)AudioToolbox.AudioFormatType.LinearPCM), //AVFormat
    NSNumber.FromInt32 (2), //Channels
    NSNumber.FromInt32 (16), //PCMBitDepth
    NSNumber.FromBoolean (false), //IsBigEndianKey
    NSNumber.FromBoolean (false) //IsFloatKey
            };

            //Set up the NSObject Array of keys that will be combined with the values to make the NSDictionary
            NSObject[] keys = new NSObject[]
            {
    AVAudioSettings.AVSampleRateKey,
    AVAudioSettings.AVFormatIDKey,
    AVAudioSettings.AVNumberOfChannelsKey,
    AVAudioSettings.AVLinearPCMBitDepthKey,
    AVAudioSettings.AVLinearPCMIsBigEndianKey,
    AVAudioSettings.AVLinearPCMIsFloatKey
            };

            //Set Settings with the Values and Keys to create the NSDictionary
            settings = NSDictionary.FromObjectsAndKeys(values, keys);

        }

        public static string startRecordAudio()
        {
            if (settings == null) initialize(); 

            string fileName = string.Format("MyRecording{0}.wav", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string audioFilePath = Path.Combine( FilePaths.allAudiosPath, fileName);

            Console.WriteLine("Audio File Path: " + audioFilePath);

            url = NSUrl.FromFilename(audioFilePath);

            //Set recorder parameters
            recorder = AVAudioRecorder.Create(url, new AudioSettings(settings), out error);
            recorder.Record(); 

            return audioFilePath; 
        }

        public static void stopRecordAudio()
        {
            recorder.Stop(); 
        }

        public static double getAudioDuration(string audioFilePath)
        {
            NSUrl songURL = NSUrl.FromFilename(audioFilePath);
            NSError err;
            AVAudioPlayer temp = new AVAudioPlayer(songURL, "wav", out err);
            return temp.Duration; 
        }

        public static void playAudio(string audioFilePath)
        {
            // Any existing sound effect?
            if (player != null)
            {
                // Stop and dispose of any sound effect
                player.Stop();
                player.Dispose();
            }

            // Initialize background music
            NSUrl songURL = NSUrl.FromFilename(audioFilePath);
            NSError err;

            player = new AVAudioPlayer(songURL, "wav", out err);
            //soundEffect.Volume = EffectsVolume;
            player.FinishedPlaying += delegate {
                //player = null;
                audioPlayFinished?.Invoke(); 
            };
            player.NumberOfLoops = 0;
            player.Play();
        }
        public static void stopPlayAudio()
        {
            player.Stop(); 
        }
        public static void pausePlayAudio(){
            player.Pause(); 
        }
        public static void resumePlayAudio()
        {
            player.Play(); 
        }
#endif

#if __ANDROID__

        public static void playAudio(string filePath)
        {

            MessagingCenter.Send<string>("PlayAudio", filePath);
        }
        public static double getAudioDuration(string audioFilePath) { return 0; }

        public static string startRecordAudio() { return ""; }

        public static void stopRecordAudio() {  }

        public static void stopPlayAudio()
        {

        }
        public static void pausePlayAudio()
        {

        }
        public static void resumePlayAudio()
        {

        }
#endif
    }
}
