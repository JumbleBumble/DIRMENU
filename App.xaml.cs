using System.Diagnostics;
using System.IO;
using System.Windows;

namespace DIRMENU
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public string dataPakPath { get; set; } = "Select Data0.pak";
        public App()
        {
            InitializeDataPakPath();
        }

        private void InitializeDataPakPath()
        {
            if (File.Exists("DataPakPath.txt"))
            {
                dataPakPath = File.ReadAllText("DataPakPath.txt").Trim();
            }
        }
    }
}
