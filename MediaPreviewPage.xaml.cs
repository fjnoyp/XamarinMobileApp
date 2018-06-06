using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Cards.Core;
using System.Collections.ObjectModel;

namespace Cards
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MediaPreviewPage : ContentPage
	{
        AbMediaContent media;
        Dir card;
        

		public MediaPreviewPage ()
		{
            InitializeComponent ();
        }
        
        public void initialize(AbMediaContent media, Dir card)
        {
            this.addLinkView.initialize(media, card,this);

            this.deleteMediaView.intialize(media, card);

            this.linkedMediaView.initialize("Linked Media", card.linkManager.getLinkedMedia(media), card,this);

            this.mediaDisplayView.initialize(true, media, card);
        }

        protected override void OnBindingContextChanged()
        {
            CardMediaViewModel cardModel = BindingContext as CardMediaViewModel;
            this.media = cardModel.media;
            this.card = cardModel.card;
            initialize(this.media, this.card); 

            base.OnBindingContextChanged();
        }

        protected override void OnAppearing()
        {
            this.linkedMediaView.setItemSource(card.linkManager.getLinkedMedia(media)); 
            base.OnAppearing();
        }
    }
}