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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HotelManagement.Utilities;
using HotelManagement.Converters;
using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
using System.Drawing;

namespace HotelManagement.Pages
{
	/// <summary>
	/// Interaction logic for RevenueReportPage.xaml
	/// </summary>
	public partial class RevenueReportPage : System.Windows.Controls.Page
    {
		public delegate void BackDashboard();
		public event BackDashboard BackDashboardEvent;

		private DatabaseUtilities _databaseUtilities = DatabaseUtilities.GetDatabaseInstance();
		private ApplicationUtilities _applicationUtilities = ApplicationUtilities.GetAppInstance();
		private AbsolutePathConverter _absolutePathConverter = new AbsolutePathConverter();

		private int _month;
		private bool _hasRevenueReport, _hasDensityReport;
		private List<LoaiPhong> roomCategories;
		private List<Phong> rooms;
		public RevenueReportPage()
		{
			InitializeComponent();
		}

		public RevenueReportPage(int month, bool hasRevenueReport, bool hasDensityReport)
		{
			InitializeComponent();

			_month = month;
			_hasRevenueReport = hasRevenueReport;
			_hasDensityReport = hasDensityReport;

			reportMonthTextBlock.Text = "Tháng " + _month.ToString();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			bool check = true;

			if (_hasRevenueReport)
            {
				loadRevenueReport();
			}
			else
            {
				revenueListContainer.Visibility = Visibility.Collapsed;
				check = false;
            }
			
			if (_hasDensityReport)
            {
				loadDensityReport();
			}
			else
			{
				densityListContainer.Visibility = Visibility.Collapsed;
				check = false;
			}

			if (!check)
            {
				switchReportButton.Visibility = Visibility.Collapsed;
            }
		}

		private void backButton_Click(object sender, RoutedEventArgs e)
		{
			BackDashboardEvent?.Invoke();
		}

		private void switchReportButton_Click(object sender, RoutedEventArgs e)
		{
			if (densityListContainer.Visibility == Visibility.Collapsed)
			{
				densityListContainer.Visibility = Visibility.Visible;
				revenueListContainer.Visibility = Visibility.Collapsed;

				reportTitleTextBlock.Text = "Báo cáo mật độ sử dụng phòng";

				switchButtonText.Text = "BÁO CÁO DOANH THU";
			} else
			{
				densityListContainer.Visibility = Visibility.Collapsed;
				revenueListContainer.Visibility = Visibility.Visible;

				reportTitleTextBlock.Text = "BÁO CÁO DANH THU THEO LOẠI PHÒNG";

				switchButtonText.Text = "BÁO CÁO MẬT ĐỘ";
			}
		}

		private void loadRevenueReport()
        {
			roomCategories = _databaseUtilities.getAllRoomCategory();
			double total = 0;

			List<double> revenues = new List<double>();

			for (int i = 0; i < roomCategories.Count; ++i)
			{
				var category = roomCategories[i];
				revenues.Add(_databaseUtilities.getRevenueByRoomCategory(category.ID_LoaiPhong, _month) ?? 0);

				total += revenues[i];

				roomCategories[i].Revenue_For_Binding = _applicationUtilities.getMoneyForBinding((int)revenues[i]);
			}

			double temp = 0;
			bool isOK = true;
			for (int i = 0; i < roomCategories.Count - 1; ++i)
			{
				var percent = Math.Round(((revenues[i] * 1.0) / total) * 100, 2);

				if (Double.IsNaN(percent))
				{
					isOK = false;
					percent = 0;
				}

				temp += percent;

				roomCategories[i].Percent_For_Binding = percent.ToString() + "%";
			}

			if (isOK)
            {
				roomCategories[roomCategories.Count - 1].Percent_For_Binding = Math.Round(100.0 - temp).ToString() + "%";
			} else
            {
				roomCategories[roomCategories.Count - 1].Percent_For_Binding = "0%";
			}

			roomRevenueList.ItemsSource = roomCategories;
		}

        private void exportExcelButton_Click(object sender, RoutedEventArgs e)
        {
			_applicationUtilities.createExportedDirectory("Exported Report");
			string _reportFileName = $"Report_{DateTime.Now.ToString("ddMMyyyyHHmmss")}.xlsx";

			_applicationUtilities.copyFileToDirectory(
				_absolutePathConverter.Convert($"Assets/XLSXTemplate/Report_template.xlsx", null, null, null).ToString(),
				"Exported Report",
				_reportFileName);

			ExcelPackage.LicenseContext = LicenseContext.Commercial;
			using (var excelPackage = new ExcelPackage(new FileInfo("Exported Report/" + _reportFileName)))
            {

				if (_hasRevenueReport && _hasDensityReport)
				{
					exportRevenue(excelPackage);
					exportDensity(excelPackage);
				}
				else if (_hasRevenueReport)
				{
					exportRevenue(excelPackage);
				}
				else if (_hasDensityReport)
				{
					exportDensity(excelPackage);
				}

				excelPackage.Save();
			}

			notiMessageSnackbar.MessageQueue.Enqueue($"Xuất báo cáo thành công: {_reportFileName}", "OK", () => { });
		}

		public void exportRevenue(ExcelPackage excelPackage)
        {
			var revenueWorksheet = excelPackage.Workbook.Worksheets["Revenue Report"];

			for (int i = 1; i <= roomCategories.Count; ++i)
			{
				revenueWorksheet.Cells[i + 5, 1].Value = roomCategories[i - 1].ID_LoaiPhong; //STT
				revenueWorksheet.Cells[i + 5, 2].Value = roomCategories[i - 1].TenLoaiPhong; //Loai phòng
				revenueWorksheet.Cells[i + 5, 3].Value = roomCategories[i - 1].Revenue_For_Binding; //Doanh Thu
				revenueWorksheet.Cells[i + 5, 4].Value = double.Parse(roomCategories[i - 1].Percent_For_Binding.Substring(0, roomCategories[i - 1].Percent_For_Binding.Length - 1)) / 100.0; //Tỉ lệ
			}

		}

		public void exportDensity(ExcelPackage excelPackage)
		{
			var densityWorksheet = excelPackage.Workbook.Worksheets["Density Report"];

			for (int i = 1; i <= rooms.Count; ++i)
			{
				densityWorksheet.Cells[i + 5, 1].Value = rooms[i - 1].STT_For_Binding; //STT
				densityWorksheet.Cells[i + 5, 2].Value = rooms[i - 1].ID_For_Binding; //Loai phòng
				densityWorksheet.Cells[i + 5, 3].Value = rooms[i - 1].Density_For_Binding; //Doanh Thu
				densityWorksheet.Cells[i + 5, 4].Value = double.Parse(rooms[i - 1].Percent_For_Binding.Substring(0, rooms[i - 1].Percent_For_Binding.Length - 1)) / 100.0; //Tỉ lệ
			}
		}

		private void loadDensityReport()
        {
			rooms = _databaseUtilities.getAllRoom();
			double total = 0;

			List<int> densities = new List<int>();

			for (int i = 0; i < rooms.Count; ++i)
			{
				densities.Add(_databaseUtilities.getRoomDensity(rooms[i].SoPhong, _month));

				total += densities[i];
				rooms[i].Density_For_Binding = densities[i].ToString() + " ngày";
			}

			double temp = 0;
			bool isOK = true;
			for (int i = 0; i < rooms.Count - 1; ++i)
			{
				var percent = Math.Round(((densities[i] * 1.0) / total) * 100, 2);

				if (Double.IsNaN(percent))
				{
					isOK = false;
					percent = 0;
				}

				temp += percent;

				rooms[i].Percent_For_Binding = percent.ToString() + "%";
			}

			if (isOK)
			{
				rooms[rooms.Count - 1].Percent_For_Binding = Math.Round(100.0 - temp).ToString() + "%";
			}
			else
			{
				rooms[rooms.Count - 1].Percent_For_Binding = "0%";
			}

			roomDensityList.ItemsSource = rooms;
		}
    }
}
