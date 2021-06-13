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


        public string GetMoneyForBinding(int money)
        {
            string result = string.Format("{0:n0}", money);

            result += " đ";

            return result;
        }
    }
}
