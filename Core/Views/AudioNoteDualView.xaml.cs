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
	public partial class AudioNoteDualView : ContentView
	{
 
        private Card card;
        private ContentPage parentPage;
        private MediaContentType curMediaType; 

        public AudioNoteDualView ()
		{
			InitializeComponent ();
		}

        public void initialize(Card card, ContentPage parentPage)
        {
            this.card = card;
            this.parentPage = parentPage; 

            this.sortView.initialize(false, this.listView);
            this.headerView.initialize(true, listView);

            headerView.itemClicked += openMediaPreview;
            headerView.itemDoubleClicked += openMediaPreview;
            headerView.headerClicked += openMediaListPage;
            headerView.addClicked += addClicked;

            updateCurMediaType(); 
            changeMediaType(curMediaType);
        }

        private void changeMediaType(MediaContentType mediaType)
        {
            curMediaType = mediaType;
            ObservableCollection<AbMediaContent> curMediaCollection = card.getMediaListByType(curMediaType);

            sortView.updateDataSet(curMediaCollection);
            headerView.initializeSearch(sortView.baseDataSet);
        }

        public async void addClicked()
        {
            switch (curMediaType)
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

        private async void openMediaPreview(ISortableCardItem item)
        {
            AbMediaContent media = item as AbMediaContent;
            await Navigation.PushAsync(new MediaPreviewCarouselPage(media, card, parentPage));
        }

        private async void openMediaListPage()
        {
            await Navigation.PushAsync(new MediaListPage(curMediaType.ToString(), card.getMediaListByType(curMediaType), card));
        }

        private void updateCurMediaType()
        {
            curMediaType = (audioNoteSwitch.IsToggled ? MediaContentType.Audio : MediaContentType.Note);
        }

        void audioNoteSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            updateCurMediaType();
            changeMediaType(curMediaType);
        }
    }
}