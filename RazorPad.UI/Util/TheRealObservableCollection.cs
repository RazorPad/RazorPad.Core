using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RazorPad.UI
{
    public class TheRealObservableCollection<T> : ObservableCollection<T>
        where T : INotifyPropertyChanged
    {
        // The ObservableCollection give us some excellent functionality out of box, 
        // but it does not expose property changes to the developer, for those events
        // we have to do a bit more work.

        // This is the event is what we will use to expose the property change to our code
        public event ItemPropertyChangedEventHandler ItemPropertyChanged;
        public delegate void ItemPropertyChangedEventHandler(T sender, PropertyChangedEventArgs e);

        // Overriding the InsertItem method gives us a chance to add a handler to the item's PropertyChanged event
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            item.PropertyChanged -= Item_PropertyChanged;
            item.PropertyChanged += Item_PropertyChanged;
        }

        // Make sure you clean up your handlers
        protected override void RemoveItem(int index)
        {
            this[index].PropertyChanged -= Item_PropertyChanged;
            base.RemoveItem(index);
        }

        // This will catch the item's property change event and bubble it up from the collection
        protected void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ItemPropertyChanged != null)
            {
                ItemPropertyChanged((T)sender, e);
            }
        }


        // picking up c# slack again - constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RazorPad.UI.TheRealObservableCollection`1"/> class.
        /// </summary>
        public TheRealObservableCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RazorPad.UI.TheRealObservableCollection`1"/> class that contains elements copied from the specified list.
        /// </summary>
        /// <param name="list">The list from which the elements are copied.</param><exception cref="T:System.ArgumentNullException">The <paramref name="list"/> parameter cannot be null.</exception>
        public TheRealObservableCollection(List<T> list)
            : base(list)
        {
            foreach (var item in list)
            {
                item.PropertyChanged -= Item_PropertyChanged;
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RazorPad.UI.TheRealObservableCollection`1"/> class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection">The collection from which the elements are copied.</param><exception cref="T:System.ArgumentNullException">The <paramref name="collection"/> parameter cannot be null.</exception>
        public TheRealObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {
            foreach (var item in collection)
            {
                item.PropertyChanged -= Item_PropertyChanged;
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

    }

}
