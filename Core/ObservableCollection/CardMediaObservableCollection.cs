using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace Cards.Core
{
    /// <summary>
    /// Observable collection that displays media for a card 
    /// But selectively shows/hides media types and always has items in chronological order
    /// </summary>
    public class CardMediaObservableCollection : ObservableCollectionGroup<AbMediaContent>
    {

        private List<MediaContentType> mediaTypesToShow;
        private Dir card; 

        public CardMediaObservableCollection(Dir card) : base()
        {
            this.card = card; 

            mediaTypesToShow = new List<MediaContentType>() { MediaContentType.Image, MediaContentType.Video, MediaContentType.Audio, MediaContentType.Note };

            foreach (var mediaCollection in card.getAllMediaLists())
            {
                this.addParentCollection(mediaCollection);
            }

            ObservableCollectionSorter.Sort<AbMediaContent>(this, new CardItemDateComparer<AbMediaContent>(true)); 

        }

        /*
        public void toggleMediaVisibility(MediaContentType mediaType)
        {
            if (mediaTypesToShow.Contains(mediaType))
            {
                hideMediaType(mediaType); 
            }
            else
            {
                showMediaType(mediaType); 
            }
        }
        */

        public void showOnlyMediaType(MediaContentType mediaType)
        {
            hideAllMediaType();
            showMediaType(mediaType); 
        }
        public void showAllMediaType()
        {
            showMediaType(MediaContentType.Image);
            showMediaType(MediaContentType.Video);
            showMediaType(MediaContentType.Audio);
            showMediaType(MediaContentType.Note);
        }
        private void hideAllMediaType()
        {
            hideMediaType(MediaContentType.Image);
            hideMediaType(MediaContentType.Video);
            hideMediaType(MediaContentType.Audio);
            hideMediaType(MediaContentType.Note);
        }

        public bool isMediaTypeVisible(MediaContentType mediaType)
        {
            return mediaTypesToShow.Contains(mediaType); 
        }
        public bool isMediaTypeOnlyVisible(MediaContentType mediaType)
        {
            return mediaTypesToShow.Contains(mediaType) && mediaTypesToShow.Count == 1;
        }
        public void showMediaType(MediaContentType mediaType)
        {
            if (!mediaTypesToShow.Contains(mediaType))
            {
                mediaTypesToShow.Add(mediaType);
                addItems(card.getMediaListByType(mediaType)); 
            }
        }
        public void hideMediaType(MediaContentType mediaType)
        {
            if (mediaTypesToShow.Contains(mediaType))
            {
                mediaTypesToShow.Remove(mediaType);
                removeItems(card.getMediaListByType(mediaType));
            }
        }


        protected override void addItems(IEnumerable<AbMediaContent> medias)
        {
            foreach(AbMediaContent media in medias)
            {
                if (mediaTypesToShow.Contains(media.mediaType))
                {
                    chronAdd(media);
                }
            }
        }

        //Add media into its chronological position 
        protected void chronAdd(AbMediaContent media)
        {
            //TO DO: switch to binary insert 
            for(int i = 0; i<this.Count; i++)
            {
                if(this[i].creationTime < media.creationTime)
                {
                    this.InsertItem(i, media);
                    return; 
                }
            }
            this.Add(media);
        }

    }
}
