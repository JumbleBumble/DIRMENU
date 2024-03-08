using DIRMENU.Utils;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Path = System.IO.Path;

namespace DIRMENU.Pages
{
    /// <summary>
    /// Interaction logic for WeaponEditor.xaml
    /// </summary>
    public partial class ItemEditor : Page
    {

        private string currentCategory = "CraftPart";
        public static Dictionary<string, Dictionary<string, Dictionary<string, string>>> Inventory = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

        public ItemEditor()
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

        private void LoadCategory( object sender, SelectionChangedEventArgs e)
        {
            currentCategory = DatapakUtils.LoadCategory(sender, Inventory, WeaponList);
        }

        Button? currentButton = null;
        private void LoadItem( object sender, RoutedEventArgs e )
        {
            currentButton = DatapakUtils.LoadItem(sender, Inventory, currentButton, WeaponParams, currentCategory);
        }

        private void SubmitItem(object sender, RoutedEventArgs e)
        {
            DatapakUtils.Repack("inventory.scr", Inventory);
        }


        private void LoadCurrent(object sender, RoutedEventArgs e)
        {
            DatapakUtils.LoadCurrent("inventory.scr", Inventory, WeaponList, CategoryList);
        }

    }
}
