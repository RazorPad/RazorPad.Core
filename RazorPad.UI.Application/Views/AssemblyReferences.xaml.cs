using System.Windows.Controls;
using System.Windows.Data;
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
			if (Filter.IsEmpty)
				Result.Content = "No filter applied. To search for a reference by name, enter few letters in the textbox.";
			else
			{
				Result.Content = string.Format("{0} item{1} containing \"{2}\".",
				                               ReferencesListView.Items.Count,
				                               ReferencesListView.Items.Count == 1 ? "" : "s",
											   FilterText.Text);
			}
		}
	}
}
