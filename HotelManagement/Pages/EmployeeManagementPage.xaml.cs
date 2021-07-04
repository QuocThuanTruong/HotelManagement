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
using BCrypt.Net;
using System.Windows.Forms;
using System.Data;
using System.IO;
using ExcelDataReader;

namespace HotelManagement.Pages
{
	/// <summary>
	/// Interaction logic for EmployeeManagementPage.xaml
	/// </summary>
	public partial class EmployeeManagementPage : Page
	{
		private DatabaseUtilities _databaseUtilities = DatabaseUtilities.GetDatabaseInstance();
		private ApplicationUtilities _applicationUtilities = ApplicationUtilities.GetAppInstance();

		public List<NhanVien> employees;

		private int _editedID = -1;
		public EmployeeManagementPage()
		{
			InitializeComponent();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			employees = _databaseUtilities.getAllEmployee();

			employeeListView.ItemsSource = employees;
		}

		private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
			var deleteId = Convert.ToInt32(((System.Windows.Controls.Button)sender).Tag);
			NhanVien selectedRoomCategory = (from em in employees
											  where em.ID_NhanVien == deleteId
											  select em).First();

			_databaseUtilities.deleteEmployee(selectedRoomCategory);

			employees = (from em in employees
						 where em.ID_NhanVien != deleteId
						select em).ToList();

			employeeListView.ItemsSource = null;
			employeeListView.ItemsSource = employees;
		}

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
			iconAddEmployee.Source = (ImageSource)FindResource("IconWhiteUpdate");
			//iconAddRoom.Source = (ImageSource)FindResource("IconWhiteAdd");

			_editedID = Convert.ToInt32(((System.Windows.Controls.Button)sender).Tag);
			NhanVien selectedEmployee = (from em in employees
											  where em.ID_NhanVien == _editedID
											  select em).First();

			nameTextBox.Text = selectedEmployee.HoTen;
			CMNDTextBox.Text = selectedEmployee.CMND;
			usernameTextBox.Text = selectedEmployee.Username;

			employeeRoleComboBox.SelectedIndex = (selectedEmployee.LoaiNhanVien ?? false) ? 1 : 0;
		}

        private void addCustomerButton_Click(object sender, RoutedEventArgs e)
        {
			if (_editedID != -1)
			{
				NhanVien selectedEmployee = (from em in employees
											 where em.ID_NhanVien == _editedID
											 select em).First();

				selectedEmployee.HoTen = nameTextBox.Text;
				if (selectedEmployee.HoTen.Length <= 0)
				{
					notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống tên nhân viên", "OK", () => { });
					return;
				}

				selectedEmployee.CMND = CMNDTextBox.Text;
				if (selectedEmployee.CMND.Length <= 0)
				{
					notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống CMND nhân viên", "OK", () => { });
					return;
				}

				selectedEmployee.Username = usernameTextBox.Text;
				if (selectedEmployee.Username.Length <= 0)
				{
					notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống tên đăng nhập của nhân viên", "OK", () => { });
					return;
				}

				selectedEmployee.HidenPassword = passwordTextBox.Text;
				if (selectedEmployee.HidenPassword.Length > 0)
				{
					selectedEmployee.Password = BCrypt.Net.BCrypt.HashPassword(selectedEmployee.HidenPassword, workFactor: 10);
				}

				selectedEmployee.LoaiNhanVien = employeeRoleComboBox.SelectedIndex == 0 ? true : false;

				_databaseUtilities.updateEmployee(selectedEmployee);
				employees = _databaseUtilities.getAllEmployee();

				employeeListView.ItemsSource = null;
				employeeListView.ItemsSource = employees;

				//Rest
				_editedID = -1;
				nameTextBox.Text = "";
				CMNDTextBox.Text = "";
				usernameTextBox.Text = "";
				passwordTextBox.Text = "";

				iconAddEmployee.Source = (ImageSource)FindResource("IconWhiteAdd");
			}
			else
			{
				NhanVien newEmployee = new NhanVien();
				newEmployee.ID_NhanVien = _databaseUtilities.getMaxIdEmployee() + 1;

				newEmployee.HoTen = nameTextBox.Text;
				if (newEmployee.HoTen.Length <= 0)
				{
					notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống tên nhân viên", "OK", () => { });
					return;
				}

				newEmployee.CMND = CMNDTextBox.Text;
				if (newEmployee.CMND.Length <= 0)
				{
					notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống CMND nhân viên", "OK", () => { });
					return;
				}

				newEmployee.Username = usernameTextBox.Text;
				if (newEmployee.Username.Length <= 0)
				{
					notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống tên đăng nhập của nhân viên", "OK", () => { });
					return;
				}

				newEmployee.Password = passwordTextBox.Text;
				if (newEmployee.Username.Length <= 0)
				{
					notiMessageSnackbar.MessageQueue.Enqueue($"Không được bỏ trống mật khẩu của nhân viên", "OK", () => { });
					return;
				}
				newEmployee.Password = BCrypt.Net.BCrypt.HashPassword(newEmployee.Password, workFactor: 10);

				newEmployee.LoaiNhanVien = employeeRoleComboBox.SelectedIndex == 0 ? true : false;

				_databaseUtilities.addNewEmployee(newEmployee);

				employees = _databaseUtilities.getAllEmployee();

				employeeListView.ItemsSource = null;
				employeeListView.ItemsSource = employees;

				//Rest
				nameTextBox.Text = "";
				CMNDTextBox.Text = "";
				usernameTextBox.Text = "";
				passwordTextBox.Text = "";
			}
		}

        private void importExcelButton_Click(object sender, RoutedEventArgs e)
        {
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Multiselect = true;
				openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
				openFileDialog.Filter = "Excel Files|*.xls;*.xlsx";

				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					DataTableCollection tables;

					using (var stream = File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
					{
						using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
						{
							DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
							{
								ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
								{
									UseHeaderRow = true
								}
							});

							tables = result.Tables;
						}

						DataTable dt = tables["employee"];
						if (dt != null)
						{
							List<NhanVien> list = new List<NhanVien>();
							for (int i = 0; i < dt.Rows.Count; i++)
							{
								NhanVien employee = new NhanVien();
								employee.HoTen = dt.Rows[i]["HoTen"].ToString();

								bool exists = _databaseUtilities.checkExistsEmployeeByName(employee.HoTen);
								if (exists)
								{
									employee.ID_NhanVien = _databaseUtilities.getEmployeeByName(employee.HoTen).ID_NhanVien;
								}
								else
								{
									employee.ID_NhanVien = _databaseUtilities.getMaxIdEmployee() + 1;
								}

								employee.CMND = dt.Rows[i]["CMND"].ToString();
								employee.LoaiNhanVien = Convert.ToBoolean(dt.Rows[i]["LoaiNhanVien"].ToString());
								employee.Username = dt.Rows[i]["Username"].ToString();
								employee.Password = BCrypt.Net.BCrypt.HashPassword(dt.Rows[i]["Password"].ToString(), workFactor: 10);
								employee.Active = true;

								if (exists)
								{
									_databaseUtilities.updateEmployee(employee);
								}
								else
								{
									employees.Add(employee);

									_databaseUtilities.addNewEmployee(employee);
								}
							}
							Page_Loaded(null, null);
							notiMessageSnackbar.MessageQueue.Enqueue($"Thêm dữ liệu thành công", "OK", () => { });
						}
						else
						{
							notiMessageSnackbar.MessageQueue.Enqueue($"Sheet chứa dữ liệu phòng cần được đổi tên thành \"employee\"", "OK", () => { });
						}
					}
				}
			}
		}
    }
}
