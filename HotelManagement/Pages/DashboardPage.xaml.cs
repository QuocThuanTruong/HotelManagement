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
using LiveCharts;
using LiveCharts.Wpf;
using System.Diagnostics;
using HotelManagement.CustomViews;

namespace HotelManagement.Pages
{
	/// <summary>
	/// Interaction logic for DashboardPage.xaml
	/// </summary>
	public partial class DashboardPage : Page
	{
		public delegate void ShowReportPage(int month);
		public event ShowReportPage ShowReportPageEvent;

		private DatabaseUtilities _databaseUtilities = DatabaseUtilities.GetDatabaseInstance();
		private static readonly string[] StaticListMonth = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12"};
		

		public DashboardPage()
		{
			
			InitializeComponent();
			
			
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			int currentMonth = DateTime.Now.Month;

			monthCombobox.ItemsSource = StaticListMonth;
			monthCombobox.SelectedIndex = currentMonth - 1;

			loadDashboard();
			
		}

		private void yearCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void monthCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			
			loadDashboard();
			
		}

		private void showReportButton_Click(object sender, RoutedEventArgs e)
		{
			int month = monthCombobox.SelectedIndex + 1;

			ShowReportPageEvent?.Invoke(month);
		}

		private void loadDashboard()
        {
			loadStatistical();

			if (!loadColumnChart() || !loadCircleChart())
			{
				notFoundContainer.Visibility = Visibility.Visible;
				roomDensityChart.Visibility = Visibility.Collapsed;
				revenueByCategoryChart.Visibility = Visibility.Collapsed;
				showReportButton.Visibility = Visibility.Collapsed;
			} else
			{
				notFoundContainer.Visibility = Visibility.Collapsed;
				revenueByCategoryChart.Visibility = Visibility.Visible;
				roomDensityChart.Visibility = Visibility.Visible;
				showReportButton.Visibility = Visibility.Visible;
			}
		}

		private void loadStatistical()
        {
			tbNumCheckIn.Text = _databaseUtilities.getNumCheckIn().ToString();
			tbNumCheckOut.Text = _databaseUtilities.getNumCheckOut().ToString();
			tbNumEmpty.Text = _databaseUtilities.getNumEmpty().ToString();
			tbNumRenting.Text = _databaseUtilities.getNumRenting().ToString();
		}

		private bool loadColumnChart()
        {
			bool check = false;

			var rooms = _databaseUtilities.getAllRoom();
			var roomsID = _databaseUtilities.getAllRoomID();
			int month = monthCombobox.SelectedIndex + 1;
			Random r = new Random();

			var densities = new ChartValues<double>();
            foreach (var room in rooms)
            {
				var res = _databaseUtilities.getRoomDensity(room.SoPhong, month);

				if (res != 0)
                {
					check = true;
                }

				densities.Add(res);
            }

            var densitiesByMonthAndRoom = new SeriesCollection()
            {
                new ColumnSeries
                {
				
                    Title = "",
                    Values = densities,
				}
            };

			roomDensityChart.Series = densitiesByMonthAndRoom;

			ColorsCollection colors = new ColorsCollection();

			for (int i = 0; i < rooms.Count; ++i)
			{
				colors.Add(Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 233)));
			}

			roomDensityChart.SeriesColors = colors;

			roomDensityChartAxisX.Labels = roomsID;

			return check;
        }

		private bool loadCircleChart()
		{
			bool check = false;

			var roomCategories = _databaseUtilities.getAllRoomCategory();
			int month = monthCombobox.SelectedIndex + 1;

			var revenueByTypeCollection = new SeriesCollection();

			foreach (var category in roomCategories)
			{

				var res = _databaseUtilities.getRevenueByRoomCategory(category.ID_LoaiPhong, month) ?? 0;

				if (res != 0)
                {
					check = true;
                }

				revenueByTypeCollection.Add(new PieSeries
				{
					Title = category.TenLoaiPhong,
					Values = new ChartValues<double> {res}
				});
			}

			revenueByCategoryChart.Series = revenueByTypeCollection;

			return check;
		}

       
    }
}
