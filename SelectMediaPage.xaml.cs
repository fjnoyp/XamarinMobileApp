using DLToolkit.Forms.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Cards
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectMediaPage : ContentPage
    {
        public SelectMediaPage(FlowListView flowListView)
        {


            this.Content = flowListView; 
        }
    }
}