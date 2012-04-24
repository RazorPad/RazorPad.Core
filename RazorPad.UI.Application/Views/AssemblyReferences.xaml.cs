using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RazorPad.ViewModels;

namespace RazorPad.Views
{
	/// <summary>
	/// Interaction logic for AssemblyReferences.xaml
	/// </summary>
	public partial class AssemblyReferences : UserControl
	{
		// parses the filter string
		private Filter _filter = new Filter();

		private class Filter
		{
			// main filer text
			private string _text;

			/// <summary>
			/// Indicates if the filter is empty.
			/// </summary>
			public bool IsEmpty
			{
				get { return string.IsNullOrEmpty(_text); }
			}

			/// <summary>
			/// Parse the filter text, determines if doing an operation,
			/// for example 'red > 100'.
			/// </summary>
			public void Parse(string text)
			{
				// store main text
				_text = text.Trim().ToLower();

				// enhancement: add regex filtering support				
			}

			/// <summary>
			/// Return true if the Reference matches the filter.
			/// </summary>
			public bool Matches(Reference item)
			{
				// check name
				return item.Name.ToLower().Contains(_text);
			}
		}

		public AssemblyReferences()
		{
			InitializeComponent();
			UpdateResult();
			FilterText.Focus();
		}

		private void FilterTextTextChanged(object sender, TextChangedEventArgs e)
		{
			FilterList();
			UpdateResult();
		}

		/// <summary>
		/// Filter the list synchronously. Alternately, you can use Dispatcher 
		/// to filter the list in the background, or BackgroundWorker to filter 
		/// the list using a separate thread.
		/// </summary>
		private void FilterList()
		{
			// parse the filter string once, the _filter object
			// is used later in the FilterCallback method
			_filter.Parse(FilterText.Text);

			// get the data the ListView is bound to
			var view = CollectionViewSource.GetDefaultView(ReferencesListView.ItemsSource);

			// clear the list if the filter is empty, otherwise filter the list
			view.Filter = (_filter.IsEmpty) ? null :
				view.Filter = FilterCallback;
		}

		/// <summary>
		/// Called for each item in the list. Return true if it
		/// should be in the list, or false to not be in the list.
		/// </summary>
		private bool FilterCallback(object item)
		{
			return (_filter.Matches(item as Reference));
		}

		/// <summary>
		/// Update the number of items in the list.
		/// </summary>
		private void UpdateResult()
		{
			Result.Content = string.Format("{0} item{1}",
				ReferencesListView.Items.Count,
				ReferencesListView.Items.Count == 1 ? "" : "s");
		}
	}
}
