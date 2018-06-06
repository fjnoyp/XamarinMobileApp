using Cards.Core.Platform.Manager;
using Plugin.MediaManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Cards.Core.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AudioView : ContentView, IDisappearedListener
	{
        AudioMediaContent audioMedia;
        bool isPlaying; 

		public AudioView ()
		{
			InitializeComponent ();
		}

        public AudioView(AbMediaContent media)
        {
            InitializeComponent();

            this.mediaNameLabel.Text = System.IO.Path.GetFileNameWithoutExtension(media.filePath);

            DateTime audioDuration = new DateTime((long)AudioPlayerManager.getAudioDuration(media.filePath));
            recordingDuration.Text = audioDuration.ToString("HH:mm:ss");

            this.audioMedia = media as AudioMediaContent;

            stopButton.IsEnabled = false;

            // WARNING TODO MEMORY LEAK not unsubscribing from event 
            // will need to add to some preparefordispose method called from parent page
            AudioPlayerManager.audioPlayFinished += audioPlayFinished;
        }

        private async void playButton_Clicked(object sender, EventArgs e)
        {
            if (isPlaying)
            {
                if(playButton.Text == "Pause")
                {
                    AudioPlayerManager.pausePlayAudio();
                    playButton.Text = "Play";
                }
                else if (playButton.Text == "Play")
                {
                    AudioPlayerManager.resumePlayAudio();
                    playButton.Text = "Pause";
                }
            }
            else
            {
                AudioPlayerManager.playAudio(audioMedia.filePath);

                playButton.Text = "Pause";
                stopButton.IsEnabled = true;

                isPlaying = true;
            }
        }


        private void stopButton_Clicked(object sender, EventArgs e)
        {
            AudioPlayerManager.stopPlayAudio();

            audioPlayFinished(); 
        }

        private void audioPlayFinished()
        {
            playButton.IsEnabled = true;
            playButton.Text = "Play"; 
            stopButton.IsEnabled = false;

            isPlaying = false; 
        }

        public void onDisappear()
        {
        }
    }
}