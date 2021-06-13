using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Utilities
{
    class DatabaseUtilities
    {
        private DatabaseUtilities() { }

        private static DatabaseUtilities _databaseInstance;
        private static HotelManagementEntities _databaseHotelManagement;

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
            var result = _databaseHotelManagement
               .Database
               .SqlQuery<Phong>("SELECT P.ID_LoaiPhong, P.SoPhong, P.TinhTrang, P.GhiChu, LP.TenLoaiPhong, LP.DonGia, LP.SLKhachToiDa, P.Active FROM dbo.Phong P, dbo.LoaiPhong LP WHERE P.ID_LoaiPhong = LP.ID_LoaiPhong AND P.Active = 1")
               .ToList();

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

        public int getRevenueByRoomCategory(int loaiPhong, int month) 
        {
            var result = _databaseHotelManagement
              .Database
              .SqlQuery<int>($"select [dbo].[func_GetRevenueByRoomCat]({loaiPhong}, {month})")
              .Single();

            return result;
        }
    }
}
