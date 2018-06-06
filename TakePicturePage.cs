using Cards.Core;
using Cards.Core.FileReaders;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Cards
{
	public class TakePicturePage : ContentPage
	{
        Card card;

        //NOTE: card can be null 
        public TakePicturePage(Card card)
        {
            this.card = card;
            
            takePictures(); 
        }

        public async void takePictures() { 
            //Take picture camera loop 
            bool isCamera = true;
            while (isCamera)
            {
                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions { Name = "photo.jpg", Directory="Pictures", SaveToAlbum = false});

                if (file != null)
                {
                    ImageMediaContent newMedia = new ImageMediaContent(file.Path);
                    card?.addMedia(newMedia);
                    MediaManager.addNewMedia(newMedia);
                }
                else
                {
                    isCamera = false;
                }
            }

            await Navigation.PopAsync(); 
        }
	}
}