using HotelManagement.Utilities;
using System.Windows.Forms;
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
using System.Data;
using System.IO;
using ExcelDataReader;

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

			_editedIndex = Convert.ToInt32(((System.Windows.Controls.Button)sender).Tag) - 1;
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
					notiMessageSnackbar.MessageQueue.Enqueue($"Không được sửa đổi số phòng ", "OK", () => {});
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

				//Reset
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
			var deleteIndex = Convert.ToInt32(((System.Windows.Controls.Button)sender).Tag) - 1;

			_databaseUtilities.deActiveRoom(rooms[deleteIndex]);

			for (int i = deleteIndex + 1; i < rooms.Count; ++i)
			{
				rooms[i].STT_For_Binding -= 1;
			}

			rooms.RemoveAt(deleteIndex);

			roomList.ItemsSource = null;
			roomList.ItemsSource = rooms;
		}

        private void importExcelButton_Click(object sender, RoutedEventArgs e)
        {
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Multiselect = true;
				openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
				openFileDialog.Filter = "Excel Files|*.xls;*.xlsx";

				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					DataTableCollection tables;

					using (var stream = File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
					{
						using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
						{
							DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
							{
								ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
								{
									UseHeaderRow = true
								}
							});

							tables = result.Tables;
						}

						DataTable dt = tables["room"];
						if (dt != null)
						{
							List<Phong> list = new List<Phong>();
							for (int i = 0; i < dt.Rows.Count; i++)
							{
								Phong room = new Phong();
								room.SoPhong = Convert.ToInt32(dt.Rows[i]["SoPhong"].ToString());
								room.ID_LoaiPhong = Convert.ToInt32(dt.Rows[i]["ID_LoaiPhong"].ToString());
								room.TinhTrang = Convert.ToBoolean(dt.Rows[i]["TinhTrang"].ToString());
								room.GhiChu = dt.Rows[i]["GhiChu"].ToString();
								room.Active = true;

								if (_databaseUtilities.checkExistRoom(room.SoPhong))
                                {
									_databaseUtilities.updateRoom(room);
                                } 
								else
                                {
									rooms.Add(room);

									_databaseUtilities.addNewRoom(room);
								}
							}
							Page_Loaded(null, null);
							notiMessageSnackbar.MessageQueue.Enqueue($"Thêm dữ liệu thành công", "OK", () => { });

						} else
                        {
							notiMessageSnackbar.MessageQueue.Enqueue($"Sheet chứa dữ liệu phòng cần được đổi tên thành \"room\"", "OK", () => { });
						}
					}
				}
			}
		}
    }
}
