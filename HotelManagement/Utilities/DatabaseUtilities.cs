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


    }
}
