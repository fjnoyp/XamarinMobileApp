using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Cards.Core;
using Cards.Core.FileReaders;

namespace Cards.Core.Views
{

    /// <summary>
    /// Scrolling horizontal list view display of media 
    /// All encompassing : 
    /// Handles title, sort, click events, etc. 
    /// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HorizontalMediaView : ContentView
	{
        private ObservableCollection<AbMediaContent> mediaCollection;
        private Card card;
        private ContentPage parentPage;

        private string title; 
        
        public HorizontalMediaView()
        {
            InitializeComponent();

            headerView.itemClicked += openMediaPreview;
            headerView.itemDoubleClicked += openMediaPreview;
            headerView.headerClicked += openMediaListPage;
            headerView.addClicked += addClicked;
        }

        public void initialize(String title, ObservableCollection<AbMediaContent> mediaCollection, Card card, ContentPage parentPage) {

            this.title = title;
            this.card = card;
            this.mediaCollection = mediaCollection;

            //initialize horizontal listview 
            this.horizontalListView.initialize(mediaCollection); 
            ListView listView = this.horizontalListView.listView; 

            //initialize header view 
            this.headerView.initialize(false, listView);
            headerView.setLabel(title);
            

            //initialize sort view 
            sortView.initialize(true, listView);
            sortView.updateDataSet(mediaCollection); 

            this.parentPage = parentPage; 

        }

        public async void addClicked()
        {
            MediaContentType mediaType;

            if (Enum.TryParse<MediaContentType>(this.title, out mediaType) ) { 

                switch (mediaType)
                {
                    case MediaContentType.Image:
                        await MediaCaptureUtilities.takePicture(this.card, this.parentPage); 
                        break;
                    case MediaContentType.Video:
                        await MediaCaptureUtilities.takeVideo(this.card, this.parentPage); 
                        break;
                    default: //audio or note not supported 
                        break; 
                }
            }

        }

        private async void openMediaPreview(ISortableCardItem item)
        {
            headerView.IsEnabled = false; 

            AbMediaContent media = item as AbMediaContent;

            //await Navigation.PushAsync(new MediaPreviewCarouselPage(media, card, parentPage));

            headerView.IsEnabled = true; 

        }

        private async void openMediaListPage()
        {
            await Navigation.PushAsync(new MediaListPage(title, mediaCollection, card)); 
        }

    }
}