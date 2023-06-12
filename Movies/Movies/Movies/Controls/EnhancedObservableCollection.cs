using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Movies.Controls
{
    public class EnhancedObservableCollection<T> : ObservableCollection<T>
    {
        public EnhancedObservableCollection(IEnumerable<T> items) : base(items)
        {
            
        }
        
        public void AddRange(IEnumerable<T> collection, NotifyCollectionChangedAction notificationMode = NotifyCollectionChangedAction.Add)
        {
            if (notificationMode != NotifyCollectionChangedAction.Add && notificationMode != NotifyCollectionChangedAction.Reset)
                throw new ArgumentException("Mode must be either Add or Reset for AddRange.", nameof(notificationMode));
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            CheckReentrancy();

            var startIndex = Count;

            var itemsAdded = AddArrangeCore(collection);

            if (!itemsAdded)
                return;

            if (notificationMode == NotifyCollectionChangedAction.Reset)
            {
                RaiseChangeNotificationEvents(action: NotifyCollectionChangedAction.Reset);

                return;
            }

            var changedItems = collection is List<T> ? (List<T>)collection : new List<T>(collection);

            RaiseChangeNotificationEvents(
                action: NotifyCollectionChangedAction.Add,
                changedItems: changedItems,
                startingIndex: startIndex);
        }

        private bool AddArrangeCore(IEnumerable<T> collection)
        {
            var itemAdded = false;
            foreach (var item in collection)
            {
                Items.Add(item);
                itemAdded = true;
            }
            return itemAdded;
        }

        private void RaiseChangeNotificationEvents(NotifyCollectionChangedAction action, List<T> changedItems = null, int startingIndex = -1)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));

            if (changedItems is null)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(action));
            else
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItems: changedItems, startingIndex: startingIndex));
        }
    }
}