using HotelManagement.Utilities;
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

namespace HotelManagement.Pages
{
	/// <summary>
	/// Interaction logic for RoomManagementPage.xaml
	/// </summary>
	public partial class RoomManagementPage : Page
	{
		private DatabaseUtilities _databaseUtilities = DatabaseUtilities.GetDatabaseInstance();
		private ApplicationUtilities _applicationUtilities = ApplicationUtilities.GetAppInstance();

		public List<Phong> rooms;
		public List<LoaiPhong> roomCategories;

		private int _editedIndex = -1;
		public RoomManagementPage()
		{
			InitializeComponent();
		}

		//Bấm edit thì binding data dòng đó lên, sử dụng kĩ thuật set tag/get tag id cho mỗi dòng và đổi icon update, update xong thì trả icon về 
		private void editButton_Click(object sender, RoutedEventArgs e)
		{

			iconAddRoom.Source = (ImageSource)FindResource("IconWhiteUpdate");
			//iconAddRoom.Source = (ImageSource)FindResource("IconWhiteAdd");

			_editedIndex = Convert.ToInt32(((Button)sender).Tag) - 1;
			Phong selectedRoom = rooms[_editedIndex];

			roomIdTextBox.Text = selectedRoom.SoPhong.ToString();

			for (int i = 0; i < roomCategories.Count; ++i)
            {
				if (selectedRoom.ID_LoaiPhong == roomCategories[i].ID_LoaiPhong)
                {
					roomTypeComboBox.SelectedIndex = i;
					break;
				}
            }

			noteTextBox.Text = selectedRoom.GhiChu;
		}

		private void addRoomButton_Click(object sender, RoutedEventArgs e)
		{
			if (_editedIndex != -1)
            {
				int soPhong = Convert.ToInt32(roomIdTextBox.Text);

				if (soPhong != rooms[_editedIndex].SoPhong)
                {
					//notiMessageSnackbar.MessageQueue.Enqueue($"Không được sửa đổi số phòng ", "OK", () => {});
					return;
				}

				rooms[_editedIndex].ID_LoaiPhong = roomCategories[roomTypeComboBox.SelectedIndex].ID_LoaiPhong;
				rooms[_editedIndex].TenLoaiPhong = roomCategories[roomTypeComboBox.SelectedIndex].TenLoaiPhong;
				rooms[_editedIndex].DonGia = roomCategories[roomTypeComboBox.SelectedIndex].DonGia;
				rooms[_editedIndex].DonGiaPerDay_For_Binding = _applicationUtilities.getMoneyForBinding(Convert.ToInt32(rooms[_editedIndex].DonGia ?? 0)) + "/ngày"; ;
				rooms[_editedIndex].GhiChu = noteTextBox.Text;
				
				roomList.ItemsSource = null;
				roomList.ItemsSource = rooms;

				_databaseUtilities.updateRoom(rooms[_editedIndex]);

				//Rest
				_editedIndex = -1;
				roomIdTextBox.Text = "";
				roomTypeComboBox.SelectedIndex = 0;
				noteTextBox.Text = "";

				iconAddRoom.Source = (ImageSource)FindResource("IconWhiteAdd");
			} 
			else
            {
				Phong newRoom = new Phong();
				newRoom.SoPhong = Convert.ToInt32(roomIdTextBox.Text);
				newRoom.ID_LoaiPhong = roomCategories[roomTypeComboBox.SelectedIndex].ID_LoaiPhong;
				newRoom.TenLoaiPhong = roomCategories[roomTypeComboBox.SelectedIndex].TenLoaiPhong;
				newRoom.DonGia = roomCategories[roomTypeComboBox.SelectedIndex].DonGia;
				newRoom.DonGiaPerDay_For_Binding = _applicationUtilities.getMoneyForBinding(Convert.ToInt32(newRoom.DonGia ?? 0)) + "/ngày"; ;
				newRoom.GhiChu = noteTextBox.Text;
				newRoom.STT_For_Binding = rooms.Count();
				newRoom.ID_For_Binding = "P." + newRoom.SoPhong.ToString();

				rooms.Add(newRoom);

				_databaseUtilities.addNewRoom(newRoom);

				//Rest
				roomIdTextBox.Text = "";
				roomTypeComboBox.SelectedIndex = 0;
				noteTextBox.Text = "";
			}
		}

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
			rooms = _databaseUtilities.getAllRoom();
			roomCategories = _databaseUtilities.getAllRoomCategory();

			roomList.ItemsSource = rooms;

			roomTypeComboBox.ItemsSource = roomCategories;

		}

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
			var deleteIndex = Convert.ToInt32(((Button)sender).Tag) - 1;

			_databaseUtilities.deActiveRoom(rooms[deleteIndex]);

			for (int i = deleteIndex + 1; i < rooms.Count; ++i)
			{
				rooms[i].STT_For_Binding -= 1;
			}

			rooms.RemoveAt(deleteIndex);

			roomList.ItemsSource = null;
			roomList.ItemsSource = rooms;
		}
    }
}
