using Cards.Core;
using Cards.Core.Platform.Manager;
using DLToolkit.Forms.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Cards.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MediaSelectView : ContentView
    {
        private FlowListView flowListView;
        private List<AbMediaContent> selectedMedia = new List<AbMediaContent>();

        public delegate void Closed();
        public event Closed onClosed;

        public MediaSelectView()
        {
            InitializeComponent();
        }

        public void initialize(FlowListView flowListView)
        {
            this.flowListView = flowListView;
            flowListView.FlowItemTapped += FlowListView_FlowItemTapped;
        }

        private void FlowListView_FlowItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (IsVisible)
            {
                AbMediaContent media = e.Item as AbMediaContent;

                if (media.mediaType == MediaContentType.MediaCount)
                {
                    MediaCountBox mediaBox = media as MediaCountBox;
                    mediaBox.toggleOpen();
                }
                else
                { 
                    selectMedia(media);
                }
            }
        }


        private void disableSelectionIcons()
        {
            shareImage.IsEnabled = false;
            addImage.IsEnabled = false;
            deleteImage.IsEnabled = false;

            shareImage.Opacity = .5;
            addImage.Opacity = .5;
            deleteImage.Opacity = .5; 
        }
        private void enableSelectionIcons()
        {
            shareImage.IsEnabled = true;
            addImage.IsEnabled = true;
            deleteImage.IsEnabled = true;

            shareImage.Opacity = 1;
            addImage.Opacity = 1;
            deleteImage.Opacity = 1;
        }

        public void startSelection()
        {
            this.IsVisible = true; 
            clearSelection(); 
        }
        public void endSelection()
        {
            closeView(); 
        }
        private void clearSelection()
        {
            numSelectedLabel.Text = "Select Media";
            foreach (AbMediaContent media in selectedMedia)
            {
                media.isSelected = false; 
            }
            selectedMedia.Clear();
            disableSelectionIcons(); 
        }

        private void selectMedia(AbMediaContent media)
        {
            if (media.isSelected)
            {
                selectedMedia.Remove(media); 
            }
            else
            {
                selectedMedia.Add(media);
            }
            media.isSelected = !media.isSelected;

            if (selectedMedia.Count == 1) enableSelectionIcons();
            else if (selectedMedia.Count == 0) disableSelectionIcons(); 

            numSelectedLabel.Text = selectedMedia.Count + " selected";
        }

        private void closeView()
        {
            this.IsVisible = false;
            clearSelection();
            onClosed?.Invoke(); 
        }

        private void close_Tapped(object sender, EventArgs e)
        {
            closeView(); 
        }
        private void share_Tapped(object sender, EventArgs e)
        {
            MediaShareManager.shareMedia(selectedMedia); 

            closeView(); 
        }
        private async void add_Tapped(object sender, EventArgs e)
        {
            // Show add to card page 
            AllCardsPage allCardsPage = new AllCardsPage(selectedMedia); 
            EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

            allCardsPage.Disappearing += (s, v) =>
            {
                waitHandle.Set();
            };

            await Navigation.PushAsync(allCardsPage);
            await Task.Run(() => waitHandle.WaitOne());

            closeView();
        }
        private void delete_Tapped(object sender, EventArgs e)
        {
            foreach(AbMediaContent media in selectedMedia)
            {
                MediaManager.deleteMediaAsync(media); 
            }
            closeView(); 
        }
    }
}