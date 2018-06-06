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
	public partial class TestPropertiesPage : ContentPage
	{
        /*
        public static readonly BindableProperty testProperty =
  BindableProperty.Create("test", typeof(string), typeof(TestPropertiesPage), null);

        public string test
        {
            get { return (string)GetValue(testProperty); }
            set { SetValue(testProperty, value); }
        }
        */

        public TestPropertiesPage ()
		{
			InitializeComponent ();


		}
	}
}