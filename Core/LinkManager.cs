using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cards.Core
{

    //Manages links for media contained within a card 
    public class LinkManager
    {
        //Relate media file name to linked media file names 
        [JsonProperty]
        private Dictionary<string, ObservableCollection<AbMediaContent>> cardMediaLinks;

        public LinkManager()
        {
            cardMediaLinks = new Dictionary<string, ObservableCollection<AbMediaContent>>();
        }

        public void addLink(AbMediaContent rootMedia, AbMediaContent otherMedia)
        {
            if (linkExists(rootMedia, otherMedia)) return;

            ObservableCollection<AbMediaContent> links;
            cardMediaLinks.TryGetValue(rootMedia.filePath, out links);

            if (links == null)
                links = new ObservableCollection<AbMediaContent>();

            //Create new media content to reference to 
            if (!links.Contains(otherMedia))
                links.Add(otherMedia); 

            
            cardMediaLinks[rootMedia.filePath] = links;
        }
        public void removeLink(AbMediaContent rootMedia, AbMediaContent otherMedia)
        {
            ObservableCollection<AbMediaContent> links;
            cardMediaLinks.TryGetValue(rootMedia.filePath, out links);

            if (links == null)
                links = new ObservableCollection<AbMediaContent>();

            if (links.Contains(otherMedia))
                links.Remove(otherMedia);

            cardMediaLinks[rootMedia.filePath] = links;
        }

        public bool linkExists(AbMediaContent aMedia, AbMediaContent bMedia)
        {
            ObservableCollection<AbMediaContent> links;
            cardMediaLinks.TryGetValue(aMedia.filePath, out links);

            if (links == null) return false;
            return links.Contains(bMedia); 
        }

        public ObservableCollection<AbMediaContent> getLinkedMedia(AbMediaContent media)
        {
            if (!cardMediaLinks.ContainsKey(media.filePath))
            {
                cardMediaLinks[media.filePath] = new ObservableCollection<AbMediaContent>();
                
            }

            return cardMediaLinks[media.filePath];

        }


        


    }

}

