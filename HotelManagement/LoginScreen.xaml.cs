using HotelManagement.Utilities;
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
using System.Windows.Shapes;

namespace HotelManagement
{
	/// <summary>
	/// Interaction logic for LoginScreen.xaml
	/// </summary>
	public partial class LoginScreen : Window
	{

		private DatabaseUtilities _databaseUtilities = DatabaseUtilities.GetDatabaseInstance();
		private ApplicationUtilities _applicationUtilities = ApplicationUtilities.GetAppInstance();

		public LoginScreen()
		{
			InitializeComponent();
		}

		private void closeWindowButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void minimizeWindowButton_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}

		private void loginButton_Click(object sender, RoutedEventArgs e)
		{
            //get username and password
            //compare
            //get role
            //role = 0 - nhân viên, 1 - quản lí

           /* if (userNameTextBox.Text.Length == 0)
            {
                //notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống tên đăng nhập", "OK", () => { });

                return;
            }

            if (passwordTextBox.Text.Length == 0)
            {
                //notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống mật khẩu", "OK", () => { });

                return;
            }

            Global.staticCurrentEmployee = _databaseUtilities.checkLogin(userNameTextBox.Text, passwordTextBox.Text);

            if (Global.staticCurrentEmployee != null)
            {
                LoginScreen loginScreen = this;

                int role = 1;

                if (Global.staticCurrentEmployee.LoaiNhanVien == true)
                {
                    role = 1;
                }
                else
                {
                    role = 0;
                }

                MainScreen mainScreen = new MainScreen(role);
                mainScreen.Show();
                loginScreen.Close();
            }*/

            //notiMessageSnackbar.MessageQueue.Enqueue($"Sai tên đăng nhập hoặc mật khẩu", "OK", () => { });

            LoginScreen loginScreen = this;
            int role = 1;
            MainScreen mainScreen = new MainScreen(role);
            mainScreen.Show();
            loginScreen.Close();
        }
	}
}
