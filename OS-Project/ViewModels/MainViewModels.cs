using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OS_Project.ViewModels
{
    public class MainViewModels : ObservableObject
    {
        private bool _isMaximized { get; set; }

        #region Prototypes Window State functions
        /// <summary>
        /// Prototypes Window State functions
        /// </summary>
        public RelayCommand CloseWindowCommand { get; set; }
        public RelayCommand MaximizeWindowCommand { get; set; }
        public RelayCommand MinimizeWindowCommand { get; set; }
        #endregion



        public bool IsMaximized { 
            get { return _isMaximized; }
            set
            {
                _isMaximized = value;
                OnPropertyChanged("IsMaximized");
            }
        }

        public MainViewModels()
        {
            CloseWindowCommand = new RelayCommand(CloseWindow);
            MaximizeWindowCommand = new RelayCommand(MaximizeWindow);
            MinimizeWindowCommand = new RelayCommand(MinimizeWindow);

            _isMaximized = false;
        }

        #region Window State Functions
        /// <summary>
        /// Window state functions
        /// </summary>
        /// <param name="something"></param>
        public void CloseWindow(object something)
        {
            Application.Current.MainWindow.Close();   
        }

        public void MaximizeWindow(object something)
        {
            if (_isMaximized)
            {
                _isMaximized = false;
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            } else
            {
                _isMaximized = true;
                Application.Current.MainWindow.WindowState = WindowState.Maximized;

            }
            OnPropertyChanged("IsMaximized");
        }

        public void MinimizeWindow(object something)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        #endregion

    }


}
