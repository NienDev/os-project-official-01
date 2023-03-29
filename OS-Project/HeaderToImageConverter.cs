using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Runtime.CompilerServices;
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
                if (temp.info.fullpath.Contains(".jpg"))
                {
                    image = "images/jpg.png";
                } else if (temp.info.fullpath.Contains(".zip"))
                {
                    image = "images/zip.png";
                } else if (temp.info.fullpath.Contains(".txt"))
                {
                    image = "images/txt.png";
                } else if (temp.info.fullpath.Contains(".xls"))
                {
                    image = "images/xls.png";
                }
                else if (temp.info.fullpath.Contains(".xml"))
                {
                    image = "images/xml.png";
                }
                else if (temp.info.fullpath.Contains(".py"))
                {
                    image = "images/py.png";
                }
                else if (temp.info.fullpath.Contains(".rar"))
                {
                    image = "images/rar.png";
                }
                else if (temp.info.fullpath.Contains(".svg"))
                {
                    image = "images/svg.png";
                }
                else if (temp.info.fullpath.Contains(".php"))
                {
                    image = "images/php.png";
                }
                else if (temp.info.fullpath.Contains(".png"))
                {
                    image = "images/png.png";
                }
                else if (temp.info.fullpath.Contains(".ppt"))
                {
                    image = "images/ppt.png";
                }
                else if (temp.info.fullpath.Contains(".js"))
                {
                    image = "images/js.png";
                }
                else if (temp.info.fullpath.Contains(".pdf"))
                {
                    image = "images/pdf.png";
                }
                else if (temp.info.fullpath.Contains(".gif")) 
                {
                    image = "images/gif.png";
                }
                else if (temp.info.fullpath.Contains(".html"))
                {
                    image = "images/html.png";
                }
                else if (temp.info.fullpath.Contains(".dat"))
                {
                    image = "images/dat.png";
                }
                else if (temp.info.fullpath.Contains(".docx"))
                {
                    image = "images/docx.png";
                }
                else if (temp.info.fullpath.Contains(".exe"))
                {
                    image = "images/exe.png";
                }
                else if (temp.info.fullpath.Contains(".cs"))
                {
                    image = "images/cs.png";
                }
                else if (temp.info.fullpath.Contains(".css"))
                {
                    image = "images/css.png";
                }
                else if (temp.info.fullpath.Contains(".csv"))
                {
                    image = "images/csv.png";
                }
                else if (temp.info.fullpath.Contains(".bin"))
                {
                    image = "images/bin.png";
                }
                else if (temp.info.fullpath.Contains(".c++"))
                {
                    image = "images/c++.png";
                }
                else if (temp.info.fullpath.Contains(".sql"))
                {
                    image = "images/sql.png";
                }
            } else
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
