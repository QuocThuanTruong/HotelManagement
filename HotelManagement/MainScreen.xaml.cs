using HotelManagement.Pages;
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

namespace HotelManagement
{
	/// <summary>
	/// Interaction logic for MainScreen.xaml
	/// </summary>
	public partial class MainScreen : Window
	{
		private List<Tuple<Button, Image, string, string, TextBlock>> _mainScreenButtons;
		private int role;

		public MainScreen()
		{
			InitializeComponent();
		}

		public MainScreen(int role)
		{
			InitializeComponent();
			this.role = role;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			_mainScreenButtons = new List<Tuple<Button, Image, string, string, TextBlock>>()
			{
				new Tuple<Button, Image, string, string, TextBlock>(dashboardPageButton, iconDashboardPage, "IconWhiteDashboard", "IconPurpleDashboard", dashboardPageName),
				new Tuple<Button, Image, string, string, TextBlock>(roomPageButton, iconRoomPage, "IconWhiteRoomKey", "IconPurpleRoomKey", roomPageName),
				new Tuple<Button, Image, string, string, TextBlock>(roomMngPageButton, iconRoomMngPage, "IconWhiteRoom", "IconPurpleRoom", roomMngPageName),
				new Tuple<Button, Image, string, string, TextBlock>(rentBillPageButton, iconRentBillPage, "IconWhiteRentBill", "IconPurpleRentBill", rentBillPageName),
				new Tuple<Button, Image, string, string, TextBlock>(invoicePageButton, iconInvoicePage, "IconWhiteInvoice", "IconPurpleInvoice", invoicePageName),
				new Tuple<Button, Image, string, string, TextBlock>(roomCatPageButton, iconRoomCatPage, "IconWhiteRoomCat", "IconPurpleRoomCat", romCatPageName),
				new Tuple<Button, Image, string, string, TextBlock>(cusCatPageButton, iconCusCatPage, "IconWhiteCustomer", "IconPurpleCustomer", cusCatPageName),
				new Tuple<Button, Image, string, string, TextBlock>(empPageButton, iconEmpPage, "IconWhiteEmployee", "IconPurpleEmployee", empPageName),
				new Tuple<Button, Image, string, string, TextBlock>(configPageButton, iconConfigPage, "IconWhiteConfig", "IconPurpleConfig", configPageName),
				new Tuple<Button, Image, string, string, TextBlock>(settingPageButton, iconSettingPage, "IconWhiteSetting", "IconPurpleSetting", settingPageName),
				new Tuple<Button, Image, string, string, TextBlock>(helpPageButton, iconHelpPage, "IconWhiteHelp", "IconPurpleHelp", helpPageName),
				new Tuple<Button, Image, string, string, TextBlock>(aboutPageButton, iconAboutPage, "IconWhiteAbout", "IconPurpleAbout", aboutPageName)
			};

			Page startPage = new DashboardPage();
			Button startButton = dashboardPageButton;

			if (role == 0)
			{
				startPage = new ListRoomPage();
				startButton = roomPageButton;
			}

			pageNavigation.NavigationService.Navigate(startPage);
		
			DrawerButton_Click(startButton, e);

			initVisibleButton();

			greetingTextBox.Text = "Hello mother fuker!";
		}

		private void initVisibleButton()
		{
			if (role == 0)
			{
				dashboardPageBtnContainer.Visibility = Visibility.Collapsed;
				roomMngBtnContainer.Visibility = Visibility.Collapsed;
				roomCatPageBtnContainer.Visibility = Visibility.Collapsed;
				cusCatPageBtnContainer.Visibility = Visibility.Collapsed;
				empPageBtnContainer.Visibility = Visibility.Collapsed;
				configPageBtnContainer.Visibility = Visibility.Collapsed;
			}
		}

		private void DrawerButton_Click(object sender, RoutedEventArgs e)
		{
			/** Highlight selected button**/
			var selectedButton = (Button)sender;

			/** Default property of button
			 * <Setter Property="Background" Value="Transparent"/>
			 * <Setter Property="BorderThickness" Value="1"/>**/

			foreach (var button in _mainScreenButtons)
			{
				if (button.Item1.Name != selectedButton.Name)
				{
					button.Item1.Background = Brushes.Transparent;
					button.Item1.IsEnabled = true;

					button.Item2.Source = (ImageSource)FindResource(button.Item3);
					button.Item5.Foreground = Brushes.White;
				}
			}

			//Highlight
			//selectedButton.Background = (Brush)FindResource("MyPurpleGradient");
			selectedButton.Background = Brushes.White;
			selectedButton.IsEnabled = false;
			/****/

			/** Navigating page **/
			pageNavigation.NavigationService.Navigate(getPageFromButton(selectedButton));
		}

		private Page getPageFromButton(Button selectedButton)
		{
			Page result = new DashboardPage();

			if (selectedButton.Name == dashboardPageButton.Name)
			{
				iconDashboardPage.Source = (ImageSource)FindResource(_mainScreenButtons[0].Item4);
				dashboardPageName.Foreground = (Brush)FindResource("MyPurple");
				DashboardPage dashboard = new DashboardPage();
				dashboard.ShowReportPageEvent += Dashboard_ShowReportPageEvent;
				result = dashboard;
			}
			else if (selectedButton.Name == roomPageButton.Name)
			{
				iconRoomPage.Source = (ImageSource)FindResource(_mainScreenButtons[1].Item4);
				roomPageName.Foreground = (Brush)FindResource("MyPurple");
				ListRoomPage listRoom = new ListRoomPage();
				listRoom.CreateRentBillEvent += ListRoom_CreateRentBillEvent;
				listRoom.CreateInvoiceEvent += ListRoom_CreateInvoiceEvent;
				result = listRoom;

			}
			else if (selectedButton.Name == roomMngPageButton.Name)
			{
				iconRoomMngPage.Source = (ImageSource)FindResource(_mainScreenButtons[2].Item4);
				roomMngPageName.Foreground = (Brush)FindResource("MyPurple");
				result = new RoomManagementPage();

			}
			else if (selectedButton.Name == rentBillPageButton.Name)
			{
				iconRentBillPage.Source = (ImageSource)FindResource(_mainScreenButtons[3].Item4);
				rentBillPageName.Foreground = (Brush)FindResource("MyPurple");
				RentBillManagementPage rentBill = new RentBillManagementPage();
				rentBill.EditRentBillEvent += RentBill_EditRentBillEvent;
				result = rentBill;

			}
			else if (selectedButton.Name == invoicePageButton.Name)
			{
				iconInvoicePage.Source = (ImageSource)FindResource(_mainScreenButtons[4].Item4);
				invoicePageName.Foreground = (Brush)FindResource("MyPurple");
				InvoiceManagementPage invoiceManagement = new InvoiceManagementPage();
				invoiceManagement.ViewInvoiceEvent += InvoiceManagement_ViewInvoiceEvent;
				result = invoiceManagement;

			}
			else if (selectedButton.Name == roomCatPageButton.Name)
			{
				iconRoomCatPage.Source = (ImageSource)FindResource(_mainScreenButtons[5].Item4);
				romCatPageName.Foreground = (Brush)FindResource("MyPurple");
				result = new RoomCategoriesManagementPage();

			}
			else if (selectedButton.Name == cusCatPageButton.Name)
			{
				iconCusCatPage.Source = (ImageSource)FindResource(_mainScreenButtons[6].Item4);
				cusCatPageName.Foreground = (Brush)FindResource("MyPurple");
				result = new CustomerCategoriesManagementPage();

			}
			else if (selectedButton.Name == empPageButton.Name)
			{
				iconEmpPage.Source = (ImageSource)FindResource(_mainScreenButtons[7].Item4);
				empPageName.Foreground = (Brush)FindResource("MyPurple");
				result = new EmployeeManagementPage();

			}
			else if (selectedButton.Name == configPageButton.Name)
			{
				iconConfigPage.Source = (ImageSource)FindResource(_mainScreenButtons[8].Item4);
				configPageName.Foreground = (Brush)FindResource("MyPurple");
				result = new ConfigPage();

			}
			else if (selectedButton.Name == settingPageButton.Name)
			{
				iconSettingPage.Source = (ImageSource)FindResource(_mainScreenButtons[9].Item4);
				settingPageName.Foreground = (Brush)FindResource("MyPurple");
				result = new ChangePasswordPage();

			}
			else if (selectedButton.Name == helpPageButton.Name)
			{
				iconHelpPage.Source = (ImageSource)FindResource(_mainScreenButtons[10].Item4);
				helpPageName.Foreground = (Brush)FindResource("MyPurple");
				result = new HelpPage();
			}
			else if (selectedButton.Name == aboutPageButton.Name)
			{
				iconAboutPage.Source = (ImageSource)FindResource(_mainScreenButtons[11].Item4);
				aboutPageName.Foreground = (Brush)FindResource("MyPurple");
				result = new AboutPage();
			}

			return result;
		}

		private void InvoiceManagement_ViewInvoiceEvent(int id)
		{
			CreateInvoicePage createInvoicePage = new CreateInvoicePage(id, invoicePageButton);
			createInvoicePage.BackPageEvent += All_BackPageEvent;
			pageNavigation.NavigationService.Navigate(createInvoicePage);
			clearDrawerButton();
		}

		private void All_BackPageEvent(Button page)
		{
			DrawerButton_Click(page, null);
		}

		private void RentBill_EditRentBillEvent(int id)
		{
			CreateRentBillPage createRentBill = new CreateRentBillPage(id, rentBillPageButton);
			createRentBill.BackPageEvent += All_BackPageEvent;
			pageNavigation.NavigationService.Navigate(createRentBill);
			clearDrawerButton();
		}

		private void ListRoom_CreateInvoiceEvent(int id)
		{
			CreateInvoicePage createInvoicePage = new CreateInvoicePage(id, roomPageButton);
			createInvoicePage.BackPageEvent += All_BackPageEvent;
			pageNavigation.NavigationService.Navigate(createInvoicePage);
			clearDrawerButton();
		}

		private void ListRoom_CreateRentBillEvent(int id)
		{
			CreateRentBillPage createRentBill = new CreateRentBillPage(id, roomPageButton);
			createRentBill.BackPageEvent += All_BackPageEvent;
			pageNavigation.NavigationService.Navigate(createRentBill);
			clearDrawerButton();
		}

		private void Dashboard_ShowReportPageEvent()
		{
			RevenueReportPage reportPage = new RevenueReportPage();
			reportPage.BackDashboardEvent += ReportPage_BackDashboardEvent;
			pageNavigation.NavigationService.Navigate(reportPage);
			clearDrawerButton();
		}

		private void ReportPage_BackDashboardEvent()
		{

			DrawerButton_Click(dashboardPageButton, null);
		}

		//private void CakeDetailPage_UpdateOrder(int value)
		//{
		//	if (value > 0)
		//	{
		//		badgeButton.Visibility = Visibility.Visible;
		//		badgeTextBlock.Text = value.ToString();
		//	}
		//	else
		//	{
		//		badgeButton.Visibility = Visibility.Collapsed;
		//	}
		//}



		private void clearDrawerButton()
		{
			foreach (var button in _mainScreenButtons)
			{

				button.Item1.Background = Brushes.Transparent;
				button.Item1.IsEnabled = true;

				button.Item2.Source = (ImageSource)FindResource(button.Item3);
				button.Item5.Foreground = Brushes.White;

			}
		}

		private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}

		private void closeWindowButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void minimizeWindowButton_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}
	}
}
