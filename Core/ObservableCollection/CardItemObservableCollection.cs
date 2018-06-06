using Cards.Core.FileReaders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Cards.Core
{
    public class CardItemObservableCollection : ObservableCollection<ISortableCardItem>
    {

        private List<IEnumerable<ISortableCardItem>> parentEnumerables;

        public CardItemObservableCollection(ObservableCollection<Card> dirs) : this()
        {
            addParentCollection(dirs); 
        }
        public CardItemObservableCollection(ObservableCollection<AbMediaContent> medias) : this()
        {
            addParentCollection(medias);
        }
        public CardItemObservableCollection()
        {
            this.parentEnumerables = new List<IEnumerable<ISortableCardItem>>(); 
        }

        //Set the collection we'll be listening to 
        public void addParentCollection(ObservableCollection<Card> dirs)
        {
            AddItems(dirs);
            dirs.CollectionChanged += OnParentCollectionChanges;
            this.parentEnumerables.Add(dirs);  
        }
        public void addParentCollection(ObservableCollection<AbMediaContent> medias)
        {
            AddItems(medias);
            medias.CollectionChanged += OnParentCollectionChanges;
            this.parentEnumerables.Add(medias);
        }

        void OnParentCollectionChanges(object source, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddItems(args.NewItems.Cast<ISortableCardItem>());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveItems(args.OldItems.Cast<ISortableCardItem>());
                    break;
                //Note how one parent enumerable reset forces complete reset 
                case NotifyCollectionChangedAction.Reset:
                    Clear();
                    foreach(var parent in parentEnumerables)
                        AddItems(parent); 
                    break;
                case NotifyCollectionChangedAction.Replace:
                    RemoveItems(args.OldItems.Cast<ISortableCardItem>());
                    AddItems(args.NewItems.Cast<ISortableCardItem>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void AddItems(IEnumerable<ISortableCardItem> items)
        {
            foreach (var me in items)
                Add(me);
        }

        void RemoveItems(IEnumerable<ISortableCardItem> items)
        {
            foreach (var me in items)
                Remove(me);
        }
    }
}

