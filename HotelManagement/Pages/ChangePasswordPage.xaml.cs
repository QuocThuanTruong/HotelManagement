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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HotelManagement.Pages
{
	/// <summary>
	/// Interaction logic for ChangePasswordPage.xaml
	/// </summary>
	public partial class ChangePasswordPage : Page
	{

		private DatabaseUtilities _databaseUtilities = DatabaseUtilities.GetDatabaseInstance();
		private ApplicationUtilities _applicationUtilities = ApplicationUtilities.GetAppInstance();
		public ChangePasswordPage()
		{
			InitializeComponent();
		}

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
			string oldPassword = oldPwdTextBox.Text;
			string newPassword = newPwdTextBox.Text;
			string retypePassword = retypePwdTextBox.Text;

			if (oldPassword != Global.staticCurrentEmployee.Password)
            {

				notiMessageSnackbar.MessageQueue.Enqueue($"Mật khẩu không chính xác", "OK", () => { });

				return;
            }

			if (newPassword.Length == 0)
            {
				notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống mật khẩu mớp", "OK", () => { });

				return;
			}

			if (newPassword != retypePassword)
			{
				notiMessageSnackbar.MessageQueue.Enqueue($"Mật khẩu mới và lặp lại mật khẩu không chính xác", "OK", () => { });

				return;
			}

			_databaseUtilities.updatePassword(newPassword);

		}
    }
}
