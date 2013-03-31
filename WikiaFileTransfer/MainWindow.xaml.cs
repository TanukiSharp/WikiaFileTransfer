using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
using WikiaLibrary;
using WikiaFileTransfer.ViewModels;

namespace WikiaFileTransfer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RootViewModel rootViewModel;

        public MainWindow()
        {
            InitializeComponent();

            rootViewModel = new RootViewModel(this);
            DataContext = rootViewModel;

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            rootViewModel.ConnectCommand.Execute(null);
        }
    }
}
