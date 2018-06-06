using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cards.Core.Platform.Manager
{
    public class ChangeListener : Java.Lang.Object, IValueEventListener
    {

        private IChangeListener changeListener;

        public ChangeListener(IChangeListener changeListener)
        {
            this.changeListener = changeListener;
        }

        public void OnCancelled(DatabaseError error)
        {
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            changeListener.dataChanged(snapshot);
        }
    }

    public interface IChangeListener
    {
        void dataChanged(DataSnapshot snapshot);
    }
}