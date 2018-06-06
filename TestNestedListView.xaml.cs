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
	public partial class TestNestedListView : ContentPage
	{

        public ObservableCollection<ParentListClass> parentCollection { get; set; }

		public TestNestedListView ()
		{
			InitializeComponent ();

            parentCollection = new ObservableCollection<ParentListClass>(); 

            for (int j = 0; j < 10; j++)
            {
                ParentListClass a = new ParentListClass();
                a.parentTitle = "AAAAAAAAAAAAAAAAAA";
                
                a.childCollection = new ObservableCollection<ChildListClass>();
                for (int i = 0; i < 10; i++)
                {
                    a.childCollection.Add(new ChildListClass() { childTitle = "ASDFASDW" });
                }
                
                parentCollection.Add(a); 
            }

            ReferencesNew.ItemsSource = parentCollection; 
		}
	}

    public class ParentListClass
    {
        public string parentTitle { get; set; }

        public ObservableCollection<ChildListClass> childCollection { get; set; }
    }

    public class ChildListClass
    {
        public string childTitle { get; set; }

        //public Xamarin.Forms.ImageSource image { get { return ImageSource.FromFile("Icon.png"); }  }
    }
}