using Cards.Core;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Cards
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MediaCapturePage : ContentPage
    {
        public MediaCapturePage()
        {
            InitializeComponent();
        }
        async void takePicture(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Name = "photo.jpg",
                SaveToAlbum = false
            });

            if (file == null)
                return;

            await DisplayAlert("File Location", file.Path, "OK");

            image.Source = ImageSource.FromFile(file.Path);

        }

        async void pickPicture(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("No Pick Photo", ":( Pick photo is not available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.PickPhotoAsync();

            if (file == null)
                return;

            await DisplayAlert("File Location", file.AlbumPath + "|" + file.Path, "OK");

            image.Source = ImageSource.FromFile(file.Path);

        }

        /*
    async void takeVideo(object sender, EventArgs e)
    {
        await CrossMedia.Current.Initialize();

        if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakeVideoSupported)
        {
            DisplayAlert("No Camera", ":( No camera for video available.", "OK");
            return;
        }

        var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
        {
            Directory = "CardsApp/AllVideos",
            Name = "test"
        });

        if (file == null)
            return;

        await DisplayAlert("File Location", file.Path, "OK");

        video.Source = VideoSource.FromFile(file.Path);

    }

    async void pickVideo(object sender, EventArgs e)
    {
        await CrossMedia.Current.Initialize();

        if (!CrossMedia.Current.IsPickVideoSupported)
        {
            DisplayAlert("No Pick Video", ":( Pick video is not available.", "OK");
            return;
        }

        var file = await CrossMedia.Current.PickVideoAsync(); 

        if (file == null)
            return;

        await DisplayAlert("File Location", file.Path, "OK");

        video.Source = VideoSource.FromFile(file.Path);

    }
    */
    }
}