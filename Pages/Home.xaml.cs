using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace DIRMENU.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
        }

        private void NavZEditor(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ZombieEditor());
        }

        private void NavWepEditor(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ItemEditor());
        }

        private void NavItemGenEditor(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ItemGenEditor());
        }

        private void SelectDataPath(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "PAK files (*.pak)|*.pak|All files (*.*)|*.*";
                openFileDialog.InitialDirectory = @"C:\";

                if (openFileDialog.ShowDialog() == true)
                {
                    ((App)Application.Current).dataPakPath = openFileDialog.FileName;
                    button.Content = openFileDialog.FileName;
                }
            }
        }
    }
}
