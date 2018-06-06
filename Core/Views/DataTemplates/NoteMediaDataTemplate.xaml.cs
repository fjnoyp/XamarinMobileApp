using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Cards.Core.Views.DataTemplates
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NoteMediaDataTemplate : ContentView
	{
		public NoteMediaDataTemplate ()
		{
			InitializeComponent ();
		}
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
        }
    }
}