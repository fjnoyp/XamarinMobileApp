
#if __ANDROID__
using Android.Graphics;
using Android.Media;
using Android.Provider;
#endif

#if __IOS__
using AVFoundation;
using CoreMedia;
using Foundation;
using UIKit;
#endif


using PCLStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace Cards.Core.Platform
{
    public static class VideoThumbnailHelper
    {


        // Create a video thumbnail preview for the video 
        public static async Task createVideoThumbnailAsync(string videoFilePath) { 


           #if __IOS__

            // Extract name of video file, use same name for video thumbnail file 
            string videoFileName = System.IO.Path.GetFileNameWithoutExtension(videoFilePath);
            string videoThumbnailFilePath = System.IO.Path.Combine(
                FilePaths.allVideoThumbnailsPath,
                videoFileName + ".jpg");


            await Task.Run( () =>
            {
                // Adapted from : https://forums.xamarin.com/discussion/61903/code-snippet-how-to-generate-video-thumbnail
                CMTime actualTime;
                NSError outError;
                using (var asset = AVAsset.FromUrl(NSUrl.FromFilename(videoFilePath)))
                using (var imageGen = new AVAssetImageGenerator(asset))
                using (var imageRef = imageGen.CopyCGImageAtTime(new CMTime(1, 1), out actualTime, out outError))
                {
                    UIImage videoThumbnail = UIImage.FromImage(imageRef);
                    NSData imgData = videoThumbnail.AsJPEG();
                    imgData.Save(videoThumbnailFilePath, false);
                }
            }
            ); 
#endif

#if __ANDROID__
            // Generate bitmap 
            Bitmap videoBitmap = await createVideoBitmapAsync(100, 100, videoFilePath);

            // Extract name of video file, use same name for video thumbnail file 
            string videoFileName = System.IO.Path.GetFileNameWithoutExtension(videoFilePath);
            string videoThumbnailFilePath = System.IO.Path.Combine(
                FilePaths.allVideoThumbnailsPath,
                videoFileName + ".jpg"); 

            // Save to file 
            await exportBitmapAsPNGAsync(videoThumbnailFilePath, videoBitmap);
#endif
    }



#if __ANDROID__
        public static async Task<Bitmap> createVideoBitmapAsync(int width, int height, string videoFilePath)
        {
            Bitmap videoThumbnail = await ThumbnailUtils.CreateVideoThumbnailAsync(videoFilePath, ThumbnailKind.MicroKind);
            return Bitmap.CreateScaledBitmap(videoThumbnail, width, height, false);
        }

        // Adapted from: https://stackoverflow.com/questions/25976002/saving-bitmap-to-file-xamarin-monodroid
        public static async Task exportBitmapAsPNGAsync(string thumbnailFilePath, Bitmap bitmap)
        {

            using (FileStream fs = new FileStream(thumbnailFilePath, FileMode.OpenOrCreate))
            {
                await bitmap.CompressAsync(Bitmap.CompressFormat.Jpeg, 100, fs);
            }

        }
#endif

}
}
