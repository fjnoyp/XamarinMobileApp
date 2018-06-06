using Cards.Core.FileReaders;
using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Cards.Core.Views.CardItemHeaderView;

namespace Cards.Core.Views
{
    /// <summary>
    /// Manages header functionality for a card item list view 
    /// Manages listview of ISortableCardItems 
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CardItemSortView : ContentView
    {
        public CardItemObservableCollection baseDataSet;

        private bool showOnlyFavorites = false;

        private CardItemSorter<ISortableCardItem> cardItemSorter;

        public event CardItemAddClicked sortClicked;
        

        private ListView cardItemsList;

        private bool hideAlphaSort;

        public ISortableCardItem selectedItem
        {
            get { return this.cardItemsList.SelectedItem as ISortableCardItem; }
        }

        public CardItemSortView()
        {
            InitializeComponent();
        }

        public void initialize(bool hideAlphaSort, ListView cardItemListView)
        {
            this.hideAlphaSort = hideAlphaSort;
            this.cardItemsList = cardItemListView;
        }

   
        /// <summary>
        /// Call in case ObservableCollection not updated properly.  Resets underlying observable collection 
        /// </summary>
        /// <param name="baseDataSet"></param>
        public void updateDataSet(ObservableCollection<Card> parentDataSet)
        {
            this.baseDataSet = new CardItemObservableCollection(parentDataSet); 
            updateShownDataSet();
        }
        public void updateDataSet(ObservableCollection<AbMediaContent> parentDataSet)
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
            bool sortAscending = (cardItemSorter == null ? false : cardItemSorter.sortAscending);

            if (showOnlyFavorites)
            {
                ObservableCollection<ISortableCardItem> favDataSet = new ObservableCollection<ISortableCardItem>(baseDataSet.Where(x => x.isFavorited));

                this.cardItemSorter = new CardItemSorter<ISortableCardItem>(hideAlphaSort, sortAscending, this.sortTypeLabel, favDataSet);
                this.cardItemsList.ItemsSource = favDataSet;
            }
            else
            {
                this.cardItemSorter = new CardItemSorter<ISortableCardItem>(hideAlphaSort, sortAscending, this.sortTypeLabel, baseDataSet);
                this.cardItemsList.ItemsSource = baseDataSet;
            }
            this.cardItemsList.Focus();
        }

        private void sort_Clicked(object sender, EventArgs e)
        {
            sortClicked?.Invoke();

            cardItemSorter.displaySortOptions();
            //this.cardItemsList.Focus();
        }
        private void sortDirection_Tapped(object sender, EventArgs e)
        {
            sortClicked?.Invoke();

            if (cardItemSorter.sortAscending)
                sortDirectionImage.Source = "UpArrowIcon.png";
            else
                sortDirectionImage.Source = "DownArrowIcon.png";

            cardItemSorter.changeSortDirection(); 
        }

       





        /*
/// <summary>
/// Refresh data set
/// </summary>
protected override void OnAppearing()
{
    List<Dir> cards = showOnlyFavorites ? FSManager.getAllFavCards() : FSManager.getAllCards();

    initialize(cards);
    base.OnAppearing();
}
*/

    }
}