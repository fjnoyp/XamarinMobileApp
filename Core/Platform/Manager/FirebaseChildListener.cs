using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cards.Core.Platform.Manager
{

    public class FirebaseChildListener : Java.Lang.Object, IChildEventListener
    {

        private IChildListener childListener;

        public FirebaseChildListener(IChildListener childListener)
        {
            this.childListener = childListener;
        }

        public void OnCancelled(DatabaseError error)
        {
        }

        public void OnChildAdded(DataSnapshot snapshot, string previousChildName)
        {
            childListener.childAdded(snapshot, previousChildName);
        }

        public void OnChildChanged(DataSnapshot snapshot, string previousChildName)
        {
            childListener.childChanged(snapshot, previousChildName);
        }

        public void OnChildMoved(DataSnapshot snapshot, string previousChildName)
        {
            childListener.childMoved(snapshot, previousChildName);
        }

        public void OnChildRemoved(DataSnapshot snapshot)
        {
            childListener.childRemoved(snapshot);
        }
    }

    public interface IChildListener
    {
        void childAdded(DataSnapshot snapshot, string previousChildName);
        void childChanged(DataSnapshot snapshot, string previousChildName);
        void childMoved(DataSnapshot snapshot, string previousChildName);
        void childRemoved(DataSnapshot snapshot);
    }

}