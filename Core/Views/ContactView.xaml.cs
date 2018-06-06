using Cards.Core.FileReaders;
using PCLStorage;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Cards.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactView : ContentView, IMediaCapturer, IDisappearedListener
    {
        ContactMediaContent contactMedia;
        Card parentCard;
        Page parentPage;

        string imageFilePath = "";

        public AbMediaContent capturedMedia { get; set; }

        private Size origContactImageSize; 

        public ContactView()
        {
            InitializeComponent();
        }

        //Open existing contact 
        public ContactView(bool isEditable, AbMediaContent media)
        {
            InitializeComponent();

            this.contactMedia = media as ContactMediaContent;
            this.contactImage.Source = contactMedia.image;
            this.imageFilePath = contactMedia.imageFilePath; 
            this.nameEntry.Text = contactMedia.name;
            this.phoneEntry.Text = contactMedia.phoneNumber;

            /*
            if(imageFilePath == null || imageFilePath == "")
            {
                this.changeImageLabel.Text = "Set Image";   
            }
            else
                this.changeImageLabel.Text = "Change Image";
                */

            this.parentPage = Navigation.NavigationStack.LastOrDefault();

            
        }

        //Create new contact
        public ContactView(Card card, Page parentPage)
        {
            InitializeComponent();

            this.parentCard = card;
            this.parentPage = parentPage;

            //this.changeImageLabel.Text = "Set Image";
        }

        private async void gallery_Tapped(object sender, EventArgs e)
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                parentPage.DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                return;
            }

            var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,

            });

            if (file == null)
                return;

            // Move file to contact info 

            string fileExtension = Path.GetExtension(file.Path);
            string newFileLocation = Path.Combine(FilePaths.allContactInfosPath, $@"{Guid.NewGuid()}" + fileExtension);

            File.Move(file.Path, newFileLocation);

            // store string of new contact image location 
            this.imageFilePath = newFileLocation;
            contactImage.Source = ImageSource.FromFile(imageFilePath);
        }
        private async void camera_Tapped(object sender, EventArgs e)
        {
            AbMediaContent image = await MediaCaptureUtilities.takePicture(parentCard, parentPage);

            if (image != null)
            {
                this.imageFilePath = image.filePath;
                contactImage.Source = ImageSource.FromFile(imageFilePath);
            }
        }

        private bool contactImageExpanded = false; 
        private void contactImage_Tapped(object sender, EventArgs e)
        {
            if(this.origContactImageSize.Height.Equals(0))
            {
                origContactImageSize = new Size(contactImage.Width, contactImage.Height); 
            }

            contactImageExpanded = !contactImageExpanded;

            if (contactImageExpanded)
            {
                contactImage.HeightRequest = origContactImageSize.Height * 3;
                contactImage.WidthRequest = origContactImageSize.Width * 3;

                contactImageControlLayout.IsVisible = false;
                parentLayout.ForceLayout(); 
            }
            else
            {
                contactImage.HeightRequest = origContactImageSize.Height;
                contactImage.WidthRequest = origContactImageSize.Width; 

                contactImageControlLayout.IsVisible = true;
                parentLayout.ForceLayout();
            }

        }

        private async void entry_Unfocused(object sender, FocusEventArgs e)
        {
            await saveChangesAsync();

            if(nameEntry.IsFocused || phoneEntry.IsFocused)
            {

            }
            else
                doneButton.IsEnabled = false;


        }
        private void entry_Focused(object sender, FocusEventArgs e)
        {
            doneButton.IsEnabled = true;
        }


        private async void doneButton_Clicked(object sender, EventArgs e)
        {
            nameEntry.Unfocus();
            phoneEntry.Unfocus(); 
        }

        private async Task saveChangesAsync()
        {
            if (nameEntry.Text == null || nameEntry.Text == "")
            {
                await parentPage.DisplayAlert("Warning", "Contact must have name", "Ok");
                return;
            }

            string filePath; 

            if(this.contactMedia == null)
            {
                filePath = Path.Combine(FilePaths.allContactsPath, $@"{Guid.NewGuid()}" + ".contact");
                IFile file = await FileSystem.Current.LocalStorage.CreateFileAsync(filePath, CreationCollisionOption.FailIfExists);
            }
            else
            {
                filePath = contactMedia.filePath; 
            }

            ContactMediaContent newContactMedia = contactMedia;

            // update fields on contact media 
            if (newContactMedia == null) newContactMedia = new ContactMediaContent(filePath);
            newContactMedia.initialize(nameEntry.Text, phoneEntry.Text, imageFilePath);
            

            // If null add to card
            if (this.contactMedia == null)
            {
                parentCard?.addMedia(newContactMedia);
                MediaManager.addNewMedia(newContactMedia); 
            }

            this.capturedMedia = newContactMedia;
            this.contactMedia = this.capturedMedia as ContactMediaContent; 
            JSONSerialManager.serialize(filePath, newContactMedia); 

            //await Navigation.PopAsync(); 
        }

        public void onDisappear()
        {
            saveChangesAsync();
        }
    }
}