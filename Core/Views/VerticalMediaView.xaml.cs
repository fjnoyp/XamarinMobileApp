using Cards.Core.FileReaders;
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
	public partial class VerticalMediaView : ContentView
	{
        private string title;
        private ObservableCollection<AbMediaContent> mediaCollection;
        private Card card;
        private ContentPage parentPage;

        public VerticalMediaView ()
		{
			InitializeComponent ();
		}

        public void initialize(String title, ObservableCollection<AbMediaContent> mediaCollection, Card card, ContentPage parentPage)
        {

            this.sortView.initialize(false, this.listView);
            sortView.updateDataSet(mediaCollection);

            this.headerView.initialize(true, listView);
            headerView.setLabel(title);
            headerView.initializeSearch(sortView.baseDataSet);

            headerView.itemClicked += openMediaPreview;
            headerView.itemDoubleClicked += openMediaPreview;
            headerView.headerClicked += openMediaListPage;
            headerView.addClicked += addClicked; 
        }

        public async void addClicked()
        {
            MediaContentType mediaType;

            if (Enum.TryParse<MediaContentType>(this.title, out mediaType))
            {

                switch (mediaType)
                {
                    case MediaContentType.Note:
                        await MediaCaptureUtilities.takeNote(this.card, this.parentPage);
                        break;
                    case MediaContentType.Audio:
                        await MediaCaptureUtilities.takeAudio(this.card, this.parentPage);
                        break;
                    default: //audio or note not supported 
                        break;
                }
            }

        }

        private async void openMediaPreview(ISortableCardItem item)
        {
            AbMediaContent media = item as AbMediaContent;

            //await Navigation.PushAsync(new MediaPreviewCarouselPage(media, card, parentPage));
        }

        private async void openMediaListPage()
        {
            await Navigation.PushAsync(new MediaListPage(title, mediaCollection, card));
        }
    }
}