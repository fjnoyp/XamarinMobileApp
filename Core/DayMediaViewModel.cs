using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Cards.Core
{
    /// <summary>
    /// Representation of media for a day 
    /// </summary>
    public class DayMediaViewModel
    {
        public DateTime date { get; set; }
        public ObservableCollection<AbMediaContent> media { get; set; }
    }
}
