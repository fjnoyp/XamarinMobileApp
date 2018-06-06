using Cards.Core.FileReaders;
using CarouselView.FormsPlugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Cards.Core.Views
{
    class MediaLinkCaptureView : MediaControlView
    {

        private CardMediaCarouselPage cardMediaCarouselPage;

        private AbMediaContent rootMedia; 

        public MediaLinkCaptureView() : base()
        {

        }

        public void setLinkToMedia(AbMediaContent media)
        {
            this.rootMedia = media; 
        }

        /*
        // TODO call a setMedia method from CardMediaCarouselPage!!!! 
        public void initialize(CardMediaCarouselPage cardMediaCarouselPage)
        {
            this.cardMediaCarouselPage = cardMediaCarouselPage;
        }
        */

        protected override async Task handleImageTapped(MediaContentType mediaType)
        {
            AbMediaContent otherMedia = await MediaCaptureUtilities.takeMedia(mediaType, base.card, base.parentPage);

            if (otherMedia != null)
            {
                LinkManager linkManager;
                
                if(card != null)
                    linkManager = card.linkManager;     
                else
                    linkManager = MediaManager.mediaLinkManager; 

                linkManager.addLink(this.rootMedia, otherMedia);

                if (card != null)
                    card.saveToFile(); 
                else
                    MediaManager.saveMediaLinkManager(); 
            }
        }

    }
}
