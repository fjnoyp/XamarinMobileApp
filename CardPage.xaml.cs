using Cards.Core;
using Cards.Core.FileReaders;
using Cards.Core.Platform.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Cards
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CardPage : ContentPage
    {
        Card card;
        private bool isSelecting = false;

        public CardPage(Card card)
        {
            InitializeComponent();
            this.Title = card.name;

            this.card = card;
            this.flowListView.FlowItemsSource = card.mediaList;

            this.mediaSelectView.initialize(flowListView);
            mediaSelectView.onClosed += endSelect;

            this.mediaControlView.initialize(this, card);

            mediaSelectView.onClosed += endSelect;
        }

        private async void flowListView_FlowItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!isSelecting)
            {
                AbMediaContent curMedia = e.Item as AbMediaContent;
                await Navigation.PushAsync(new CardMediaCarouselPage(curMedia, card.mediaList.ToList<AbMediaContent>(), card));
            }
        }

        private void select_Clicked(object sender, EventArgs e)
        {
            isSelecting = !isSelecting; 

            if (isSelecting)
            {
                selectToolbarItem.Text = "Cancel";
                mediaSelectView.startSelection();
            }
            else
            {
                selectToolbarItem.Text = "Select"; 
                mediaSelectView.endSelection(); 
            }
            
        }

        private void share_Clicked(object sender, EventArgs e)
        {
            //FirebaseUploadManager firebaseManager = new FirebaseUploadManager();
            //firebaseManager.uploadCard(card); 
        }

        private void endSelect()
        {
            isSelecting = false;

            selectToolbarItem.Text = "Select";
        }
    }
}