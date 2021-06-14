using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using HotelManagement.Converters;

namespace HotelManagement.Utilities
{
    class DatabaseUtilities
    {
        private DatabaseUtilities() { }

        private static DatabaseUtilities _databaseInstance;
        private static HotelManagementEntities _databaseHotelManagement;

        private ApplicationUtilities _applicationUtilities = ApplicationUtilities.GetAppInstance();

        private AbsolutePathConverter _absolutePathConverter = new AbsolutePathConverter();

        public static DatabaseUtilities GetDatabaseInstance()
        {
            if (_databaseInstance == null)
            {
                _databaseInstance = new DatabaseUtilities();
                _databaseHotelManagement = new HotelManagementEntities();
            }
            else
            {
                //Do Nothing
            }

            return _databaseInstance;
        }

        public NhanVien checkLogin(string username, string password)
        {
            NhanVien result = null;

            int isExist = _databaseHotelManagement
                .Database
                .SqlQuery<int>($"SELECT dbo.func_AuthenticateUser('{username}' , '{password}')")
                .Single();

            if (isExist == 1)
            {
                result = _databaseHotelManagement
                .Database
                .SqlQuery<NhanVien>($"SELECT * FROM NhanVien Where Username = '{username}' AND Password = '{password}'")
                .Single();
            }

            return result;
        }

        public int getNumCheckIn()
        {
            var result = _databaseHotelManagement
                 .Database
                 .SqlQuery<int>("select [dbo].[func_GetCheckin]()")
                 .Single();

            return result;
        }

        public int getNumCheckOut()
        {
            var result = _databaseHotelManagement
                 .Database
                 .SqlQuery<int>("select [dbo].[func_GetCheckout]()")
                 .Single();

            return result;
        }

        public int getNumRenting()
        {
            var result = _databaseHotelManagement
                 .Database
                 .SqlQuery<int>("select [dbo].[func_GetNumRenting]()")
                 .Single();

            return result;
        }

        public int getNumEmpty()
        {
            var result = _databaseHotelManagement
                 .Database
                 .SqlQuery<int>("select [dbo].[func_GetNumEmpty]()")
                 .Single();

            return result;
        }

        public List<Phong> getAllRoom()
        {
            var rooms = _databaseHotelManagement.func_getAllRoom().ToList();

            var result = _applicationUtilities.convertToPhong(rooms);

            for (int i = 0; i < result.Count; ++i)
            {
                result[i].DonGia_For_Binding = _applicationUtilities.getMoneyForBinding(Convert.ToInt32(result[i].DonGia ?? 0));

                string uri = "";
                if (result[i].TinhTrang == true)
                {
                    uri = (string)_absolutePathConverter.Convert("Assets/Images/badage-rented.png", null, null, null);
                }
                else
                {
                    uri = (string)_absolutePathConverter.Convert("Assets/Images/badage-empty.png", null, null, null);
                }

                BitmapImage bitmap = new BitmapImage();

                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(uri, UriKind.Relative);
                bitmap.EndInit();

                result[i].Badage_Status_For_Binding = bitmap;
                result[i].STT_For_Binding = i + 1;
                result[i].ID_For_Binding = "P." + rooms[i].SoPhong;
            }

            return result;
        }

        public int getRoomDensity(int soPhong, int month)
        {
            var result = _databaseHotelManagement
              .Database
              .SqlQuery<int>($"select [dbo].[func_GetRoomDensity]({soPhong}, {month})")
              .Single();

            return result;
        }

        public List<string> getAllRoomID()
        {
            var result_int = _databaseHotelManagement
              .Database
              .SqlQuery<int>($"select SoPhong from dbo.Phong")
              .ToList();

            var result_string = new List<string>();

            for (int i = 0; i < result_int.Count; ++i)
            {
                result_string.Add(result_int[i].ToString());
            }

            return result_string;
        }

        public List<LoaiPhong> getAllRoomCategory()
        {
            var result = _databaseHotelManagement
                .Database
                .SqlQuery<LoaiPhong>("Select * from dbo.LoaiPhong")
                .ToList();

            return result;
        }

        public List<LoaiKhach> getAllCustomerCategory()
        {
            var result = _databaseHotelManagement
                .Database
                .SqlQuery<LoaiKhach>("Select * from dbo.LoaiKhach")
                .ToList();

            return result;
        }

        public int getRevenueByRoomCategory(int loaiPhong, int month)
        {
            var result = _databaseHotelManagement
              .Database
              .SqlQuery<int>($"select [dbo].[func_GetRevenueByRoomCat]({loaiPhong}, {month})")
              .Single();

            return result;
        }

        public int getMaxIdCustomer()
        {
            var result = _databaseHotelManagement
              .Database
              .SqlQuery<int>($"select max(ID_KhachHang) from KhachHang")
              .Single();

            return result;
        }
        public int getMaxIdRental()
        {
            var result = _databaseHotelManagement
              .Database
              .SqlQuery<int>($"select max(ID_PhieuThue) from PhieuThue")
              .Single();

            return result;
        }

        public void addNewCustomer(KhachHang customer)
        {
            _databaseHotelManagement
                .Database
                .ExecuteSqlCommand($"INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES ({customer.ID_KhachHang}, N'{customer.HoTen}', N'{customer.CMND}', N'{customer.DiaChi}', {customer.ID_LoaiKhach})");
        }

        public void addNewRental(PhieuThue newRental)
        {
            _databaseHotelManagement
                .Database
                .ExecuteSqlCommand($"INSERT[dbo].[PhieuThue]([ID_PhieuThue], [NgayBatDau]) VALUES({newRental.ID_PhieuThue}, CAST(N'{newRental.NgayBatDau}' AS DateTime))");
        }

        public void addNewRentalDetail(ChiTietPhieuThue newRentalDetail)
        {
            _databaseHotelManagement
                .Database
                .ExecuteSqlCommand($"INSERT [dbo].[ChiTietPhieuThue] ([ID_KhachHang], [ID_PhieuThue], [SoPhong], [ID_NhanVien], [Active]) VALUES ({newRentalDetail.ID_KhachHang}, {newRentalDetail.ID_PhieuThue}, {newRentalDetail.SoPhong}, {newRentalDetail.ID_NhanVien}, 1)");
        }

        public void updateRentedRoom(int IdRoom)
        {
            _databaseHotelManagement
                .Database
                .ExecuteSqlCommand($"UPDATE Phong SET TinhTrang = 'true' Where SoPhong = {IdRoom}");
        }

        public void updateEmptyRoom(int IdRoom)
        {
            _databaseHotelManagement
                .Database
                .ExecuteSqlCommand($"UPDATE Phong SET TinhTrang = 'false' Where SoPhong = {IdRoom}");
        }

        public int getCurrentRentBillIdBelongToRoom(int IdRoom)
        {
            var result = _databaseHotelManagement
                .Database
                .SqlQuery<int>($"SELECT ID_PhieuThue FROM ChiTietPhieuThue WHERE Active = 'true' AND SoPhong = {IdRoom}")
                .First();

            return result;
        }

        public PhieuThue getRentBillById(int IdRentBill)
        {
            var result = _databaseHotelManagement
               .Database
               .SqlQuery<PhieuThue>($"SELECT * FROM PhieuThue WHERE ID_PhieuThue = {IdRentBill}")
               .Single();

            return result;
        }

        public List<KhachHang> getCurrentCustomerInRoom(int IdRentBill)
        {
            var rentBillDetail = _databaseHotelManagement
                .Database
                .SqlQuery<ChiTietPhieuThue>($"SELECT * FROM ChiTietPhieuThue WHERE ID_PhieuThue = {IdRentBill}")
                .ToList();

            List<KhachHang> result = new List<KhachHang>();

            foreach (var detail in rentBillDetail)
            {
                KhachHang customer = _databaseHotelManagement
                    .Database
                    .SqlQuery<KhachHang>($"SELECT * FROM KhachHang WHERE ID_KhachHang = {detail.ID_KhachHang}")
                    .Single();

                result.Add(customer);
            }

            return result;
        }

        public Phong getRoomById(int IdRoom)
        {
            var rooms = getAllRoom();

            foreach (var room in rooms)
            {
                if (room.SoPhong == IdRoom)
                {
                    return room;
                }
            }

            return null;
        }

        public int getMaxIdInvoice()
        {
            var result = _databaseHotelManagement
              .Database
              .SqlQuery<int>($"select max(ID_HoaDon) from HoaDon")
              .Single();

            return result;
        }

        public void addNewInvoice(HoaDon newInvoice)
        {
            _databaseHotelManagement
                .Database
                .ExecuteSqlCommand($"INSERT [dbo].[HoaDon] ([ID_HoaDon], [ID_PhieuThue], [ID_NhanVien], [NgayTraPhong], [TongTien], [Active]) VALUES ({newInvoice.ID_HoaDon}, {newInvoice.ID_PhieuThue}, {newInvoice.ID_NhanVien}, CAST(N'{newInvoice.NgayTraPhong}' AS DateTime), {newInvoice.TongTien}, 1)");
        }

        public void updateRentalBillDetail(int IdRentBill)
        {
            _databaseHotelManagement
                .Database
                .ExecuteSqlCommand($"UPDATE ChiTietPhieuThue Set Active = 'false' WHERE ID_PhieuThue = {IdRentBill}");
        }
    }
}
