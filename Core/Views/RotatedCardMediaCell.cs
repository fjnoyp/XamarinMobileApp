using System;
using System.Collections.Generic;
using System.Text;

namespace Cards.Core.Views
{

    /// <summary>
    /// Rotated version of CardMediaCell
    /// Includes additional support for displaying CardMediaContent
    /// </summary>
    public class RotatedCardMediaCell : CardMediaCell
    {
        public RotatedCardMediaCell() : base()
        {
            this.Rotation = 90; 
        }

        protected override void OnBindingContextChanged()
        {
            AbMediaContent media = this.BindingContext as AbMediaContent; 

            if(media.mediaType == MediaContentType.Card)
            {
                hideAll();
                base.cardMediaVisible(); 
            }
            else
                base.OnBindingContextChanged();
        }
    }
}
