using System.Collections.Generic;
using System.Collections.ObjectModel;
using Movies.Controls;

namespace Movies.Helpers
{
    public class GroupedCollection<TH, T> : EnhancedObservableCollection<T>
    {
        public GroupedCollection(TH header, List<T> items) : base(items)
        {
            Header = header;
        }
        
        public TH Header { get; }
    }
}