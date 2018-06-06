using Cards.Core.Views.DataTemplates;
using DLToolkit.Forms.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Cards.Core
{
    // NOT WORKING WRONG TEMPLATE IS SELECTED 
    public class MediaDataTemplateSelector : FlowTemplateSelector
    {

        public DataTemplate imageTemplate { get; set; }
        public DataTemplate videoTemplate { get; set; }
        public DataTemplate audioTemplate { get; set; }
        public DataTemplate noteTemplate { get; set; }

        /*
        readonly DataTemplate imageTemplate = new DataTemplate(typeof(ImageMediaDataTemplate));
        readonly DataTemplate videoTemplate = new DataTemplate(typeof(VideoMediaDataTemplate));
        readonly DataTemplate noteTemplate = new DataTemplate(typeof(NoteMediaDataTemplate));
        readonly DataTemplate audioTemplate = new DataTemplate(typeof(AudioMediaDataTemplate)); 
        */

        protected override DataTemplate OnSelectTemplate(object item, int columnIndex, BindableObject container)
        {
            if (item is ImageMediaContent)
                return imageTemplate;
            else if (item is VideoMediaContent)
                return videoTemplate;
            else if (item is AudioMediaContent)
                return audioTemplate;
            else if (item is NoteMediaContent)
                return noteTemplate;
            else throw new Exception("broken wrong media type");
        }

    }
}
