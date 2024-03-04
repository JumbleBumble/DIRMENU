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
using Path = System.IO.Path;

namespace DIRMENU.Pages
{
    /// <summary>
    /// Interaction logic for WeaponEditor.xaml
    /// </summary>
    public partial class WeaponEditor : Page
    {

        private string currentCategory = "CraftPart";
        private string currentItem = "CraftPart_Algae";

        public WeaponEditor()
        {
            InitializeComponent();
            string? executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string? filePath = Path.Combine(executablePath ?? "", "inventory.txt");
            AddInventory(filePath);
        }

        private void NavigateHome(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Home());
        }

        public static Dictionary<string, Dictionary<string, Dictionary<string, string>>> Inventory = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

        private void AddInventory(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            string category = "";
            string itemName = "";
            Dictionary<string, string> itemProperties = new Dictionary<string, string>();

            foreach (string line in lines)
            {
                if (line.Trim().StartsWith("Item("))
                {
                    category = line.Split(",")[1].Trim().Substring(13);
                    category = category.Remove(category.Length - 1);
                    itemName = line.Split(",")[0].Trim().Substring(6);
                    itemName = itemName.Remove(itemName.Length - 1);
                    if (category == "CraftPart")
                    {
                        WeaponList.Items.Add(itemName);
                    }
                    if (!CategoryList.Items.Contains(category))
                    {
                        CategoryList.Items.Add(category);
                    }
                    if (!Inventory.ContainsKey(category))
                    {
                        Inventory[category] = new Dictionary<string, Dictionary<string, string>>();
                    }
                }
                else if (line.Contains("("))
                {
                    string propertyName = line.Split("(")[0].Trim();
                    string propertyValue = new string(line.Split("(")[1].Where(c => !");".Contains(c)).ToArray());
                    if (propertyValue.Length > 0)
                    {
                        itemProperties[propertyName] = propertyValue;
                    }
                }
                else if (line.Trim() == "};" || line.Trim() == "}")
                {               
                    if (itemProperties.Count > 0)
                    {
                        Inventory[category][itemName] = new Dictionary<string, string>(itemProperties);
                    }
                    itemProperties.Clear();
                }
            }
        }

        private void loadCategory( object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                if (comboBox.SelectedItem != null)
                {
                    currentCategory = comboBox.SelectedItem.ToString() ?? "CraftPart";
                    foreach (var categoryEntry in Inventory)
                    { 
                        if (categoryEntry.Key == currentCategory)
                        {
                            WeaponList.Items.Clear();
                            foreach (var itemEntry in categoryEntry.Value)
                            {
                                WeaponList.Items.Add(itemEntry.Key);
                            }
                        }
                    }
                }
            }
        }

        private void loadItem( object sender, RoutedEventArgs e )
        {
            if (sender is Button button)
            {
                currentItem = button.Content as string ?? "CraftPart_Algae";
                foreach (var categoryEntry in Inventory)
                {
                    if (categoryEntry.Key == currentCategory)
                    {
                        foreach (var itemEntry in categoryEntry.Value)
                        {
                            if (itemEntry.Key == currentItem) 
                            {
                                WeaponParams.Items.Clear();
                                foreach (var propertyEntry in itemEntry.Value)
                                {
                                    WeaponParams.Items.Add(new ParameterWepEntry(propertyEntry.Key, propertyEntry.Value, currentCategory, currentItem, Inventory));
                                }
                            }
                        }
                    }                     
                }
            }
        }

        private const string dataPakPath = @"C:\SteamLibrary\steamapps\common\DIRDE\DIR\Data0.pak";
        private void submitItems(object sender, RoutedEventArgs e)
        {
            string? executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (executablePath == string.Empty || executablePath == null)
            {
                return;
            }
            if (!File.Exists(dataPakPath))
            {
                MessageBox.Show("ERROR: Data0.pak does not exist, please verify files or reinstall.");
                return;
            }
            string tempDir = Path.Combine(executablePath, "TempUnzip");
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
            try
            {
                ZipFile.ExtractToDirectory(dataPakPath, tempDir);

                string invDataPath = Path.Combine(tempDir, "data", "inventory.scr");
                string? currentLoopItem = null;
                string? currentLoopCategory = null;

                if (invDataPath != null)
                {
                    string[] lines = File.ReadAllLines(invDataPath);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        string line = lines[i];
                        if (line.Trim().StartsWith("Item("))
                        {
                            currentLoopCategory = line.Split(",")[1].Trim().Substring(13);
                            currentLoopCategory = currentLoopCategory.Remove(currentLoopCategory.Length - 1);
                            currentLoopItem = line.Split(",")[0].Trim().Substring(6);
                            currentLoopItem = currentLoopItem.Remove(currentLoopItem.Length - 1);
                        } 
                        else if (line.Contains("(") && line.Trim().Length > 3 && currentLoopItem != null && currentLoopCategory != null)
                        {
                            string propertyName = line.Split("(")[0].Trim();
                            if (
                             propertyName.Length > 0 &&
                             Inventory.ContainsKey(currentLoopCategory) &&
                             Inventory[currentLoopCategory] != null &&
                             Inventory[currentLoopCategory].ContainsKey(currentLoopItem) &&
                             Inventory[currentLoopCategory][currentLoopItem] != null &&
                             Inventory[currentLoopCategory][currentLoopItem].ContainsKey(propertyName) &&
                             Inventory[currentLoopCategory][currentLoopItem][propertyName] != null &&
                             Inventory[currentLoopCategory][currentLoopItem][propertyName].Length > 0
                             )
                            {
                                lines[i] = $"        {propertyName}({Inventory[currentLoopCategory][currentLoopItem][propertyName]});";
                            }
                        }
                        else if (line.Contains("}"))
                        {
                            currentLoopItem = null;
                            currentLoopCategory = null;
                        }
                    }

                    File.WriteAllLines(invDataPath, lines);
                }

                string newPakPath = Path.Combine(executablePath, "Data0.pak");
                ZipFile.CreateFromDirectory(tempDir, newPakPath);

                File.Copy(newPakPath, dataPakPath, true);
            }
            catch
            {
                string newPakPath = Path.Combine(executablePath, "Data0.pak");
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
                if (File.Exists(newPakPath))
                {
                    File.Delete(newPakPath);
                }
                MessageBox.Show("Error occured during modification attempt.");
                return;
            }
            finally
            {
                string newPakPath = Path.Combine(executablePath, "Data0.pak");
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
                if (File.Exists(newPakPath))
                {
                    File.Delete(newPakPath);
                }
                MessageBox.Show("Modification Successful!");
            }
        }

    }
}
