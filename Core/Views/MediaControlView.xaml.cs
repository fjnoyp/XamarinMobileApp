using Cards.Core.FileReaders;
using DLToolkit.Forms.Controls;
using FFImageLoading;
using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Cards.Core.Views
{
    /// <summary>
    /// Functionality:
    /// 1) plus tapped + camera,video,etc. tapped => open media capture 
    /// 2) camera,video,etc. tapped => filter shown media 
    /// Objects Needed:
    /// 1) card for storing media to and getting media to display
    /// 2) listview to update displayed data source 
    /// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MediaControlView : ContentView
    {
        protected Card card;
        protected Page parentPage;


        public MediaControlView()
        {
            InitializeComponent();
        }

        public virtual void initialize(Page parentPage, Card card = null)
        {
            this.card = card;
            this.parentPage = parentPage;
        }

        private async void camera_Tapped(object sender, EventArgs e)
        {
            await handleImageTapped(MediaContentType.Image);
        }

        private async void video_Tapped(object sender, EventArgs e)
        {
            await handleImageTapped(MediaContentType.Video);
        }

        private async void audio_Tapped(object sender, EventArgs e)
        {
            await handleImageTapped(MediaContentType.Audio);
        }

        private async void note_Tapped(object sender, EventArgs e)
        {
            await handleImageTapped(MediaContentType.Note);
        }

        private async void contact_Tapped(object sender, EventArgs e)
        {
            await handleImageTapped(MediaContentType.Contact); 
        }

        protected virtual async Task handleImageTapped(MediaContentType mediaType)
        {
            await MediaCaptureUtilities.takeMedia(mediaType, card, parentPage);
        }

    }
}