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
	/// Interaction logic for CreateRentBillPage.xaml
	/// </summary>
	public partial class CreateRentBillPage : Page
	{
		public delegate void BackPage(Button page);
		public event BackPage BackPageEvent;
		private int id;
		private Button backPage;
		public CreateRentBillPage()
		{
			InitializeComponent();
		}

		public CreateRentBillPage(int id, Button backPage)
		{
			InitializeComponent();
			this.id = id;
			this.backPage = backPage;
		}

		private void backButton_Click(object sender, RoutedEventArgs e)
		{
			BackPageEvent?.Invoke(backPage);
		}
	}
}
