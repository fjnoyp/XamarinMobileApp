using PCLStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Cards.Core
{
    public static class FilePaths
    {

        public static IFolder allCardsDir;

        public static IFolder allImagesDir;
        public static IFolder allVideosDir;
        public static IFolder allAudiosDir; 
        public static IFolder allNotesDir;
        public static IFolder allContactsDir;

        public static IFolder allShareCardsDir; 

        public async static Task initFoldersAsync()
        {

            //test(); 

            IFolder localStorage = FileSystem.Current.LocalStorage;

            allCardsDir = await localStorage.CreateFolderAsync(Path.Combine(FilePaths.rootPath, "Cards"), CreationCollisionOption.OpenIfExists);


            allImagesDir = await localStorage.CreateFolderAsync(Path.Combine(FilePaths.rootPath, "Pictures"), CreationCollisionOption.OpenIfExists);
            allVideosDir = await localStorage.CreateFolderAsync(Path.Combine(FilePaths.rootPath, "Movies"), CreationCollisionOption.OpenIfExists);
            allNotesDir = await localStorage.CreateFolderAsync(Path.Combine(FilePaths.rootPath, "Notes"), CreationCollisionOption.OpenIfExists);
            allAudiosDir = await localStorage.CreateFolderAsync(Path.Combine(FilePaths.rootPath, "Audios"), CreationCollisionOption.OpenIfExists);
            allContactsDir = await localStorage.CreateFolderAsync(Path.Combine(FilePaths.rootPath, "Contacts"), CreationCollisionOption.OpenIfExists);

            allShareCardsDir = await localStorage.CreateFolderAsync(Path.Combine(FilePaths.rootPath, "ShareCards"), CreationCollisionOption.OpenIfExists); 

            await localStorage.CreateFolderAsync(Path.Combine(FilePaths.rootPath, "ContactInfos"), CreationCollisionOption.OpenIfExists);
            await localStorage.CreateFolderAsync(Path.Combine(FilePaths.rootPath, "VideoThumbnails"), CreationCollisionOption.OpenIfExists);
            await localStorage.CreateFolderAsync(Path.Combine(FilePaths.rootPath, "ImageThumbnails"), CreationCollisionOption.OpenIfExists);

    }

    //https://stackoverflow.com/questions/43342127/xamarin-media-plugin-get-android-image-path
    public static string rootPath
        {
            get
            {
#if __ANDROID__
                return Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath;
#endif
      
#if __IOS__
                return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
#endif
            }
        }

        public static string allImagesPath
        {

            get { return Path.Combine(rootPath, "Pictures"); }

        }

        public static string allImageThumbnailsPath
        {
     
            get { return Path.Combine(rootPath, "PictureThumbnails"); }
        }

        public static string allVideosPath
        {
      
            get { return Path.Combine(rootPath, "Movies"); }
        }

        public static string allVideoThumbnailsPath
        {
       
            get { return Path.Combine(rootPath, "VideoThumbnails"); }
        }

        public static string allNotesPath
        {
           
            get { return Path.Combine(rootPath, "Notes"); }
        }

        public static string allAudiosPath
        {
      
            get { return Path.Combine(rootPath, "Audios");  }
        }

        public static string allContactsPath
        {
            get { return Path.Combine(rootPath, "Contacts"); }
        }

        public static string allContactInfosPath
        {
            get { return Path.Combine(rootPath, "ContactInfos"); }
        }

        public static string allShareCardsPath
        {
            get { return Path.Combine(rootPath, "ShareCards"); }
        }

        public static string allCardsPath
        {
       
            get { return Path.Combine(rootPath, "Cards"); }
        }

        public static string allAlbumsPath
        {
         
            get { return Path.Combine(rootPath, "Albums"); }
        }




    }
}

