using Cards.Core;
using Cards.Core.FileReaders;
using Cards.Core.Platform.Manager;
using Cards.Core.Views;
using CarouselView.FormsPlugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Cards
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CardMediaCarouselPage : ContentPage
    {
        Card card;
        ObservableCollection<CardMediaViewModel> cardMediaViewModels;

        
        /// <param name="cardMediaList"> Card media to display, we aren't always including all the card media, hence this list </param>
        public CardMediaCarouselPage(AbMediaContent startingMedia, List<AbMediaContent> cardMediaList, Card card = null )
        {
            InitializeComponent();

            this.card = card;
            this.cardMediaViewModels = new ObservableCollection<CardMediaViewModel>();

            foreach (AbMediaContent media in cardMediaList)
            {
                cardMediaViewModels.Add(new CardMediaViewModel() { media = media, card = card, parentPage = this });

            }
            this.carouselView.ItemsSource = cardMediaViewModels;

            this.carouselView.Position = cardMediaList.IndexOf(startingMedia);
            this.carouselView.PositionSelected += carouselView_PositionSelected;

            mediaLinkCaptureView.initialize(this, card);

            MessagingCenter.Subscribe<Object>(this, "FullScreenRequest", (sender) =>
            {
                toggleOverlays(); 
            });

        }

        private void carouselView_PositionSelected(object sender, PositionSelectedEventArgs e)
        {
            AbMediaContent curMedia = cardMediaViewModels[carouselView.Position].media;
            isFavoritedImage.Source = (curMedia.isFavorited ? "FavoriteIcon.png" : "NonFavoriteIcon");

            mediaLinkCaptureView.setLinkToMedia(curMedia); 

            
        }

        public void toggleOverlays()
        {
            if (overlayVisible()) hideOverlays();
            else showOverlay(); 
        }
        public bool overlayVisible()
        {
            return mediaLinkToolbar.IsVisible || linkChoiceToolbar.IsVisible || mainToolbar.IsVisible; 
        }

        private void hideOverlays()
        {
            if (mainToolbar.IsVisible) toggleToolbarVisible(false, mainToolbar); 
            if (mediaLinkToolbar.IsVisible) toggleToolbarVisible(false, mediaLinkToolbar);
            if (linkChoiceToolbar.IsVisible) toggleToolbarVisible(false, linkChoiceToolbar); 
        }
        private void showOverlay()
        {
            if (!mainToolbar.IsVisible) toggleToolbarVisible(true, mainToolbar); 
        }

        private async void toggleToolbarVisible(bool show, VisualElement view, double moveFactor = 1)
        {
            //If element hidden before, show so that animation is visible 
            if (show) view.IsVisible = show; 

            int showFactor = show ? -1 : 1;
            double height = moveFactor * showFactor * view.Height;
            await Xamarin.Forms.ViewExtensions.TranslateTo(view, 0, height, 250, Easing.CubicOut);
            view.IsVisible = show; 
        }

        private void add_Clicked(object sender, EventArgs e)
        {
            
            if (mediaLinkToolbar.IsVisible)
            {
                toggleToolbarVisible(false, mediaLinkToolbar); 
            }
            else // toggle link choice visibility 
            {
                toggleToolbarVisible(!linkChoiceToolbar.IsVisible, linkChoiceToolbar); 
            }


        }

        private void addLink_Clicked(object sender, EventArgs e)
        {
            // Hide choice toolbar 
            toggleToolbarVisible(false, linkChoiceToolbar);

            // Show media link toolbar 
            toggleToolbarVisible(true, mediaLinkToolbar); 
        }

        private async void addToCard_Clicked(object sender, EventArgs e)
        {
            // Hide choice toolbar 
            toggleToolbarVisible(false, linkChoiceToolbar);

            // Show add to card page 
            await Navigation.PushAsync(new AllCardsPage(getCurMedia(), card)); 
        }

        private void delete_Clicked(object sender, EventArgs e)
        {
            toggleToolbarVisible(true, deleteConfirmationBar, .5);
        }
        private void confirmDelete_Clicked(object sender, EventArgs e)
        {
            AbMediaContent curMedia = getCurMedia();

            if (card == null)
            {
                MediaManager.deleteMediaAsync(curMedia);
            }
            else
            {
                this.card.removeMedia(curMedia);
            }

            cardMediaViewModels.RemoveAt(carouselView.Position);

            toggleToolbarVisible(false, deleteConfirmationBar, .5); 
        }
        private void cancelDelete_Clicked(object sender, EventArgs e)
        {
            toggleToolbarVisible(false, deleteConfirmationBar, .5); 
        }

        private void share_Clicked(object sender, EventArgs e)
        {
            MediaShareManager.shareMedia( getCurMedia() ); 
        }

        private async void gallery_Clicked(object sender, EventArgs e)
        {
            if(card != null)
            {
                await Navigation.PushAsync(new LinkedMediaListPage(getCurMedia(), card.linkManager, card));
            }
            else
            {
                await Navigation.PushAsync(new LinkedMediaListPage(getCurMedia(), MediaManager.mediaLinkManager, null));
            }


        }

        public AbMediaContent getCurMedia()
        {
            return cardMediaViewModels[carouselView.Position].media;
        }

        private void isFavorite_Clicked(object sender, EventArgs e)
        {
            AbMediaContent curMedia = cardMediaViewModels[carouselView.Position].media;

            MediaManager.favoriteManager.setFavorite(!curMedia.isFavorited, curMedia);
            MediaManager.saveFavoriteManager(); 

            isFavoritedImage.Source = (curMedia.isFavorited ? "FavoriteIcon.png" : "NonFavoriteIcon");

            //save media changes to its parent card 
            card?.saveToFile();
        }


        // Deal with main toolbar overlay showing on appearing ( showOverlay was not working in constructor )
        bool firstAppearing = true; 
        protected override void OnAppearing()
        {

            base.OnAppearing();

            if (firstAppearing)
            {
                showOverlay(); 
                firstAppearing = false; 
            }
           // this.Animate("", (s) => Layout(new Rectangle(X, (1 - s) * Height, Width, Height)), 0, 600, Easing.SpringIn, null, null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();


           //this.Animate("", (s) => Layout(new Rectangle(X, (s - 1) * Height, Width, Height)), 0, 600, Easing.SpringIn, null, null);
        }
    }


}