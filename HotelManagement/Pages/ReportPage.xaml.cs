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
using HotelManagement.Utilities;

namespace HotelManagement.Pages
{
	/// <summary>
	/// Interaction logic for RevenueReportPage.xaml
	/// </summary>
	public partial class RevenueReportPage : Page
	{
		public delegate void BackDashboard();
		public event BackDashboard BackDashboardEvent;

		private DatabaseUtilities _databaseUtilities = DatabaseUtilities.GetDatabaseInstance();
		private ApplicationUtilities _applicationUtilities = ApplicationUtilities.GetAppInstance();

		private int _month;
		public RevenueReportPage()
		{
			InitializeComponent();
		}

		public RevenueReportPage(int month)
		{
			InitializeComponent();

			_month = month;

			reportMonthTextBlock.Text = "Tháng " + _month.ToString();
			
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			loadRevenueReport();
			loadDensityReport();
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
			var roomCategories = _databaseUtilities.getAllRoomCategory();
			double total = 0;

			List<int> revenues = new List<int>();

			for (int i = 0; i < roomCategories.Count; ++i)
			{
				var category = roomCategories[i];
				revenues.Add(_databaseUtilities.getRevenueByRoomCategory(category.ID_LoaiPhong, _month));

				total += revenues[i];

				roomCategories[i].Revenue_For_Binding = _applicationUtilities.getMoneyForBinding(revenues[i]);
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

		private void loadDensityReport()
        {
			var rooms = _databaseUtilities.getAllRoom();
			double total = 0;

			List<int> densities = new List<int>();

			for (int i = 0; i < rooms.Count; ++i)
			{
				densities.Add(_databaseUtilities.getRoomDensity(rooms[i].SoPhong, _month));

				total += densities[i];

				rooms[i].Density_For_Binding = densities[i].ToString() + " ngày";
				rooms[i].STT_For_Binding = i + 1;
				rooms[i].ID_For_Binding = "P." + rooms[i].SoPhong;
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
