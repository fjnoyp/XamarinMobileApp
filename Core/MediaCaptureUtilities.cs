using System;
using System.Collections.Generic;
using System.Text;
using Plugin.Media;
using Xamarin.Forms;
using Cards.Core.Platform;
using System.Threading.Tasks;
using Cards.Core.Views;
using System.Threading;
using Cards.Core.FileReaders;

namespace Cards.Core
{
    public static class MediaCaptureUtilities
    {
        public async static Task<AbMediaContent> takePicture(Card card, Page parentPage)
        {
            //NOTE: must set compile using to "Android 6" and target android version 6
            //Otherwise a null pointer error occurs when using Android 7.1 ... 
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await parentPage.DisplayAlert("No Camera", ":( No camera available.", "OK");
                return null;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(
                new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Name = $@"{Guid.NewGuid()}.jpg",

#if __IOS__
                   Directory = "Pictures",
#endif 

                    SaveToAlbum = false
                }
                );

            if (file == null) return null;

            ImageMediaContent newMedia = new ImageMediaContent(file.Path);
            card?.addMedia(newMedia);
            MediaManager.addNewMedia(newMedia);

            return newMedia; 
        }

        public async static Task<AbMediaContent> takeVideo(Card card, Page parentPage)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakeVideoSupported)
            {
                await parentPage.DisplayAlert("No Camera", ":( No camera for video available.", "OK");
                return null;
            }

            var file = await CrossMedia.Current.TakeVideoAsync(
                new Plugin.Media.Abstractions.StoreVideoOptions
                {
                    Name = $@"{Guid.NewGuid()}.mp4",

#if __IOS__
                   Directory = "Movies",
#endif

                    SaveToAlbum = false
                }
                );

            if (file == null)
                return null;

            await VideoThumbnailHelper.createVideoThumbnailAsync(file.Path);

            VideoMediaContent newMedia = new VideoMediaContent(file.Path); 
            card?.addMedia(newMedia);
            MediaManager.addNewMedia(newMedia);

            return newMedia; 
        }

        public async static Task<AbMediaContent> takeAudio(Card card, Page parentPage)
        {
            ContentPage recordAudioPage = new ContentPage();
            RecordAudioView recordAudioView = new RecordAudioView(card);
            recordAudioPage.Content = recordAudioView;

            return await getMediaResult(recordAudioView, recordAudioView, recordAudioPage, parentPage); 
        }

        public async static Task<AbMediaContent> takeNote(Card card, Page parentPage)
        {
            ContentPage editNotePage = new ContentPage();
            NoteView noteCapture = new NoteView(card, editNotePage);
            editNotePage.Disappearing += (e,v) => { noteCapture.onDisappear(); }; // TODO should move to constructor 
            editNotePage.Content = noteCapture;

            return await getMediaResult(noteCapture, noteCapture, editNotePage, parentPage); 
        }

        public async static Task<AbMediaContent> takeContact(Card card, Page parentPage)
        {
            ContentPage editContactPage = new ContentPage();
            ContactView contactCapture = new ContactView(card, editContactPage);
            editContactPage.Disappearing += (e, v) => { contactCapture.onDisappear(); }; // TODO should move to constructor 
            editContactPage.Content = contactCapture;

            return await getMediaResult(contactCapture, contactCapture, editContactPage, parentPage); 
        }

        private async static Task<AbMediaContent> getMediaResult(IMediaCapturer mediaCapturer, IDisappearedListener disappearedListener, Page page, Page parentPage)
        {
            EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

            page.Disappearing += (e, v) =>
            {
                waitHandle.Set();
            };

            await parentPage.Navigation.PushAsync(page);
            await Task.Run(() => waitHandle.WaitOne());

            //disappearedListener.onDisappear(); 
            return mediaCapturer.capturedMedia;

        }

        public async static Task<AbMediaContent> takeMedia(MediaContentType mediaType, Card card, Page parentPage)
        {
            switch (mediaType)
            {
                case MediaContentType.Image:
                    return await takePicture(card, parentPage);
            
                case MediaContentType.Video:
                    return await takeVideo(card, parentPage);
                 
                case MediaContentType.Audio:
                    return await takeAudio(card, parentPage);
            
                case MediaContentType.Note:
                    return await takeNote(card, parentPage);
            
                case MediaContentType.Contact:
                    return await takeContact(card, parentPage);
 
            }
            return null; 
        }
    }
}
