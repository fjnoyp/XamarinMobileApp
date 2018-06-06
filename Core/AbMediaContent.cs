using System;

using System.Threading.Tasks;

using Newtonsoft.Json;
using FFImageLoading.Work;
using PCLStorage;
using Cards.Core;
using System.IO;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Cards.Core.Platform;
using System.Collections.Generic;
using System.Linq;
using Cards.Core.FileReaders;

namespace Cards.Core
{
    public enum MediaContentType { Image, Video, Note, Audio, Contact, MediaCount, Card };

    /// <summary>
    /// Common interface for representing a media in a list view and getting its View preview
    /// </summary>
    public abstract class AbMediaContent : INotifyPropertyChanged, ISortableCardItem
    {
        //TODO 11/21/17 --- do for setter value on setter !!!! 

        //Bindable properties for XAML forms
        //These will be used to display this media content in lists and in full screen 

        private Xamarin.Forms.ImageSource _image;
        [JsonIgnore] //This is a property because image/video thumbnail generation sets this property after completion
        public Xamarin.Forms.ImageSource image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged("image"); 
            }
        }

        public virtual String displayText { get; set;  }
        public virtual String name { get; set; }
        public DateTime creationTime { get; set; }

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

        //Serialize file path 
        [JsonProperty]
        public string filePath { get; private set; }

        [JsonConstructor]
        public AbMediaContent(string filePath)
        {
                this.filePath = filePath;

                this.creationTime = File.GetCreationTime(filePath);
        }

        [JsonIgnore]
        public abstract MediaContentType mediaType
        {
            get;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        
        private bool _isSelected = false;
        [JsonIgnore]
        public bool isSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("isSelected");
            }
        }

        //TO DO , NOT SAVING FAVORITED MEDIA 
        public void saveToFile()
        {
            //WARNING NOT IMPLEMENTED 
        }

        public static AbMediaContent createMediaContent(MediaContentType mediaType, String filePath)
        {
            if(mediaType == MediaContentType.Image)
            {
                return new ImageMediaContent(filePath); 
            }
            else if(mediaType == MediaContentType.Video)
            {
                return new VideoMediaContent(filePath); 
            }
            else if(mediaType == MediaContentType.Note)
            {
                return new NoteMediaContent(filePath); 
            }
            else if(mediaType == MediaContentType.Audio)
            {
                return new AudioMediaContent(filePath); 
            }
            else if(mediaType == MediaContentType.Contact)
            {
                return new ContactMediaContent(filePath); 
            }
            else
            {
                throw new Exception("ERROR - AbMediaContent.createMediaContent incorrect mediaType given");
            }
        }

        public override bool Equals(object obj)
        {
            AbMediaContent otherMedia = obj as AbMediaContent;
            return otherMedia.filePath == this.filePath; 
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    /// <summary>
    /// IMediaContent for a Video file 
    /// </summary>
    public class VideoMediaContent : AbMediaContent
    {
        [JsonIgnore]
        public override string name { get { return ""; } }

        public override MediaContentType mediaType { get { return MediaContentType.Video; } }

        [JsonConstructor]
        public VideoMediaContent(String filePath) : base(filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string thumbnailFilePath = Path.Combine(FilePaths.allVideoThumbnailsPath, fileName + ".jpg");

            if (!File.Exists(thumbnailFilePath))
            {
                Task.Run(async () =>
               {
                   await VideoThumbnailHelper.createVideoThumbnailAsync(filePath);
                   this.image = Xamarin.Forms.ImageSource.FromFile(thumbnailFilePath);
               });
            }
            else
            {
                this.image = Xamarin.Forms.ImageSource.FromFile(
                       thumbnailFilePath);
            }
        }

    }

    /// <summary>
    /// IMediaContent for an Image file 
    /// </summary>
    public class ImageMediaContent : AbMediaContent
    {
        [JsonIgnore]
        public override string name { get { return ""; } }

        public override MediaContentType mediaType { get { return MediaContentType.Image; } }

        [JsonIgnore]
        public Xamarin.Forms.ImageSource fullImage { get; set; }

        [JsonConstructor]
        public ImageMediaContent(string filePath) : base(filePath)
        {
            //Get image 
            //this.text = filePath; 
            
            this.fullImage = Xamarin.Forms.ImageSource.FromFile(filePath);

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string thumbnailFilePath = Path.Combine(FilePaths.allImageThumbnailsPath, fileName + ".jpg");

            // ISSUE TODO image resize file saving not working on IOS 

            this.image = fullImage; 
            
            if (!File.Exists(thumbnailFilePath))
            {
                Task.Run(async () =>
                {
                    await ImageResizeHelper.createImageThumbnailAsync(filePath);
                    this.image = Xamarin.Forms.ImageSource.FromFile(thumbnailFilePath);
                });
            }
            else
            {
                this.image = Xamarin.Forms.ImageSource.FromFile(
                       thumbnailFilePath);
            }
            

        }

    }

    public class NoteMediaContent : AbMediaContent
    {
        public override MediaContentType mediaType { get { return MediaContentType.Note; } }

        //public event PropertyChangedEventHandler PropertyChanged;

        [JsonIgnore]
        private String _name; 
        [JsonIgnore]
        public override String name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("name");

                displayText = ""; //update display text
            }
        }

        [JsonIgnore]
        private String _bodyText;
        [JsonIgnore]
        public String bodyText
        {
            get { return _bodyText; }
            set
            {
                _bodyText = value;
                OnPropertyChanged("bodyText");

                displayText = ""; //update displayText; 
            }
        }

        [JsonIgnore]
        private String _displayText; 
        [JsonIgnore]
        public override string displayText
        {
            get { return _displayText;  }
            set
            {
                _displayText = name + "\n" + bodyText;
                OnPropertyChanged("displayText"); 
            }
        }

        [JsonConstructor]
        public NoteMediaContent(String filePath) : base(filePath)
        {
            initializeAsync(filePath);

            //this.image = Xamarin.Forms.ImageSource.FromFile("NoteIcon.png");
        }

        private async void initializeAsync(string filePath)
        {
           

            if (File.Exists(filePath))
            {
                IFile textFile = await FileSystem.Current.GetFileFromPathAsync(filePath);
                String fullText = await textFile.ReadAllTextAsync();

                //Extract out text before new line, this is title 
                String[] contents = fullText.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.None);

                this.name = contents[0];
                if (contents.Length > 1)
                    this.bodyText = contents[1];
            }
            else
            {
                IFile file = await FileSystem.Current.LocalStorage.CreateFileAsync(filePath, CreationCollisionOption.FailIfExists);
            }

        }


    }

    public class AudioMediaContent : AbMediaContent
    {
        public override MediaContentType mediaType { get { return MediaContentType.Audio; } }

        [JsonConstructor]
        public AudioMediaContent(String filePath) : base(filePath)
        {
            this.name = Path.GetFileNameWithoutExtension(filePath);

            //this.image = Xamarin.Forms.ImageSource.FromFile("MicIcon.png");
        }

    }

    public class ContactMediaContent : AbMediaContent
    {
        public override MediaContentType mediaType { get { return MediaContentType.Contact;  } }

        [JsonProperty]
        private String _name;
        [JsonProperty]
        public override String name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("name");
            }
        }

        [JsonProperty]
        public string phoneNumber { get; set; }

        [JsonProperty]
        public string imageFilePath { get; set; }

        [JsonConstructor]
        public ContactMediaContent(String filePath) : base(filePath)
        {
            /*
            if (File.Exists(filePath))
            {
                ContactMediaContent contactMedia = JSONSerialManager.deserialize<ContactMediaContent>(filePath);

                if (contactMedia != null)
                {

                    initialize(contactMedia.name, contactMedia.phoneNumber, contactMedia.imageFilePath);

                }
            
            }
            */

            if (imageFilePath == null || imageFilePath == "")
            {
                image = Xamarin.Forms.ImageSource.FromFile("ContactIcon.png");
            }
            else
            {
                image = Xamarin.Forms.ImageSource.FromFile(imageFilePath);
            }
        }

        public void initialize(string name, string phoneNumber, string imageFilePath)
        {
            this.name = name;
            this.phoneNumber = phoneNumber;
            this.imageFilePath = imageFilePath;

            if (imageFilePath == null || imageFilePath == "")
            {
                image = Xamarin.Forms.ImageSource.FromFile("ContactIcon.png");
            }
            else
            {
                image = Xamarin.Forms.ImageSource.FromFile(imageFilePath);
            }
        }
        
    }

    public class MediaCountBox : AbMediaContent
    {
        IMediaBoxParent parent;

        public List<AbMediaContent> mediaList;

        public int _mediaCount; 
        public int mediaCount
        {
            get { return _mediaCount; }
            set
            {
                _mediaCount = value;
                OnPropertyChanged("mediaCount"); 
            }
        }

        public bool isOpen = false; 

        public override MediaContentType mediaType => MediaContentType.MediaCount;

        // NOTE: imageFileName used as Unique Identifier for MediaBoxes 
        public MediaCountBox(String imageFileName, List<AbMediaContent> mediaList, IMediaBoxParent parent) : base(imageFileName)
        {
            if (mediaList == null) this.mediaList = new List<AbMediaContent>(); 
            else this.mediaList = mediaList; 

            mediaCount = mediaList.Count();

            this.parent = parent; 
            this.image = Xamarin.Forms.ImageSource.FromFile(imageFileName);
        }

        public void toggleOpen()
        {
            isOpen = !isOpen; 
            parent.toggleMediaBoxOpen(this); 
        }

        public void addNewMedia(AbMediaContent audioMedia)
        {
            mediaCount++;
            mediaList.Add(audioMedia); 
        }

        public void removeMedia(AbMediaContent audioMedia)
        {
            mediaCount--;
            mediaList.Remove(audioMedia); 
        }


    }

    /// <summary>
    /// Media content representation for a file 
    /// </summary>
    public class CardMediaContent : AbMediaContent
    {
        [JsonIgnore]
        public override MediaContentType mediaType => MediaContentType.Card;
        
        [JsonConstructor]
        public CardMediaContent(String filePath) : base(filePath) // card name used as unique identifier 
        {
            this.name = filePath;
        }


    }
}