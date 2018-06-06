using System;
using System.Collections.Generic;
using System.Text;

namespace Cards
{
    public class Song
    {
        public string Artist { get; set; }
        public string Timestamp { get; set; }
        public string TrackId { get; set; }
        public string Title { get; set; }

        public override string ToString()
        {
            return (Title);
        }
    }
}
