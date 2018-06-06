using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Cards.Core.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CardMediaCell : ContentView
	{
        MediaContentType? oldMediaType = null;

		public CardMediaCell ()
		{
			InitializeComponent ();

          

        }

        private void Image_Finish(object sender, CachedImageEvents.FinishEventArgs e)
        {
            image.IsVisible = true; 
            image.Finish -= Image_Finish; 
        }

        protected override void OnBindingContextChanged()
        {
            if (this.BindingContext == null) return;

            AbMediaContent media = this.BindingContext as AbMediaContent;
            MediaContentType mediaType = media.mediaType;

            if (oldMediaType == mediaType)
            {
                base.OnBindingContextChanged(); 
                return;
            }

            hideAll();
            grid.BackgroundColor = Color.Transparent;

            base.OnBindingContextChanged(); 

            // Only show image once image loading is Finished
            switch (mediaType)
            {
                case MediaContentType.Image:
                case MediaContentType.Video:
                case MediaContentType.Card:
                case MediaContentType.MediaCount:
                    image.Finish += Image_Finish;
                    break;
            }

            //base.OnBindingContextChanged();

            switch (mediaType)
            {

                case MediaContentType.Image:
                    this.image.IsVisible = true;
                    break;
                case MediaContentType.Video:
                    this.image.IsVisible = true;
                    this.videoImage.Source = ImageSource.FromFile("VideoIcon.png"); 
                    this.videoImage.IsVisible = true;
                    break;
                case MediaContentType.Audio:
                    this.nameLabel.IsEnabled = true;
                    this.nameLabel.IsVisible = true;

                    this.videoImage.Source = ImageSource.FromFile("AudioIcon.png");
                    this.videoImage.IsVisible = true;

                    grid.BackgroundColor = Color.Coral;
                    break;
                case MediaContentType.Note:
                    this.nameLabel.IsEnabled = true;
                    this.nameLabel.IsVisible = true;

                    this.bodyLabel.IsEnabled = true;
                    this.bodyLabel.IsVisible = true;

                    grid.BackgroundColor = Color.LightYellow; 
                    break;
                case MediaContentType.Contact:
                    this.image.IsVisible = true;

                    this.videoImage.Source = ImageSource.FromFile("ContactIcon.png");
                    this.videoImage.IsVisible = true;
                    break;
                case MediaContentType.MediaCount:
                    this.image.IsVisible = true;

                    this.countFrame.IsVisible = true; 

                    this.countLabel.IsEnabled = true;
                    this.countLabel.IsVisible = true;
                    break;
                case MediaContentType.Card:
                    this.nameLabel.IsEnabled = true;
                    this.nameLabel.IsVisible = true;

                    grid.BackgroundColor = Color.LightGreen;
                    break;
            }

            oldMediaType = mediaType;


        }



        protected void hideAll()
        {
            this.nameLabel.IsEnabled = false; 
            this.nameLabel.IsVisible = false;

            this.bodyLabel.IsEnabled = false;
            this.bodyLabel.IsVisible = false;

       
            this.countFrame.IsVisible = false;
            this.countLabel.IsEnabled = false;
            this.countLabel.IsVisible = false; 

            this.image.IsVisible = false;
            this.secondImage.SetBinding(CachedImage.IsVisibleProperty, new Binding("isSelected")); 

            this.secondImage.Source = ImageSource.FromFile("SelectedIcon.png"); 

            this.videoImage.IsVisible = false;
        }

        protected void cardMediaVisible()
        {
            this.nameLabel.IsEnabled = true;
            this.nameLabel.IsVisible = true; 
        }
        


    }
}