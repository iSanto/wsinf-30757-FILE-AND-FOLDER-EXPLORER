using System;
using System.IO;
using System.Collections.Generic;
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

namespace explorer_wsinf
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var drive in Directory.GetLogicalDrives())
            {
                var item = new TreeViewItem();

                item.Header = drive;
                item.Tag = drive;
                item.Items.Add(null);


                //Listen 
                item.Expanded += FolderExpanded;
                explorerView.Items.Add(item);

            }


            var all = (TreeViewItem)explorerView.Items[0];
            string userName = Environment.UserName;
            JumpToNode(all, $"C:\\");
            Console.WriteLine(all.Tag);

        }
        private void ItemSelected(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem)sender;

            Console.WriteLine((string)item.Tag);
        }


        private void FolderExpanded(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem)sender;
            if (item.Items.Count != 1 || item.Items[0] != null)
            {
                return;
            }
            item.Items.Clear();
            var fullPath = item.Tag.ToString();

            #region Get Folders
            var directories = new List<string>();

            try
            {
                var dirs = Directory.GetDirectories(fullPath);
                if (dirs.Length > 0)
                {
                    directories.AddRange(dirs);
                }
            }
            catch { }


            foreach (var directoryPath in directories)
            {
                var subItem = new TreeViewItem();
                subItem.Header = GetFileFolderName(directoryPath);
                subItem.Tag = directoryPath;


                subItem.Items.Add(null);
                subItem.Expanded += FolderExpanded;
                item.Items.Add(subItem);

            }
            
            var files = new List<string>();

            try
            {
                var fs = Directory.GetFiles(fullPath);
                if (fs.Length > 0)
                {
                    files.AddRange(fs);
                }
            }
            catch { }


            foreach (var filePath in files)
            {
                var subItem = new TreeViewItem();
                subItem.Header = GetFileFolderName(filePath);
                subItem.Tag = filePath;


                //add to parent
                item.Items.Add(subItem);

            }

            #endregion

        }
        private void SelectedItem(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selected = (TreeViewItem)e.NewValue;
            var fullPath = (string)selected.Tag;
            var metaData = new FileInfo(fullPath);
            image.Source = new BitmapImage();


            try
            {
                var files = new List<DirectoryInfo>();

                var details = new List<FileFolderDetails>();
                var allFiles = new DirectoryInfo(fullPath).GetFiles();
                var allFolders = new DirectoryInfo(fullPath).GetDirectories();



                for (int i = 0; i < allFiles.Length; i++)
                {
                    var fd = new FileFolderDetails();
                    Console.WriteLine(allFiles[i].Extension);
                    if (allFiles[i].Extension == ".txt")
                        fd.FileImage = $"pack://application:,,,/Images/file.ico";
                    else
                        fd.FileImage = $"pack://application:,,,/Images/txt.ico";

                    fd.FileName = allFiles[i].Name;
                    fd.FileCreation = allFiles[i].CreationTime.ToString();
                   
                    fd.IsFile = true;
                    details.Add(fd);
                }


                for (int i = 0; i < allFolders.Length; i++)
                {
                    var fd = new FileFolderDetails();
                    fd.FileName = allFolders[i].Name;
                    fd.FileCreation = allFolders[i].CreationTime.ToString();
                    fd.FileImage = $"pack://application:,,,/Images/folder.ico";
                    fd.IsFolder = true;
                    fd.Path = fullPath + "\\" + allFolders[i].Name;
                    details.Add(fd);
                }


            }
            catch
            {
                

                var fileInfo_str = "";
                fileInfo_str = fileInfo_str + "Nazwa: " + metaData.Name + "\n";
                fileInfo_str = fileInfo_str + "Typ: " + metaData.Extension + "\n";
                fileInfo_str = fileInfo_str + "Data utowrzenia: " + metaData.CreationTime + "\n";
                fileInfo_str = fileInfo_str + "Rozmiar: " + (metaData.Length / 1024.0) + " kb (" + metaData.Length + ")\n";
                fileInfo_str = imgAndFileText(metaData, fileInfo_str);

                textBlock.Text = fileInfo_str;



            }

        }

        private string imgAndFileText(FileInfo metaData, string fileInfo_str)
        {
            if (metaData.Extension == ".txt" || metaData.Extension == ".css" || metaData.Extension == ".html")
            {
                
                string text = System.IO.File.ReadAllText(@metaData.FullName);
                string[] lines = System.IO.File.ReadAllLines(@metaData.FullName);
                var i = 0;
                foreach (string line in lines)
                {
                    if (i < 5)
                    {
                        fileInfo_str = fileInfo_str + line + "\n";
                        i++;
                    }
                }
            }
            else if (metaData.Extension == ".jpg" || metaData.Extension == ".png")
            {
                string selectedFileName = metaData.FullName;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedFileName);
                bitmap.EndInit();
                image.Source = bitmap;
            }

            return fileInfo_str;
        }

        private void SingleFileSelected(object sender, SelectionChangedEventArgs e)
        {
            

            var source = (ListBox)e.Source;
            var selected = (FileFolderDetails)source.SelectedItem;

        }
        TreeViewItem TryGetClickedItem(TreeView treeView, MouseButtonEventArgs e)
        {
            var hit = e.OriginalSource as DependencyObject;
            while (hit != null && !(hit is TreeViewItem))
                hit = VisualTreeHelper.GetParent(hit);

            return hit as TreeViewItem;
        }

        private void ItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            var clickedItem = TryGetClickedItem(explorerView, e);
            if (clickedItem == null)
                return;

            e.Handled = true;

            FileAttributes attr = File.GetAttributes(@clickedItem.Tag.ToString());

            if (attr.HasFlag(FileAttributes.Directory))
                return;
            else
                System.Diagnostics.Process.Start(clickedItem.Tag.ToString());

        }
      


        void JumpToNode(TreeViewItem tvi, string NodeName)
        {
            if (tvi.Tag.ToString() == NodeName)
            {
                tvi.IsExpanded = true;
                tvi.BringIntoView();
                return;
            }
            else
                tvi.IsExpanded = false;

            if (tvi.HasItems)
            {
                foreach (var item in tvi.Items)
                {
                    TreeViewItem temp = item as TreeViewItem;
                    JumpToNode(temp, NodeName);
                }
            }
        }
        public static string GetFileFolderName(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }


            var normalizedPath = path.Replace('/', '\\');
            var lastIndex = normalizedPath.LastIndexOf('\\');

            if (lastIndex <= 0)
            {
                return path;
            }

            return path.Substring(lastIndex + 1);


        }
    }
}
