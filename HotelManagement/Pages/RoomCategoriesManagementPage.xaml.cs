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
	/// Interaction logic for RoomCategoriesManagementPage.xaml
	/// </summary>
	public partial class RoomCategoriesManagementPage : Page
	{
		private DatabaseUtilities _databaseUtilities = DatabaseUtilities.GetDatabaseInstance();
		private ApplicationUtilities _applicationUtilities = ApplicationUtilities.GetAppInstance();

		public List<Phong> rooms;
		public List<LoaiPhong> roomCategories;
		private int _editedID = -1;
		public RoomCategoriesManagementPage()
		{
			InitializeComponent();
		}

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
			roomCategories = _databaseUtilities.getAllRoomCategory();

			roomCategoriesListView.ItemsSource = roomCategories;
		}

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
			iconAddRoomCategory.Source = (ImageSource)FindResource("IconWhiteUpdate");
			//iconAddRoom.Source = (ImageSource)FindResource("IconWhiteAdd");

			_editedID = Convert.ToInt32(((Button)sender).Tag);
			LoaiPhong selectedRoomCategory = (from r in roomCategories
											  where r.ID_LoaiPhong == _editedID
											  select r).First();

			roomCatTextBox.Text = selectedRoomCategory.TenLoaiPhong;
			memberReceiptMoneyTextBox.Text = (selectedRoomCategory.DonGia ?? 0).ToString();
			maxNumOfCustomerTextBox.Text = selectedRoomCategory.SLKhachToiDa.ToString();
		}

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
			var deleteId = Convert.ToInt32(((Button)sender).Tag);
			LoaiPhong selectedRoomCategory = (from r in roomCategories
											  where r.ID_LoaiPhong == deleteId
											  select r).First();
			_databaseUtilities.deleteRoomCategory(selectedRoomCategory);

			roomCategories = (from r in roomCategories
								where r.ID_LoaiPhong != deleteId
								select r).ToList();

			roomCategoriesListView.ItemsSource = null;
			roomCategoriesListView.ItemsSource = roomCategories;
		}

        private void addCustomerButton_Click(object sender, RoutedEventArgs e)
        {
			if (_editedID != -1)
			{

				LoaiPhong selectedRoomCategory = (from r in roomCategories
												 where r.ID_LoaiPhong == _editedID
												 select r).First();

				selectedRoomCategory.TenLoaiPhong = roomCatTextBox.Text;

				if (selectedRoomCategory.TenLoaiPhong.Length <= 0)
				{
					//notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống tên loại phòng", "OK", () => { });
					return;
				}

				if (memberReceiptMoneyTextBox.Text.Length <= 0)
				{
					//notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống đơn giá", "OK", () => { });
					return;
				}
				selectedRoomCategory.DonGia = Convert.ToInt32(memberReceiptMoneyTextBox.Text);

				if (maxNumOfCustomerTextBox.Text.Length <= 0)
				{
					//notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống SL khách tối đa", "OK", () => { });
					return;
				}
				selectedRoomCategory.SLKhachToiDa = Convert.ToInt32(maxNumOfCustomerTextBox.Text);

				_databaseUtilities.updateRoomCategory(selectedRoomCategory);
				roomCategories = _databaseUtilities.getAllRoomCategory();

				roomCategoriesListView.ItemsSource = null;
				roomCategoriesListView.ItemsSource = roomCategories;

				//Rest
				_editedID = -1;
				roomCatTextBox.Text = "";
				memberReceiptMoneyTextBox.Text = "";
				maxNumOfCustomerTextBox.Text = "";

				iconAddRoomCategory.Source = (ImageSource)FindResource("IconWhiteAdd");
			}
			else
            {
				LoaiPhong newRoomCategory = new LoaiPhong();
				newRoomCategory.ID_LoaiPhong = _databaseUtilities.getMaxIdRoomCategory() + 1;
				newRoomCategory.TenLoaiPhong = roomCatTextBox.Text;

				if (newRoomCategory.TenLoaiPhong.Length <= 0)
				{
					//notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống tên loại phòng", "OK", () => { });
					return;
				}

				if (memberReceiptMoneyTextBox.Text.Length <= 0)
				{
					//notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống đơn giá", "OK", () => { });
					return;
				}
				newRoomCategory.DonGia = Convert.ToInt32(memberReceiptMoneyTextBox.Text);

				if (maxNumOfCustomerTextBox.Text.Length <= 0)
				{
					//notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống SL khách tối đa", "OK", () => { });
					return;
				}
				newRoomCategory.SLKhachToiDa = Convert.ToInt32(maxNumOfCustomerTextBox.Text);

				_databaseUtilities.addNewRoomCategory(newRoomCategory);

				roomCategories = _databaseUtilities.getAllRoomCategory();

				roomCategoriesListView.ItemsSource = null;
				roomCategoriesListView.ItemsSource = roomCategories;

				//Rest
				roomCatTextBox.Text = "";
				memberReceiptMoneyTextBox.Text = "";
				maxNumOfCustomerTextBox.Text = "";
			}
		}
    }
}
