using DIRMENU.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DIRMENU.Pages;

namespace DIRMENU.Utils
{
    public class DatapakUtils
    {

        public static void Repack(string fileName, Dictionary<string, Dictionary<string, Dictionary<string, string>>> Inventory, Boolean NolastItem = false)
        {
            string? executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (executablePath == string.Empty || executablePath == null)
            {
                return;
            }
            if (((App)Application.Current).dataPakPath == "Select Data0.pak")
            {
                MessageBox.Show("ERROR: Please select the location of your Data0.pak (Should be in your games directory in the DIR folder)");
                return;
            }
            if (!File.Exists(((App)Application.Current).dataPakPath))
            {
                MessageBox.Show("ERROR: Data0.pak does not exist or wasn't selected properly, please verify files or reinstall.");
                return;
            }
            string tempDir = Path.Combine(executablePath, "TempUnzip");
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
            try
            {
                ZipFile.ExtractToDirectory(((App)Application.Current).dataPakPath, tempDir);

                string invDataPath = Path.Combine(tempDir, "data", fileName);
                string? currentLoopItem = null;
                string? currentLoopCategory = null;
                string? lastItem = null;

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
                            if (currentLoopItem == lastItem && NolastItem)
                            {
                                continue;
                            }
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
                            lastItem = currentLoopItem;
                            currentLoopItem = null;
                            currentLoopCategory = null;
                        }
                    }

                    File.WriteAllLines(invDataPath, lines);
                }

                string newPakPath = Path.Combine(executablePath, "Data0.pak");
                ZipFile.CreateFromDirectory(tempDir, newPakPath);

                File.Copy(newPakPath, ((App)Application.Current).dataPakPath, true);
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

        public static Button? LoadItem(object sender, Dictionary<string, Dictionary<string, Dictionary<string, string>>> Inventory, Button? currentButton, ListBox List, string currentCategory)
        {
            if (sender is Button button)
            {
                string currentItem = button.Content as string ?? "CraftPart_Algae";
                foreach (var categoryEntry in Inventory)
                {
                    if (categoryEntry.Key == currentCategory)
                    {
                        foreach (var itemEntry in categoryEntry.Value)
                        {
                            if (itemEntry.Key == currentItem)
                            {
                                List.Items.Clear();
                                if (currentButton != null)
                                {
                                    currentButton.Background = Brushes.White;
                                    //currentButton.Foreground = Brushes.Black;
                                    currentButton.BorderBrush = Brushes.White;
                                    currentButton.BorderThickness = new Thickness(0);
                                    currentButton.MouseEnter += (sender, e) =>
                                    {
                                        var btn = sender as Button;
                                        if (btn != null)
                                        {
                                            btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0"));
                                        }
                                    };
                                    currentButton.MouseLeave += (sender, e) =>
                                    {
                                        var btn = sender as Button;
                                        if (btn != null)
                                        {
                                            btn.Background = Brushes.White;
                                        }
                                    };
                                }
                                currentButton = button;
                                currentButton.BorderBrush = Brushes.Black;
                                currentButton.BorderThickness = new Thickness(2);
                                currentButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0"));
                                currentButton.MouseLeave += (sender, e) =>
                                {
                                    var btn = sender as Button;
                                    if (btn != null)
                                    {
                                        btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0"));
                                    }
                                };
                                //currentButton.Foreground = Brushes.White;
                                foreach (var propertyEntry in itemEntry.Value)
                                {
                                    List.Items.Add(new ParameterWepEntry(propertyEntry.Key, propertyEntry.Value, currentCategory, currentItem, Inventory));
                                }
                            }
                        }
                    }
                }
            }
            return currentButton;
        }

        public static string LoadCategory(object sender, Dictionary<string, Dictionary<string, Dictionary<string, string>>> Inventory, ListBox List)
        {
            if (sender is ComboBox comboBox)
            {
                if (comboBox.SelectedItem != null)
                {
                    string currentCategory = comboBox.SelectedItem.ToString() ?? "CraftPart";
                    foreach (var categoryEntry in Inventory)
                    {
                        if (categoryEntry.Key == currentCategory)
                        {
                            List.Items.Clear();
                            foreach (var itemEntry in categoryEntry.Value)
                            {
                                List.Items.Add(itemEntry.Key);
                            }
                        }
                    }
                    return currentCategory;
                }
            }
            return "";
        }

        public static void AddInventory(string filePath, Dictionary<string, Dictionary<string, Dictionary<string, string>>> Inventory, ListBox DisplayList, ComboBox CategoryList, string startingCategory = "CraftPart")
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
                    if (category == startingCategory && !Inventory.ContainsKey(category) || category == startingCategory && Inventory.ContainsKey(category) &&!Inventory[category].ContainsKey(itemName))
                    {
                        DisplayList.Items.Add(itemName);
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
                else if (line.Contains("(") && line.Trim() != "sub main()")
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
                    if (itemProperties.Count > 0 && !Inventory[category].ContainsKey(itemName))
                    {
                        Inventory[category][itemName] = new Dictionary<string, string>(itemProperties);
                    }
                    itemProperties.Clear();
                }
            }
        }

        public static void LoadCurrent(string fileName, Dictionary<string, Dictionary<string, Dictionary<string, string>>> Inventory, ListBox DisplayList, ComboBox CategoryList, string startingCategory = "CraftPart")
        {
            string? executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (executablePath == string.Empty || executablePath == null)
            {
                return;
            }
            if (((App)Application.Current).dataPakPath == "Select Data0.pak")
            {
                MessageBox.Show("ERROR: Please select the location of your Data0.pak (Should be in your games directory in the DIR folder");
                return;
            }
            if (!File.Exists(((App)Application.Current).dataPakPath))
            {
                MessageBox.Show("ERROR: Data0.pak does not exist or wasn't selected properly, please verify files or reinstall.");
                return;
            }
            string tempDir = Path.Combine(executablePath, "TempUnzip");
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
            try
            {
                Inventory.Clear();
                DisplayList.Items.Clear();
                ZipFile.ExtractToDirectory(((App)Application.Current).dataPakPath, tempDir);
                string invDataPath = Path.Combine(tempDir, "data", fileName);
                AddInventory(invDataPath, Inventory, DisplayList, CategoryList, startingCategory);
            }
            catch
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
                MessageBox.Show("Error occured when retrieving current values.");
                return;
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
                MessageBox.Show("Loaded current values.");
            }
        }

    }
}
