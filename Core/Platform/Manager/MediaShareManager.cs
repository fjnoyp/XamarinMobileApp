
#if __ANDROID__
using Android.Content;
using Cards.Droid.Activities;
#endif 

#if __IOS__
using Foundation;
using UIKit;
#endif 

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Xamarin.Forms;

namespace Cards.Core.Platform.Manager
{
    public class MediaShareManager
    {
        // Share code adapted from very helpful post: 
        // https://forums.xamarin.com/discussion/91460/video-and-image-sharing-on-cross-platform



        public static void shareMedia(AbMediaContent media)
        {

#if __IOS__

            string fileName = Path.GetFileName(media.filePath);
            NSUrl url = NSUrl.FromFilename(fileName);
            NSObject item = url.Copy();

            NSObject[] activityItems = new[] { item };
            UIActivityViewController activityController = new UIActivityViewController(activityItems, null);
            UIViewController topController = UIApplication.SharedApplication.KeyWindow.RootViewController;

            while (topController.PresentedViewController != null)
            {
                topController = topController.PresentedViewController;
            }

            topController.PresentViewController(activityController, true, () => { });
#endif

#if __ANDROID__
            //Note, keeping intent type but it doesn't seem to matter ... 
            string intentType = (media.mediaType == MediaContentType.Image) ? "image/*" : "video/*";
            MessagingCenter.Send<string,List<string>>(intentType, "Share", new List<string> { media.filePath });
#endif

        }


        public static void shareMedia(List<AbMediaContent> medias)
        {

#if __IOS__

            List<NSObject> nsObjects = new List<NSObject>(medias.Count);

            foreach (AbMediaContent media in medias)
            {
                string fileName = Path.GetFileName(media.filePath);
                NSUrl url = NSUrl.FromFilename(fileName);
                nsObjects.Add(url.Copy()); 
            }
            NSObject[] activityItems = nsObjects.ToArray();
            UIActivityViewController activityController = new UIActivityViewController(activityItems, null);
            UIViewController topController = UIApplication.SharedApplication.KeyWindow.RootViewController;

            while (topController.PresentedViewController != null)
            {
                topController = topController.PresentedViewController;
            }

            topController.PresentViewController(activityController, true, () => { });
#endif

#if __ANDROID__
            List<string> mediaFilePaths = new List<string>(medias.Count); 
            foreach(AbMediaContent media in medias)
            {
                mediaFilePaths.Add(media.filePath); 
            }
            MessagingCenter.Send<string, List<string>>("image/*", "Share", mediaFilePaths);
#endif
        }


    }
    }
