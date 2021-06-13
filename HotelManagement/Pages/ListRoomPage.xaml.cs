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
	/// Interaction logic for ListRoomPage.xaml
	/// </summary>
	public partial class ListRoomPage : Page
	{
		public delegate void CreateRentBill(int id);
		public delegate void CreateInvoice(int id);

		public event CreateRentBill CreateRentBillEvent;
		public event CreateInvoice CreateInvoiceEvent;

		private DatabaseUtilities _databaseUtilities = DatabaseUtilities.GetDatabaseInstance();
		private ApplicationUtilities _applicationUtilities = ApplicationUtilities.GetAppInstance();

		private List<Phong> _rooms = new List<Phong>();
		private List<LoaiPhong> _roomcategories = new List<LoaiPhong>();

		const int TOTAL_JOURNEY_PER_PAGE = 6;

		private int _currentPage;
		private int _maxPage = 0;
		private string _searchText = "";
		private bool _isSearching = false;

		public ListRoomPage()
		{
			InitializeComponent();

			_rooms = _databaseUtilities.getAllRoom();
			_currentPage = 1;
			_maxPage = getMaxPage(_rooms.Count);
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			_roomcategories.Add(new LoaiPhong(0, "Tất cả"));

			_roomcategories.AddRange(_databaseUtilities.getAllRoomCategory());
			
			catComboBox.ItemsSource = _roomcategories;
		}

		private int getMaxPage(int totalResult)
		{
			var result = Math.Ceiling((double)totalResult / TOTAL_JOURNEY_PER_PAGE);

			return (int)result;
		}

		private void loadListRoom()
        {
			var rooms = _rooms;

			int statusIndex = statusComboBox.SelectedIndex;

			switch (statusIndex)
			{
				case 1:
					rooms = (from room in rooms
							 where room.TinhTrang == false
							 select room).ToList();
					break;
				case 2:
					rooms = (from room in rooms
							 where room.TinhTrang == true
							 select room).ToList();
					break;
			}

			if (IsLoaded)
            {
				int categoryIndex = catComboBox.SelectedIndex;

				if (categoryIndex != 0)
                {
					rooms = (from room in rooms
							 where room.ID_LoaiPhong == _roomcategories[categoryIndex].ID_LoaiPhong
							 select room).ToList();
				}
				
				int sortIndex = sortTypeComboBox.SelectedIndex;

				if (_isSearching)
                {
					rooms = (from room in rooms
							 where room.SoPhong.ToString().Contains(_searchText)
							 select room).ToList();
				}

				switch (sortIndex)
                {
					case 0:
						rooms = (from room in rooms
								 orderby room.SoPhong 
								 select room).ToList();
						break;
					case 1:
						rooms = (from room in rooms
								 orderby room.SoPhong descending
								 select room).ToList();
						break;
					case 2:
						rooms = (from room in rooms
								 orderby room.DonGia
								 select room).ToList();
						break;
					case 3:
						rooms = (from room in rooms
								 orderby room.DonGia descending
								 select room).ToList();
						break;
				}

				_maxPage = getMaxPage(rooms.Count);

				var totalResult = rooms.Count;
				
				currentPageTextBlock.Text = $"{_currentPage} of {_maxPage}";

				rooms = rooms
				.Skip((_currentPage - 1) * TOTAL_JOURNEY_PER_PAGE)
				.Take(TOTAL_JOURNEY_PER_PAGE)
				.ToList();

				currentResultTextBlock.Text = $"Hiển thị {rooms.Count} trong tổng số {totalResult} phòng";

				roomListView.ItemsSource = rooms;
			}
		}

		private void lastPageButton_Click(object sender, RoutedEventArgs e)
		{
			_currentPage = _maxPage;
			loadListRoom();
		}

		private void nextPageButton_Click(object sender, RoutedEventArgs e)
		{
			if (_currentPage < (int)_maxPage)
			{
				++_currentPage;
			}

			loadListRoom();
		}

		private void prevPageButton_Click(object sender, RoutedEventArgs e)
		{
			if (_currentPage > 1)
			{
				--_currentPage;
			}

			loadListRoom();
		}

		private void firstPageButton_Click(object sender, RoutedEventArgs e)
		{
			_currentPage = 1;

			loadListRoom();
		}

		private void roomListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			CreateRentBillEvent?.Invoke(((Phong)roomListView.SelectedItem).SoPhong);
			//Doi sang da thue BadageRented
			//Doi text nut thanh THANH TOÁN rentButtonText
			//Sau khi tạo phieu thue thi binding them ID phieu thue vo luon item do, de lat bam THANH TOAN cho de query

			//CreateInvoiceEvent?.Invoke(1);
		}

        private void statusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			_currentPage = 1;
			loadListRoom();
		}

        private void catComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			_currentPage = 1;
			loadListRoom();
		}

        private void sortTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			_currentPage = 1;
			loadListRoom();
		}

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
			_searchText = searchTextBox.Text;

			if (_searchText.Length > 0)
            {
				_isSearching = true;
            }
			else
            {
				_isSearching = false;
            }

			loadListRoom();
		}
    }
}
