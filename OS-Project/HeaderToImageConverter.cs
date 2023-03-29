using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using static OS_Project.Views.TreeView;

namespace OS_Project
{

    [ValueConversion(typeof(string), typeof(BitmapImage))]
    public class HeaderToImageConverter : IValueConverter
    {
       

        public static HeaderToImageConverter Instance = new HeaderToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isFile;
            bool isExpanded;

            
                Node temp = (Node)value;
                if (temp.info.type != null)
                {
                    isFile = temp.info.type == "File" ? true : false;
                }else
                {
                    isFile = temp.info.isArchive == "True" ? true : false;
                }
                
                isExpanded = temp.info.isExpanded;
            

            string image = "images/open-folder.png";

            if (isFile)
            {
                image = "images/file.png";
            }
            else
            {
                if (!isExpanded)
                {
                    image = "images/folder.png";
                }
            }
            

            return new BitmapImage(new Uri($"pack://application:,,,/{image}"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
