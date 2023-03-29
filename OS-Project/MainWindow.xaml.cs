using OS_Project.ViewModels;
using System;
using System.Collections.Generic;
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


namespace OS_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModels();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CC.Content = new TreeViewViewModel();
            DefaultBtn.IsChecked = true;
        }

    


        private void ShowHome_Click(object sender, RoutedEventArgs e)
        {
            CC.Content = new TreeViewViewModel();
        }

        private void ShowAbout_Click(object sender, RoutedEventArgs e)
        {
            CC.Content = new AboutViewModel();
        }

        private void ShowMembers_Click(object sender, RoutedEventArgs e)
        {
            CC.Content = new MemberViewModel();
        }

       
    }
}
