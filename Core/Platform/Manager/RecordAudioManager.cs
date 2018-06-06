#if __ANDROID__
using Android.Content;
using Cards.Core.FileReaders;
using Cards.Core.Platform.Android;
#endif

using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Cards.Core.Platform.Manager
{
    /// <summary>
    /// Common class for interfacing with android and ios implementations for audio recording
    /// </summary>
    public class RecordAudioManager
    {

#if __ANDROID__
        public void launchRecordAudioScreen(Card card)
        {
            var intent = new Intent(Forms.Context, typeof(RecordAudioActivity));
            intent.PutExtra("CardName", card.name);
            Forms.Context.StartActivity(intent);
        }
#endif

    }
}
