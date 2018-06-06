using Acr.UserDialogs;
using Cards.Core;
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
    public partial class AlbumsPage : ContentPage
    {
        private ObservableCollection<Dir> albumsToDisplay;
        private ObservableCollection<Dir> cardsToDisplay;

        private CardCategory cardCategory;

        private static readonly Dictionary<CardCategory, string> specialAlbumNames
    = new Dictionary<CardCategory, string>
{
    { CardCategory.Event, "All Event Cards" },
    { CardCategory.People, "All People Cards" },
        { CardCategory.Private, "All Private Cards" }
    };

        //For xamarin forms previewer 
        public AlbumsPage()
        {
            InitializeComponent();
        }

        public AlbumsPage(CardCategory cardCategory)
        {
            InitializeComponent();

            this.cardCategory = cardCategory;


            //Get albums to display 
            string specialAlbumName;
            specialAlbumNames.TryGetValue(cardCategory, out specialAlbumName);
            albumsToDisplay = new ObservableCollection<Dir>(FSManager.getAllAlbums());
            albumsToDisplay.Insert(0, FSManager.createDummyDir(specialAlbumName));

            this.Title = specialAlbumName; 

            //Get cards to display 
            cardsToDisplay = new ObservableCollection<Dir>(FSManager.getAllSectionCards(cardCategory));


            //Setup views 
            this.allCardsView.initialize("Cards", cardsToDisplay);
            this.allAlbumsView.initialize("Albums", albumsToDisplay);

            //Album sort view popup 
            allAlbumsView.headerView.addClicked += createAlbum;
            allAlbumsView.headerView.itemClicked += switchParentAlbum;
            allAlbumsView.headerView.itemDoubleClicked += (e) =>
            {
                albumsView.IsVisible = false;
            };

            //Code for registering click away from album sort view popup 
            //allAlbumsView.Unfocused += (e, v) => { albumsView.IsVisible = false; }; 

            allCardsView.sortView.sortClicked += () => { albumsView.IsVisible = false; };
            allCardsView.headerView.itemClicked += (e) => { albumsView.IsVisible = false; };


            //Card sort view 
            allCardsView.headerView.addClicked += createCard;
            allCardsView.headerView.itemDoubleClicked += openCard;
        }

        //Need to refresh card data set in case it changed (card deleted, etc.) 
        //Albums can only change in this page so no update necessary ... 
        protected override void OnAppearing()
        {
            Dir album = allAlbumsView.headerView.selectedItem as Dir;
            if (album == null)
                switchParentAlbum(null);
            else
                switchParentAlbum(album); 
            base.OnAppearing();
        }

        private async void createAlbum()
        {
            PromptConfig prompt = new PromptConfig();
            prompt.SetCancelText("Cancel");
            prompt.SetOkText("Create");
            prompt.SetMessage("Create New Album");
            prompt.SetInputMode(InputType.Default);

            PromptResult promptResult = await UserDialogs.Instance.PromptAsync(prompt);

            if (!promptResult.Ok) return;

            if (FSManager.albumExists(promptResult.Text))
                await DisplayAlert("Notice", "Album name already exists", "OK");
            else
            {
                Dir newAlbum = await FSManager.addNewAlbumAsync(promptResult.Text);
                this.albumsToDisplay.Add(newAlbum);
            }
        }

        private void deleteAlbum(ISortableCardItem cardItem)
        {
            Dir album = cardItem as Dir; 
            if (specialAlbumNames.ContainsValue(album.name)) return;
            FSManager.deleteAlbumAsync(album.name);
            albumsToDisplay.Remove(album);
        }

        private void switchParentAlbum(ISortableCardItem item)
        {
           
            if (item == null || specialAlbumNames.ContainsValue(item.name)) setToAllCategoryCards();
            else
            {
                string albumName = item.name;

                //On album switch update shown cards 
                Dir album = FSManager.getAlbum(albumName); 
                this.cardsToDisplay = new ObservableCollection<Dir>(album.childCardDirs);
                this.allCardsView.sortView.updateDataSet(cardsToDisplay);
                this.Title = albumName;

                //Update isFavorited icon
                isFavoriteBarItem.Icon = (album.isFavorited ? "FavoriteIcon.png" : "NonFavoriteIcon");
            }

            


        }

        private async void createCard()
        {
            Dir parentAlbum = allAlbumsView.headerView.selectedItem as Dir; 

            //special album is dummy album that does not exist!
            if (parentAlbum != null && specialAlbumNames.Values.Contains(parentAlbum.name)) parentAlbum = null;

            CreateCardPage createCardPage = new CreateCardPage(cardCategory, parentAlbum);
            /*
            createCardPage.cardCreated += (Dir card) =>
            {
                this.cardsToDisplay.Add(card); 
            };
            */
            await Navigation.PushAsync(createCardPage); 

        }

        private void deleteCard(ISortableCardItem cardItem)
        {
            Dir card = cardItem as Dir; 
            FSManager.deleteCardAsync(card.name);
            cardsToDisplay.Remove(card);
        }

        private async void openCard(ISortableCardItem item)
        {
            await Navigation.PushAsync(new CardPage(item.name));
        }


        public void setToAllCategoryCards()
        {
            this.cardsToDisplay = new ObservableCollection<Dir>(FSManager.getAllSectionCards(this.cardCategory));
            this.allCardsView.sortView.updateDataSet(cardsToDisplay);
            this.Title = specialAlbumNames[this.cardCategory];
        }

        private void toggleAlbums_Clicked(object sender, EventArgs e)
        {
            this.albumsView.IsVisible = !this.albumsView.IsVisible;
            if (albumsView.IsVisible) albumsView.Focus(); 
        }

        private void allCardsView_Tapped(object sender, EventArgs e)
        {
            if (this.albumsView.IsVisible)
                this.albumsView.IsVisible = false; 
        }

        //Favoriting of current album 
        private void isFavoriteBarItem_Clicked(object sender, EventArgs e)
        {
            Dir curAlbum = allAlbumsView.headerView.selectedItem as Dir;

            if (curAlbum == null) return;

            curAlbum.isFavorited = !curAlbum.isFavorited;
            isFavoriteBarItem.Icon = (curAlbum.isFavorited ? "FavoriteIcon.png" : "NonFavoriteIcon");

            //If not special album save to file 
            if (!specialAlbumNames.Values.Contains(curAlbum.name))
            {
                curAlbum.saveToFile(); 
            }

        }
    }
}