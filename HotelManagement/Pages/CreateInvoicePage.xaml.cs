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
using HotelManagement.Converters;
using OfficeOpenXml;
using System.IO;

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
		private int _idInvoice;
		private Button backPage;
		private int _idRentBill;
		private int _customerSelectedIndex = 0;

		public HoaDon invoice;
		public Phong selectedRoom;
		public PhieuThue currentRentBill;

		public int surcharge;
		public int resultPrice;

		private DatabaseUtilities _databaseUtilities = DatabaseUtilities.GetDatabaseInstance();
		private ApplicationUtilities _applicationUtilities = ApplicationUtilities.GetAppInstance();
		private AbsolutePathConverter _converter = new AbsolutePathConverter();

		public DateTime now;
		public List<KhachHang> currentCustomers;

		private bool _isView;
		private HoaDon _invoice;
		public CreateInvoicePage()
		{
			InitializeComponent();
		}

		public CreateInvoicePage(int id, bool isView, Button backPage)
		{
			InitializeComponent();
			this._idRoom = id;
			this._isView = isView;

			if (this._isView)
            {
				this._idInvoice = id;
				_invoice = _databaseUtilities.getInvoiceById(_idInvoice);

			} 
			else
            {
				exportExcelButton.Visibility = Visibility.Collapsed;
				this._idRoom = id;
            }

			this.backPage = backPage;
		}

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
			if (!_isView) 
			{
				selectedRoom = _databaseUtilities.getRoomById(_idRoom);

				reciptionistTextBlock.Text = Global.staticCurrentEmployee.HoTen;

				checkoutDateTextBlock.Text = DateTime.Now.ToString("dd/MM/yyyy");

				CauHinh config = _databaseUtilities.getConfig();
				maxCustomerTextBlock.Text = $"Phòng {config.DieuKien.Substring(2)} khách trở lên phụ thu {Convert.ToDouble(config.GiaTri) * 100}% 1 người";

				_idRentBill = _databaseUtilities.getCurrentRentBillIdBelongToRoom(_idRoom);

				currentCustomers = _databaseUtilities.getCurrentCustomerInRoom(_idRentBill);

				customerNameComboBox.ItemsSource = currentCustomers;

				currentRentBill = _databaseUtilities.getRentBillById(_idRentBill);

				currentRentBill.SoPhong_For_Binding = selectedRoom.SoPhong;
				currentRentBill.Total_Customer_For_Binding = currentCustomers.Count;
				currentRentBill.Total_Day_For_Binding = Convert.ToInt32((DateTime.Now - currentRentBill.NgayBatDau).Value.TotalDays);

				currentRentBill.Total_Day_For_Binding = currentRentBill.Total_Day_For_Binding == 0 ? 1 : currentRentBill.Total_Day_For_Binding;

				currentRentBill.Ratio_For_Binding = 1;

				currentRentBill.Ratio_For_Binding = _databaseUtilities.getRentBillFactor(_idRentBill);

				currentRentBill.Price_Per_Day_For_Binding = selectedRoom.DonGia_For_Binding;
				currentRentBill.TotalPrice = Convert.ToInt32((selectedRoom.DonGia ?? 0) * currentRentBill.Ratio_For_Binding) * currentRentBill.Total_Day_For_Binding;
				currentRentBill.Total_Price_For_Binding = _applicationUtilities.getMoneyForBinding(currentRentBill.TotalPrice);

				checkinDateTextBlock.Text = (currentRentBill.NgayBatDau ?? DateTime.Now).ToString("dd/MM/yyyy"); ;

				List <PhieuThue> source = new List<PhieuThue>();
				source.Add(currentRentBill);
				roomRevenueList.ItemsSource = source;

				surcharge = Convert.ToInt32(currentRentBill.TotalPrice * (currentRentBill.Total_Customer_For_Binding - Convert.ToInt32(config.DieuKien.Substring(2)) + 1) * Convert.ToDouble(config.GiaTri));

				if (surcharge < 0)
				{
					surcharge = 0;
				}

				resultPrice = surcharge + currentRentBill.TotalPrice;

				surchargeTextBlock.Text = _applicationUtilities.getMoneyForBinding(surcharge);
				resultPriceTextBlock.Text = _applicationUtilities.getMoneyForBinding(resultPrice);
				exportExcelButton.Visibility = Visibility.Collapsed;
			} 
			else
            {
				CauHinh config = _databaseUtilities.getConfig();
				invoice = _databaseUtilities.getInvoiceById(_idInvoice);
				customerNameComboBox.IsReadOnly = true;
				customerNameComboBox.IsEnabled = false;
		

				reciptionistTextBlock.Text = invoice.HoTenNV_For_Binding;
				checkoutDateTextBlock.Text = (invoice.NgayTraPhong ?? DateTime.Now).ToString("dd/MM/yyyy");
				
				currentCustomers = _databaseUtilities.getCurrentCustomerInRoom(invoice.ID_PhieuThue);

				customerNameComboBox.ItemsSource = currentCustomers;

				for (; _customerSelectedIndex < currentCustomers.Count; ++_customerSelectedIndex)
                {
					if (currentCustomers[_customerSelectedIndex].ID_KhachHang == invoice.ID_KhachHang)
                    {
						break;
                    }
                }

				customerNameComboBox.SelectedIndex = _customerSelectedIndex;

				currentRentBill = _databaseUtilities.getRentBillById(invoice.ID_PhieuThue);

				currentRentBill.SoPhong_For_Binding = invoice.SoPhong;
				currentRentBill.Total_Customer_For_Binding = currentCustomers.Count;
				currentRentBill.Total_Day_For_Binding = Convert.ToInt32(invoice.NumDayRent_For_Binding);
				currentRentBill.Ratio_For_Binding = 1;

				currentRentBill.Ratio_For_Binding = _databaseUtilities.getRentBillFactor(invoice.ID_PhieuThue);

				currentRentBill.Price_Per_Day_For_Binding = invoice.DonGia_For_Binding;
				currentRentBill.TotalPrice = Convert.ToInt32(invoice.DonGia * currentRentBill.Ratio_For_Binding) * currentRentBill.Total_Day_For_Binding;
				currentRentBill.Total_Price_For_Binding = _applicationUtilities.getMoneyForBinding(currentRentBill.TotalPrice);
				checkinDateTextBlock.Text = (currentRentBill.NgayBatDau ?? DateTime.Now).ToString("dd/MM/yyyy"); ;


				List<PhieuThue> source = new List<PhieuThue>();
				source.Add(currentRentBill);
				roomRevenueList.ItemsSource = source;

				surcharge = Convert.ToInt32(currentRentBill.TotalPrice * (currentRentBill.Total_Customer_For_Binding - Convert.ToInt32(config.DieuKien.Substring(2)) + 1) * Convert.ToDouble(config.GiaTri));

				if (surcharge < 0)
				{
					surcharge = 0;
				}

				resultPrice = surcharge + currentRentBill.TotalPrice;

				surchargeTextBlock.Text = _applicationUtilities.getMoneyForBinding(surcharge);
				resultPriceTextBlock.Text = _applicationUtilities.getMoneyForBinding(resultPrice);

				finishButton.Visibility = Visibility.Collapsed;
				exportExcelButton.Visibility = Visibility.Visible;
			}

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
			newInvoice.ID_KhachHang = currentCustomers[customerNameComboBox.SelectedIndex].ID_KhachHang;
			newInvoice.NgayTraPhong = DateTime.Now;
			newInvoice.TongTien = resultPrice;

			_databaseUtilities.addNewInvoice(newInvoice);

			_databaseUtilities.updateEmptyRoom(_idRoom);
			_databaseUtilities.finishRentalBill(_idRentBill);

			_invoice = newInvoice;

			exportExcelButton.Visibility = Visibility.Visible;

			notiMessageSnackbar.MessageQueue.Enqueue($"Thanh toán thành công", "OK", () => { BackPageEvent?.Invoke(backPage); });

			finishButton.Visibility = Visibility.Collapsed;
		}

        private void customerNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			if (this.IsLoaded)
            {
				if (_isView)
                {
					customerNameComboBox.SelectedIndex = _customerSelectedIndex;
                }
            }
        }

		private void exportExcelButton_Click(object sender, RoutedEventArgs e)
		{
			Guid guid = Guid.NewGuid();
			string uid = guid.ToString();

			_applicationUtilities.copyFileToDirectory(
				_converter.Convert($"Assets/XLSXTemplate/Hóa-đơn-template.xlsx", null, null, null).ToString(),
				$"Hóa-đơn-{uid}.xlsx");

			ExcelPackage.LicenseContext = LicenseContext.Commercial;
			using (var excelPackage = new ExcelPackage(new FileInfo($"Hóa-đơn-{uid}.xlsx")))
			{
				var rentBill = _databaseUtilities.getRentBillById(_invoice.ID_PhieuThue);
				var room = _databaseUtilities.getRoomById(rentBill.SoPhong_For_Binding);

				var serviceInvoice = excelPackage.Workbook.Worksheets["Service Invoice"];

				serviceInvoice.Cells["E4"].Value = currentCustomers[_customerSelectedIndex].HoTen; 
				serviceInvoice.Cells["E5"].Value = currentCustomers[_customerSelectedIndex].CMND;
				serviceInvoice.Cells["E6"].Value = currentCustomers[_customerSelectedIndex].DiaChi;
				serviceInvoice.Cells["E7"].Value = (rentBill.NgayBatDau ?? DateTime.Now).ToString("dd/MM/yyyy");
				serviceInvoice.Cells["E8"].Value = (_invoice.NgayTraPhong ?? DateTime.Now).ToString("dd/MM/yyyy");
				serviceInvoice.Cells["I4"].Value = _invoice.ID_HoaDon;
				serviceInvoice.Cells["I5"].Value = DateTime.Now.ToString("dd/MM/yyyy");

				serviceInvoice.Cells["C12"].Value = "1";
				serviceInvoice.Cells["D12"].Value = rentBill.SoPhong_For_Binding;
				serviceInvoice.Cells["E12"].Value = currentCustomers.Count;
				_invoice.NumDayRent_For_Binding = Convert.ToInt32((_invoice.NgayTraPhong - rentBill.NgayBatDau).Value.TotalDays);
				_invoice.NumDayRent_For_Binding = _invoice.NumDayRent_For_Binding == 0 ? 1 : _invoice.NumDayRent_For_Binding;
				serviceInvoice.Cells["F12"].Value = _invoice.NumDayRent_For_Binding;
				serviceInvoice.Cells["G12"].Value = _databaseUtilities.getRentBillFactor(_invoice.ID_PhieuThue);
				serviceInvoice.Cells["H12"].Value = _applicationUtilities.getMoneyForBinding2(Convert.ToInt32(room.DonGia));
				rentBill.TotalPrice = Convert.ToInt32(room.DonGia * _databaseUtilities.getRentBillFactor(_invoice.ID_PhieuThue)) * _invoice.NumDayRent_For_Binding;
				serviceInvoice.Cells["I12"].Value = _applicationUtilities.getMoneyForBinding2(rentBill.TotalPrice);
				serviceInvoice.Cells["I22"].Value = _applicationUtilities.getMoneyForBinding2(rentBill.TotalPrice);

				CauHinh config = _databaseUtilities.getConfig();
				surcharge = Convert.ToInt32(rentBill.TotalPrice * (currentCustomers.Count - Convert.ToInt32(config.DieuKien.Substring(2)) + 1) * Convert.ToDouble(config.GiaTri));

				if (surcharge < 0)
				{
					surcharge = 0;
				}

				serviceInvoice.Cells["H23"].Value = $"{Convert.ToDouble(config.GiaTri) * 100}%";
				serviceInvoice.Cells["I23"].Value = _applicationUtilities.getMoneyForBinding2(surcharge);

				resultPrice = surcharge + rentBill.TotalPrice;
				serviceInvoice.Cells["I24"].Value = _applicationUtilities.getMoneyForBinding2(resultPrice);

				serviceInvoice.Cells["H34"].Value = Global.staticCurrentEmployee.HoTen;

				excelPackage.Save();

				notiMessageSnackbar.MessageQueue.Enqueue($"Đã tạo Hóa-đơn-{uid}.xlsx", "OK", () => { BackPageEvent?.Invoke(backPage); });
			}
		}
    }
}
