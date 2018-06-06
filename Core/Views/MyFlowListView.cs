using DLToolkit.Forms.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cards.Core.Views
{
    public class MyFlowListView : FlowListView
    {
        public MyFlowListView() : base(Xamarin.Forms.ListViewCachingStrategy.RecycleElementAndDataTemplate)
        {
        }
    }
}
