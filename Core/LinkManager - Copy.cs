using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cards.Core
{

    //Manages links for media contained within a card 
    public class LinkManager
    {
        //Relate media file name to linked media file names 
        private Dictionary<AbMediaContent, ObservableCollection<AbMediaContent>> cardMediaLinks;

        public LinkManager()
        {
            cardMediaLinks = new Dictionary<AbMediaContent, ObservableCollection<AbMediaContent>>();
        }

        public void addLink(AbMediaContent rootMedia, AbMediaContent otherMedia)
        {
            ObservableCollection<AbMediaContent> links;
            cardMediaLinks.TryGetValue(rootMedia, out links);

            if (links == null)
                links = new ObservableCollection<AbMediaContent>();

            //Create new media content to reference to 
            if (!links.Contains(otherMedia))
                links.Add(AbMediaContent.createMediaContent(otherMedia.getMediaType(),otherMedia.filePath));

            
            cardMediaLinks[rootMedia] = links;
        }
        public void removeLink(AbMediaContent rootMedia, AbMediaContent otherMedia)
        {
            ObservableCollection<AbMediaContent> links;
            cardMediaLinks.TryGetValue(rootMedia, out links);

            if (links == null)
                links = new ObservableCollection<AbMediaContent>();

            if (links.Contains(otherMedia))
                links.Remove(otherMedia);

            cardMediaLinks[rootMedia] = links;
        }

        public bool linkExists(AbMediaContent aMedia, AbMediaContent bMedia)
        {
            ObservableCollection<AbMediaContent> links;
            cardMediaLinks.TryGetValue(aMedia, out links);

            if (links == null) return false;
            return links.Contains(bMedia); 
        }

        public ObservableCollection<AbMediaContent> getLinkedMedia(AbMediaContent media)
        {
            ObservableCollection<AbMediaContent> linkedMediaInfo = new ObservableCollection<AbMediaContent>(); 

            cardMediaLinks.TryGetValue(media, out linkedMediaInfo);

            return linkedMediaInfo;
        }
        


    }

}

