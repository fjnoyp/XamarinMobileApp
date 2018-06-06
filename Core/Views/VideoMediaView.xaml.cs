using System;
using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions;
using Xamarin.Forms;
using Plugin.Media.Abstractions;
using Cards.Core;
using Plugin.MediaManager.Forms;

namespace Cards.Core.Views
{
	public partial class VideoMediaView : ContentView, IDisappearedListener
	{
        private IPlaybackController PlaybackController => CrossMediaManager.Current.PlaybackController;

        private string mediaFile;

        private bool isPlaying = false; 

        public VideoMediaView()
        {
            InitializeComponent();
        }
        
        public VideoMediaView(AbMediaContent media)
        {
            InitializeComponent();

            //this.image.Source = media.image;

            double temp = videoViewStack.Width;
            double other = videoViewStack.Height; 

            CrossMediaManager.Current.PlayingChanged += (sender, e) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ProgressBar.Progress = e.Progress;
                    //Duration.Text = "" + e.Duration.TotalSeconds + " seconds";
                });
            };

            CrossMediaManager.Current.MediaFinished += (sender, e) =>
            {
                videoControlImage.Source = ImageSource.FromFile("VideoPlayIcon.png");
                isPlaying = false; 
            };

            RaiseChild(videoControlImage); 
            
            this.mediaFile = media.filePath;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

           // double temp = videoViewStack.Width;
           // double other = videoViewStack.Height;
        }

        private void videoControl_Tapped(object sender, EventArgs e)
        {
            if (isPlaying)
            {
                Pause(); 
            }
            else
            {
                Play(); 
            }
        }

        private void play_Clicked(object sender, EventArgs e)
        {
            Play(); 
        }

        void Play()
        {
            this.videoViewStack.Children.Clear();

            this.playImage.IsVisible = false;
            this.playImage.IsEnabled = false;

            VideoView videoView = new VideoView();

            videoView.HeightRequest = videoViewStack.Height;
            videoView.WidthRequest = videoViewStack.Width; 

            videoView.HorizontalOptions = LayoutOptions.FillAndExpand;
            videoView.VerticalOptions = LayoutOptions.FillAndExpand;
            videoView.AspectMode = Plugin.MediaManager.Abstractions.Enums.VideoAspectMode.AspectFit;
            videoView.Source = "file://" + mediaFile;

            videoViewStack.Children.Add(videoView);

            isPlaying = true;
            videoControlImage.Source = ImageSource.FromFile("VideoPauseIcon.png"); 
            PlaybackController.Play();

        }

        void Pause()
        {
            isPlaying = false;
            videoControlImage.Source = ImageSource.FromFile("VideoPlayIcon.png");
            PlaybackController.Pause();
        }

        void Stop()
        {
            isPlaying = false;
            videoControlImage.Source = ImageSource.FromFile("VideoPlayIcon.png");
            PlaybackController.Stop();

            //this.playImage.IsVisible = true;
            //this.playImage.IsEnabled = false;
        }

        private void video_Tapped(object sender, EventArgs e)
        {
            MessagingCenter.Send<Object>(this, "FullScreenRequest");
        }

        public void onDisappear()
        {
        }
    }
}