using System;
using System.Windows;
using System.Windows.Forms;
using RazorPad.ViewModels;
using MessageBox = System.Windows.MessageBox;

namespace RazorPad.Views
{
	/// <summary>
	/// Interaction logic for ReferencesDialogWindow.xaml
	/// </summary>
	public partial class ReferencesDialogWindow
	{
		public ReferencesDialogWindow()
		{
			InitializeComponent();
		}

		private void OkButtonClick(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void BrowseButtonClicked(object sender, RoutedEventArgs e)
		{
			var ofd = new OpenFileDialog {
				DefaultExt = ".dll", 
				Filter = "Component Files (.dll)|*.dll"
			};

			var result = ofd.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				var vm = DataContext as ReferencesViewModel;
				if (vm != null)
				{
					foreach (var filePath in ofd.FileNames)
						try
						{
							vm.RecentReferences.References.Add(new Reference(filePath) { Selected = true });
						}
						catch (ArgumentException aex)
						{
							MessageBox.Show("Could not add reference due to: " + aex.Message, "Add Reference Error", MessageBoxButton.OK,
							                MessageBoxImage.Error);
						}
				}
			}
		}
	}
}
