using DIRMENU.Models;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
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
    /// Interaction logic for ZombieEditor.xaml
    /// </summary>
    public partial class ZombieEditor : Page
    {
        public Dictionary<string, float> paramFloats = new Dictionary<string, float>();
        public Dictionary<string, int> paramBools = new Dictionary<string, int>();
        private string currentFile = "vessel_data";
        private string currentType = "zombie";

        public ZombieEditor()
        {
            InitializeComponent();
            string? executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string? filePath = Path.Combine(executablePath ?? "", "vessel_data.txt");
            LoadFile(filePath);
        }

        private Dictionary<string, (string file, string type)> loadButtonMap = new Dictionary<string, (string file, string type)>
        {
            { "loadWalker", ("vessel_data", "zombie") },
            { "loadInfected", ("infected_data", "infected") },
            { "loadHuman", ("human_data", "human") },
            { "loadThug", ("vessel_data_preset_1", "zombie") },
            { "loadSuicider", ("vessel_data_preset_custom_15", "zombie") },
            { "loadFloater", ("vessel_data_preset_custom_16", "zombie") },
            { "loadDrowned", ("vessel_data_preset_custom_17", "zombie") },
            { "loadButcher", ("vessel_data_preset_custom_18", "zombie") },
            { "loadScreamer", ("vessel_data_preset_custom_19", "zombie") },
            { "loadWrestler", ("vessel_data_preset_custom_20", "zombie") },
            { "loadGrenadier", ("vessel_data_preset_custom_21", "zombie") },
            { "loadScreamerALt", ("vessel_data_preset_custom_44", "zombie") },
            { "loadRam", ("infected_data_preset_custom_6", "infected") }
        };

        Button? currentButton = null;
        private void LoadFile_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && loadButtonMap.TryGetValue(button.Uid, out var fileInfo))
            {
                string file = fileInfo.file;
                string type = fileInfo.type;
                if (currentButton != null)
                {
                    currentButton.Background = Brushes.White;
                    currentButton.Foreground = Brushes.Black;
                }
                currentButton = button;
                currentButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6a6a6a"));
                currentButton.Foreground = Brushes.White;

                if (currentFile != file || currentType != type)
                {
                    currentFile = file;
                    currentType = type;
                    LoadFile(currentFile + ".txt");
                }
            }
        }

        private void NavigateHome(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Home());
        }

        private void LoadFile(string filePath)
        {
            ParamList.Items.Clear();
            paramFloats.Clear();
            paramBools.Clear();
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                if (line.StartsWith("ParamFloat"))
                {
                    string trimLine = line.Trim();
                    string paramName = trimLine.Split(new[] { "(", ",", ")" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim('"');
                    float paramValue = float.Parse(trimLine.Split(',')[1].Substring(0, trimLine.Split(',')[1].Length - 1));
                    paramFloats[paramName] = paramValue;
                }
                else if (line.StartsWith("ParamBool"))
                {
                    string trimLine = line.Trim();
                    string paramName = trimLine.Split(new[] { "(", ",", ")" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim('"');
                    int boolint = int.Parse(trimLine.Split(',')[1].Substring(0, trimLine.Split(',')[1].Length - 1).Trim());
                    paramBools[paramName] = boolint;
                }
            }

            foreach (var entry in paramFloats)
            {
                ParamList.Items.Add(new ParameterEntry(entry.Key, entry.Value.ToString(), paramFloats, paramBools));
            }

            foreach (var entry in paramBools)
            {
                ParamList.Items.Add(new ParameterEntry(entry.Key, entry.Value.ToString(), paramFloats, paramBools));
            }

        }

        private const string dataPakPath = @"C:\SteamLibrary\steamapps\common\DIRDE\DIR\Data0.pak";
        private void Submit_Click(object sender, RoutedEventArgs e)
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

                string vesselDataPath = Path.Combine(tempDir, "data", "ai", currentType, currentFile + ".scr");

                if (vesselDataPath != null)
                {
                    string[] lines = File.ReadAllLines(vesselDataPath);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        string line = lines[i];
                        foreach (var entry in paramFloats)
                        {
                            string paramName = entry.Key;
                            string paramValue = entry.Value.ToString();
                            string searchString = $"ParamFloat(\"{paramName}\",";
                            if (line.Contains(searchString))
                            {
                                lines[i] = $"ParamFloat(\"{paramName}\", {paramValue})";
                            }
                        }
                        foreach (var entry in paramBools)
                        {
                            string paramName = entry.Key;
                            int paramValue = entry.Value;
                            string searchString = $"ParamBool(\"{paramName}\", ";
                            if (line.Contains(searchString))
                            {
                                lines[i] = $"ParamBool(\"{paramName}\", {paramValue})";
                            }
                        }
                    }

                    File.WriteAllLines(vesselDataPath, lines);
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
