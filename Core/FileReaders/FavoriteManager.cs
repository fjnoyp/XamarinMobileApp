using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cards.Core.FileReaders
{
    public class FavoriteManager
    {
        // Store which medias have been favorited 
        // This is because media in day view are not serialized 
        [JsonProperty]
        public Dictionary<AbMediaContent, bool> favoritedMedia;

        public FavoriteManager()
        {
            favoritedMedia = new Dictionary<AbMediaContent, bool>(); 
        }

        public void setFavorite(bool isFavorited, AbMediaContent media)
        {
            if (isFavorited) addFavorite(media);
            else removeFavorite(media);

            media.isFavorited = isFavorited; 
        }

        public void addFavorite(AbMediaContent media)
        {
            if (!favoritedMedia.ContainsKey(media))
            {
                favoritedMedia[media] = true; 
            }
        }

        public void removeFavorite(AbMediaContent media)
        {
            if (favoritedMedia.ContainsKey(media))
            {
                favoritedMedia.Remove(media); 
            }
        }

    }
}
