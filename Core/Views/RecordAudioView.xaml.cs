using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Acr.UserDialogs;
using System.IO;
using Cards.Core;
using Cards.Core.Platform.Manager;
using PCLStorage;
using Xamarin.Forms.Xaml;
using Cards.Core.FileReaders;

namespace Cards.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecordAudioView : ContentView, IMediaCapturer, IDisappearedListener
    {
        Card parentCard;

        string _audioFilePath;
        string audioFilePath
        {
            get
            {
                return _audioFilePath;
            }
            set
            {
                _audioFilePath = value;
                recordingName.Text = Path.GetFileNameWithoutExtension(_audioFilePath);
            }
        }

        bool isRecording = false;
        bool isPlaying = false;

        public AbMediaContent capturedMedia { get; set; }

        public RecordAudioView()
        {
            InitializeComponent(); 
        }

        public RecordAudioView(Card card)
        {
            InitializeComponent();

            this.parentCard = card; 

            recordMode();
        }

        void recordButton_Clicked(object sender, EventArgs e)
        {
            audioFilePath = AudioPlayerManager.startRecordAudio();

            startRecordMode();
        }

        void playButton_Clicked(object sender, EventArgs e)
        {
            if (isPlaying)
            {
                if (playButton.Text == "Pause")
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
                AudioPlayerManager.playAudio(audioFilePath);

                startPlayMode();
            }
        }

        async void stopButton_Clicked(object sender, EventArgs e)
        {
            if (isRecording)
            {
                AudioPlayerManager.stopRecordAudio();

                DateTime audioDuration = new DateTime((long)AudioPlayerManager.getAudioDuration(audioFilePath));
                recordingDuration.Text = audioDuration.ToString("HH:mm:ss");

                PromptConfig prompt = new PromptConfig();
                prompt.SetCancelText("Cancel");
                prompt.SetOkText("Create");
                prompt.SetMessage("Name audio recording");
                prompt.SetInputMode(InputType.Default);

                PromptResult promptResult = await UserDialogs.Instance.PromptAsync(prompt);

                if (promptResult.Ok)
                {

                    // prompt rename audio file 
                    // Create a file, if one doesn't already exist.
                    IFile audioFile = await FileSystem.Current.LocalStorage.GetFileAsync(audioFilePath);

                    await audioFile.RenameAsync(promptResult.Text + ".wav");

                    audioFilePath = audioFile.Path;

                }

                this.capturedMedia = new AudioMediaContent(audioFilePath);

                parentCard?.addMedia(capturedMedia); 
                MediaManager.addNewMedia(capturedMedia);


                stopRecordMode();


            }
            else if (isPlaying)
            {
                AudioPlayerManager.stopPlayAudio();

                stopPlayMode();
            }

            //audioPlayerFinished();
        }

        private void audioPlayerFinished()
        {
            stopPlayMode();
        }

        /// <summary>
        /// Only for starting mode when there is nothing to play yet 
        /// </summary>
        private void recordMode()
        {
            recordButton.IsEnabled = true;
            stopButton.IsEnabled = false;
            playButton.IsEnabled = false;
        }
        private void startRecordMode()
        {
            recordButton.IsEnabled = false;
            stopButton.IsEnabled = true;
            playButton.IsEnabled = false;
            isRecording = true;
        }
        private void stopRecordMode()
        {
            playMode();
            isRecording = false;
        }

        private void playMode()
        {
            recordButton.IsEnabled = true;
            stopButton.IsEnabled = false;
            playButton.IsEnabled = true;
            playButton.Text = "Play";
        }
        private void startPlayMode()
        {
            recordButton.IsEnabled = false;
            stopButton.IsEnabled = true;
            playButton.IsEnabled = true;
            isPlaying = true;
            playButton.Text = "Pause";
        }
        private void stopPlayMode()
        {
            playMode();
            playButton.Text = "Play";
            isPlaying = false;
        }

        public void onDisappear()
        {
            //throw new NotImplementedException();
        }
    }
}