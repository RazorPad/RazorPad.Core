using System;
using System.Collections.Generic;
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

namespace RazorPad.Views
{
	/// <summary>
	/// Interaction logic for ListViewFilterText.xaml
	/// </summary>
	public partial class ListViewFilterText : UserControl
	{
		/// <summary>
		/// Gets or sets the text content of the filter control.
		/// </summary>
		public string Text
		{
			get { return FilterTextBox.Text; }
			set { FilterTextBox.Text = value; }
		}


		public ListViewFilterText()
		{
			InitializeComponent();
			ShowResetButton();
		}

		/// <summary>
		/// Set the focus to the filter control.
		/// </summary>
		public new void Focus()
		{
			FilterTextBox.Focus();
		}

		/// <summary>
		/// The reset button was clicked, clear the filter control.
		/// </summary>
		private void FilterButtonClick(object sender, RoutedEventArgs e)
		{
			this.Text = string.Empty;
		}

		/// <summary>
		/// The filter text changed, show or hide the reset button.
		/// </summary>
		private void FilterTextBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			ShowResetButton();
		}

		/// <summary>
		/// Show the reset button if there is any text in the filter,
		/// otherwise hide the reset button.
		/// </summary>
		private void ShowResetButton()
		{
			FilterButton.Visibility = (FilterTextBox.Text.Trim().Length > 0) ?
				Visibility.Visible : Visibility.Hidden;
		}
	}
}
