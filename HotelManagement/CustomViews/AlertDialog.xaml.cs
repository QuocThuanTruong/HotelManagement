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
using System.Windows.Shapes;

namespace HotelManagement.CustomViews
{
	/// <summary>
	/// Interaction logic for AlertDialog.xaml
	/// </summary>
	public partial class AlertDialog : Window
	{
		public AlertDialog()
		{
			InitializeComponent();
			
		}

		public AlertDialog(string message)
		{
			InitializeComponent();
			messageTextBlock.Text = message;
		}

		public AlertDialog(string message, bool isSucess)
		{
			InitializeComponent();
			messageTextBlock.Text = message;

			if (isSucess)
			{
				gifSuccess.Visibility = Visibility.Visible;
				gifFailed.Visibility = Visibility.Collapsed;
				messageTextBlock.Foreground = (Brush)FindResource("MyGreenGradient");
			}
			
		}

		private void closeWindowButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
