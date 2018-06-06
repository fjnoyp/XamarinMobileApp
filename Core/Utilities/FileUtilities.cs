using System;
using System.Collections.Generic;
using System.Text;

namespace Cards.Core.Utilities
{
    public static class FileUtilities
    {
        public static MediaContentType filePathToMediaType(string filePath)
        {
            if (filePath.EndsWith(".jpg"))
            {
                return MediaContentType.Image;
            }
            else if (filePath.EndsWith(".mp4"))
            {
                return MediaContentType.Video;
            }
            else if (filePath.EndsWith(".wav"))
            {
                return MediaContentType.Audio;
            }
            else if (filePath.EndsWith(".txt"))
            {
                return MediaContentType.Note;
            }
            else if (filePath.EndsWith(".contact"))
            {
                return MediaContentType.Contact;
            }
            else
            {
                return MediaContentType.Contact;
                //throw new Exception("Media readin error, file read is not of recognized media type");
            }
        }
    }
}
