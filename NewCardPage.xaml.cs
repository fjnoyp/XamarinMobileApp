using Acr.UserDialogs;
using Cards.Core;
using Cards.Core.Views;
using DLToolkit.Forms.Controls;
using FFImageLoading;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Cards
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NewCardPage : ContentPage
	{

        public FlowObservableCollection<DayMediaObservableCollection> dayMedia { get; set; }

        private bool isSelecting = false;

        public NewCardPage()
        {

            InitializeComponent();

            mediaSelectView.initialize(flowListView);
            mediaSelectView.onClosed += endSelect;

            mediaControlView.initialize(this);

            initialize();
        }

        private async void initialize()
        {

            

            await FSManager.initializeAsync();


            flowListView.FlowItemsSource = MediaManager.allMediaCollection;

            MediaManager.collectionRefresh += () =>
            {
                flowListView.FlowItemsSource = null;
                flowListView.FlowItemsSource = MediaManager.allMediaCollection;
            };
        }

        // TODO perhaps move behavior to DayMediaObservableCollection 
        private async void flowListView_FlowItemTapped(object sender, ItemTappedEventArgs e)
        {

            AbMediaContent media = e.Item as AbMediaContent; 

            if (!isSelecting) {

                if (media.mediaType == MediaContentType.MediaCount)
                {
                    MediaCountBox mediaBox = media as MediaCountBox;
                    mediaBox.toggleOpen(); 
                }
                else // NOTE: reusing CardMediaCarouselPage will break disappear logic in MediaDisplayView 
                    await Navigation.PushAsync(new CardMediaCarouselPage(media, MediaManager.allMedia));
                
            }

        }

        private void select_Clicked(object sender, EventArgs e)
        {
            isSelecting = !isSelecting; 

            if (isSelecting)
            {
                mediaSelectView.startSelection();
                selectButton.Text = "Cancel";
                viewCardsButton.IsEnabled = false; 

            }
            else
            {
                mediaSelectView.endSelection(); 
                selectButton.Text = "Select";
                viewCardsButton.IsEnabled = true; 
            }
        }
        // Called by MediaSelectView OnClosed event 
        private void endSelect()
        {
            isSelecting = false;

            selectButton.Text = "Select";
            viewCardsButton.IsEnabled = true;
        }

        private async void viewCards_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AllCardsPage());
        }

        private async void date_Clicked(object sender, EventArgs e)
        {
            DatePromptConfig prompt = new DatePromptConfig();
            prompt.MaximumDate = DateTime.Today;
            prompt.SelectedDate = DateTime.Today; 

            var result = await UserDialogs.Instance.DatePromptAsync(prompt);


            if (prompt.SelectedDate != null)
            {
                DateTime date = (DateTime)result.SelectedDate;
                DayMediaObservableCollection scrollToItem;

                MediaManager.allMediaDict.TryGetValue(date, out scrollToItem); 

                //Get a date after the time selected 
                if(scrollToItem == null)
                {
                    //Dates goes from smallest to largest (latest to earliest chronologically ...)
                    List<DateTime> dates = MediaManager.allMediaDict.Keys.ToList();

                    if (date > dates[dates.Count - 1]) date = dates[dates.Count - 1];
                    else if (date < dates[0]) date = dates[0];

                    for (int i = 0; i <= dates.Count; i++)
                    {
                        if(date <= dates[i])
                        {
                            date = dates[i];
                            break; 
                        }
                    }

                    scrollToItem = MediaManager.allMediaDict[date]; 
                }


flowListView.FlowScrollTo(scrollToItem.getFirstItem(), ScrollToPosition.Center, false);
            }
        }

        
        /*
       //Calculate row and column dimensions to be squares for flow list view 
       private bool hasAppeared = false; 
       protected override void OnAppearing()
       {
           base.OnAppearing();

           if (!hasAppeared)
           {
               //NOTE: If the screen happens to be very WIDE, this calculation might result in images that are too wide and not tall enough 
               hasAppeared = true;

               double width = flowListView.Width < flowListView.Height ? flowListView.Width : flowListView.Height;

               flowListView.FlowColumnCount = 3; 
               flowListView.FlowColumnMinWidth = width / 3.0;
               flowListView.RowHeight = (int)(width / 3.0);
               flowListView.ForceReload();

           }  
       }
       */

    }
}