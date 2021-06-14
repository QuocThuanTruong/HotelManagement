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
	/// Interaction logic for CreateInvoicePage.xaml
	/// </summary>
	public partial class CreateInvoicePage : Page
	{
		public delegate void BackPage(Button page);
		public event BackPage BackPageEvent;

		private int _idRoom;
		private Button backPage;
		private int _idRentBill;

		public Phong selectedRoom;
		public PhieuThue currentRentBill;

		public int surcharge;
		public int resultPrice;

		private DatabaseUtilities _databaseUtilities = DatabaseUtilities.GetDatabaseInstance();
		private ApplicationUtilities _applicationUtilities = ApplicationUtilities.GetAppInstance();

		public DateTime now;
		public List<KhachHang> currentCustomers;
		public CreateInvoicePage()
		{
			InitializeComponent();
		}

		public CreateInvoicePage(int id, Button backPage)
		{
			InitializeComponent();
			this._idRoom = id;
			this.backPage = backPage;
		}

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
			selectedRoom = _databaseUtilities.getRoomById(_idRoom);

			reciptionistTextBlock.Text = Global.staticCurrentEmployee.HoTen;

			checkoutDateTextBlock.Text = DateTime.Now.ToString("dd/MM/yyyy");
			maxCustomerTextBlock.Text = $"Phòng {selectedRoom.SLKhachToiDa + 1} khách trở lên phụ thu 25% 1 người";

			_idRentBill = _databaseUtilities.getCurrentRentBillIdBelongToRoom(_idRoom);

			currentCustomers = _databaseUtilities.getCurrentCustomerInRoom(_idRentBill);

			customerNameComboBox.ItemsSource = currentCustomers;

			currentRentBill = _databaseUtilities.getRentBillById(_idRentBill);

			currentRentBill.SoPhong_For_Binding = selectedRoom.SoPhong;
			currentRentBill.Total_Customer_For_Binding = currentCustomers.Count;
			currentRentBill.Total_Day_For_Binding = Convert.ToInt32((DateTime.Now - currentRentBill.NgayBatDau).Value.TotalDays);
			currentRentBill.Ratio_For_Binding = 1;

			foreach (var customer in currentCustomers)
            {
				if (customer.TenLoaiKhach == "Nước ngoài")
                {
					currentRentBill.Ratio_For_Binding = 1.5;
				}
            }

			currentRentBill.Price_Per_Day_For_Binding = selectedRoom.DonGia_For_Binding;
			currentRentBill.TotalPrice = Convert.ToInt32((selectedRoom.DonGia ?? 0) * currentRentBill.Ratio_For_Binding);
			currentRentBill.Total_Price_For_Binding = _applicationUtilities.getMoneyForBinding(currentRentBill.TotalPrice);

			List<PhieuThue> source = new List<PhieuThue>();
			source.Add(currentRentBill);
			roomRevenueList.ItemsSource = source;


			surcharge = Convert.ToInt32(currentRentBill.TotalPrice * (currentRentBill.Total_Customer_For_Binding - selectedRoom.SLKhachToiDa) * 0.25);

			if (surcharge < 0)
            {
				surcharge = 0;
			}

			resultPrice = surcharge + currentRentBill.TotalPrice;

			surchargeTextBlock.Text = _applicationUtilities.getMoneyForBinding(surcharge);
			resultPriceTextBlock.Text = _applicationUtilities.getMoneyForBinding(resultPrice);
		}

		private void backButton_Click(object sender, RoutedEventArgs e)
		{
			BackPageEvent?.Invoke(backPage);
		}

        private void finishButton_Click(object sender, RoutedEventArgs e)
        {
			HoaDon newInvoice = new HoaDon();

			newInvoice.ID_HoaDon = _databaseUtilities.getMaxIdInvoice() + 1;
			newInvoice.ID_PhieuThue = currentRentBill.ID_PhieuThue;
			newInvoice.ID_NhanVien = Global.staticCurrentEmployee.ID_NhanVien;
			newInvoice.NgayTraPhong = DateTime.Now;
			newInvoice.TongTien = resultPrice;

			_databaseUtilities.addNewInvoice(newInvoice);

			//notiMessageSnackbar.MessageQueue.Enqueue($"Thanh toán thành công", "OK", () => { BackPageEvent?.Invoke(backPage) });

			_databaseUtilities.updateEmptyRoom(_idRoom);
			_databaseUtilities.finishRentalBill(_idRentBill);

			BackPageEvent?.Invoke(backPage);
		}
    }
}
