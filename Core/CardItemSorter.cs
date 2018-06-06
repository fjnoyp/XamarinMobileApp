using Acr.UserDialogs;
using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Cards.Core
{
    public enum SortType { Alphabetically, ModTime }

    /// <summary>
    /// Sorts an observable collection of dir 
    /// NOTE: I would normally turn this into a view but Xamarin Forms doesn't support generic types for ContentView ...
    /// THUS: this class manages a CachedImage view and a Label view to display current sorting information 
    /// </summary>
    public class CardItemSorter<T> where T : ISortableCardItem
    {
        //dir collection to sort 
        private ObservableCollection<T> cardItemCollection;

        //text to update what sort type is being used 

        private bool sortAscending;
        private SortType sortType; 


        

        public CardItemSorter(ObservableCollection<T> cardItemCollection)
        {
            this.cardItemCollection = cardItemCollection; 
        }

        public void setDataSet(ObservableCollection<T> cardItemCollection)
        {
            this.cardItemCollection = cardItemCollection; 
            doSort(this.sortAscending, this.sortType); 
        }

        public void doSort(bool sortAscending, SortType sortType)
        {
            this.sortAscending = sortAscending;
            this.sortType = sortType; 

            switch (sortType)
            {
                case SortType.Alphabetically: 
                    ObservableCollectionSorter.Sort<T>(cardItemCollection, new CardItemNameComparer<T>(sortAscending));
                    break;
                case SortType.ModTime: 
                    ObservableCollectionSorter.Sort<T>(cardItemCollection, new CardItemDateComparer<T>(sortAscending));
                    break;
                default:
                    break;
            }
        }

        /*
        public async void displaySortOptions()
        {

            //Support for not having alphabetical sorting (images and videos)
            string[] buttons; 
            if (hideAlphaSort)
            {
                buttons = new string[] { "modified time" };
            }
            else
            {
                buttons = new string[] { "alphabetically", "modified time" };
            }

            string userChoice = await UserDialogs.Instance.ActionSheetAsync("Sort", "Cancel", "", null, buttons); 

            if(userChoice != "Cancel" && userChoice != null)
            {
                sortTypeLabel.Text = userChoice;
                doSort(sortAscending, userChoice); 
            }
        }
        */

    }

    public class CardItemDateComparer<T> : IComparer<T> where T : ISortableCardItem 
    {
        int modifier = -1; 

        public CardItemDateComparer(bool earliest)
        {
            if (!earliest) modifier = 1;
        }
        public int Compare(T x, T y)
        {
            return modifier * x.creationTime.CompareTo(y.creationTime);
        }
    }
    public class CardItemNameComparer<T> : IComparer<T> where T : ISortableCardItem
    {

        int modifier = 1;

        public CardItemNameComparer(bool a2z)
        {
            if (!a2z) modifier = -1;
        }
        public int Compare(T x, T y)
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

}
