
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Xamarin.Forms;

#if __IOS__
using Photos;
using UIKit;
#endif

namespace Cards.Core.Platform.Manager
{
#if __IOS__
    public class UserGalleryLoadManager
    {


        private PHFetchResult imageFetchResults;
        private PHFetchResult videoFetchResults;

        private PHImageManager imageManager; 

        // TODO MAKE ASYNC 
        public List<ImageMediaContent> loadImages()
        {
            List<ImageMediaContent> imageMedia = new List<ImageMediaContent>(); 

            PHFetchResult fetchResult = PHAsset.FetchAssets(PHAssetMediaType.Image, null);

            nint index = 0; 
            foreach( PHAsset phAsset in fetchResult)
            {
                String filePath = "";

                UIImage image; 

                imageManager.RequestImageForAsset((PHAsset)imageFetchResults[index], new SizeF(160, 160),
                    PHImageContentMode.AspectFill, new PHImageRequestOptions(), (img, info) =>
                    {
                        image = img;
                    });

                index++; 
                
                phAsset.RequestContentEditingInput(new PHContentEditingInputRequestOptions(), (input, v) =>
                {
                    filePath = input.FullSizeImageUrl.Path;
                });

                imageMedia.Add(new ImageMediaContent(filePath)); 
                
            }

            return imageMedia; 
        }

        /*
        public void loadImages()
        {
           imageManager = new PHImageManager();

            imageFetchResults = PHAsset.FetchAssets(PHAssetMediaType.Image, null);
            videoFetchResults = PHAsset.FetchAssets(PHAssetMediaType.Video, null); 

            //imageManager.StartCaching( imageFetchResults, new CoreGraphics.CGSize(100,100), PHImageContentMode.AspectFill, PHImageRequestOptions.)
        }

        public void getImage()
        {
            nint index = 1;

            imageManager.RequestImageForAsset((PHAsset)imageFetchResults[index], new SizeF(160, 160),
                PHImageContentMode.AspectFill, new PHImageRequestOptions (), (img, info) =>
                {
                    image = img;
                }); 
        }
        */

    }
#endif

}
