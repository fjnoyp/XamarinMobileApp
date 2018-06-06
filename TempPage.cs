using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Cards
{
	public class TempPage : ContentPage
	{
		public TempPage ()
		{
            CrossMediaManager.Current.Play("https://archive.org/download/BigBuckBunny_328/BigBuckBunny_512kb.mp4", MediaFileType.Video);

        }
    }
}