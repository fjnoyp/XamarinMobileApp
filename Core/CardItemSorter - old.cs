using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Cards.Core
{

    /// <summary>
    /// Sorts an observable collection of dir 
    /// </summary>
    public class CardItemSorter
    {
        //dir collection to sort 
        private ObservableCollection<ISortableCardItem> cardItemCollection;

        //text to update what sort type is being used 
        private Label sortTypeLabel; 

        private enum SortType { time, a2z, z2a };

        public CardItemSorter(Label sortTypeLabel, ObservableCollection<ISortableCardItem> cardItemCollection)
        {
            this.sortTypeLabel = sortTypeLabel; 
            this.cardItemCollection = cardItemCollection;

            if (sortTypeLabel.Text != null)
            {
                doSort(sortTypeLabel.Text);
            }
            else
            {
                ObservableCollectionSorter.Sort<ISortableCardItem>(cardItemCollection, new CardItemDateComparer(true));
                sortTypeLabel.Text = "earliest";
            }
        }

        private void doSort(string sortType)
        {
            switch (sortType)
            {
                case "earliest":
                    ObservableCollectionSorter.Sort<ISortableCardItem>(cardItemCollection, new CardItemDateComparer(true));
                    break;
                case "latest":
                    ObservableCollectionSorter.Sort<ISortableCardItem>(cardItemCollection, new CardItemDateComparer(false));
                    break;
                case "A-Z":
                    ObservableCollectionSorter.Sort<ISortableCardItem>(cardItemCollection, new CardItemNameComparer(true));
                    break;
                case "Z-A":
                    ObservableCollectionSorter.Sort<ISortableCardItem>(cardItemCollection, new CardItemNameComparer(false));
                    break;
                default:
                    break;
            }
        }

        public async void displaySortOptions(Page page)
        {
            var action = await page.DisplayActionSheet("Sort", null, "Cancel", "earliest", "latest", "A-Z", "Z-A");

            if (action != "Cancel" && action != null)
            {
                sortTypeLabel.Text = action;
                doSort(action);
            }
        }

    }

    public class CardItemDateComparer : IComparer<ISortableCardItem>
    {
        int modifier = -1; 

        public CardItemDateComparer(bool earliest)
        {
            if (!earliest) modifier = 1;
        }
        public int Compare(ISortableCardItem x, ISortableCardItem y)
        {
            return modifier * x.lastModTime.CompareTo(y.lastModTime);
        }
    }
    public class CardItemNameComparer : IComparer<ISortableCardItem>
    {

        int modifier = 1;

        public CardItemNameComparer(bool a2z)
        {
            if (!a2z) modifier = -1;
        }
        public int Compare(ISortableCardItem x, ISortableCardItem y)
        {
            return modifier * x.name.CompareTo(y.name);
        }
    }

    public static class ObservableCollectionSorter
    {
        public static void Sort<T>(ObservableCollection<T> collection, IComparer<T> comparer)
        {
            var sortableList = new List<T>(collection);
            sortableList.Sort(comparer);

            for (int i = 0; i < sortableList.Count; i++)
            {
                collection.Move(collection.IndexOf(sortableList[i]), i);
            }
        }

    }

    public class Other<T> where T : ISortableCardItem
    {
        public void qwe(ObservableCollection<T> temp)
        {

        }
    }
}
