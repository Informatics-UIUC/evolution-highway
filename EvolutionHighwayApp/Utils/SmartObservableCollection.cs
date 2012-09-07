using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace EvolutionHighwayApp.Utils
{
    public class SmartObservableCollection<T> : ObservableCollection<T>
    {
        protected bool SuspendCollectionChangedNotification { get; set; }

        public SmartObservableCollection()
        {
            SuspendCollectionChangedNotification = false;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!SuspendCollectionChangedNotification)
                base.OnCollectionChanged(e);
        }

        public void AddRange(ICollection<T> items)
        {
            if (items.IsEmpty()) return;
            SuspendCollectionChangedNotification = true;
            AddRangeInternal(items);
            SuspendCollectionChangedNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void RemoveRange(ICollection<T> items)
        {
            if (items.IsEmpty()) return;
            SuspendCollectionChangedNotification = true;
            items.ForEach(item => Remove(item));
            SuspendCollectionChangedNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void ReplaceWith(IEnumerable<T> items)
        {
            if (items == Items) return;

            SuspendCollectionChangedNotification = true;
            ClearItems();
            AddRangeInternal(items);
            SuspendCollectionChangedNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void AddRangeInternal(IEnumerable<T> items)
        {
            items.ForEach(item => InsertItem(Count, item));
        }
    }
}
