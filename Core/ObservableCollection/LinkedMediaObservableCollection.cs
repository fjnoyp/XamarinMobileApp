using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Cards.Core.ObservableCollection
{
    /// <summary>
    /// Displays an ObservableCollection of links 
    /// </summary>
    public class LinkedMediaObservableCollection : ObservableCollection<AbMediaContent>, IMediaBoxParent
    {
        private ObservableCollection<AbMediaContent> parentLinkCollection;
        private MediaCountBox cardBox; 

        public LinkedMediaObservableCollection (ObservableCollection<AbMediaContent> parentLinkCollection)
        {
            this.parentLinkCollection = parentLinkCollection;

            initialize(); 
        }


        public void initialize()
        {
            List<AbMediaContent> mediaList = parentLinkCollection.ToList<AbMediaContent>(); 

            //Create card count box 
            List<AbMediaContent> cardList = mediaList.Where
                (media =>
                media.mediaType == MediaContentType.Card).ToList<AbMediaContent>();
           cardBox = new MediaCountBox("CardIcon.png", cardList, this);

            if(cardBox.mediaCount > 0)
            {
                Add(cardBox); 
            }

            mediaList = mediaList.Where
                (media =>
                media.mediaType != MediaContentType.Card).ToList<AbMediaContent>();

            
            foreach(AbMediaContent media in mediaList)
            {
                Add(media); 
            }

            parentLinkCollection.CollectionChanged += collectionChanged;
        }

        private void collectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                IEnumerable<AbMediaContent> newItems = e.NewItems.Cast<AbMediaContent>();

                foreach(AbMediaContent media in newItems)
                {
                    if(media.mediaType == MediaContentType.Card)
                    {
                        cardBox.addNewMedia(media);
                        if (cardBox.isOpen) Insert(IndexOf(cardBox) + cardBox.mediaCount, media);

                        if (cardBox.mediaCount == 1) Insert(0, cardBox); 
                    }
                    else
                    {
                        Add(media); 
                    }
                }
            }
            else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                IEnumerable<AbMediaContent> oldItems = e.OldItems.Cast<AbMediaContent>();

                foreach (AbMediaContent media in oldItems)
                {
                    if (media.mediaType == MediaContentType.Card)
                    {
                        cardBox.removeMedia(media);

                        if (cardBox.isOpen) Remove(media);

                        if (cardBox.mediaCount == 0)
                        {
                            Remove(cardBox);
                            cardBox.isOpen = false; 
                        }
                    }
                    else
                    {
                        Remove(media);
                    }
                }
            }
        }



        public void toggleMediaBoxOpen(MediaCountBox mediaBox)
        {
            if (mediaBox.isOpen)
            {
                int mediaBoxIndex = this.IndexOf(mediaBox) + 1;

                // TODO SLOW OPERATION 
                List<AbMediaContent> mediaList = mediaBox.mediaList; 
                for(int i = mediaBox.mediaList.Count-1; i >= 0; i--) 
                {
                    Insert(mediaBoxIndex, mediaList[i]);
                }
            }
            else
            {
                // TODO SLOW OPERATION 
                foreach (AbMediaContent media in mediaBox.mediaList)
                {
                    Remove(media);
                }
            }
        }
    }
}
