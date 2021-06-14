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
	/// Interaction logic for InvoideManagementPage.xaml
	/// </summary>
	public partial class InvoiceManagementPage : Page
	{
		public delegate void ViewInvoice(int id);
		public event ViewInvoice ViewInvoiceEvent;

		private DatabaseUtilities _databaseUtilities = DatabaseUtilities.GetDatabaseInstance();
		private ApplicationUtilities _applicationUtilities = ApplicationUtilities.GetAppInstance();


		public List<HoaDon> invoices;

		public InvoiceManagementPage()
		{
			InitializeComponent();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			invoices = _databaseUtilities.getAllInvoice();

			invoiceList.ItemsSource = invoices;
		}

		private void viewButton_Click(object sender, RoutedEventArgs e)
		{
			ViewInvoiceEvent?.Invoke(1);
		}

    }
}
