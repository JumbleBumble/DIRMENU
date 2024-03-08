using DIRMENU.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DIRMENU.Utils;
using Path = System.IO.Path;

namespace DIRMENU.Pages
{
    /// <summary>
    /// Interaction logic for WeaponEditor.xaml
    /// </summary>
    public partial class WeaponEditor : Page
    {

        private string currentCategory = "CraftPart";
        public static Dictionary<string, Dictionary<string, Dictionary<string, string>>> Inventory = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

        public WeaponEditor()
        {
            InitializeComponent();
            string? executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string? filePath = Path.Combine(executablePath ?? "", "inventory.txt");
            DatapakUtils.AddInventory(filePath, Inventory, WeaponList, CategoryList);
        }

        private void NavigateHome(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Home());
        }

        private void loadCategory( object sender, SelectionChangedEventArgs e)
        {
            currentCategory = DatapakUtils.LoadCategory(sender, Inventory, WeaponList);
        }

        Button? currentButton = null;
        private void loadItem( object sender, RoutedEventArgs e )
        {
            currentButton = DatapakUtils.LoadItem(sender, Inventory, currentButton, WeaponParams, currentCategory);
        }

        private const string dataPakPath = @"C:\SteamLibrary\steamapps\common\DIRDE\DIR\Data0.pak";
        private void submitItems(object sender, RoutedEventArgs e)
        {
            DatapakUtils.Repack("inventory.scr", dataPakPath, Inventory);
        }


        private void loadCurrent(object sender, RoutedEventArgs e)
        {
            DatapakUtils.LoadCurrent("inventory.scr", dataPakPath, Inventory, WeaponList, CategoryList);
        }

    }
}
