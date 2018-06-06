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
    public partial class CardsHeaderView : ContentView
    {
        // Search vars
        private ListView cardItemsList;
        private CardItemObservableCollection baseDataSet;

        // Sort and Favorite vars 
        private bool showOnlyFavorites = false;
        private CardItemSorter<ISortableCardItem> cardItemSorter;

        private SortType curSort;

        private bool modTimeAscending = false;
        private bool alphabetAscending = false; 
  

        public CardsHeaderView()
        {
            InitializeComponent();
        }

        public void initialize(ObservableCollection<Card> parentDataSet, ListView cardItemListView)
        {
            this.cardItemsList = cardItemListView;

            this.baseDataSet = new CardItemObservableCollection(parentDataSet);
            this.cardItemSorter = new CardItemSorter<ISortableCardItem>(this.baseDataSet);
            updateShownDataSet();

            alphabetSort_Clicked(null, null);
            modTimeFrame.Opacity = .5;

        }
        public void updateDataSet(ObservableCollection<Card> parentDataSet)
        {
            this.baseDataSet = new CardItemObservableCollection(parentDataSet);
            updateShownDataSet();
        }


        private void showOnlyFavorites_Clicked(object sender, EventArgs e)
        {
            this.showOnlyFavorites = !showOnlyFavorites;
            string iconName = (showOnlyFavorites ? "FavoriteIcon.png" : "NonFavoriteIcon");
            this.showFavoritesImage.Source = ImageSource.FromFile(iconName);

            updateShownDataSet();
        }

        private void updateShownDataSet()
        {

            if (showOnlyFavorites)
            {
                ObservableCollection<ISortableCardItem> favDataSet = new ObservableCollection<ISortableCardItem>(baseDataSet.Where(x => x.isFavorited));

                cardItemSorter.setDataSet(favDataSet); 
                this.cardItemsList.ItemsSource = favDataSet;
            }
            else
            {
                cardItemSorter.setDataSet(baseDataSet); 
                this.cardItemsList.ItemsSource = baseDataSet;
            }
            this.cardItemsList.Focus();
        }

        private void timeSort_Clicked(object sender, EventArgs e)
        {
            if (curSort == SortType.ModTime)
            {
                modTimeAscending = !modTimeAscending;
                modTimeDirImage.Rotation = modTimeAscending ? -90 : 90; 
            }
            else
            {
                curSort = SortType.ModTime;

                alphaSortFrame.Opacity = .5;
                modTimeFrame.Opacity = 1; 
            }

            cardItemSorter.doSort(modTimeAscending, SortType.ModTime);

        }
        private void alphabetSort_Clicked(object sender, EventArgs e)
        {
            if (curSort == SortType.Alphabetically)
            {
                alphabetAscending = !alphabetAscending;
                alphaSortDirImage.Rotation = alphabetAscending ? -90 : 90;
            }
            else
            {
                curSort = SortType.Alphabetically;

                alphaSortFrame.Opacity = 1;
                modTimeFrame.Opacity = .5;
            }

            cardItemSorter.doSort(alphabetAscending, SortType.Alphabetically);

        }

        /*
        private void sort_Clicked(object sender, EventArgs e)
        {
            cardItemSorter.displaySortOptions();
        }
        private void sortDirection_Tapped(object sender, EventArgs e)
        {
            if (cardItemSorter.sortAscending)
                sortDirectionImage.Source = "UpArrowIcon.png";
            else
                sortDirectionImage.Source = "DownArrowIcon.png";

            cardItemSorter.changeSortDirection();
        }
        */

        // Search Functionality ===================================================

        public void initializeSearch(CardItemObservableCollection baseDataSet)
        {
            this.baseDataSet = baseDataSet;
        }

        private void search_Tapped(object sender, ItemTappedEventArgs e)
        {
            searchBar.IsVisible = true; 
        }
        private void searchBack_Tapped(object sender, ItemTappedEventArgs e)
        {
            searchBar.IsVisible = false;
        }

        private void searchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            cardItemsList.BeginRefresh();

            if (string.IsNullOrWhiteSpace(e.NewTextValue))
                cardItemsList.ItemsSource = baseDataSet;
            else
                cardItemsList.ItemsSource = baseDataSet.Where(i => i.name.ToLower().Contains(e.NewTextValue.ToLower()));

            cardItemsList.EndRefresh();
        }
    }
}