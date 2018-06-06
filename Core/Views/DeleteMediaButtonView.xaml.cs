using PCLStorage;
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
	public partial class DeleteMediaButtonView : ContentView
	{

        AbMediaContent media;
        Dir card;

        public DeleteMediaButtonView ()
		{
			InitializeComponent ();
		}

        public void intialize(AbMediaContent media, Dir card)    
        {
            this.media = media;
            this.card = card; 
        }

        private async void deleteButton_Clicked(object sender, EventArgs e)
        {
         
            this.card.removeMedia(this.media);
            FSManager.deleteMediaAsync(this.media); 
            
            //and close out of the view 
            await this.Navigation.PopAsync();
        }
    }
}