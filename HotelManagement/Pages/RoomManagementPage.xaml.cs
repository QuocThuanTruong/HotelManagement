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
	/// Interaction logic for RoomManagementPage.xaml
	/// </summary>
	public partial class RoomManagementPage : Page
	{
		public RoomManagementPage()
		{
			InitializeComponent();
		}

		//Bấm edit thì binding data dòng đó lên, sử dụng kĩ thuật set tag/get tag id cho mỗi dòng và đổi icon update, update xong thì trả icon về 
		private void editButton_Click(object sender, RoutedEventArgs e)
		{
			iconAddRoom.Source = (ImageSource)FindResource("IconWhiteUpdate");
			//iconAddRoom.Source = (ImageSource)FindResource("IconWhiteAdd");
		}

		private void addRoomButton_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
