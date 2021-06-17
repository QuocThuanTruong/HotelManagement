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
	/// Interaction logic for CustomerCategoriesManagementPage.xaml
	/// </summary>
	public partial class CustomerCategoriesManagementPage : Page
	{
		private DatabaseUtilities _databaseUtilities = DatabaseUtilities.GetDatabaseInstance();
		private ApplicationUtilities _applicationUtilities = ApplicationUtilities.GetAppInstance();

		public List<Phong> rooms;
		public List<LoaiKhach> customerCategories;
		private int _editedID = -1;

		public CustomerCategoriesManagementPage()
		{
			InitializeComponent();
		}

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
			customerCategories = _databaseUtilities.getAllCustomerCategory();

			customerCategoryList.ItemsSource = customerCategories;
		}

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
			iconAddCustomerCategory.Source = (ImageSource)FindResource("IconWhiteUpdate");
			//iconAddRoom.Source = (ImageSource)FindResource("IconWhiteAdd");

			_editedID = Convert.ToInt32(((Button)sender).Tag);
			LoaiKhach selectedCustomerCategory = (from c in customerCategories
												  where c.ID_LoaiKhach == _editedID
											  select c).First();

			customercatTextBox.Text = selectedCustomerCategory.TenLoaiKhach;
			customerAddrTextBox.Text = (selectedCustomerCategory.HeSo ?? 0).ToString();
		}

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
			var deleteId = Convert.ToInt32(((Button)sender).Tag);
			LoaiKhach selectedCustomerCategory = (from c in customerCategories
											  where c.ID_LoaiKhach == deleteId
											  select c).First();

			_databaseUtilities.deleteCustomerCategory(selectedCustomerCategory);

			customerCategories = (from c in customerCategories
								  where c.ID_LoaiKhach != deleteId
							  select c).ToList();

			customerCategoryList.ItemsSource = null;
			customerCategoryList.ItemsSource = customerCategories;
		}

        private void addCustomerButton_Click(object sender, RoutedEventArgs e)
        {
			if (_editedID != -1)
			{

				LoaiKhach selectedCustomerCategory = (from c in customerCategories
													  where c.ID_LoaiKhach == _editedID
												  select c).First();

				selectedCustomerCategory.TenLoaiKhach = customercatTextBox.Text;

				if (selectedCustomerCategory.TenLoaiKhach.Length <= 0)
				{
					notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống tên loại phòng", "OK", () => { });
					return;
				}

				if (customerAddrTextBox.Text.Length <= 0)
				{
					notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống đơn giá", "OK", () => { });
					return;
				}
				selectedCustomerCategory.HeSo = Convert.ToDouble(customerAddrTextBox.Text);

				_databaseUtilities.updateCustomerCategory(selectedCustomerCategory);
				customerCategories = _databaseUtilities.getAllCustomerCategory();

				customerCategoryList.ItemsSource = null;
				customerCategoryList.ItemsSource = customerCategories;

				//Rest
				_editedID = -1;
				customercatTextBox.Text = "";
				customerAddrTextBox.Text = "";

				iconAddCustomerCategory.Source = (ImageSource)FindResource("IconWhiteAdd");
			}
			else
			{
				LoaiKhach newCustomerCategory = new LoaiKhach();
				newCustomerCategory.ID_LoaiKhach = _databaseUtilities.getMaxIdCustomerCategory() + 1;
				newCustomerCategory.TenLoaiKhach = customercatTextBox.Text;

				if (newCustomerCategory.TenLoaiKhach.Length <= 0)
				{
					notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống tên loại phòng", "OK", () => { });
					return;
				}

				if (customercatTextBox.Text.Length <= 0)
				{
					notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống đơn giá", "OK", () => { });
					return;
				}
				newCustomerCategory.HeSo = Convert.ToDouble(customerAddrTextBox.Text);

				_databaseUtilities.addNewCustomerCategory(newCustomerCategory);

				customerCategories = _databaseUtilities.getAllCustomerCategory();

				customerCategoryList.ItemsSource = null;
				customerCategoryList.ItemsSource = customerCategories;

				//Rest
				customercatTextBox.Text = "";
				customerAddrTextBox.Text = "";
			}
		}
    }
}
