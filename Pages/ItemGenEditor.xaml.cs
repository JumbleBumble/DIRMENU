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
    public partial class ItemGenEditor : Page
    {
        private string currentCategory = "Melee";
        public static Dictionary<string, Dictionary<string, Dictionary<string, string>>> Inventory = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

        public ItemGenEditor()
        {
            InitializeComponent();
            string? executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string? filePath = Path.Combine(executablePath ?? "", "inventory_gen.txt");
            DatapakUtils.AddInventory(filePath, Inventory, WeaponList, CategoryList, "Melee");
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

        private void SubmitItems(object sender, RoutedEventArgs e)
        {
            DatapakUtils.Repack("inventory_gen.scr", Inventory, true);
        }


        private void LoadCurrent(object sender, RoutedEventArgs e)
        {
            DatapakUtils.LoadCurrent("inventory_gen.scr", Inventory, WeaponList, CategoryList, "Melee");
        }

    }
}
