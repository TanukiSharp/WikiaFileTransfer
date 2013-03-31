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
        private static readonly string AppTitle = "Wikia File Transfer";

        public MainWindow()
        {
            InitializeComponent();
            Title = AppTitle;

            DataContext = new RootViewModel();

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            var result = await RunLoginProcess();
            if (result)
                Title = AppTitle;
            else
                Title = string.Format("{0} - Not connected", AppTitle);
        }

        private async Task<bool> RunLoginProcess()
        {
            string url = null;
            string username = null;

            while (true)
            {
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);

                var loginWindow = new LoginWindow(url, username);
                loginWindow.Owner = this;

                if (loginWindow.ShowDialog() != true)
                    return false;

                Title = string.Format("{0} - Connecting...", AppTitle);

                client.BaseAddress = new Uri(loginWindow.Url, UriKind.Absolute);

                url = loginWindow.Url;
                username = loginWindow.Username;

                var result = await Utility.LoginAsync(loginWindow.Username, loginWindow.Password, cookieContainer, client);
                if (result)
                {
                    App.Client = client;
                    break;
                }

                Title = string.Format("{0} - Failed to connect", AppTitle);
            }

            return true;
        }
    }
}
