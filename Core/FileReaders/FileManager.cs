using System;
using System.Collections.Generic;
using System.Collections; 

using Newtonsoft.Json;
using System.IO;
using PCLStorage;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using DLToolkit.Forms.Controls;
using Cards.Core.FileReaders;
using Cards.Core.Platform.Manager;

namespace Cards.Core
{
    public enum CardCategory { People, Event, Private, Other }
    public enum DirType { Album, Card }

    /// <summary>
    /// Manages retrieval and writing of JSON album, card, and media objects 
    /// Albums and Cards stored as JSON file 
    /// All Media stored into parent file subdirectories 
    /// Manages creation/deletion of all albums, cards, and respective media 
    /// </summary>
    public static class FSManager
    {

        private static Dictionary<String,Card> allCardsDict { get; set; } //every card name must be unique otherwise section card view and all card views will have duplicate  names 

        // TODO created ObservableCollection of Card !!! 
        public static List<Card> getAllCards()
        {
            return new List<Card>(allCardsDict.Values); 
        }
        public static int getNumCards()
        {
            return allCardsDict.Count; 
        }
        public static List<Card> getAllFavCards()
        {
            List<Card> allCards = getAllCards();
            return allCards.Where(x => x.isFavorited).ToList();
        }


        public static bool cardContainsMedia(AbMediaContent media)
        {
            foreach(Card card in allCardsDict.Values)
            {
                if (card.containsMedia(media)) return true; 
            }
            return false; 
        }
        public static void removeMediaFromCards(AbMediaContent media)
        {
            foreach(Card card in allCardsDict.Values)
            {
                if (card.containsMedia(media))
                {
                    card.removeMedia(media); 
                }
            }
        }

        /// <summary>
        /// Must be called in starting activity!
        /// </summary>
        public async static Task initializeAsync()
        {
            await FilePaths.initFoldersAsync(); 

            allCardsDict = new Dictionary<string, Card>();

            //Read in and deserialize all JSON file cards 
            List<Card> allCards = await JSONSerialManager.deserializeDirAsync<Card>(FilePaths.allCardsDir);
            foreach (Card card in allCards)
            {
                //NOTE: exception handling for messed up serialization leading to null writes 
                if (card != null)
                    allCardsDict.Add(card.name, card);
            }

            //Read in all MEDIA 
            await MediaManager.initializeAsync();

            //FirebaseDownloadManager downloadManager = new FirebaseDownloadManager();
            //downloadManager.initialize(); 

        }

        public static async Task<Card> addNewCardAsync(string name)
        {
            //TODO: DO NOT ALLOW DUPLICATE CARD NAMES 
            //if (allCardsDict.ContainsKey(name)) throw new Exception("ERROR: New Card name already exists, this may corrupt file structure");

            // For now alllow overwrite of existing card, this happens when an online card is accepted again
            if (allCardsDict.ContainsKey(name))
            {
                allCardsDict.Remove(name); 
            }

            //Create new card and add to album and storage 
            Card newCard = new Card { name = name, creationTime = DateTime.Now, linkManager = new LinkManager() }; 
          
            allCardsDict.Add(name, newCard);

            //Serialize JSON to a newly created file 
            IFile newCardFile = await FileSystem.Current.LocalStorage.CreateFileAsync(Path.Combine(FilePaths.allCardsPath, name), CreationCollisionOption.ReplaceExisting); 
            JSONSerialManager.serialize(newCardFile.Path, newCard);

            return newCard; 
        }

        /// <summary>
        /// Delete card file, card from dictionary, and associated media 
        /// </summary>
        /// <param name="cardName"></param>
        public async static void deleteCardAsync(string cardName)
        {
            // Don't do anything if no card
            if (!FSManager.cardExists(cardName)) return; 

            // Delete card file 
            IFile cardFile = await FileSystem.Current.LocalStorage.GetFileAsync(Path.Combine(FilePaths.allCardsPath, cardName));
            await cardFile.DeleteAsync(); 

            // Remove card from allCardDict 
            Card card;
            allCardsDict.TryGetValue(cardName, out card);

            // Remove card from dict 
            allCardsDict.Remove(cardName);
            
        }

        public static Card getCard(string cardName)
        {
            if (!cardExists(cardName)) return null; 

            Card card;
            allCardsDict.TryGetValue(cardName, out card);
            return card; 
        }
         
        public static bool cardExists(string name)
        {
            if (name == null) return false; 
            
            return allCardsDict.ContainsKey(name); 
        }

    }
    
}
