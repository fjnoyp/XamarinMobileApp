using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using PCLStorage;
using System.IO;
using Cards.Core;
using Acr.UserDialogs;

namespace Cards
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            
            IFileSystem fileSystem = FileSystem.Current;
            IFolder localStorage = fileSystem.LocalStorage;

           
            //localStorage.CreateFolderAsync(Path.Combine(FilePaths.rootPath, "Albums"), CreationCollisionOption.OpenIfExists).Wait();
            //localStorage.CreateFolderAsync(Path.Combine(FilePaths.rootPath, "Cards"), CreationCollisionOption.OpenIfExists).Wait();
        }

        async void OpenPeople(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new AlbumsPage(CardCategory.People));

        }
        async void OpenEvents(object sender, EventArgs e)
        {
           // await Navigation.PushAsync(new AlbumsPage(CardCategory.Event));
        }
        async void OpenPrivate(object sender, EventArgs e)
        {

            //await Navigation.PushAsync(new AlbumsPage(CardCategory.Private));
            //await Navigation.PushAsync(new SimpleGalleryPage()); 
        }
        async void OpenAllCards(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AllCardsPage());
        }
        async void CreateCard(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new CreateCardPage());

        }
    }
}
