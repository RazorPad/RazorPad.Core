using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
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
            var ofd = new OpenFileDialog
            {
                DefaultExt = ".dll",
                Filter = "Component Files (.dll)|*.dll"
            };

            var result = ofd.ShowDialog();
            if (result != System.Windows.Forms.DialogResult.OK) return;
            
            var vm = DataContext as ReferencesViewModel;
            if (vm == null) return;

            foreach (var filePath in ofd.FileNames)
                try
                {
                    string message;
                    AssemblyReference assemblyReference;

                    var refCanBeLoaded = AssemblyReference.TryLoadReference(filePath, out assemblyReference, out message);
                    if (refCanBeLoaded)
                    {
                        assemblyReference.IsInstalled = assemblyReference.IsRecent = true;
                        vm.RecentReferences.References.Add(assemblyReference);
                        vm.InstalledReferences.References.Add(assemblyReference);
                    }
                    else
                    {
                        MessageBox.Show("Could not add reference due to: " + message, "Add Reference Error", MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                }
                catch (ArgumentException aex)
                {
                    MessageBox.Show("Could not add reference due to: " + aex.Message, "Add Reference Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            UpdateRecentReferences();
            base.OnClosing(e);
        }

        void UpdateRecentReferences()
        {
            var vm = DataContext as ReferencesViewModel;
            if (vm == null) return;

            var recentReferences = vm.RecentReferences.References;

            using (var fs = File.OpenWrite("RecentReferences.txt"))
            {
                var txt = string.Join(Environment.NewLine, recentReferences.Distinct().Take(50).Select(r => r.Location));
                fs.Write(Encoding.ASCII.GetBytes(txt), 0, txt.Length);
                fs.Close();
            }
        }
    }
}
