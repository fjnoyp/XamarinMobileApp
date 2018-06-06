using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Cards.Core.Views
{
    /// <summary>
    /// For displaying abMediaContent 
    /// </summary>
	public class MediaDisplayView : ContentView
	{

        private IDisappearedListener disappearListener;
        private Page parentPage; 

        public MediaDisplayView() { }

        public void initialize(Page parentPage)
        {
            if (this.parentPage == null)
            {
                this.parentPage = parentPage;
                parentPage.Disappearing += onDisappear;
            }
        }

        private void onDisappear(object sender, EventArgs e)
        {
            notifyMediaViewDisappeared();
            parentPage.Disappearing -= onDisappear; 
        }

        private void notifyMediaViewDisappeared()
        {
            IDisappearedListener disappearedListener = this.Content as IDisappearedListener;
            if (disappearedListener != null)
            {
               disappearedListener.onDisappear();
            }
        }

        public void setMedia(bool isEditable, AbMediaContent media){

            notifyMediaViewDisappeared();

            switch (media.mediaType)
            {
                case MediaContentType.Image:
                    this.Content = new PictureView(media as ImageMediaContent); 
                    break;
                case MediaContentType.Video:
                    this.Content = new VideoMediaView(media);
                    break;
                case MediaContentType.Note:
                    //No card necessary, we write changes directly to text file 
                    this.Content = new NoteView(isEditable, media);
                    break; 
                case MediaContentType.Audio:
                    this.Content = new AudioView(media);
                    break;
                case MediaContentType.Contact:
                    this.Content = new ContactView(isEditable, media);
                    break;
                default:
                    break;
            }

        }
	}
}