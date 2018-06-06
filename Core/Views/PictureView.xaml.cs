using FFImageLoading;
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
    public partial class PictureView : ContentView, IDisappearedListener
    {
        public PictureView()
        {
            InitializeComponent();
        }

        public PictureView(ImageMediaContent imageMedia)
        {
            InitializeComponent();

            this.BindingContext = imageMedia;

        }

        public void onDisappear()
        {
        }

        private void picture_Tapped(object sender, EventArgs e)
        {
            MessagingCenter.Send<Object>(this, "FullScreenRequest"); 
        }
    }
}