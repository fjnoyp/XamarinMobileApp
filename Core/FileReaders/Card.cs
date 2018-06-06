using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Cards.Core.FileReaders
{
    public class Card : INotifyPropertyChanged, ISortableCardItem
    {
        public string name { get; set; }

        [JsonIgnore]
        private LinkManager _linkManager;
        public LinkManager linkManager
        {
            get {
                if (_linkManager == null)
                    _linkManager = new LinkManager(); 
                return _linkManager; }
            set
            {
                _linkManager = value; 
            }
        } 

        [JsonIgnore]
        private DateTime _creationTime;

        public DateTime creationTime
        {
            get { return _creationTime; }
            set
            {
                _creationTime = value;
                OnPropertyChanged("creationTime");
            }
        }

        public string lastModTime { get { return creationTime.ToString("d");  } }

        private bool _isFavorited;
        public bool isFavorited
        {
            get { return _isFavorited; }
            set
            {
                _isFavorited = value;
                OnPropertyChanged("isFavorited");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [JsonProperty]
        public ObservableCollection<AbMediaContent> mediaList = new ObservableCollection<AbMediaContent>();


        public void addMedia(AbMediaContent media)
        {
            mediaList.Add(media);

            this.saveToFile();
        }

        public void removeMedia(AbMediaContent media)
        {
            mediaList.Remove(media);

            this.saveToFile();
        }

        public bool containsMedia(AbMediaContent media)
        {
            return mediaList.Contains(media);
        }

        public void saveToFile()
        {
            JSONSerialManager.serialize(Path.Combine(FilePaths.allCardsPath, this.name), this);
        }

        public override string ToString()
        {
            return name;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
