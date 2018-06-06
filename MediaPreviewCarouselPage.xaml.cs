using Cards.Core;
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
	public partial class MediaPreviewCarouselPage : ContentPage
	{
        Dir card;
        ObservableCollection<CardMediaViewModel> cardMediaViewModels;


		public MediaPreviewCarouselPage (AbMediaContent startingMedia, Dir card, ContentPage parentPage)
		{
			InitializeComponent ();

            this.card = card;
            this.cardMediaViewModels = new ObservableCollection<CardMediaViewModel>();


            ObservableCollection<AbMediaContent> mediaList = card.getMediaListByType(startingMedia.mediaType); 
            
            foreach(AbMediaContent media in mediaList)
            {
                cardMediaViewModels.Add(new CardMediaViewModel() { media = media, card = card, parentPage = parentPage}); 

            }
            this.carouselView.ItemsSource = cardMediaViewModels;

            this.carouselView.Position = mediaList.IndexOf(startingMedia);

            this.carouselView.PositionSelected += carouselView_PositionSelected;
		}

        private void carouselView_PositionSelected(object sender, PositionSelectedEventArgs e)
        {
            AbMediaContent curMedia = cardMediaViewModels[carouselView.Position].media;

            isFavoriteBarItem.Icon = (curMedia.isFavorited ? "FavoriteIcon.png" : "NonFavoriteIcon");
        }

        private async void addLink_Clicked(object sender, EventArgs e)
        {
            AbMediaContent curMedia = cardMediaViewModels[carouselView.Position].media;

            var action = await DisplayActionSheet("Choose media type to link to", "Cancel", null, "Image", "Video", "Audio", "Note");

            if (action != "Cancel")
            {
                MediaContentType mediaType;
                Enum.TryParse<MediaContentType>(action, out mediaType);

                //await Navigation.PushAsync(new LinkedMediaListPage(mediaType, curMedia, card));
            }
        }

        private void delete_Clicked(object sender, EventArgs e)
        {
            AbMediaContent curMedia = cardMediaViewModels[carouselView.Position].media;

            //TODO if card is null delete media async else just delete from the card 
            this.card.removeMedia(curMedia);
            MediaManager.deleteMediaAsync(curMedia);

            cardMediaViewModels.RemoveAt(carouselView.Position);             
        }

        private void isFavoriteBarItem_Clicked(object sender, EventArgs e)
        {
            AbMediaContent curMedia = cardMediaViewModels[carouselView.Position].media;

            curMedia.isFavorited = !curMedia.isFavorited;
            isFavoriteBarItem.Icon = (curMedia.isFavorited ? "FavoriteIcon.png" : "NonFavoriteIcon");

            //save media changes to its parent card 
            this.card.saveToFile();
        }
    }
}