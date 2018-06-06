using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Cards.Core.Views
{
    public class FixedLabel : Label
    {
        double width;
        double height; 

        protected override void OnSizeAllocated(double width, double height)
        {
            this.width = width;
            this.height = height; 

            base.OnSizeAllocated(width, height);
        }

        protected override SizeRequest OnSizeRequest(double widthConstraint, double heightConstraint)
        {
            return new SizeRequest(new Size(width, height)); 
            //return base.OnSizeRequest(widthConstraint, heightConstraint);
        }

    }
}
