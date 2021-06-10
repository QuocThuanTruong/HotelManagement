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
	/// Interaction logic for ListRoomPage.xaml
	/// </summary>
	public partial class ListRoomPage : Page
	{
		public delegate void CreateRentBill(int id);
		public delegate void CreateInvoice(int id);

		public event CreateRentBill CreateRentBillEvent;
		public event CreateInvoice CreateInvoiceEvent;

		public ListRoomPage()
		{
			InitializeComponent();
		}

		private void lastPageButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void nextPageButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void prevPageButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void firstPageButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void roomListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			CreateRentBillEvent?.Invoke(1);
			//CreateInvoiceEvent?.Invoke(1);
		}
	}
}
