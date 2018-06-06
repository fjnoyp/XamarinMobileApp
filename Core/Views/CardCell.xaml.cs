using Cards.Core.FileReaders;
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
	public partial class CardCell : ViewCell
	{
		public CardCell ()
		{
			InitializeComponent ();
		}

        private void favoriteImage_Tapped(object sender, EventArgs e)
        {
            Card curCard = BindingContext as Card;
            curCard.isFavorited = !curCard.isFavorited;
            curCard.saveToFile(); 
        }

    }
}