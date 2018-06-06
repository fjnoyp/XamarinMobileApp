using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Cards.Core.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MediaCell : ViewCell
	{
        //For when using recycle view in listview 
        //https://github.com/luberda-molinet/FFImageLoading/wiki/Xamarin.Forms-Advanced
        public MediaCell ()
		{
            InitializeComponent ();
		}
	}
}