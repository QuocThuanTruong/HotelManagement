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
                result[i].DonGiaPerDay_For_Binding = _applicationUtilities.getMoneyForBinding(Convert.ToInt32(result[i].DonGia ?? 0)) + "/ngày";
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
            var exists = _databaseHotelManagement
                .Database
                .SqlQuery<KhachHang>($"SELECT * FROM KhachHang WHERE ID_KhachHang = {customer.ID_KhachHang}")
                .FirstOrDefault();

            if (exists == null)
            {
                _databaseHotelManagement
                    .Database
                    .ExecuteSqlCommand($"INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES ({customer.ID_KhachHang}, N'{customer.HoTen}', N'{customer.CMND}', N'{customer.DiaChi}', {customer.ID_LoaiKhach})");
            }

        }

        public void addNewRental(PhieuThue newRental)
        {
            _databaseHotelManagement
                .Database
                .ExecuteSqlCommand($"INSERT[dbo].[PhieuThue]([ID_PhieuThue], [NgayBatDau], [Active]) VALUES({newRental.ID_PhieuThue}, CAST(N'{newRental.NgayBatDau}' AS DateTime), 1)");
        }

        public void addNewRentalDetail(ChiTietPhieuThue newRentalDetail)
        {
            var exists = _databaseHotelManagement
                           .Database
                           .SqlQuery<ChiTietPhieuThue>($"SELECT * FROM ChiTietPhieuThue WHERE ID_KhachHang = {newRentalDetail.ID_KhachHang} AND ID_PhieuThue = {newRentalDetail.ID_PhieuThue}")
                           .FirstOrDefault();

            if (exists == null)
            {
                _databaseHotelManagement
                    .Database
                    .ExecuteSqlCommand($"INSERT [dbo].[ChiTietPhieuThue] ([ID_KhachHang], [ID_PhieuThue], [SoPhong], [ID_NhanVien], [Active]) VALUES ({newRentalDetail.ID_KhachHang}, {newRentalDetail.ID_PhieuThue}, {newRentalDetail.SoPhong}, {newRentalDetail.ID_NhanVien}, 1)");
            }
            else 
            {
                if (exists.Active == false)
                {
                    _databaseHotelManagement
                        .Database
                        .ExecuteSqlCommand($"UPDATE ChiTietPhieuThue Set Active = 'true' WHERE ID_PhieuThue = {newRentalDetail.ID_PhieuThue} AND ID_KhachHang = {newRentalDetail.ID_KhachHang}");
                }
            }
        }

        public void deleteCustomerInRentBillDetail(ChiTietPhieuThue rentalDetail)
        {
            var exists = _databaseHotelManagement
                           .Database
                           .SqlQuery<ChiTietPhieuThue>($"SELECT * FROM ChiTietPhieuThue WHERE ID_KhachHang = {rentalDetail.ID_KhachHang} AND ID_PhieuThue = {rentalDetail.ID_PhieuThue}")
                           .FirstOrDefault();

            if (exists != null)
            {
                _databaseHotelManagement
                   .Database
                   .ExecuteSqlCommand($"UPDATE ChiTietPhieuThue Set Active = 'false' WHERE ID_PhieuThue = {rentalDetail.ID_PhieuThue} AND ID_KhachHang = {rentalDetail.ID_KhachHang}"); ;
            }
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
            var rentBills = _databaseHotelManagement
                .Database
                .SqlQuery<int>($"SELECT ID_PhieuThue FROM PhieuThue WHERE Active = 1")
                .ToList();

            int result = -1;

            foreach (var r in rentBills)
            {
                result = _databaseHotelManagement
                    .Database
                    .SqlQuery<int>($"Select ID_PhieuThue FROM ChiTietPhieuThue WHERE SoPhong = {IdRoom} AND ID_PhieuThue = {r}")
                    .FirstOrDefault();

                if (result != 0)
                {
                    return result;
                }
            }

            return result;
        }

        public PhieuThue getRentBillById(int IdRentBill)
        {
            var result = _databaseHotelManagement
               .Database
               .SqlQuery<PhieuThue>($"SELECT * FROM PhieuThue WHERE ID_PhieuThue = {IdRentBill}")
               .Single();

            ChiTietPhieuThue rentBillDetail = _databaseHotelManagement
                    .Database
                    .SqlQuery<ChiTietPhieuThue>($"SELECT DISTINCT * FROM ChiTietPhieuThue WHERE ID_PhieuThue = {result.ID_PhieuThue}")
                    .First();

            result.TenNhanVienLapPhieu = _databaseHotelManagement
                .Database
                .SqlQuery<string>($"SELECT HoTen FROM NhanVien WHERE ID_NhanVien = {rentBillDetail.ID_NhanVien}")
                .Single();

            result.ID_NhanVien = _databaseHotelManagement
               .Database
               .SqlQuery<int>($"SELECT ID_NhanVien FROM NhanVien WHERE ID_NhanVien = {rentBillDetail.ID_NhanVien}")
               .Single();

            if (result.Active == 1)
            {
                result.Status = "Chưa thanh toán";
            }
            else if (result.Active == 2)
            {
                result.Status = "Đã thanh toán";
            }

            result.SoPhong_For_Binding = rentBillDetail.SoPhong;

            return result;
        }

        public List<KhachHang> getCurrentCustomerInRoom(int IdRentBill)
        {
            var rentBillDetail = _databaseHotelManagement
                .Database
                .SqlQuery<ChiTietPhieuThue>($"SELECT * FROM ChiTietPhieuThue WHERE ID_PhieuThue = {IdRentBill} AND Active = 'true'")
                .ToList();

            List<KhachHang> result = new List<KhachHang>();

            int STT = 1;
            foreach (var detail in rentBillDetail)
            {
                KhachHang customer = _databaseHotelManagement
                    .Database
                    .SqlQuery<KhachHang>($"SELECT * FROM KhachHang WHERE ID_KhachHang = {detail.ID_KhachHang}")
                    .Single();

                customer.STT_For_Binding = STT++;
                customer.TenLoaiKhach = _databaseHotelManagement
                    .Database
                    .SqlQuery<string>($"SELECT TenLoaiKhach From LoaiKhach WHERE ID_LoaiKhach = {customer.ID_LoaiKhach}")
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
                .ExecuteSqlCommand($"INSERT [dbo].[HoaDon] ([ID_HoaDon], [ID_PhieuThue], [ID_NhanVien], [ID_KhachHang], [NgayTraPhong], [TongTien], [Active]) VALUES ({newInvoice.ID_HoaDon}, {newInvoice.ID_PhieuThue}, {newInvoice.ID_NhanVien}, {newInvoice.ID_KhachHang}, CAST(N'{newInvoice.NgayTraPhong}' AS DateTime), {newInvoice.TongTien}, 1)");
        }

        public void updateRentalBillDetail(int IdRentBill, bool active)
        {
            string sActive = active ? "true" : "false";

            _databaseHotelManagement
                .Database
                .ExecuteSqlCommand($"UPDATE ChiTietPhieuThue Set Active = '{sActive}' WHERE ID_PhieuThue = {IdRentBill}");
        }

        public void finishRentalBill(int IdRentalBill)
        {
            _databaseHotelManagement
                .Database
                .ExecuteSqlCommand($"UPDATE PhieuThue Set Active = 2 WHERE ID_PhieuThue = {IdRentalBill}");
        }

        public void updateRentalBill(PhieuThue rentBill)
        {
            _databaseHotelManagement
                .Database
                .ExecuteSqlCommand($"UPDATE PhieuThue Set Active = {rentBill.Active}, NgayBatDau = CAST(N'{rentBill.NgayBatDau}' AS DateTime WHERE ID_PhieuThue = {rentBill.ID_PhieuThue}");
        }

        public void updateRoom(Phong room)
        {
            _databaseHotelManagement
                .Database
                .ExecuteSqlCommand($"UPDATE Phong Set ID_LoaiPhong = {room.ID_LoaiPhong}, GhiChu = N'{room.GhiChu}' WHERE SoPhong = {room.SoPhong}");
        }

        public void deActiveRoom(Phong room)
        {
            _databaseHotelManagement
                .Database
                .ExecuteSqlCommand($"UPDATE Phong Set Active = 'false' WHERE SoPhong = {room.SoPhong}");
        }

        public void addNewRoom(Phong room)
        {
            _databaseHotelManagement
                .Database
                .ExecuteSqlCommand($"INSERT INTO Phong VALUES({room.SoPhong}, {room.ID_LoaiPhong}, 'false', N'{room.GhiChu}', 'true')");
        }

        public List<PhieuThue> getAllRentedBill()
        {
            var result = _databaseHotelManagement
                .Database
                .SqlQuery<PhieuThue>($"SELECT * FROM PhieuThue Where Active <> 0")
                .ToList();

            for (int i = 0; i < result.Count; ++i)
            {
                result[i] = getRentBillById(result[i].ID_PhieuThue);
            }
  
            return result;
        }

        public void deleteRentalBill(int IdRetalBill)
        {
            _databaseHotelManagement
                .Database
                .ExecuteSqlCommand($"UPDATE PhieuThue Set Active = 0 WHERE ID_PhieuThue = {IdRetalBill}");
        }

        public HoaDon getInvoiceById(int IdInvoice)
        {
            var result = _databaseHotelManagement
                .Database
                .SqlQuery<HoaDon>($"SELECT * FROM HoaDon WHERE ID_HoaDon = {IdInvoice}")
                .Single();

            result.HoTenNV_For_Binding = _databaseHotelManagement
                .Database
                .SqlQuery<string>($"SELECT HoTen FROM NhanVien WHERE ID_NhanVien = {result.ID_NhanVien}")
                .Single();

            result.TenKH_For_Binding = _databaseHotelManagement
                .Database
                .SqlQuery<string>($"SELECT HoTen FROM KhachHang WHERE ID_KhachHang = {result.ID_KhachHang}")
                .Single();

            result.TotalPrice_For_Binding = _applicationUtilities.getMoneyForBinding(Convert.ToInt32(result.TongTien));

            PhieuThue rentBill = getRentBillById(result.ID_PhieuThue);
            DateTime start = rentBill.NgayBatDau ?? DateTime.Now;
            DateTime end = result.NgayTraPhong ?? DateTime.Now;

            result.NumDayRent_For_Binding = end.Subtract(start).TotalDays.ToString() + " ngày";


            Phong room = getRoomById(rentBill.SoPhong_For_Binding);

            result.DonGia_For_Binding = room.DonGiaPerDay_For_Binding;

            return result;
        }
        
        public List<HoaDon> getAllInvoice()
        {
            var result = _databaseHotelManagement
                .Database
                .SqlQuery<HoaDon>($"SELECT * FROM HoaDon")
                .ToList();

            for (int i = 0; i < result.Count; ++i)
            {
                result[i] = getInvoiceById(result[i].ID_HoaDon);
            }


            return result;
        }
    }
}
