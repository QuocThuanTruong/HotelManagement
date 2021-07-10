using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace HotelManagement.CustomViews
{
	/// <summary>
	/// Interaction logic for PasswordTextField.xaml
	/// </summary>
	public partial class PasswordTextField : UserControl
	{
        public String Password { get; set; }
        public String PasswordHint { get; set; } = "Mật khẩu";

        private bool _showPassword = false;


		public PasswordTextField()
		{
			InitializeComponent();
            DataContext = this;
            
		}

        private void btnShowPassword_Click(object sender, RoutedEventArgs e)
        {
            if (_showPassword == false)
            {
                showPasswordImage.Source = (ImageSource)FindResource("IconPurpleEyeOff");
                _showPassword = true;
                showPasswordTextBox.Text = passwordTextBox.Password;
                showPasswordTextBox.CaretIndex = showPasswordTextBox.Text.Length;
                passwordTextBox.Visibility = Visibility.Collapsed;
                showPasswordTextBox.Visibility = Visibility.Visible;
                showPasswordTextBox.Focus();
            }
            else
            {
                showPasswordImage.Source = (ImageSource)FindResource("IconPurpleEye");
                _showPassword = false;
                passwordTextBox.Password = showPasswordTextBox.Text;
                SetSelection(passwordTextBox, showPasswordTextBox.Text.Length, 0);
                showPasswordTextBox.Visibility = Visibility.Collapsed;
                passwordTextBox.Visibility = Visibility.Visible;
                passwordTextBox.Focus();
            }

        }


        private void SetSelection(PasswordBox passwordBox, int start, int length)
        {
            passwordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(passwordBox, new object[] { start, length });
        }

        private void showPasswordTextBox_PreviewTextInput(object sender, EventArgs e)
        {
            if (passwordTextBox.Password.Length > 0 || showPasswordTextBox.Text.Length > 0)
            {
                btnShowPassword.Visibility = Visibility.Visible;
            }
            else
            {
                btnShowPassword.Visibility = Visibility.Collapsed;
            }


            if (_showPassword == false)
            {
                showPasswordTextBox.Text = passwordTextBox.Password;
            }
            else
            {

                passwordTextBox.Password = showPasswordTextBox.Text;
            }

            Password = showPasswordTextBox.Text;

        }

        private void showPasswordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (passwordTextBox.Password.Length > 0 || showPasswordTextBox.Text.Length > 0)
            {
                btnShowPassword.Visibility = Visibility.Visible;
            }
            else
            {
                btnShowPassword.Visibility = Visibility.Collapsed;
            }
        }

        private void showPasswordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (passwordTextBox.Password.Length > 0 || showPasswordTextBox.Text.Length > 0)
            {
                btnShowPassword.Visibility = Visibility.Visible;
            }
            else
            {
                btnShowPassword.Visibility = Visibility.Collapsed;
            }
        }

    }
}
