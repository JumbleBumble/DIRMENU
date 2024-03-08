using DIRMENU.Pages;
using System.Windows;

namespace DIRMENU
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Home());
        }
    }
}
