using Cards.Core;
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
	public partial class MediaSelectPage : ContentPage
	{
        AbMediaContent media;

        public MediaSelectPage(AbMediaContent media)
        {
            InitializeComponent();

            this.media = media; 



            mediaDisplayView.setMedia(false, media);

            //linkSwitch.IsToggled = media.isSelected; 

        }

        private async void select_Clicked(object sender, EventArgs e)
        {
            media.isSelected = true;
            await Navigation.PopAsync();
        }

        /*
        void linkSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            media.isSelected = e.Value;
        }
        */

    }
}