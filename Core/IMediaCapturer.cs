using System;
using System.Collections.Generic;
using System.Text;

namespace Cards.Core
{
    public interface IMediaCapturer
    {
        AbMediaContent capturedMedia { get; set; } 
    }
}
