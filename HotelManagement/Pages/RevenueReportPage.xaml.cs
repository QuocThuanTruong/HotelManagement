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

		private void densityButton_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
