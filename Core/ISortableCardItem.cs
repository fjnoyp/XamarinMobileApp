using System;
using System.Collections.Generic;
using System.Text;

namespace Cards.Core
{
    /// <summary>
    /// Basic sortable item.  Meant to be common interface for sorting of Card, Album, or MediaContent 
    /// </summary>
    public interface ISortableCardItem
    {
        DateTime creationTime { get; set; }
        string name { get; set; } 

        bool isFavorited { get; set; }

        void saveToFile(); 
    }
}
