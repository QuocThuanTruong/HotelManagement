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
			int role = (new Random()).Next(int.MaxValue) & 1;
			LoginScreen loginScreen = this;

			MainScreen mainScreen = new MainScreen(role);
			mainScreen.Show();
			loginScreen.Close();

		}
	}
}
