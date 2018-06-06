
#if __ANDROID__
using Android.Graphics;
#endif

#if __IOS__
using Foundation;
using UIKit;
#endif

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Cards.Core.Platform
{
    public static class ImageResizeHelper
    {



        public async static Task createImageThumbnailAsync(string imageFilePath)
        {
            //Extract name of image file, use same name for image thumbnail file 
            string imageFileName = System.IO.Path.GetFileNameWithoutExtension(imageFilePath);
            string imageThumbnailFilePath = System.IO.Path.Combine(
                FilePaths.allImageThumbnailsPath,
                imageFileName + ".jpg");
            await Task.Run( () => resizeImage(imageFilePath, imageThumbnailFilePath, 100, 100)); 

        }

#if __IOS__
        public static void resizeImage(string sourceFile, string targetFile, float width, float height)
        {
            /*
            UIImage sourceImage = new UIImage(sourceFile);
            var sourceSize = sourceImage.Size;

            UIGraphics.BeginImageContext(new SizeF(maxWidth, maxHeight));

            sourceImage.Draw(new RectangleF(0, 0, maxWidth, maxHeight));
            UIImage resultImage = UIGraphics.GetImageFromCurrentImageContext();
            NSData imgData = resultImage.AsJPEG();
            imgData.Save(targetFile, false);

            UIGraphics.EndImageContext();
            */

            UIImage sourceImage = UIImage.FromFile("file://" + sourceFile);

            UIGraphics.BeginImageContext(new SizeF(width, height));
            sourceImage.Draw(new RectangleF(0, 0, width, height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            NSData imgData = resultImage.AsJPEG();
            imgData.Save(targetFile, false); 
        }
#endif

#if __ANDROID__
        public static void resizeImage(string sourceFile, string targetFile, float maxWidth, float maxHeight)
        {

            if (!File.Exists(targetFile) && File.Exists(sourceFile))
            {
                // First decode with inJustDecodeBounds=true to check dimensions
                var options = new BitmapFactory.Options()
                {
                    InJustDecodeBounds = false,
                    InPurgeable = true,
                };

                using (var image = BitmapFactory.DecodeFile(sourceFile, options))
                {
                    if (image != null)
                    {
                        var sourceSize = new Xamarin.Forms.Size((int)image.GetBitmapInfo().Height, (int)image.GetBitmapInfo().Width);

                        var maxResizeFactor = Math.Min(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);

                        string targetDir = System.IO.Path.GetDirectoryName(targetFile);
                        if (!Directory.Exists(targetDir))
                            Directory.CreateDirectory(targetDir);

                        if (maxResizeFactor > 0.9)
                        {
                            File.Copy(sourceFile, targetFile);
                        }
                        else
                        {
                            var width = (int)(maxResizeFactor * sourceSize.Width);
                            var height = (int)(maxResizeFactor * sourceSize.Height);

                            /*
                            Bitmap scaledBitmap = Bitmap.CreateScaledBitmap(image, width, height, false);
                            VideoThumbnailHelper.exportBitmapAsPNGAsync(targetFile, scaledBitmap); 
                            */

                            
                            using (var bitmapScaled = Bitmap.CreateScaledBitmap(image, height, width, true))
                            {
                                using (Stream outStream = File.Create(targetFile))
                                {
                                    if (targetFile.ToLower().EndsWith("png"))
                                        bitmapScaled.Compress(Bitmap.CompressFormat.Png, 100, outStream);
                                    else
                                        bitmapScaled.Compress(Bitmap.CompressFormat.Jpeg, 100, outStream);
                                }
                                bitmapScaled.Recycle();
                            }
                            
                        }

                        image.Recycle();
                    }
                    else
                    {
                        throw new Exception("ImageResizeHelper.cs - Image scaling failed of " + sourceFile);
                    }
                }
            }
            }
#endif


    }
}
