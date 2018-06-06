using Cards.Core;
using Cards.Core.FileReaders;
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
	public partial class MediaListPage : ContentPage
	{
        private Card card; 

        public MediaListPage ()
		{
			InitializeComponent ();
		}

        public MediaListPage(String title, ObservableCollection<AbMediaContent> mediaCollection, Card card)
        {
            InitializeComponent();

            initialize(title, mediaCollection, card); 
        }

        public void initialize(String title, ObservableCollection<AbMediaContent> mediaCollection, Card card)
        {
            this.card = card;

            this.flowListView.FlowItemsSource = mediaCollection;

        }

        private async void flowListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            AbMediaContent media = e.Item as AbMediaContent;

                //await Navigation.PushAsync(new MediaPreviewCarouselPage(media, card, this));
        }
    }
}