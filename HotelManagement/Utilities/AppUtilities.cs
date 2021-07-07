using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelManagement.Converters;

namespace HotelManagement.Utilities
{
    class ApplicationUtilities
    {
        private ApplicationUtilities() { }

        private static ApplicationUtilities _applicationInstance;
        private AbsolutePathConverter _absolutePathConverter = new AbsolutePathConverter();


        public static ApplicationUtilities GetAppInstance()
        {
            if (_applicationInstance == null)
            {
                _applicationInstance = new ApplicationUtilities();
            }
            else
            {
                //Do Nothing
            }

            return _applicationInstance;
        }

        public void createExportedDirectory(string name)
        {
            string path = (string)(_absolutePathConverter.Convert(name, null, null, null));

            if (Directory.Exists(path))
            {
                return;
            }
            else
            {
                Directory.CreateDirectory(path);
            }
        }

        public string getStandardName(string name, int maxLength)
        {
            var result = name;

            if (result.Length > maxLength)
            {
                result = result.Substring(0, maxLength - 3);
                result += "...";
            }

            return result;
        }


        public string getMoneyForBinding(int money)
        {
            string result = string.Format("{0:n0}", money);

            result += " đ";

            return result;
        }

        public string getMoneyForBinding2(int money)
        {
            string result = string.Format("{0:n0}", money);

            result += " VND";

            return result;
        }

        public List<Phong> convertToPhong(List<func_getAllRoom_Result> rooms)
        {
            List<Phong> result = new List<Phong>();

            foreach (var room in rooms)
            {
                Phong r = new Phong();

                r.SoPhong = room.SoPhong;
                r.ID_LoaiPhong = room.ID_LoaiPhong;
                r.TinhTrang = room.TinhTrang;
                r.GhiChu = room.GhiChu;
                r.TenLoaiPhong = room.TenLoaiPhong;
                r.DonGia = room.DonGia;
                r.SLKhachToiDa = room.SLKhachToiDa;
                r.Active = room.Active;

                result.Add(r);
            }

            return result;
        }

        public void copyFileToDirectory(string srcPath, string subPath, string nameFile)
        {
            var destPath = (string)_absolutePathConverter.Convert($"{subPath}/{nameFile}", null, null, null);

            File.Copy(srcPath, destPath, true);

        }
    }
}
