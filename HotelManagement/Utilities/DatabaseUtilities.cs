using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media.Imaging;
using HotelManagement.Converters;
using BCrypt.Net;

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

            int count = _databaseHotelManagement
                .Database
                .SqlQuery<int>($"SELECT COUNT(*) from NhanVien where Username = N'{username}'")
                .FirstOrDefault();

            if (count > 0)
            {
                
                result = _databaseHotelManagement
                   .Database
                   .SqlQuery<NhanVien>($"SELECT * from NhanVien where Username = N'{username}'")
                   .FirstOrDefault();

                if (BCrypt.Net.BCrypt.Verify(password, result.Password) == false)
                {
                    result = null;
                }
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
                    result[i].Status_For_Binding = "THANH TOÁN";
                }
                else
                {
                    uri = (string)_absolutePathConverter.Convert("Assets/Images/badage-empty.png", null, null, null);
                    result[i].Status_For_Binding = "THUÊ NGAY";
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

        public bool checkExistRoom(int ID_Room)
        {
            var count = _databaseHotelManagement
                .Database
                .SqlQuery<int>($"Select count(*) from Phong where SoPhong = {ID_Room}")
                .Single();

            if (count > 0)
            {
                return true;
            } 
            else
            {
                return false;
            }
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
                .SqlQuery<LoaiPhong>("Select * from dbo.LoaiPhong where Active = 'true'")
                .ToList();

            for (int i = 0; i < result.Count; ++i)
            {
                result[i].DonGia_For_Binding = _applicationUtilities.getMoneyForBinding(Convert.ToInt32(result[i].DonGia ?? 0)) + "/ngày";
                result[i].SLKhachToiDa_For_Binding = result[i].SLKhachToiDa.ToString() + " người";
            }

            return result;
        }

        public List<LoaiKhach> getAllCustomerCategory()
        {
            var result = _databaseHotelManagement
                .Database
                .SqlQuery<LoaiKhach>("Select * from dbo.LoaiKhach where Active = 'true'")
                .ToList();

            return result;
        }

        public Nullable<int> getRevenueByRoomCategory(int loaiPhong, int month)
        {
      
            var result = _databaseHotelManagement
              .Database
              .SqlQuery<Nullable<int>>($"select [dbo].[func_GetRevenueByRoomCat]({loaiPhong}, {month})")
              .FirstOrDefault();
   
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
                result.Visible_View_For_Bingding = "Collapsed";
                result.Visible_Edit_Delete_For_Bingding = "Visible";
            }
            else if (result.Active == 2)
            {
                result.Status = "Đã thanh toán";
                result.Visible_View_For_Bingding = "Visible";
                result.Visible_Edit_Delete_For_Bingding = "Collapsed";
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

            result.NumDayRent_For_Binding = Convert.ToInt32(end.Subtract(start).TotalDays);
            result.NumDayRent_For_Binding = result.NumDayRent_For_Binding == 0 ? 1 : result.NumDayRent_For_Binding;


            Phong room = getRoomById(rentBill.SoPhong_For_Binding);

            result.DonGia_For_Binding = room.DonGiaPerDay_For_Binding;
            result.DonGia = Convert.ToInt32(room.DonGia ?? 0);

            result.SoPhong = room.SoPhong;
            result.Room = room;

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

        public void updateRoomCategory(LoaiPhong roomCategory)
        {
            _databaseHotelManagement
                .Database
                .ExecuteSqlCommand($"UPDATE LoaiPhong Set TenLoaiPhong = N'{roomCategory.TenLoaiPhong}', SLKhachToiDa = {roomCategory.SLKhachToiDa}, DonGia = {roomCategory.DonGia} WHERE ID_LoaiPhong = {roomCategory.ID_LoaiPhong}");
        }

        public void deleteRoomCategory(LoaiPhong roomCategory)
        {
            _databaseHotelManagement
                  .Database
                  .ExecuteSqlCommand($"UPDATE LoaiPhong Set Active = 'false' WHERE ID_LoaiPhong = {roomCategory.ID_LoaiPhong}");
        }

        public void addNewRoomCategory(LoaiPhong roomCategory)
        {
            _databaseHotelManagement
                  .Database
                  .ExecuteSqlCommand($"INSERT [dbo].[LoaiPhong] ([ID_LoaiPhong], [TenLoaiPhong], [DonGia], [SLKhachToiDa], [Active]) VALUES ({roomCategory.ID_LoaiPhong}, N'{roomCategory.TenLoaiPhong}', {roomCategory.DonGia}, {roomCategory.SLKhachToiDa}, 1)");
        }

        public int getMaxIdRoomCategory()
        {
            var result = _databaseHotelManagement
              .Database
              .SqlQuery<int>($"select max(ID_LoaiPhong) from LoaiPhong")
              .Single();

            return result;
        }

        public bool checkExistsRoomCategoryByName(string name)
        {
            var count = _databaseHotelManagement
              .Database
              .SqlQuery<int>($"select count(*) from LoaiPhong where TenLoaiPhong = N'{name}'")
              .Single();

            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public LoaiPhong getRoomCategoryByName(string name)
        {
            var result = _databaseHotelManagement
              .Database
              .SqlQuery<LoaiPhong>($"select * from LoaiPhong where TenLoaiPhong = N'{name}'")
              .Single();

            return result;
        }

        public bool checkExistsCustomerCategoryByName(string name)
        {
            var count = _databaseHotelManagement
              .Database
              .SqlQuery<int>($"select count(*) from LoaiKhach where TenLoaiKhach = N'{name}'")
              .Single();

            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public LoaiKhach getCustomerCategoryByName(string name)
        {
            var result = _databaseHotelManagement
              .Database
              .SqlQuery<LoaiKhach>($"select * from LoaiKhach where TenLoaiKhach = N'{name}'")
              .Single();

            return result;
        }

        public void updateCustomerCategory(LoaiKhach customerCategory)
        {
            _databaseHotelManagement
                .Database
                .ExecuteSqlCommand($"UPDATE LoaiKhach Set TenLoaiKhach = N'{customerCategory.TenLoaiKhach}', HeSo = {customerCategory.HeSo} WHERE ID_LoaiKhach = {customerCategory.ID_LoaiKhach}");
        }

        public void deleteCustomerCategory(LoaiKhach customerCategory)
        {
            _databaseHotelManagement
                  .Database
                  .ExecuteSqlCommand($"UPDATE LoaiKhach Set Active = 'false' WHERE ID_LoaiKhach = {customerCategory.ID_LoaiKhach}");
        }

        public void addNewCustomerCategory(LoaiKhach customerCategory)
        {
            _databaseHotelManagement
                  .Database
                  .ExecuteSqlCommand($"INSERT [dbo].[LoaiKhach] ([ID_LoaiKhach], [TenLoaiKhach], [HeSo], [Active]) VALUES ({customerCategory.ID_LoaiKhach}, N'{customerCategory.TenLoaiKhach}', {customerCategory.HeSo}, 1)");
        }

        public int getMaxIdCustomerCategory()
        {
            var result = _databaseHotelManagement
              .Database
              .SqlQuery<int>($"select max(ID_LoaiKhach) from LoaiKhach")
              .Single();

            return result;
        }

        public List<NhanVien> getAllEmployee()
        {
            var result = _databaseHotelManagement
                .Database
                .SqlQuery<NhanVien>("SELECT * FROM NhanVien Where Active = 'true'")
                .ToList();

            for (int i = 0; i < result.Count; ++i)
            {
                for (int j = 0; j < 6; ++j)
                {
                    result[i].HidenPassword += "*";
                }

                if (result[i].LoaiNhanVien == true)
                {
                    result[i].Role_For_Binding = "Quản lí";
                } 
                else
                {
                    result[i].Role_For_Binding = "Nhân viên";
                }
            }

            return result;
        }

        public void updateEmployee(NhanVien employee)
        {
            string role = (employee.LoaiNhanVien == false) ? "false" : "true";

            _databaseHotelManagement
                .Database
                .ExecuteSqlCommand($"UPDATE NhanVien Set HoTen = N'{employee.HoTen}', CMND = '{employee.CMND}', LoaiNhanVien = '{role}', Username = '{employee.Username}', Password = '{employee.Password}' WHERE ID_NhanVien = {employee.ID_NhanVien}");
        }

        public void deleteEmployee(NhanVien employee)
        {
            _databaseHotelManagement
                  .Database
                  .ExecuteSqlCommand($"UPDATE NhanVien Set Active = 'false' WHERE ID_NhanVien = {employee.ID_NhanVien}");
        }

        public void addNewEmployee(NhanVien employee)
        {
            int role = (employee.LoaiNhanVien == false) ? 0 : 1;

            string query = $"INSERT [dbo].[NhanVien] ([ID_NhanVien], [HoTen], [CMND], [LoaiNhanVien], [Username], [Password], [Active]) VALUES ({employee.ID_NhanVien}, N'{employee.HoTen}', N'{employee.CMND}', {role}, N'{employee.Username}', N'{employee.Password}', 1)";
            _databaseHotelManagement
                  .Database
                  .ExecuteSqlCommand(query);
        }

        public int getMaxIdEmployee()
        {
            var result = _databaseHotelManagement
              .Database
              .SqlQuery<int>($"select max(ID_NhanVien) from NhanVien")
              .Single();

            return result;
        }

        public void updatePassword(string newPassword)
        {
            _databaseHotelManagement
                 .Database
                 .ExecuteSqlCommand($"UPDATE NhanVien Set Password = N'{newPassword}' WHERE ID_NhanVien = {Global.staticCurrentEmployee.ID_NhanVien}");
        }

        public int getMaxIdConfig()
        {
            var result = _databaseHotelManagement
              .Database
              .SqlQuery<int>($"select max(ID) from CauHinh")
              .Single();

            return result;
        }

        public CauHinh getConfig()
        {
            int ID = getMaxIdConfig();

            var result = _databaseHotelManagement
                .Database
                .SqlQuery<CauHinh>($"SELECT * FROM CauHinh WHERE ID = {ID}")
                .Single();

            return result;
        }

        public void updateConfig(CauHinh config)
        {
            _databaseHotelManagement
                  .Database
                  .ExecuteSqlCommand($"UPDATE CauHinh Set GiaTri = N'{config.GiaTri}', DieuKien = N'>={config.DieuKien}' WHERE ID = {config.ID}");
        }

        public double getRentBillFactor(int IDRenBill)
        {
            var result = _databaseHotelManagement
                .Database
                .SqlQuery<double>($"Select dbo.func_GetRentBillFactor({IDRenBill})")
                .Single();

            return result;
        }

        public bool checkExistsEmployeeByName(string name)
        {
            var count = _databaseHotelManagement
              .Database
              .SqlQuery<int>($"select count(*) from NhanVien where HoTen = N'{name}'")
              .Single();

            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public NhanVien getEmployeeByName(string name)
        {
            var result = _databaseHotelManagement
              .Database
              .SqlQuery<NhanVien>($"select * from NhanVien where HoTen = N'{name}'")
              .Single();

            return result;
        }
    }
}
