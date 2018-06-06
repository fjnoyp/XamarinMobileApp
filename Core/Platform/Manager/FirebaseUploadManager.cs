using Acr.UserDialogs;
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
using System.Threading.Tasks;

namespace Cards.Core.Platform.Manager
{

    // Card and UserID need to be globally unique ... 

    public class FirebaseUploadManager
    {
        

        public async void uploadCard(Card card)
        {
            // TODO HARDCODED USER ID 
            string userID = await AcrDialogUtilities.promptInput("Input User ID");
            string sendToUserID = await AcrDialogUtilities.promptInput("Input Share to User ID");

            if (userID == "" || sendToUserID == "") return; 

            // FirebaseDatabase - Update userID with card 
            FirebaseDatabase database = FirebaseDatabase.Instance;
            DatabaseReference dr_curUser = database.GetReference("Users").Child(userID);

            DatabaseReference dr_curUserCards = dr_curUser.Child("MyCards");
            DatabaseReference dr_curCard = dr_curUserCards.Push(); // Unique card ID generated 

            dr_curCard.Child("Name").SetValue(card.name);
            dr_curCard.Child("UploadFinished").SetValue(false);
            dr_curCard.Child("MediaNames").SetValue(stringifyCardMediaNames(card));

            // FirebaseDatabase - Update sendToUserID with card 
            DatabaseReference dr_userCardListItem = database.GetReference("Users").Child(sendToUserID).Child("OthersCards").Push();
            dr_userCardListItem.Child("FromUserID").SetValue(userID);
            dr_userCardListItem.Child("CardID").SetValue(dr_curCard.Key);
            dr_userCardListItem.Child("MediaNames").SetValue(stringifyCardMediaNames(card));
            dr_userCardListItem.Child("Name").SetValue(card.name); 

            // FirebaseStorage - Store card 
            FirebaseStorage storage = FirebaseStorage.Instance;
            StorageReference sr_cud = storage.GetReference("Users").Child(userID);

            StorageReference sr_curCard = sr_cud.Child(dr_curCard.Key); // Use unique card ID 
            StorageReference sr_cardMedias = sr_curCard.Child("Media");

            uploadCardMedia(sr_cardMedias, card);

            // Mark upload finished 
            dr_curCard.Child("UploadFinished").SetValue(true); 
        }

        private string stringifyCardMediaNames(Card card)
        {
            string cardMediaNames = "";
            foreach(AbMediaContent media in card.mediaList)
            {
                string mediaFileName = Path.GetFileName(media.filePath);
                cardMediaNames += " " + mediaFileName;
            }

            // Remove starting blank 
            if (cardMediaNames[0] == ' ') cardMediaNames.Remove(0, 1); 

            return cardMediaNames; 
        }

        /// <summary>
        /// Encapsulate Card into a single file for uploading and sharing to Firebase 
        /// </summary>
        private void uploadCardMedia(StorageReference sr_cardMedias, Card card)
        {

            // Upload media to cloud 
            foreach (AbMediaContent media in card.mediaList)
            {
                string mediaFileName = Path.GetFileName(media.filePath);
                StorageReference sr_curCardMedia = sr_cardMedias.Child(mediaFileName); 

                FileStream fs = new FileStream(media.filePath, FileMode.Open);

                //StorageMetadata metadata = new StorageMetadata.Builder()
                //    .SetCustomMetadata("MediaContentType", media.mediaType.ToString()).Build();

                UploadTask uploadTask = sr_curCardMedia.PutStream(fs);

                UploadListener uploadListener = new UploadListener(fs);
                uploadTask.AddOnFailureListener(uploadListener);
                uploadTask.AddOnSuccessListener(uploadListener); 

            }

            // TODO add upload failure and success handling? 
        }

        public class UploadListener : Java.Lang.Object, IOnFailureListener, IOnSuccessListener
        {
            FileStream fs; 

            public UploadListener(FileStream fs)
            {
                this.fs = fs; 
            }

            public void OnFailure(Java.Lang.Exception e)
            {
                fs.Dispose(); 
            }

            public void OnSuccess(Java.Lang.Object result)
            {
                fs.Dispose();
            }
        }

    }
}
