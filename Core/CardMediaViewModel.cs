using Cards.Core.FileReaders;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Cards.Core
{
    public class CardMediaViewModel
    {
        
        public CardMediaViewModel bindingObject { get { return this; }  }

        public AbMediaContent media { get; set; }

        public Card card { get; set; }

        public ContentPage parentPage { get; set; }


        /*
        public CardMediaViewModel(AbMediaContent media, Dir card)
        {
            this.media = media;
            this.card = card;
  
        }
        */
    }
}
