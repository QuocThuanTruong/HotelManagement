using HotelManagement.CustomViews;
using HotelManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BCrypt.Net;

namespace HotelManagement
{
	/// <summary>
	/// Interaction logic for LoginScreen.xaml
	/// </summary>
	public partial class LoginScreen : Window
	{

		private DatabaseUtilities _databaseUtilities = DatabaseUtilities.GetDatabaseInstance();
		private ApplicationUtilities _applicationUtilities = ApplicationUtilities.GetAppInstance();
        LoadingDialog loadingDialog;
        AlertDialog alertDialog;

        public LoginScreen()
		{
			InitializeComponent();
            userNameTextBox.Focus();
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
            loadingDialog = new LoadingDialog();
            loadingDialog.Show();
			this.Closed += LoginScreen_Closed;
            //get username and password
            //compare
            //get role
            //role = 0 - nhân viên, 1 - quản lí

            if (userNameTextBox.Text.Length == 0)
            {
                //notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống tên đăng nhập", "OK", () => { });

               
            }

            if (passwordTextField.Password.Length == 0)
            {
                //notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống mật khẩu", "OK", () => { });
                
              
            }

            Global.staticCurrentEmployee = _databaseUtilities.checkLogin(userNameTextBox.Text, passwordTextField.Password);

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
            }
            else
			{
                loadingDialog.Close();
                alertDialog = new AlertDialog("Tài khoản hoặc mật khẩu không đúng", false);
                alertDialog.Show();
			}
        }

		private void LoginScreen_Closed(object sender, EventArgs e)
		{
            if (loadingDialog != null)
            {

                loadingDialog.Close();
            }

            if (alertDialog != null)
			{
                alertDialog.Close();
            }

        }

		//private void btnShowPassword_Click(object sender, RoutedEventArgs e)
		//{
  //          if (_showPassword == false)
		//	{
  //              showPasswordImage.Source = (ImageSource)FindResource("IconPurpleEyeOff");
  //              _showPassword = true;
  //              showPasswordTextBox.Text = passwordTextBox.Password;
  //              showPasswordTextBox.CaretIndex = showPasswordTextBox.Text.Length;
  //              passwordTextBox.Visibility = Visibility.Collapsed;
  //              showPasswordTextBox.Visibility = Visibility.Visible;
  //              showPasswordTextBox.Focus();
  //          } 
  //          else
		//	{
  //              showPasswordImage.Source = (ImageSource)FindResource("IconPurpleEye");
  //              _showPassword = false;
  //              passwordTextBox.Password = showPasswordTextBox.Text;
  //              showPasswordTextBox.CaretIndex = showPasswordTextBox.Text.Length;
  //              showPasswordTextBox.Visibility = Visibility.Collapsed;
  //              passwordTextBox.Visibility = Visibility.Visible;
  //              passwordTextBox.Focus();
  //          }
           
  //      }

		//private void showPasswordTextBox_PreviewTextInput(object sender, EventArgs e)
		//{
  //          if (passwordTextBox.Password.Length > 0 || showPasswordTextBox.Text.Length > 0)
		//	{
  //              btnShowPassword.Visibility = Visibility.Visible;
		//	} else
		//	{
  //              btnShowPassword.Visibility = Visibility.Collapsed;
  //          }


  //          if (_showPassword == false)
		//	{
  //              showPasswordTextBox.Text = passwordTextBox.Password;
  //          } 
  //          else
		//	{
              
  //              passwordTextBox.Password = showPasswordTextBox.Text;
  //          }

  //          Console.WriteLine("Hide pass: " + passwordTextBox.Password);
  //          Console.WriteLine("Show pass: " + showPasswordTextBox.Text);
           
  //      }

		//private void showPasswordTextBox_GotFocus(object sender, RoutedEventArgs e)
		//{
  //          if (passwordTextBox.Password.Length > 0 || showPasswordTextBox.Text.Length > 0)
  //          {
  //              btnShowPassword.Visibility = Visibility.Visible;
  //          }
  //          else
  //          {
  //              btnShowPassword.Visibility = Visibility.Collapsed;
  //          }
  //      }

		//private void showPasswordTextBox_LostFocus(object sender, RoutedEventArgs e)
		//{
  //          if (passwordTextBox.Password.Length > 0 || showPasswordTextBox.Text.Length > 0)
  //          {
  //              btnShowPassword.Visibility = Visibility.Visible;
  //          }
  //          else
  //          {
  //              btnShowPassword.Visibility = Visibility.Collapsed;
  //          }
  //      }
	}
}
