using Cards.Core.FileReaders;
using Cards.Core.Platform;
using Cards.Core.Platform.Manager;
using Plugin.Media;
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
    public partial class MediaCaptureView : ContentView
    {
        private Card card;
        private ContentPage parentPage;

        private  ICardProvider cardProvider; 

        public MediaCaptureView()
        {
            InitializeComponent();
        }

        public MediaCaptureView(String cardName, ContentPage parentPage)
        {
            InitializeComponent();

            this.initialize(cardName, parentPage);
        }
        public void initialize(String cardName, ContentPage parentPage)
        {
            this.card = FSManager.getCard(cardName);
            this.parentPage = parentPage;
        }
        public void noCardInitialize(ICardProvider cardProvider, ContentPage parentPage)
        {
            this.cardProvider = cardProvider;
            this.parentPage = parentPage; 
        }
    
        public void setCard(Card card)
        {
            this.card = card; 
        }

        private async void takePictureButton_Clicked(object sender, EventArgs e)
        {
            //mediaCapturePreClick?.Invoke();
            if(cardProvider!=null)
            this.card = await cardProvider.getCardAsync();

            await MediaCaptureUtilities.takePicture(card, parentPage);
        }

        private async void takeVideoButton_Clicked(object sender, EventArgs e)
        {
            if (cardProvider != null)
                this.card = await cardProvider.getCardAsync();

            await MediaCaptureUtilities.takeVideo(card, parentPage);
        }

        private async void takeAudioButton_Clicked(object sender, EventArgs e)
        {
            if (cardProvider != null)
                this.card = await cardProvider.getCardAsync();

            await MediaCaptureUtilities.takeAudio(card, parentPage); 
        }

        private async void takeNoteButton_Clicked(object sender, EventArgs e)
        {
            if (cardProvider != null)
                this.card = await cardProvider.getCardAsync();

            await MediaCaptureUtilities.takeNote(card, parentPage); 

        }

    }
}