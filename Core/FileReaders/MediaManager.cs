using Cards.Core.FileReaders;
using Cards.Core.Platform.Manager;
using Cards.Core.Utilities;
using DLToolkit.Forms.Controls;
using FFImageLoading;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Cards.Core
{
    public static class MediaManager
    {
       public static SortedDictionary<DateTime, DayMediaObservableCollection> allMediaDict;
        public static List<AbMediaContent> allMedia;

        public static FlowObservableCollection<DayMediaObservableCollection> allMediaCollection;

        public static LinkManager mediaLinkManager { get; private set; }
        public static FavoriteManager favoriteManager { get; private set; }

        public delegate void RefreshCollection();
        public static event RefreshCollection collectionRefresh; 


        public static void addNewMedia(AbMediaContent newMedia)
        {
            //Update media data structures 
            DateTime date = newMedia.creationTime.Date;

            allMedia.Insert(0, newMedia); 

            DayMediaObservableCollection dayMedia = null;
            allMediaDict.TryGetValue(date, out dayMedia);

            if (dayMedia == null)
            {
                dayMedia = new DayMediaObservableCollection(date);
                //Update media collecton - TODO assuming media is at start 
                allMediaCollection.Insert(0, dayMedia);
                dayMedia.addNewMedia(newMedia);
                collectionRefresh?.Invoke(); 
            }
            else
            {
                dayMedia.addNewMedia(newMedia);
            }

            

            allMediaDict[date] = dayMedia;

        }

        public async static void deleteMediaAsync(AbMediaContent media)
        {
            //Asynchronously remove media from other cards 
            var removeCardMediaTask = Task.Run(() => FSManager.removeMediaFromCards(media));
            
                //Make sure media wasn't imported.  If not delete its file 
                if (media.filePath.Contains(FilePaths.rootPath))
                {
                    IFile mediaFile = await FileSystem.Current.GetFileFromPathAsync(media.filePath);
                    await mediaFile.DeleteAsync();
                }

            //Remove from relevant MediaManager data structures 
            allMedia.Remove(media);

            DayMediaObservableCollection dayCollection;
            allMediaDict.TryGetValue(media.creationTime.Date, out dayCollection);
            if(dayCollection != null)
            {
                dayCollection.removeMedia(media); 
            }

            //Remove empty days 
            if(dayCollection.Count == 0)
            {
                allMediaDict.Remove(media.creationTime.Date);
                allMediaCollection.Remove(dayCollection); 
            }

            //Wait for card media removal task to complete 
            await removeCardMediaTask; 
        }

        public static void saveMediaLinkManager()
        {
            JSONSerialManager.serialize(Path.Combine(FilePaths.rootPath, "AllMediaLinkManager"), mediaLinkManager);
        }
        public static void saveFavoriteManager()
        {
            JSONSerialManager.serialize(Path.Combine(FilePaths.rootPath, "AllMediaFavoriteManager"), mediaLinkManager);
        }

        public async static Task initializeAsync()
        {
            await readDayMediaAsync();
            loadMediaIntoCollection();

            string mediaLinkManagerPath = Path.Combine(FilePaths.rootPath, "AllMediaLinkManager");
            if (File.Exists(mediaLinkManagerPath))
            {
                mediaLinkManager = JSONSerialManager.deserialize<LinkManager>(Path.Combine(FilePaths.rootPath, 
                    "AllMediaLinkManager"));
            }
            if(mediaLinkManager == null)
            {
                mediaLinkManager = new LinkManager(); 
            }

            string favoriteManagerPath = Path.Combine(FilePaths.rootPath, "AllMediaFavoriteManager");
            if (File.Exists(favoriteManagerPath))
            {
                favoriteManager = JSONSerialManager.deserialize<FavoriteManager>(Path.Combine(FilePaths.rootPath,
                    "AllMediaFavoriteManager"));
            }
            if(favoriteManager == null)
            {
                favoriteManager = new FavoriteManager(); 
            }
        }

        private static void loadMediaIntoCollection()
        {
            List<DayMediaObservableCollection> days = new List<DayMediaObservableCollection>(MediaManager.allMediaDict.Values);
            days.Reverse(); 

            allMediaCollection = new FlowObservableCollection<DayMediaObservableCollection>(days);
        }

        private async static Task<SortedDictionary<DateTime, DayMediaObservableCollection>> readDayMediaAsync()
        {
            allMedia = new List<AbMediaContent>(); 
            allMediaDict = new SortedDictionary<DateTime, DayMediaObservableCollection>(); 
            List<AbMediaContent> sortedMedia = await readMediaSortAsync();

            if (sortedMedia.Count == 0) return allMediaDict;

            // temp create multiple days 
            int startIndex = 0; 
            List<AbMediaContent> curMediaList = new List<AbMediaContent>();
            DateTime curDateTime = sortedMedia[0].creationTime;

            /*
            if (sortedMedia.Count > 10)
            {
                for(int i = 0; i<5; i++)
                {
                    curMediaList.Add(sortedMedia[i]); 
                }
                allMediaDict.Add( curDateTime.AddDays(-2), new DayMediaObservableCollection(curDateTime.AddDays(-2), curMediaList));
                curMediaList = new List<AbMediaContent>(); 

                for (int i = 5; i<10; i++)
                {
                    curMediaList.Add(sortedMedia[i]); 
                }
                allMediaDict.Add(curDateTime.AddDays(-1), new DayMediaObservableCollection(curDateTime.AddDays(-1), curMediaList));
                curMediaList = new List<AbMediaContent>();

                startIndex = 10; 
            }
            */

            // Media sorted from earliest to latest 
            for (int i = startIndex; i<sortedMedia.Count; i++)
            {
                AbMediaContent media = sortedMedia[i]; 

                allMedia.Add(media); 

                if(media.creationTime.Date == curDateTime.Date)
                {
                    curMediaList.Add(media); 
                }
                else
                {
                    allMediaDict.Add(curDateTime.Date, new DayMediaObservableCollection(curDateTime,curMediaList));
                    curDateTime = media.creationTime;
                    curMediaList = new List<AbMediaContent> { media };
                }
            }
            allMediaDict.Add(curDateTime.Date, new DayMediaObservableCollection(curDateTime, curMediaList)); 

            return allMediaDict; 
        }

        private async static Task<List<AbMediaContent>> readMediaSortAsync()
        {
            List<AbMediaContent> allReadMedia;
            allReadMedia = await MediaManager.deserializeMediaFilesAsync(FilePaths.allImagesDir);


            allReadMedia.AddRange(await MediaManager.deserializeMediaFilesAsync(FilePaths.allVideosDir));

            allReadMedia.AddRange(await MediaManager.deserializeMediaFilesAsync(FilePaths.allAudiosDir));

            allReadMedia.AddRange(await MediaManager.deserializeMediaFilesAsync(FilePaths.allNotesDir));

            allReadMedia.AddRange(await MediaManager.deserializeContactFilesAsyc(FilePaths.allContactsDir));

            
            //UserGalleryLoadManager galleryLoadManager = new UserGalleryLoadManager();
            //allReadMedia.AddRange(galleryLoadManager.loadImages()); 
            

            allReadMedia.Sort(new CardItemDateComparer<AbMediaContent>(true));

            return allReadMedia; 
        }

        private async static Task<List<AbMediaContent>> deserializeContactFilesAsyc(IFolder dir)
        {
            IList<IFile> files = await dir.GetFilesAsync();
            List<AbMediaContent> filesRead = new List<AbMediaContent>(files.Count);
            foreach (IFile file in files) { 

                ContactMediaContent media = JSONSerialManager.deserialize<ContactMediaContent>(file.Path);
                if (media != null)
                {
                    // Unfortunately ContactMedia fileds are not loaded when its JSON constructor is called 
                    media.initialize(media.name, media.phoneNumber, media.imageFilePath);
                    filesRead.Add(media);
                }
            }
            return filesRead;
        }
        private async static Task<List<AbMediaContent>> deserializeMediaFilesAsync(IFolder dir)
        {
            IList<IFile> files = await dir.GetFilesAsync();
            List<AbMediaContent> filesRead = new List<AbMediaContent>(files.Count);
            foreach (IFile file in files)
            {
                filesRead.Add(AbMediaContent.createMediaContent(FileUtilities.filePathToMediaType(file.Path), file.Path));
            }
            return filesRead;
        }


    }
}
