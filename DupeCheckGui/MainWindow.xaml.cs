using DupeFileCheck.Context;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

namespace DupeCheckGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private List<string> listHash;
        private Dictionary<string, List<DupeFile>> listDupeFiles;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadJson_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string jsonFilepath = ValidateFile((string[])e.Data.GetData(DataFormats.FileDrop));

                if (!String.IsNullOrEmpty(jsonFilepath))
                    ProcessFile(jsonFilepath);
            }
        }

        private string ValidateFile(string[] list)
        {
            string json = "";

            try
            {
                if (list is null)
                    throw new NullReferenceException();

                if (list.Length != 1)
                    throw new InvalidOperationException("Cannot read multiple files");

                json = list[0];
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return json;
        }

        private void ProcessFile(string filePath)
        {
            string json = File.ReadAllText(filePath);
            Dictionary<string, List<DupeFile>> dupeFiles = JsonConvert.DeserializeObject<Dictionary<string, List<DupeFile>>>(json);

            CreateLocalLists(dupeFiles);

            UpdateHashView();
        }

        private void CreateLocalLists(Dictionary<string, List<DupeFile>> dupeFiles)
        {
            listHash = new List<string>();
            listDupeFiles = new Dictionary<string, List<DupeFile>>();

            foreach (KeyValuePair<string, List<DupeFile>> kv in dupeFiles)
            {
                listHash.Add(kv.Key);
                listDupeFiles.Add(kv.Key, kv.Value);
            }
        }

        private void UpdateHashView()
        {
            this.lViewHash.Items.Clear();
            foreach(string s in listHash)
            {
                this.lViewHash.Items.Add(s);
            }
        }

        private void UpdateFileListView()
        {
            this.lViewFiles.Items.Clear();

            foreach (DupeFile f in listDupeFiles[(string)lViewHash.SelectedItem])
            {
                this.lViewFiles.Items.Add(f.Path);
            }
        }

        private void lViewHash_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFileListView();
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            int fileIndex = this.lViewFiles.SelectedIndex;

            if (fileIndex >= 0)
            {
                string filePath = (string)this.lViewFiles.SelectedItem;

                Process p = new Process();
                p.StartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = true,
                    FileName = filePath
                };
                try
                {
                    p.Start();
                }
                catch (Exception)
                {
                    RemoveFileFromList(fileIndex);
                    MessageBox.Show("File does not exist - Removed File from list");
                    UpdateFileListView();
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {

            string filePath = (string)this.lViewFiles.SelectedItem;
            int fileIndex = this.lViewFiles.SelectedIndex;

            if(fileIndex >= 0)
            {
                MessageBoxResult mbr = System.Windows.MessageBox.Show($"Delete: {filePath} ?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (mbr == MessageBoxResult.Yes)
                {
                    try
                    {
                        FileSystem.DeleteFile(filePath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("File does not exist - Removed File from list");
                    }

                    RemoveFileFromList(fileIndex);
                }

                UpdateFileListView();
            }
        }

        private void RemoveFileFromList(int index)
        {
            listDupeFiles[(string)lViewHash.SelectedItem].RemoveAt(index);
        }
    }
}
