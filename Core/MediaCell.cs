using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Cards.Core
{
    public abstract class AbMediaCell : ViewCell
    {
        //All media has filepath on device 
        protected string filePath; 

        public AbMediaCell(string filePath)
        {
            createView(); 
        }

        protected abstract void createView(); 
    }

    public class ImageMediaCell : AbMediaCell
    {
        public ImageMediaCell(string filePath) : base(filePath)
        {
        }

        protected override void createView()
        {
            //instantiate each of our views
            var image = new Image();
            image.Source = ImageSource.FromFile(this.filePath); 

            StackLayout cellWrapper = new StackLayout();
            cellWrapper.Children.Add(image); 

            this.View = cellWrapper; 
        }
    }
}
