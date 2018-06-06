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
    public partial class HorizontalListView : ContentView
    {

        public ListView listView { get { return this.rotatedListView; } }

        private ObservableCollection<AbMediaContent> mediaCollection; 
        public int count { get { return mediaCollection.Count; } }

        public HorizontalListView()
        {
            InitializeComponent();
        }

        public void initialize(ObservableCollection<AbMediaContent> mediaCollection)
        {
            this.mediaCollection = mediaCollection; 
            this.listView.ItemsSource = mediaCollection;
            this.listView.ItemTemplate = new DataTemplate(typeof(RotatedMediaCell)); 
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            rotatedListView.BeginRefresh();

            absoluteLayout.HeightRequest = height;
            absoluteLayout.WidthRequest = width;

            Xamarin.Forms.AbsoluteLayout.SetLayoutBounds(rotatedListView, new Rectangle((width - height) / 2.0, -(width - height) / 2.0, height, width));

            rotatedListView.RowHeight = (int)height;

            rotatedListView.Rotation = 270;

            rotatedListView.EndRefresh(); 
        }
    }
}