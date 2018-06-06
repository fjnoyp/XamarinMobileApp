using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Cards.Core
{
    /// <summary>
    /// Observable collection that responds to changes to several ObservableCollections 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableCollectionGroup<T> : ObservableCollection<T>
    {
        private List<ObservableCollection<T>> parentCollections; 

        public ObservableCollectionGroup() : base() {
            parentCollections = new List<ObservableCollection<T>>(); 
        }

        public void addParentCollection(ObservableCollection<T> parent)
        {
            addItems(parent);
            parent.CollectionChanged += OnParentCollectionChanges;
            this.parentCollections.Add(parent); 
        }

        void OnParentCollectionChanges(object source, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    addItems(args.NewItems.Cast<T>());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    removeItems(args.OldItems.Cast<T>());
                    break;
                //Note how one parent enumerable reset forces complete reset 
                case NotifyCollectionChangedAction.Reset:
                    Clear();
                    foreach (var parent in parentCollections)
                        addItems(parent);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    removeItems(args.OldItems.Cast<T>());
                    addItems(args.NewItems.Cast<T>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual void addItems(IEnumerable<T> items)
        {
            foreach (var me in items)
                Add(me);
        }

        protected void removeItems(IEnumerable<T> items)
        {
            foreach (var me in items)
                Remove(me);
        }

    }
}
