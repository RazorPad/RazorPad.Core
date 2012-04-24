using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RazorPad.ViewModels
{
	/// <summary>
	/// Interaction logic for ReferencesDialogWindow.xaml
	/// </summary>
	public partial class ReferencesDialogWindow : Window
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
				Filter = "Executables (.dll)|*.dll"
			};


			var result = ofd.ShowDialog();
			// Process open file dialog box results
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				// Set value to textbox which will update the property of VM
				//txtFileName.Text = ofd.FileName;
				var vm = this.DataContext as ReferencesViewModel;
				vm.RecentReferences.References.Add(new Reference(ofd.FileName));
			}
		}
	}
}
