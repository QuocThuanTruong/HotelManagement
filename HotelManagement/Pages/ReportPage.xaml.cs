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
	/// Interaction logic for RevenueReportPage.xaml
	/// </summary>
	public partial class RevenueReportPage : Page
	{
		public delegate void BackDashboard();
		public event BackDashboard BackDashboardEvent;

		public RevenueReportPage()
		{
			InitializeComponent();
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
	}
}
