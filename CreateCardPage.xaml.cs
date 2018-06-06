using Cards.Core;
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
	public partial class CreateCardPage : ContentPage, ICardProvider
    {
        //If card opened by media capture view button click then true
        //otherwise card created by "create card" button so false 
        private bool cardAutoCreated = true;
        private bool firstAppearing = true;
        private bool secondAppearing = false; //for auto created 

        Dir parentAlbum;

        public delegate void CardCreated(Dir card);
        public event CardCreated cardCreated; 

        public CreateCardPage ()
		{
			InitializeComponent ();

            initialize(); 
		}

        //Create card page from AlbumsPage 
        public CreateCardPage(CardCategory cardCategory, Dir parentAlbum)
        {
            this.parentAlbum = parentAlbum;

            InitializeComponent();

            if (cardCategory == CardCategory.Event)
            {
                cardTypeSwitch.IsToggled = true;
            }

            initialize();
        }

        public void initialize()
        {
            this.Title = "Create New Card";

            string uniqueCardName = "Card (" + FSManager.getNumCards() + ") " + DateTime.Now.ToString();
            uniqueCardName = uniqueCardName.Replace("/", ".");

            this.cardNameEntry.Text = uniqueCardName;

            this.mediaCaptureView.noCardInitialize(this, this); 

            //this.mediaCaptureView.mediaCapturePreClick += this.createCard; 
        }

        private async void createCardButton_Clicked(object sender, EventArgs e)
        {
            await createCard();

            //card is not auto created because createCardButton was clicked 
            cardAutoCreated = false; 

            await Navigation.PushAsync(new CardPage(cardNameEntry.Text));
        }
        private async void cardNameEntry_Unfocused(object sender, FocusEventArgs e)
        {
            if(!await validateCardNameEntry())
            {
                cardNameEntry.Text = generateDefaultCardName();
            }
        }

        private string generateDefaultCardName()
        {
            string uniqueCardName = "Card (" + FSManager.getNumCards() + ") " + DateTime.Now.ToString();
            uniqueCardName = uniqueCardName.Replace("/", ".");
            return uniqueCardName; 
        }

        private async Task<bool> validateCardNameEntry()
        {
            if (FSManager.cardExists(cardNameEntry.Text))
            {
                await DisplayAlert("Notice", "Card name already exists", "OK");
                return false; 
            }
            else if (cardNameEntry.Text == "")
            {
                await DisplayAlert("Notice", "Card name cannot be blank", "OK");
                return false; 
            }
            return true; 
        }

        protected async override void OnAppearing()
        {
            if (firstAppearing)
            {
                firstAppearing = false;
                base.OnAppearing();
                return; 
            }

            //If card auto created, got to card page 
            if (cardAutoCreated)
            {
                if (!secondAppearing)
                {
                    secondAppearing = true;
                    await Navigation.PushAsync(new CardPage(cardNameEntry.Text));
                }
                else
                    await Navigation.PopAsync();
            }
            else //else we just opened card page so pop 
            {
                await Navigation.PopAsync();
            }

            base.OnAppearing();
        }

        private async Task<Dir> createCard()
        {
            /*
            bool validCardName = await validateCardNameEntry();

            if (!validCardName) cardNameEntry.Text = generateDefaultCardName();
            */

            CardCategory cardCategory = (cardTypeSwitch.IsToggled ? CardCategory.Event : CardCategory.People);

            Dir card = await FSManager.addNewCardAsync(cardNameEntry.Text, cardCategory, this.parentAlbum);

            cardCreated?.Invoke(card); 

            return card;

        }

        /// <summary>
        /// Accessor method to get card.  If this is called it means an external process is wanting us to create the card. 
        /// </summary>
        /// <returns></returns>
        public Task<Dir> getCardAsync()
        {
            cardAutoCreated = true;
            return createCard(); 
        }
    }
}