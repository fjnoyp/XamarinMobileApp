
#if __ANDROID__
using Android.Content;
using Cards.Core.FileReaders;
using Cards.Core.Platform.Android;
using Cards.Droid.Activities;
#endif 

using Cards.Core.FileReaders;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Cards.Core.Platform.Manager
{
    /// <summary>
    /// Common class for interfacing with android and ios implementations for media import 
    /// </summary>
    public class MediaImportManager
    {


        public void launchImportPicture(Card card)
        {

#if __ANDROID__
            var intent = new Intent(Forms.Context, typeof(ImportMediaActivity));
            intent.PutExtra("CardName", card.name);
            intent.PutExtra("MediaType", MediaContentType.Image.ToString());
            Forms.Context.StartActivity(intent);
#endif

        }

        public void launchImportVideo(Card card)
        {

#if __ANDROID__
            var intent = new Intent(Forms.Context, typeof(ImportMediaActivity));
            intent.PutExtra("CardName", card.name);
            intent.PutExtra("MediaType", MediaContentType.Video.ToString());
            Forms.Context.StartActivity(intent);
#endif

        }


    }
}
