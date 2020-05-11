using System;
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
using TopHope.Core;

namespace TopHope
{
    /// <summary>
    /// Interaction logic for loginPage.xaml
    /// </summary>
    public partial class loginPage : Page
    {
        private TopHopeMain topHopeMain = new TopHopeMain();
        private TopHopeAuth topHopeAuth = new TopHopeAuth();
        bool isTryLogIn;
        public loginPage()
        {
            InitializeComponent();
        }

        private async void Button_Click_login(object sender, RoutedEventArgs e)
        {
            bool isUserInfoReady = false;
            loginButton.IsEnabled = false;
            if (topHopeMain.Client != null)
            {
                MessageBoxResult MBRes = MessageBox.Show("You want login in one more time?", "Warning", MessageBoxButton.YesNo);
                if (MBRes == MessageBoxResult.Yes)
                {
                    await topHopeAuth.clientCreate();
                    isTryLogIn = await topHopeAuth.Authorization(loginTextBox.Text, PasswordTextBox.Password);
                    if (isTryLogIn)
                    {
                        topHopeMain.Client = await topHopeAuth.checkLogin();
                        if (topHopeMain.Client != null)
                        {
                            isUserInfoReady = await topHopeMain.getUserInfo();
                        }
                        loginButton.IsEnabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Something go wrong");
                        loginButton.IsEnabled = true;
                    }

                }
                else
                {
                    loginButton.IsEnabled = true;
                }
            }
            else
            {
                await topHopeAuth.clientCreate();
                isTryLogIn = await topHopeAuth.Authorization(loginTextBox.Text, PasswordTextBox.Password);
                if (isTryLogIn)
                {
                    topHopeMain.Client = await topHopeAuth.checkLogin();
                    if (topHopeMain.Client != null)
                    {
                        isUserInfoReady = await topHopeMain.getUserInfo();
                    }
                    loginButton.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("Something go wrong");
                    loginButton.IsEnabled = true;
                }
            }
            if (isUserInfoReady == true)
            {
                fillUserInfo();
            }
        }

        private void fillUserInfo()
        {
            string[] strTemp = topHopeMain.fillUserInfo();
            nickName.Content = strTemp[0];
            messages.Content = strTemp[1];
            thanks.Content = strTemp[2];
            points.Content = strTemp[3];
            if (strTemp[4] != "")
            {
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri(strTemp[4], UriKind.Absolute);
                bi3.EndInit();
                userImage.Source = bi3;
            }
            else
            {
                userImage.Source = null;
            }

        }

        private void Button_Click_logout(object sender, RoutedEventArgs e)
        {
            logoutButton.IsEnabled = false;
            if (topHopeMain.Client == null)
            {
                MessageBox.Show("You are not login in yet!");
                logoutButton.IsEnabled = true;
                return;
            }
            else
            {

                topHopeAuth.Logout();
                topHopeMain.topHopeMainLogout();
                fillUserInfo();
                logoutButton.IsEnabled = true;
            }

        }
    }
}
