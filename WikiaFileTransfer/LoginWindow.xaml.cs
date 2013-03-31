using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WikiaFileTransfer
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow(string url, string username)
        {
            InitializeComponent();

            txtUrl.Text = url;
            txtUsername.Text = username;

            Loaded += (ss, ee) => MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        public string Url { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        private void OnAccept(object sender, RoutedEventArgs e)
        {
            Url = txtUrl.Text.Trim();

            if (Url.Length > 0)
            {
                if (Regex.IsMatch(txtUrl.Text, "^[A-Za-z]+://") == false)
                    Url = "http://" + Url;

                if (Url.EndsWith("/") == false)
                    Url += "/";
            }

            Username = txtUsername.Text.Trim();
            Password = txtPassword.Password;

            DialogResult = true;
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Url = null;
            Username = null;
            Password = null;

            DialogResult = false;
        }
    }
}
