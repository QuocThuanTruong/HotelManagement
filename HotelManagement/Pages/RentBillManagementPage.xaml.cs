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
	/// Interaction logic for RentBillManagementPage.xaml
	/// </summary>
	public partial class RentBillManagementPage : Page
	{
		public delegate void EditRentBill(int id);
		public event EditRentBill EditRentBillEvent;

		private DatabaseUtilities _databaseUtilities = DatabaseUtilities.GetDatabaseInstance();
		private ApplicationUtilities _applicationUtilities = ApplicationUtilities.GetAppInstance();

		public List<PhieuThue> rentedBills;
		public RentBillManagementPage()
		{
			InitializeComponent();
		}

		private void editButton_Click(object sender, RoutedEventArgs e)
		{
			int idRental = Convert.ToInt32(((Button)sender).Tag);

			EditRentBillEvent?.Invoke(idRental);
		}

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
			rentedBills = _databaseUtilities.getAllRentedBill();

			roomRevenueList.ItemsSource = rentedBills;
		}

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
			int idRental = Convert.ToInt32(((Button)sender).Tag);

			_databaseUtilities.deleteRentalBill(idRental);

			var deleteRentBill = (from r in rentedBills
									where r.ID_PhieuThue == idRental
									select r).First();

			_databaseUtilities.updateEmptyRoom(deleteRentBill.SoPhong_For_Binding);

			rentedBills = (from r in rentedBills
						   where r.ID_PhieuThue != idRental
						   select r).ToList();

			roomRevenueList.ItemsSource = null;
			roomRevenueList.ItemsSource = rentedBills;
		}
    }
}
