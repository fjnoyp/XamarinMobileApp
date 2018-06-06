using Acr.UserDialogs;
using Cards.Core;
using Cards.Core.FileReaders;
using Cards.Core.Views;
using FFImageLoading.Forms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Cards
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllCardsPage : ContentPage
    {

        ObservableCollection<Card> allCardsToDisplay;

        List<AbMediaContent> mediasToAdd;
        Card mediasToAddCard; 

        public AllCardsPage(AbMediaContent mediaToAdd, Card card = null) : this()
        {
            this.mediasToAddCard = card; 
            this.mediasToAdd = new List<AbMediaContent>() { mediaToAdd }; 
        }
        public AllCardsPage(List<AbMediaContent> mediasToAdd, Card card = null) : this()
        {
            this.mediasToAddCard = card; 
            this.mediasToAdd = mediasToAdd; 
        }

        public AllCardsPage()
        {
            InitializeComponent();

            allCardsToDisplay = new ObservableCollection<Card>(FSManager.getAllCards());

            cardsListView.ItemsSource = allCardsToDisplay; 
            cardsHeaderView.initialize(allCardsToDisplay, cardsListView); 
        }

        /// <summary>
        /// Refresh data set
        /// </summary>
        protected override void OnAppearing()
        {
            allCardsToDisplay = new ObservableCollection<Card>(FSManager.getAllCards());
            cardsHeaderView.updateDataSet(allCardsToDisplay);         

            base.OnAppearing();
        }

        private async void createCard_Clicked(object sender, EventArgs e)
        {
            PromptConfig prompt = new PromptConfig();
            prompt.SetCancelText("Cancel");
            prompt.SetOkText("Create");
            prompt.SetMessage("Name Card");
            prompt.SetInputMode(InputType.Default);

            PromptResult promptResult = await UserDialogs.Instance.PromptAsync(prompt);

            if (promptResult.Ok)
            {
                Card newCard = await FSManager.addNewCardAsync(promptResult.Text);

                allCardsToDisplay.Add(newCard);

                if (mediasToAdd != null)
                    addMediaToCard(newCard); 
                
            }

            // Resort cards
            allCardsToDisplay = new ObservableCollection<Card>(FSManager.getAllCards());
            cardsHeaderView.updateDataSet(allCardsToDisplay);
        }

        private async void cardsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            openCard(e.Item as Card); 
        }

        private async void openCard(ISortableCardItem item)
        {
            Card card = FSManager.getCard(item.name);

            if (card != null)
            {
                if (mediasToAdd != null)
                    addMediaToCard(card); 
                
                else
                {
                    await Navigation.PushAsync(new CardPage(card));
                }
            }
        }

        /// <summary>
        /// Add media to card and link card to media 
        /// </summary>
        /// <param name="card"></param>
        private async void addMediaToCard(Card card)
        {
            LinkManager linkManager; 
            if (mediasToAddCard != null)
                linkManager = mediasToAddCard.linkManager; 
            else
                linkManager = MediaManager.mediaLinkManager; 
            
            CardMediaContent cardMediaContent = new CardMediaContent(card.name); 

            foreach (AbMediaContent media in mediasToAdd)
            {
                card.addMedia(media);
                linkManager.addLink(media, cardMediaContent); 
            }

            if (mediasToAddCard != null)
                mediasToAddCard.saveToFile();
            else
                MediaManager.saveMediaLinkManager();


            EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            CardPage cardPage = new CardPage(card);

            cardPage.Disappearing += (e, v) =>
            {
                waitHandle.Set();
            };

            await Navigation.PushAsync(cardPage);
            await Task.Run(() => waitHandle.WaitOne());

            // TODO fix issue, crashes only in IOS not android ... 
            //await Navigation.PopAsync();
            mediasToAdd = null; 
        }


    }
}