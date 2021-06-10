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
	/// Interaction logic for RentBillManagementPage.xaml
	/// </summary>
	public partial class RentBillManagementPage : Page
	{
		public delegate void EditRentBill(int id);
		public event EditRentBill EditRentBillEvent;
		public RentBillManagementPage()
		{
			InitializeComponent();
		}

		private void editButton_Click(object sender, RoutedEventArgs e)
		{
			EditRentBillEvent?.Invoke(1);
		}
	}
}
