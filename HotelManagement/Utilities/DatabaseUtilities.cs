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
