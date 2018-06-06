using Android.Gms.Tasks;
using Cards.Core.FileReaders;
using Cards.Core.Utilities;
using Firebase.Database;
using Firebase.Storage;
using Java.Lang;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cards.Core.Platform.Manager
{
    public class FirebaseDownloadManager : IChildListener
    {

        /// <summary>
        /// Listen to OtherCards database for new shared cards 
        /// </summary>
        public async void initialize()
        {
            string userID = await AcrDialogUtilities.promptInput("Input User ID");

            if (userID == "") return; 

            // FirebaseDatabase - get cards shared with userID 
            FirebaseDatabase database = FirebaseDatabase.Instance;
            DatabaseReference dr_userCardListItem = database.GetReference("Users").Child(userID).Child("OthersCards");

            Query sharedCardsQuery = dr_userCardListItem.LimitToFirst(100);
            sharedCardsQuery.AddChildEventListener(new FirebaseChildListener(this)); 
        }

        private List<string> destringifyCardMediaNames(string mediaNames)
        {
            if (mediaNames[0] == ' ') mediaNames = mediaNames.Substring(1); 

            return new List<string>(mediaNames.Split(' ')); 
        }

        // TODO delete card when all users have downloaded it 
        public async void downloadCard(string fromUserID, string cardID, string mediaNames, string cardName)
        {
            // Get proper storage reference to card to download 
            FirebaseStorage storage = FirebaseStorage.Instance;
            StorageReference sr_card = storage.GetReference("Users")
                .Child(fromUserID)
                .Child(cardID);

            // Create directory for this card 
            IFolder cardDir = await FileSystem.Current.LocalStorage.CreateFolderAsync(
                Path.Combine(FilePaths.allShareCardsPath, cardID), CreationCollisionOption.ReplaceExisting); 

            // Load in card media files
            StorageReference sr_cardMedia = sr_card.Child("Media"); 
            List<string> mediaNamesList = destringifyCardMediaNames(mediaNames);
            List<Java.IO.File> mediaFiles = new List<Java.IO.File>(mediaNamesList.Count);
            Card card = await FSManager.addNewCardAsync(cardName + " from " + fromUserID);

            MediaDownloadListener downloadListener = new MediaDownloadListener(mediaNamesList.Count, card, mediaFiles);

            foreach(string mediaName in mediaNamesList)
            {
                if (mediaName != null && mediaName != "")
                {
                    Java.IO.File file = new Java.IO.File(cardDir.Path, mediaName);
                    sr_cardMedia.Child(mediaName).GetFile(file).AddOnSuccessListener(downloadListener);

                    // TODO add download failure and success handling 

                    mediaFiles.Add(file);
                }
            }

            // TODO deletion support for shared cards !!!! 
            // Create new card media contents 
           


            
        }

        public async void childAdded(DataSnapshot snapshot, string previousChildName)
        {
            // Card metadata 
            string fromUserID = (string)snapshot.Child("FromUserID").GetValue(true);
            string cardID = (string)snapshot.Child("CardID").GetValue(true);
            string mediaNames = (string)snapshot.Child("MediaNames").GetValue(true);
            string cardName = (string)snapshot.Child("Name").GetValue(true);

            //Ask user if accept card 
            if (await AcrDialogUtilities
                .promptConfirm("Do you want to accept a card from " + fromUserID + "?"))
            {
                downloadCard(fromUserID, cardID, mediaNames, cardName); 
            }
        }

        public void childChanged(DataSnapshot snapshot, string previousChildName)
        {
        }

        public void childMoved(DataSnapshot snapshot, string previousChildName)
        {
        }

        public void childRemoved(DataSnapshot snapshot)
        {
        }

    }

    /// <summary>
    /// Waits until all media loaded before creating its card 
    /// </summary>
    public class MediaDownloadListener : Java.Lang.Object, IOnSuccessListener
    {
        int totalMediaCount;
        int targetMediaCount;

        Card card;
        List<Java.IO.File> mediaFiles; 

        public MediaDownloadListener(int targetMediaCount, Card card, List<Java.IO.File> mediaFiles)
        {
            this.targetMediaCount = targetMediaCount;

            this.card = card;
            this.mediaFiles = mediaFiles; 
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            targetMediaCount--; 

            if(targetMediaCount == 0)
            {
                createCard(); 
            }
        }

        private void createCard()
        {
            foreach (Java.IO.File file in mediaFiles)
            {
                AbMediaContent mediaContent = AbMediaContent
                    .createMediaContent(FileUtilities.filePathToMediaType(file.Path), file.Path);

                card.addMedia(mediaContent);
            }
        }
    }
}
