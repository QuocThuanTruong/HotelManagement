using HotelManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	/// Interaction logic for CreateRentBillPage.xaml
	/// </summary>
	public partial class CreateRentBillPage : Page
	{
		public delegate void BackPage(Button page);
		public event BackPage BackPageEvent;
		private int _id;
		private Button backPage;

		private DatabaseUtilities _databaseUtilities = DatabaseUtilities.GetDatabaseInstance();
		private ApplicationUtilities _applicationUtilities = ApplicationUtilities.GetAppInstance();

		private List<Phong> _rooms = new List<Phong>();
		public Phong currentRoom = new Phong();
		private List<LoaiKhach> _customerCategory = new List<LoaiKhach>();
		private int _maxCustomerID = 0;
		public List<KhachHang> customers = new List<KhachHang>();
		public CreateRentBillPage()
		{
			InitializeComponent();
		}

		public CreateRentBillPage(int id, Button backPage)
		{
			InitializeComponent();
			this._id = id;
			this.backPage = backPage;
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			_maxCustomerID = _databaseUtilities.getMaxIdCustomer();

			_rooms = _databaseUtilities.getAllRoom();
			_customerCategory = _databaseUtilities.getAllCustomerCategory();

			var currentIndex = 0;

			for (; currentIndex < _rooms.Count; ++currentIndex)
            {
				if (_rooms[currentIndex].SoPhong == _id)
                {
					currentRoom = _rooms[currentIndex];
					break;
                }
            }

			comboboxRoomList.ItemsSource = _rooms;
			comboboxRoomList.SelectedIndex = currentIndex;

			customerTypeComboBox.ItemsSource = _customerCategory;

			reciptionistTextBlock.Text = Global.staticCurrentEmployee.HoTen;
		}

		private void backButton_Click(object sender, RoutedEventArgs e)
		{
			BackPageEvent?.Invoke(backPage);
		}

		private void loadRoom()
        {
			roomCategoryTextBlock.Text = currentRoom.TenLoaiPhong;
			roomPriceTextBlock.Text = currentRoom.DonGia_For_Binding;
			maxCustomerTextBlock.Text = $"Số lượng khách tối đa: {currentRoom.SLKhachToiDa} người";
		}

        private void comboboxRoomList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			var index = comboboxRoomList.SelectedIndex;
			currentRoom = _rooms[index];
			loadRoom();
		}

        private void addCustomerButton_Click(object sender, RoutedEventArgs e)
        {
			if (customers.Count < currentRoom.SLKhachToiDa)
            {
				KhachHang customer = new KhachHang();
				customer.ID_KhachHang = ++_maxCustomerID;

				customer.HoTen = customerNameTextBox.Text;
				if (customer.HoTen.Length <= 0)
				{
					//notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống tên kháhc hàng", "OK", () => { });
					return;
				}

				customer.CMND = customerIDTextBox.Text;
				if (customer.CMND.Length <= 0)
				{
					//notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống CMND của khách hàng", "OK", () => { });
					return;
				}

				customer.DiaChi = customerAddrTextBox.Text;
				if (customer.DiaChi.Length <= 0)
				{
					//notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống địa chỉ của khách hàng", "OK", () => { });
					return;
				}

				customer.ID_LoaiKhach = ((LoaiKhach)customerTypeComboBox.SelectedItem).ID_LoaiKhach;
				customer.TenLoaiKhach = ((LoaiKhach)customerTypeComboBox.SelectedItem).TenLoaiKhach;
				customer.STT_For_Binding = customers.Count + 1;

				//Reset
				customerNameTextBox.Text = "";
				customerIDTextBox.Text = "";
				customerAddrTextBox.Text = "";

				customers.Add(customer);

				customerListView.ItemsSource = null;
				customerListView.ItemsSource = customers;
			}
		}

        private void deleteMemberButton_Click(object sender, RoutedEventArgs e)
        {
			var deleteIndex = Convert.ToInt32(((Button)sender).Tag);

			deleteIndex -= 1;

			for (int i = deleteIndex + 1; i < customers.Count; ++i)
            {
				customers[i].STT_For_Binding -= 1;
            }

			customers.RemoveAt(deleteIndex);

			customerListView.ItemsSource = null;
			customerListView.ItemsSource = customers;
		}

		private void finishButton_Click(object sender, RoutedEventArgs e)
		{
			if (customers.Count == 0)
			{
				//notiMessageSnackbar.MessageQueue.Enqueue($"Không có khách hàng", "OK", () => { });

				return;
			}

			if (!bookingDatePicker.SelectedDate.HasValue)
			{
				//notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống ngày bắt đầu thuê", "OK", () => { });
				return;
			}

			PhieuThue newRental = new PhieuThue();
			newRental.ID_PhieuThue = _databaseUtilities.getMaxIdRental() + 1;
			newRental.NgayBatDau = bookingDatePicker.SelectedDate;

			_databaseUtilities.addNewRental(newRental);

			ChiTietPhieuThue newRentalDetail = new ChiTietPhieuThue();
			newRentalDetail.ID_PhieuThue = newRental.ID_PhieuThue;
			newRentalDetail.SoPhong = currentRoom.SoPhong;
			newRentalDetail.ID_NhanVien = Global.staticCurrentEmployee.ID_NhanVien;

			foreach (var customer in customers)
			{
				_databaseUtilities.addNewCustomer(customer);

				newRentalDetail.ID_KhachHang = customer.ID_KhachHang;

				_databaseUtilities.addNewRentalDetail(newRentalDetail);
			}

			//notiMessageSnackbar.MessageQueue.Enqueue($"Thêm thành công phiếu thuê", "OK", () => { BackPageEvent?.Invoke(backPage) });

			_databaseUtilities.updateRentedRoom(currentRoom.SoPhong);

			BackPageEvent?.Invoke(backPage);
		}
	}
}
