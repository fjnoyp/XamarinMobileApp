using Cards.Core.FileReaders;
using PCLStorage;
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
	public partial class NoteView : ContentView, IMediaCapturer, IDisappearedListener
    {
        NoteMediaContent noteMedia;
        Card parentCard;
        Page parentPage;

        public AbMediaContent capturedMedia { get; set; }

        public NoteView()
        {
            InitializeComponent();
        }

        //Open existing note 
        public NoteView(bool isEditable, AbMediaContent media)
        {
            InitializeComponent();

            this.noteMedia = media as NoteMediaContent;
            this.titleEntry.Text = noteMedia.name;
            this.bodyEditor.Text = noteMedia.bodyText;

            this.bodyEditor.IsEnabled = isEditable;
            this.titleEntry.IsEnabled = isEditable;

            this.titleEntry.Focus();

            this.parentPage = Navigation.NavigationStack.LastOrDefault(); 
        }

        //Create new note 
        public NoteView(Card card, Page parentPage)
        {
            InitializeComponent();
            this.parentCard = card;
            this.titleEntry.Text = "new note \n" + DateTime.Now.Date.ToString("d"); // card.name + " note " + card.getMediaListByType(MediaContentType.Note).Count;

            this.bodyEditor.Focus(); 

            this.parentPage = parentPage; 
        }

        void titleEntry_Completed(object sender, EventArgs e)
        {
            //var text = ((Entry)sender).Text;
            this.bodyEditor.Focus();
        }


        bool firstSizeAllocated = true; 
        protected override void OnSizeAllocated(double width, double height)
        {
            if (firstSizeAllocated)
            {
                this.titleEntry.Focus(); 
                firstSizeAllocated = false; 
            }
            
            base.OnSizeAllocated(width, height);
        }

        /* Code runs when switching focus back to title entry .... 
        void bodyEditor_Completed(object sender, EventArgs e)
        {
            //var text = ((Editor)sender).Text;
            saveChangesAsync().Wait();
        }
        */

        private async void entry_Unfocused(object sender, FocusEventArgs e)
        {
            await saveChangesAsync();

            if(titleEntry.IsFocused || bodyEditor.IsFocused)
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
            titleEntry.Unfocus();
            bodyEditor.Unfocus(); 
        }


        private async Task saveChangesAsync()
        {
            if (titleEntry.Text == "")
            {
                await parentPage.DisplayAlert("Warning", "Note must have title", "Ok");
                return;
            }

            string filePath;


            //If null we are creating new text file 
            if (this.noteMedia == null)
            {
                filePath = Path.Combine(FilePaths.allNotesPath, $@"{Guid.NewGuid()}.txt");
                IFile file = await FileSystem.Current.LocalStorage.CreateFileAsync(filePath, CreationCollisionOption.FailIfExists);

            }
            else
            {
                filePath = noteMedia.filePath;
            }

            // Write to text file 
            IFile textFile = await FileSystem.Current.GetFileFromPathAsync(filePath);
            await textFile.WriteAllTextAsync(titleEntry.Text + System.Environment.NewLine + bodyEditor.Text);

            // If null add to card
            if (this.noteMedia == null)
            {
                this.noteMedia = new NoteMediaContent(filePath);
                this.noteMedia.name = titleEntry.Text;
                this.noteMedia.bodyText = bodyEditor.Text;
                parentCard?.addMedia(this.noteMedia);
                MediaManager.addNewMedia(this.noteMedia);

            }
            else
            {
                // Update actual note media instance in memory 
                this.noteMedia.name = titleEntry.Text;
                this.noteMedia.bodyText = bodyEditor.Text;
            }
            capturedMedia = noteMedia;
            // End page 
            // await Navigation.PopAsync();
        }

        public void onDisappear()
        {
            saveChangesAsync();
        }
    }
}