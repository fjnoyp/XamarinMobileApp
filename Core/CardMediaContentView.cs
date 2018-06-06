using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Cards.Core.Views
{
    /// <summary>
    /// For content views that have bindable media and card properties 
    /// </summary>
    public class CardMediaData
    {
        
        
        //Media Bindable Property 
         
        public static readonly BindableProperty mediaProperty = BindableProperty.Create(
                  propertyName: "media",
                  returnType: typeof(AbMediaContent),
                  declaringType: typeof(CardMediaDataModel),
                  defaultValue: null,
                  defaultBindingMode: BindingMode.TwoWay,
                  propertyChanged: mediaPropertyChanged);

        public AbMediaContent media
        {
            get { return base.GetValue(mediaProperty) as AbMediaContent; }
            set { base.SetValue(mediaProperty, value); }
        }

        private static void mediaPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CardMediaContentView)bindable;
            control.media = newValue as AbMediaContent; 
        }

        
        //Card Bindable Property 
        
        public static readonly BindableProperty cardProperty = BindableProperty.Create(
                  propertyName: "card",
                  returnType: typeof(Dir),
                  declaringType: typeof(CardMediaContentView),
                  defaultValue: null,
                  defaultBindingMode: BindingMode.TwoWay,
                  propertyChanged: cardPropertyChanged);

        public Dir card
        {
            get { return base.GetValue(cardProperty) as Dir; }
            set { base.SetValue(cardProperty, value); }
        }

        private static void cardPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CardMediaContentView)bindable;
            control.card = newValue as Dir;
        }
        
    }
}
