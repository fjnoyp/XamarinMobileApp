using Cards.Core.FileReaders;
using Cards.Core.ObservableCollection;
using Cards.Core.Platform.Manager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Cards.Core.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MediaPreviewView : ContentView
	{
        Card card; 
        AbMediaContent media;

        CardMediaCarouselPage parentCarouselPage;

        ObservableCollection<AbMediaContent> linkedMedia;

        bool isFullScreenMode = true;  

        public MediaPreviewView()
        {
            InitializeComponent();
            linkedMediaView.listView.ItemTapped += ListView_ItemTapped;

            MessagingCenter.Subscribe<Object>(this, "FullScreenRequest", (sender) =>
            {
                //toggleOverlays();
            });
        }

        // TODO need to make toggle overlays called on item changed (OR move code to carousel view) 
        public void toggleOverlays()
        {
            isFullScreenMode = !isFullScreenMode;

            if (isFullScreenMode)
            {
                determineLinkedMediaVisible(); 
            }
            else
            {
                hideLinkedMedia(); 
            }
        }

        public void initialize(AbMediaContent media, Card card, Page parentPage)
        {
            this.media = media;
            this.parentCarouselPage = parentPage as CardMediaCarouselPage;

            if (linkedMedia != null) linkedMedia.CollectionChanged -= linkedMedia_CollectionChanged; 

            if(card != null)
            {
                linkedMedia = card.linkManager.getLinkedMedia(media);
                this.linkedMediaView.initialize(new LinkedMediaObservableCollection(linkedMedia));
                this.card = card; 
            }
            else
            {
                linkedMedia = MediaManager.mediaLinkManager.getLinkedMedia(media);
                this.linkedMediaView.initialize(new LinkedMediaObservableCollection(linkedMedia));
            }

            linkedMedia.CollectionChanged += linkedMedia_CollectionChanged;
            determineLinkedMediaVisible();

            this.mediaDisplayView.initialize(parentPage); 
            this.mediaDisplayView.setMedia(true, media);
        }

        private void linkedMedia_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                determineLinkedMediaVisible(); 
            }
        }

        private void determineLinkedMediaVisible()
        {
            if (linkedMediaView.count == 0)
            {
                hideLinkedMedia();               
            }
            else
            {
                showLinkedMedia();
            }
        }
        private void showLinkedMedia()
        {
            linkedMediaView.IsVisible = true;
            AbsoluteLayout.SetLayoutBounds(mediaDisplayView, new Rectangle(0, 1, 1, .9));
        }
        private void hideLinkedMedia()
        {
            linkedMediaView.IsVisible = false;
            AbsoluteLayout.SetLayoutBounds(mediaDisplayView, new Rectangle(0, 0, 1, 1));
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            AbMediaContent media = e.Item as AbMediaContent;

            // If clicked media is a card open card page 
            if(media.mediaType == MediaContentType.Card)
            {
                    Card card = FSManager.getCard(media.name);
                    await Navigation.PushAsync(new CardPage(card));
                    return;
            }

            // If clicked media is media box toggle 
            if(media.mediaType == MediaContentType.MediaCount)
            {
                MediaCountBox mediaBox = media as MediaCountBox;
                mediaBox.toggleOpen();
                return; 
            }

            if(card != null)
            {
                // TODO temp this should just have the card's media 
                await Navigation.PushAsync(new CardMediaCarouselPage(media, MediaManager.allMedia));
            }
            else
            {
                await Navigation.PushAsync(new CardMediaCarouselPage(media, MediaManager.allMedia)); 
            }
          
        }

        protected override void OnBindingContextChanged()
        {
            CardMediaViewModel cardModel = BindingContext as CardMediaViewModel;

            initialize(cardModel.media, cardModel.card, cardModel.parentPage);


            base.OnBindingContextChanged();
        }

        private void media_Tapped(object sender, EventArgs e)
        {
             if(this.media.mediaType == MediaContentType.Image)
            {
                parentCarouselPage.toggleOverlays(); 
            }
        }

    }
}