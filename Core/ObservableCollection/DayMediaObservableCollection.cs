using DLToolkit.Forms.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Cards.Core
{
    public class DayMediaObservableCollection : FlowObservableCollection<AbMediaContent>, IMediaBoxParent
    { 

        public DateTime date { get; private set; }

        // For flow list view group header template property must be named "Key"
        public string Key { get {
                return date.DayOfWeek.ToString() + "    " + date.ToString("d").Replace("/"," / "); } }

        public List<AbMediaContent> mediaList;

        private Dictionary<MediaContentType, MediaCountBox> mediaBoxes; 

        public DayMediaObservableCollection(DateTime date, List<AbMediaContent> mediaList) : base()
        {
            this.date = date;

            mediaBoxes = new Dictionary<MediaContentType, MediaCountBox>(); 
            
            // Create audio count box 
            List<AbMediaContent> audioList = mediaList.Where
                (media =>
                media.mediaType == MediaContentType.Audio).ToList<AbMediaContent>();
            MediaCountBox audioBox = new MediaCountBox("AudioFileIcon.png", audioList, this);
            this.mediaBoxes.Add(MediaContentType.Audio, audioBox);

            if (audioBox.mediaCount > 0)
                Add(audioBox); 
            
            /// TODO SLOW OPERATION 

            // Create note count box 
            List<AbMediaContent> noteList = mediaList.Where
                (media =>
                media.mediaType == MediaContentType.Note).ToList<AbMediaContent>();
            MediaCountBox noteBox = new MediaCountBox("NoteFileIcon.png", noteList, this);
            this.mediaBoxes.Add(MediaContentType.Note, noteBox);

            if(noteBox.mediaCount > 0)
                Add(noteBox);

            // Create contact count box 
            List<AbMediaContent> contactList = mediaList.Where
                (media =>
                media.mediaType == MediaContentType.Contact).ToList<AbMediaContent>();
            MediaCountBox contactBox = new MediaCountBox("ContactMediaIcon.png", contactList, this);
            this.mediaBoxes.Add(MediaContentType.Contact, contactBox);

            if (contactBox.mediaCount > 0)
                Add(contactBox);


            this.mediaList = mediaList.Where
                (media => 
                media.mediaType == MediaContentType.Image
                || media.mediaType == MediaContentType.Video).ToList<AbMediaContent>();
                

            AddRange(this.mediaList); 
            
        }

        public AbMediaContent getFirstItem()
        {
            return Items[0]; 
        }

        public DayMediaObservableCollection(DateTime date) : this(date, new List<AbMediaContent>())
        {
        }

        public void removeMedia(AbMediaContent media)
        {

            MediaContentType mediaType = media.mediaType;

            if (mediaBoxes.ContainsKey(mediaType))
            {
                MediaCountBox mediaBox = mediaBoxes[mediaType];

                mediaBox.removeMedia(media);
                if (mediaBox.isOpen) Remove(media);

                // Remove empty media boxes 
                if (mediaBox.mediaCount == 0)
                {
                    Remove(mediaBox);
                    mediaBox.isOpen = false; 
                }
            }

            else
            {
                Remove(media); 
            }
        }

        public void addNewMedia(AbMediaContent media)
        {

            MediaContentType mediaType = media.mediaType; 

            if (mediaBoxes.ContainsKey(mediaType))
            {
                MediaCountBox mediaBox = mediaBoxes[mediaType]; 

                mediaBox.addNewMedia(media);
                if (mediaBox.isOpen) Insert( IndexOf(mediaBox)+1, media);

                //Create media box if not present 
                if (mediaBox.mediaCount == 1) Insert(getMediaBoxIndex(mediaType), mediaBox); 
            }

            else
            {
                Insert(getMediaIndex(), media);
            }
        }

        private int getMediaIndex()
        {
            //Contact is the last index 
            if (mediaBoxes[MediaContentType.Contact].mediaCount > 0) return getIndexAfterMediaBox(MediaContentType.Contact);
            else if (mediaBoxes[MediaContentType.Note].mediaCount > 0) return getIndexAfterMediaBox(MediaContentType.Note);
            else return getIndexAfterMediaBox(MediaContentType.Audio); 
        }

        private int getMediaBoxIndex(MediaContentType mediaType)
        {
            int index; 

            // Always insert audio as first 
            if (mediaType == MediaContentType.Audio) return 0; 

            // If possible, insert CONTACT after note 
            else if(mediaType == MediaContentType.Contact)
            {
                index = getIndexAfterMediaBox(MediaContentType.Note);
                if (index != 0) return index; 
            }

            //Insert after audio 
            index = getIndexAfterMediaBox(MediaContentType.Audio);
            return index; 
            
        }

        private int getIndexAfterMediaBox(MediaContentType mediaType)
        {
            MediaCountBox mediaBox = mediaBoxes[mediaType];
            if (mediaBox.mediaCount > 0)
            {
                int mediaBoxIndex = IndexOf(mediaBox);
                if (mediaBox.isOpen)
                {
                    return mediaBoxIndex + mediaBox.mediaCount + 1;
                }
                else
                {
                    return mediaBoxIndex + 1;
                }
            }
            return 0; 
        }

        public void toggleMediaBoxOpen(MediaCountBox mediaBox)
        {
            if (mediaBox.isOpen)
            {
                int mediaBoxIndex = this.IndexOf(mediaBox)+1;
                addRange(mediaBoxIndex, mediaBox.mediaList);
            }
            else
            {
                removeRange(mediaBox.mediaList); 
            }
        }

        public void addRange(int index, List<AbMediaContent> mediaList)
        {
            foreach (AbMediaContent media in mediaList)
                Items.Insert(index, media);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, mediaList, mediaList.Count));
        }
        public void removeRange(List<AbMediaContent> mediaList)
        {
            foreach (AbMediaContent media in mediaList)
                Items.Remove(media);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, mediaList, mediaList.Count));
        }

        public override bool Equals(object obj)
        {
            //Note usage of Date property - assuming only one DayMediaOC per day 
            return date.Date.Equals((obj as DayMediaObservableCollection).date.Date);
        }
    }
}
