using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace explorer_wsinf
{
    [ValueConversion(typeof(string), typeof(BitmapImage))]
    class iconsImageConvert : IValueConverter
    {

        public static iconsImageConvert Instance = new iconsImageConvert();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = (string)value;

            if (path == null)
            {
                return null;
            }
            var name = explorer_wsinf.MainWindow.GetFileFolderName(path);
            var icon = "file.ico";
            if (string.IsNullOrEmpty(name))
            {
                icon = "drive.ico";
            } else if (new FileInfo(path).Attributes.HasFlag(FileAttributes.Directory))
            {
                icon = "folder.ico";
            }
            else if (new FileInfo(path).Extension == ".txt" || new FileInfo(path).Extension == ".html" || new FileInfo(path).Extension == ".css")
            {
                icon = "txt.ico";
            }


            return new BitmapImage(new Uri($"pack://application:,,,/icons/{icon}"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
