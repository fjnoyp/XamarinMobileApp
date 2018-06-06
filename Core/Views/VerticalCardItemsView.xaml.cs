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
    /// <summary>
    /// Simple class thats wraps together sort, header, and list view togther for displaying CARDs and ALBUMs in a vertical list 
    /// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VerticalCardItemsView : ContentView
	{
        public ListView listView { get { return this.cardItemListView;  } }
        public CardItemSortView sortView { get { return this.cardItemSortView; } }
        public CardItemHeaderView headerView { get { return this.cardItemHeaderView; } }


		public VerticalCardItemsView ()
		{
			InitializeComponent ();
		}

        public void initialize(string label, ObservableCollection<Card> parentDataSet )
        {
            listView.ItemTemplate = new DataTemplate(typeof(CardCell));

            this.sortView.initialize(false, listView);
            sortView.updateDataSet(parentDataSet);

            this.headerView.initialize(true,listView);
            headerView.setLabel(label);
            //IT'S 6 AM NOT THINKING, NOT WORKING WELL WITH SORT !!! 
            headerView.initializeSearch(sortView.baseDataSet); 

            
        }
	}
}