﻿using DataObjects;
using LogicLayer;
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

namespace WpfPresentationLayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ICustomerManager _customerManager = new CustomerManager();
        private EmployeeManager _employeeManager = new EmployeeManager();
        private IProductOrderManager _orderManager = new ProductOrderManager();
        private IVehicleManager _vehicleManager = new VehicleManager();
        List<ProductOrder> _currentOpenOrders;
        List<Employee> employeeList;
        List<Charity> charityList;
        private List<ProductLot> _productLotList;
        private List<CommercialCustomer> _commercialCustomers;
        private List<Vehicle> _vehicleList;
        private IUserManager _userManager = new UserManager();
        private ISupplierManager _supplierManager = new SupplierManager();
        private IProductLotManager _productLotManager = new ProductLotManager();
        private IProductManager _productManager = new ProductManager();

        Employee _employee = null;

        User _user = null;
        private ICharityManager _charityManager;

        private IPackageManager _packageManager = new PackageManager();
        List<Package> _packageList = null;
        private IOrderStatusManager _orderStatusManager = new OrderStatusManager();
        List<string> _orderStatusList = null;

        public MainWindow()
        {
            InitializeComponent();
            _userManager = new UserManager();
            _charityManager = new CharityManager();
            _employeeManager = new EmployeeManager();
            DisposeFiles();
        }

        /// <summary>
        /// Eric Walton
        /// 2017/06/02
        /// 
        /// Button to load Create Commercial Customer Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Create_CommercialCustomer(object sender, RoutedEventArgs e)
        {
            _employee = _employeeManager.RetrieveEmployeeByUserName(_user.UserName);
            try
            {
                CreateCommercialCustomerWindow cCCW = new CreateCommercialCustomerWindow((int)_employee.EmployeeId);
                if (cCCW.ShowDialog() == true)
                {
                    _commercialCustomers = _customerManager.RetrieveCommercialCustomers();
                    dgCommercialCustomer.ItemsSource = _commercialCustomers;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error: An employee must be logged in to create a commercial customer.");
            }
            
        }

        /// <summary>
        /// Ariel Sigo
        /// Created 2017/10/02
        /// 
        /// Button that leads to update employee form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Update_Employee(object sender, RoutedEventArgs e)
        {
            frmUpdateEmployee fUE = new frmUpdateEmployee(_employeeManager, _employee);
            fUE.ShowDialog();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            hideTabs();
            tabSetMain.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Bobby Thorne
        /// 2/11/17
        /// 
        /// When this button is pushed it first checks to see if there is a user logged in
        /// If there is not it will use the username and password text field and check if
        /// it matches with any user if so it then recieves the user's info
        /// 
        /// Needs work on returning employee info so tabs can be 
        /// filtered and not just show all
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (_user == null)
            {

                try
                {
                    if (_userManager.AuthenticateUser(tfUsername.Text, tfPassword.Password))
                    {

                        lblPassword.Visibility = Visibility.Collapsed;
                        lblUsername.Visibility = Visibility.Collapsed;
                        tfUsername.Visibility = Visibility.Collapsed;
                        tfPassword.Visibility = Visibility.Collapsed;
                        tfPassword.Password = "";
                        btnLogin.Content = "Logout";
                        btnLogin.IsDefault = false;
                        tfPassword.Background = Brushes.White;
                        try
                        {
                            _user = _userManager.userInstance;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Failed to find user.");
                            btnLogin_Click(sender, e);
                        }
                        try
                        {
                            _employee = _employeeManager.RetrieveEmployeeByUserName(_user.UserName);
                        }
                        catch (Exception ex)
                        {
                            // Enters here if user that access this is not an employee.
                            // For now it does nothing. 
                            MessageBox.Show("Employee table is empty or DB connection error.");
                        }
                        statusMessage.Content = "Welcome " + _user.UserName;
                        showTabs(); // This needs to be updated so it will show just one that is 
                        // assigned to the employe


                    }
                    else
                    {
                        statusMessage.Content = "Username and Password did not match.";
                        tfPassword.Password = "";
                        tfPassword.Background = Brushes.Red;
                    }
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    ErrorAlert.ShowDatabaseError();
                }
            }
            else
            {
                _user = null;
                btnLogin.Content = "Login";
                btnLogin.IsDefault = true;

                statusMessage.Content = "Please Log in to continue...";
                hideTabs();
                lblPassword.Visibility = Visibility.Visible;
                lblUsername.Visibility = Visibility.Visible;
                tfUsername.Visibility = Visibility.Visible;
                tfPassword.Visibility = Visibility.Visible;

            }

        }
        private void showTabs()
        {
            tabSetMain.Visibility = Visibility.Visible;
            tabCommercialCustomer.Visibility = Visibility.Visible;
            tabEmployee.Visibility = Visibility.Visible;
            tabUser.Visibility = Visibility.Visible;

        }

        private void hideTabs()
        {
            tabSetMain.Visibility = Visibility.Hidden;
            tabCommercialCustomer.Visibility = Visibility.Collapsed;
            tabEmployee.Visibility = Visibility.Collapsed;
            tabUser.Visibility = Visibility.Collapsed;
        }

        private void btnCreateNewUser_Click(object sender, RoutedEventArgs e)
        {
            CreateNewUser fCU = new CreateNewUser();
            fCU.ShowDialog();
        }
        
        private void tabCharity_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                charityList = _charityManager.RetrieveCharityList();
                dgrdCharity.DataContext = charityList;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                ErrorAlert.ShowDatabaseError();
            }
        }

        private void btnViewCharity_Click(object sender, RoutedEventArgs e)
        {
            if (dgrdCharity.SelectedIndex >= 0)
            {
                var CharityViewInstance = new CharityView(_charityManager, charityList[dgrdCharity.SelectedIndex]);
                CharityViewInstance.ShowDialog();
                tabCharity_Selected(sender, e);
            }

        }

        private void btnAddCharity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Employee> employeeSearchList = _employeeManager.SearchEmployees(new Employee() { UserId = _user.UserId});
                int employeeId;
                if(employeeSearchList.Count > 0)
                {
                    employeeId = (int)employeeSearchList[0].EmployeeId;
                    var CharityViewInstance = new CharityView(_charityManager, employeeId);
                    CharityViewInstance.SetEditable();
                    CharityViewInstance.ShowDialog();
                    tabCharity_Selected(sender, e);
                } else
                {
                    MessageBox.Show("You are not authorized to enter new charities.");
                }
            } catch (System.Data.SqlClient.SqlException ex)
            {
                ErrorAlert.ShowDatabaseError();
            }
        }

        private void tabEmployee_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                employeeList = _employeeManager.RetrieveEmployeeList();
                dgrdEmployee.DataContext = employeeList;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                ErrorAlert.ShowDatabaseError();
            }
        }

        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            var EmployeeViewInstance = new EmployeeView(_employeeManager);
            EmployeeViewInstance.SetEditable();
            EmployeeViewInstance.ShowDialog();
            tabEmployee_Selected(sender, e);
        }

        private void tabOrder_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void btnViewEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (dgrdEmployee.SelectedIndex >= 0)
            {
                var EmployeeViewInstance = new EmployeeView(_employeeManager, employeeList[dgrdEmployee.SelectedIndex]);
                EmployeeViewInstance.ShowDialog();
                tabEmployee_Selected(sender, e);
            }
        }

        private void tabProductLot_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                _productLotList = _productLotManager.RetrieveProductLots();
                dgProductLots.ItemsSource = _productLotList;
            }
            catch (Exception ex)
            {

                MessageBox.Show("There was an error: " + ex.Message);
            }
        }

        private void AddProductLot_Click(object sender, RoutedEventArgs e)
        {
            var productLotView = new ProductLotView();
            productLotView.SetEditable();
            var addResult = productLotView.ShowDialog();
            if (addResult == true)
            {
                try
                {
                    var addInspectionFrm = new frmAddInspection(_productLotManager.RetrieveNewestProductLotBySupplier(_supplierManager.RetrieveSupplierBySupplierID(productLotView.supplierId)),
                        new GradeManager(), _employee, new ProductManager(), _supplierManager, new InspectionManager());
                    var addInspectionResult = addInspectionFrm.ShowDialog();
                    if (addInspectionResult == true)
                    {
                        MessageBox.Show("Inspection Added");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            tabProductLot_Selected(sender, e);
        }

        /// <summary>
        /// Christian Lopez
        /// Created on 2017/01/31
        /// 
        /// Open a frmAddSupplier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Last modified by Christian Lopez on 2017/02/02</remarks>
        private void btnCreateSupplier_Click(object sender, RoutedEventArgs e)
        {
            var addSupplierFrm = new frmAddSupplier(_user, _userManager, _supplierManager);
            var addSupplierResult = addSupplierFrm.ShowDialog();
            if (addSupplierResult == true)
            {
                MessageBox.Show("Supplier added!");
            }
        }


        /// <summary>
        /// Christian Lopez
        /// Created on 2017/02/15
        /// 
        /// Open a frmAddInspection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateInspection_Click(object sender, RoutedEventArgs e)
        {
            if (dgProductLots.SelectedIndex != -1)
            {
                try
                {
                    // Will need to redo method call when linked with either datagrid of ProductLots or immediately aftermaking a productLot
                    //_productLotManager.RetrieveNewestProductLotBySupplier(_supplierManager.RetrieveSupplierByUserId(_user.UserId))
                    var addInspectionFrm = new frmAddInspection((ProductLot)dgProductLots.SelectedItem,
                        new GradeManager(), _employee, new ProductManager(), _supplierManager, new InspectionManager());
                    var addInspectionResult = addInspectionFrm.ShowDialog();
                    if (addInspectionResult == true)
                    {
                        MessageBox.Show("Inspection Added");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        
        /// <summary>
        /// Eric Walton
        /// 2017/28/02
        /// Commercial Customer tab selected 
        /// Retrieves updated list of commercial customers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommercialCustomer_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                _commercialCustomers = _customerManager.RetrieveCommercialCustomers();
                dgCommercialCustomer.ItemsSource = _commercialCustomers;
            }
            catch (Exception)
            {
                ErrorAlert.ShowDatabaseError();
            }
        }

        private void tfPassword_KeyDown(object sender, KeyEventArgs e)
        {
            tfPassword.Background = Brushes.White;
        }
        
        /// <summary>
        /// Laura Simmonds
        /// 2017/27/02
        /// Button links tab to View Product window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewProductBtn(object sender, RoutedEventArgs e)
        {
            frmViewProduct viewProduct = new frmViewProduct();
            viewProduct.ShowDialog();

        }

        /// <summary>
        ///     Mason Allen
        ///     ListView that displays current open orders.  Double clicking an item will display the order details.
        /// </summary>
        /// <remarks>
        /// Robert Forbes
        /// 2017/03/01
        /// 
        /// Modified to work with drop down to select status
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabOpenOrders_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _orderStatusList = _orderStatusManager.RetrieveAllOrderStatus();
                cboOrderStatus.ItemsSource = _orderStatusList;
                cboOrderStatus.SelectedIndex = 0;
                if (cboOrderStatus.SelectedItem != null)
                {

                    _currentOpenOrders = _orderManager.RetrieveProductOrdersByStatus((string)cboOrderStatus.SelectedItem);
                    lvOpenOrders.Items.Clear();

                    for (int i = 0; i < _currentOpenOrders.Count; i++)
                    {
                        this.lvOpenOrders.Items.Add(_currentOpenOrders[i]);
                    }
                    lblStatus.Content = "Status: Success";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Content += ex.ToString();
            }
        }



        private void lvOpenOrders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ProductOrder selectedItem = (ProductOrder)lvOpenOrders.SelectedItem;
                int index = selectedItem.OrderId;

                var detailsWindow = new frmProductOrderDetails(index);
                detailsWindow.Show();
            }
            catch (Exception)
            {
                lblStatus.Content = "Nothing selected";
            }

        }

        /// <summary>
        /// Robert Forbes
        /// 2017/03/01
        /// 
        /// Updates the list of orders to match the new combo box selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboOrderStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cboOrderStatus.SelectedItem != null)
                {
                    _currentOpenOrders = _orderManager.RetrieveProductOrdersByStatus((string)cboOrderStatus.SelectedItem);
                    lvOpenOrders.Items.Clear();

                    for (int i = 0; i < _currentOpenOrders.Count; i++)
                    {
                        this.lvOpenOrders.Items.Add(_currentOpenOrders[i]);
                    }
                    lblStatus.Content = "Status: Success";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Content += ex.ToString();
            }
        }


        private void BtnAddProduct_OnClick(object sender, RoutedEventArgs e)
        {
            var frmAddProduct = new frmAddProduct(_user, _productManager);
            frmAddProduct.ShowDialog();
        }

        /// <summary>
        /// Created by Natacha Ilunga
        /// Created on 02-28-2017
        /// 
        /// Navigate to Browse Products Page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowseProduct_Click(object sender, RoutedEventArgs e)
        {
            var frmBrowseProducts = new frmBrowseProducts(_user, _productManager);
            frmBrowseProducts.Show();
        }

        
        /// <summary>
        /// Created by Michael Takrama, Natacha Ilunga
        /// Creatd on 02-28-2017
        /// 
        /// Method to cleanup cached files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DisposeFiles()
        {
            try
            {
                this.DisposeImages();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }

        }


        

        /// <summary>
        /// Robert Forbes
        /// 2017/03/01
        /// 
        /// Tab the shows a list of all packages in the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabPackages_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshPackageList();
        }

        /// <summary>
        /// Robert Forbes
        /// 2017/03/01
        /// 
        /// Updates the items source for the package list datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabPackages_GotFocus(object sender, RoutedEventArgs e)
        {
            RefreshPackageList();
            dgPackages.ItemsSource = _packageList;
            dgPackages.Items.Refresh();
        }

        /// <summary>
        /// Robert Forbes
        /// 2017/03/01
        /// 
        /// Updates the locally stored list of packages
        /// </summary>
        private void RefreshPackageList()
        {
            try
            {
                _packageList = _packageManager.RetrieveAllPackages();
            }
            catch {
                MessageBox.Show("Unable to retrieve packages from database");
            }
        }

        /// <summary>
        /// Mason Allen
        /// 03/01/2017
        /// Opens new window to create a new vehicle record
        /// </summary>
        private void btnAddNewVehicle_Click(object sender, RoutedEventArgs e)
        {
            var addNewVehicleWindow = new frmAddNewVehicle();
            addNewVehicleWindow.Show();
        }

        private void BtnManageStock_OnClick(object sender, RoutedEventArgs e)
        {
            frmManageStock fms = new frmManageStock();
            fms.ShowDialog();
        }

        /// <summary>
        /// Eric Walton 
        /// 2017/02/03
        /// Triggers when vehicle management tab is selected
        /// Will populate a list of vehicles once complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabVehicle_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                _vehicleList = _vehicleManager.RetrieveAllVehicles();
                dgVehicle.ItemsSource = _vehicleList;
            }
            catch (Exception)
            {
                ErrorAlert.ShowDatabaseError();
            }
            
        }

        /// <summary>
        /// Christian Lopez
        /// Created 2017/03/02
        /// 
        /// Uses frmAddSupplier, but this calling does not automatically set the approved status that the other one does.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApplyForSupplierAct_Click(object sender, RoutedEventArgs e)
        {
            var addSupplierFrm = new frmAddSupplier(_user, _userManager, _supplierManager, "Applying");
            var addSupplierResult = addSupplierFrm.ShowDialog();
            if (addSupplierResult == true)
            {
                MessageBox.Show("Application Submitted!");
            }
        }
    } // end of class
} // end of namespace 
