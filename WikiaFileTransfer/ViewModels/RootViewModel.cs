using PresentationToolKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WikiaLibrary;

namespace WikiaFileTransfer.ViewModels
{
    public class RootViewModel : ViewModelBase
    {
        public static HttpClient Client { get; private set; }
        public static readonly string AppTitle = "Wikia File Transfer";

        public UploadViewModel Upload { get; private set; }

        private Window window;

        public RootViewModel(Window window)
        {
            if (window == null)
                throw new ArgumentNullException("window");

            this.window = window;

            Upload = new UploadViewModel();

            ConnectCommand = new AnonymousCommand(OnConnect);
            CloseCommand = new AnonymousCommand(App.Current.Shutdown);

            AboutCommand = new AnonymousCommand(OnAbout);
        }

        public ICommand ConnectCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand AboutCommand { get; private set; }

        private void OnAbout()
        {
            var sb = new StringBuilder();

            sb.AppendLine(AppTitle);
            sb.AppendLine(Assembly.GetEntryAssembly().GetName().Version.ToString());
            sb.AppendLine();
            sb.AppendLine(string.Format("Sebastien ROBERT - 2013"));

            MessageBox.Show(sb.ToString(), string.Format("About {0}", AppTitle), MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void OnConnect()
        {
            var result = await RunLoginProcess();
            if (result)
                window.Title = AppTitle;
            else
                window.Title = string.Format("{0} - Not connected", AppTitle);
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
                loginWindow.Owner = window;

                if (loginWindow.ShowDialog() != true)
                    return false;

                window.Title = string.Format("{0} - Connecting...", AppTitle);

                client.BaseAddress = new Uri(loginWindow.Url, UriKind.Absolute);

                url = loginWindow.Url;
                username = loginWindow.Username;

                var result = await Utility.LoginAsync(loginWindow.Username, loginWindow.Password, cookieContainer, client);
                if (result)
                {
                    Client = client;
                    break;
                }

                window.Title = string.Format("{0} - Failed to connect", AppTitle);
            }

            return true;
        }
    }
}
