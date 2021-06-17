using HotelManagement.Utilities;
using System;
using System.Collections.Generic;
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
	/// Interaction logic for ConfigPage.xaml
	/// </summary>
	public partial class ConfigPage : Page
	{
		private DatabaseUtilities _databaseUtilities = DatabaseUtilities.GetDatabaseInstance();
		private ApplicationUtilities _applicationUtilities = ApplicationUtilities.GetAppInstance();

		public CauHinh currentConfig;
		public ConfigPage()
		{
			InitializeComponent();
		}

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
			currentConfig = _databaseUtilities.getConfig();

			double value = Convert.ToDouble(currentConfig.GiaTri);

			subMoneyRateTextBox.Text = (value * 100).ToString();
			numberCustomerTextBox.Text = currentConfig.DieuKien.Substring(2);
		}

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
			currentConfig.DieuKien = numberCustomerTextBox.Text;

			double value = Convert.ToDouble(subMoneyRateTextBox.Text);
			currentConfig.GiaTri = (value / 100).ToString();

			_databaseUtilities.updateConfig(currentConfig);

			Page_Loaded(null, null);
		}
    }
}
