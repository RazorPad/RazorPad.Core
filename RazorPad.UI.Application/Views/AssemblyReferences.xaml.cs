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
	public partial class AssemblyReferences
	{
		// parses the filter string
		private static readonly Filter<Reference> Filter = new Filter<Reference>();

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
		/// Filter the list synchronously. 
		/// </summary>
		private void FilterList()
		{
			// parse the filter string once, the Filter object
			// is used later in the FilterCallback method
			Filter.Parse(FilterText.Text);

			// get the data the ListView is bound to
			var view = CollectionViewSource.GetDefaultView(ReferencesListView.ItemsSource);

			// clear the list if the filter is empty, otherwise filter the list
			view.Filter = (Filter.IsEmpty)
			              	? null
			              	: view.Filter = FilterCallback;
		}

		/// <summary>
		/// Called for each item in the list. Return true if it
		/// should be in the list, or false to not be in the list.
		/// </summary>
		private static bool FilterCallback(object item)
		{
			return (Filter.Matches(item as Reference));
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

		private void HandleDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var reference = ((ListViewItem)sender).Content as Reference;
			if (reference != null)
				reference.Selected = !reference.Selected;
		}

	}
}
