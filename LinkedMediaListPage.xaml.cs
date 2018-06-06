using Cards.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Threading;
using Cards.Core.FileReaders;

namespace Cards
{
    /// <summary>
    /// Display list of same media type we can link to and have linked to 
    /// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LinkedMediaListPage : ContentPage
	{
        private AbMediaContent rootMedia;
        private Card card; 
        private LinkManager linkManager;

        private List<AbMediaContent> selectedMedia; //= new List<AbMediaContent>();

        private bool mediaSelectPageOpen = false; 

        public LinkedMediaListPage(AbMediaContent rootMedia, LinkManager linkManager, Card card = null)
        {
            InitializeComponent();

            this.rootMedia = rootMedia;
            this.linkManager = linkManager;
            this.card = card;

            flowListView.FlowItemsSource = MediaManager.allMediaCollection;

            selectedMedia = new List<AbMediaContent>();
            List<AbMediaContent> allMedia = MediaManager.allMedia; 

            foreach(AbMediaContent media in linkManager.getLinkedMedia(rootMedia).ToList<AbMediaContent>())
            {
                // Ignore linked cards for linked media 
                if (media.mediaType != MediaContentType.Card)
                {
                    //Unfortunately these AbMediaContent instances are different than existing AbMediaContent instances 
                    int index = allMedia.IndexOf(media);
                    selectedMedia.Add(allMedia[index]);

                    allMedia[index].isSelected = true;
                }
            }
        }

        private async void flowListView_FlowItemTapped(object sender, ItemTappedEventArgs e)
        {
            AbMediaContent media = e.Item as AbMediaContent;

            if (media.mediaType == MediaContentType.MediaCount)
            {
                MediaCountBox mediaBox = media as MediaCountBox;
                mediaBox.toggleOpen();
            }
            else
            {
                
                mediaSelectPageOpen = true;
                bool mediaPrevSelected = media.isSelected;

                // Use wait handle so code only runs after MediaSelectPage disappears 
                MediaSelectPage selectPage = new MediaSelectPage(media); 
                EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

                selectPage.Disappearing += (s, v) =>
                {
                    waitHandle.Set();
                };

                await Navigation.PushAsync(selectPage);
                await Task.Run(() => waitHandle.WaitOne());

                if(media.isSelected != mediaPrevSelected)
                {
                    toggleLinkMedia(media);
                    await Navigation.PopAsync(); 
                }

            }
        }

        
        private void toggleLinkMedia(AbMediaContent media)
        {
           
            if (media.isSelected)
            {
                selectedMedia.Add(media);
                linkManager.addLink(rootMedia, media); 
            }
            else
            {
                selectedMedia.Remove(media);
                linkManager.removeLink(rootMedia, media); 
            }
        }
        

        protected override void OnDisappearing()
        {
            if (mediaSelectPageOpen)
            {
                mediaSelectPageOpen = false; 
            }
            else
            {
                if (card == null)
                {
                    MediaManager.saveMediaLinkManager();
                }
                else
                {
                    card.saveToFile();
                }

                // Clear selection 
                foreach (AbMediaContent media in selectedMedia)
                {
                    media.isSelected = false;
                }
            }
        }

    }

    /*
    public class MediaLinkContent
    {
        public AbMediaContent parentMedia { get; set; }

        public Xamarin.Forms.ImageSource image { get { return parentMedia.image; } }
        public virtual String text { get { return parentMedia.name; } }
        public Color backgroundColor { get; set; }

        public MediaLinkContent(AbMediaContent parentMedia)
        {
            this.parentMedia = parentMedia;
        }
    }
    */
}
