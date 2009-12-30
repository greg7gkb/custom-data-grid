using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CustomDataGrid
{
    public partial class MainForm : Form
    {
        private DataSet myDataSet;

        public MainForm()
        {
            InitializeComponent();

            MakeDataSet();
            SetUpDataGrids();
            //AddCustomDataTableStyle(dgExample);
        }

        // Create a DataSet with two tables and populate it
        private void MakeDataSet()
        {
            #region Code from: http://msdn.microsoft.com/en-us/library/system.windows.forms.datagrid.aspx
            // Create a DataSet.
            myDataSet = new DataSet("myDataSet");

            // Create two DataTables.
            DataTable tCust = new DataTable("Customers");
            DataTable tOrders = new DataTable("Orders");

            // Create two columns, and add them to the first table.
            DataColumn cCustID = new DataColumn("CustID", typeof(int));
            DataColumn cCustName = new DataColumn("CustName");
            DataColumn cCurrent = new DataColumn("Current", typeof(bool));
            tCust.Columns.Add(cCustID);
            tCust.Columns.Add(cCustName);
            tCust.Columns.Add(cCurrent);

            // Create three columns, and add them to the second table.
            DataColumn cID =
            new DataColumn("CustID", typeof(int));
            DataColumn cOrderDate =
            new DataColumn("orderDate", typeof(DateTime));
            DataColumn cOrderAmount =
            new DataColumn("OrderAmount", typeof(decimal));
            tOrders.Columns.Add(cOrderAmount);
            tOrders.Columns.Add(cID);
            tOrders.Columns.Add(cOrderDate);

            // Add the tables to the DataSet.
            myDataSet.Tables.Add(tCust);
            myDataSet.Tables.Add(tOrders);

            // Create a DataRelation, and add it to the DataSet.
            DataRelation dr = new DataRelation
            ("custToOrders", cCustID, cID);
            myDataSet.Relations.Add(dr);

            /* Populates the tables. For each customer and order, 
            creates two DataRow variables. */
            DataRow newRow1;
            DataRow newRow2;

            // Create three customers in the Customers Table.
            const int numItems = 30;
            for (int i = 1; i <= numItems; i++)
            {
                newRow1 = tCust.NewRow();
                newRow1["custID"] = i;
                // Add the row to the Customers table.
                tCust.Rows.Add(newRow1);
            }

            // Give each customer a distinct name.
            tCust.Rows[0]["custName"] = "Customer01";
            tCust.Rows[1]["custName"] = "Customer02";
            tCust.Rows[2]["custName"] = "Customer03";
            tCust.Rows[3]["custName"] = "Customer04";
            tCust.Rows[4]["custName"] = "Customer05";
            tCust.Rows[5]["custName"] = "Customer06";
            tCust.Rows[6]["custName"] = "Customer07";
            tCust.Rows[7]["custName"] = "Customer08";
            tCust.Rows[8]["custName"] = "Customer09";
            tCust.Rows[9]["custName"] = "Customer10";

            // Give the Current column a value.
            tCust.Rows[0]["Current"] = true;
            tCust.Rows[1]["Current"] = true;
            tCust.Rows[2]["Current"] = false;
            tCust.Rows[3]["Current"] = false;
            tCust.Rows[4]["Current"] = true;
            tCust.Rows[5]["Current"] = false;
            tCust.Rows[6]["Current"] = true;
            tCust.Rows[7]["Current"] = true;
            tCust.Rows[8]["Current"] = true;
            tCust.Rows[9]["Current"] = false;

            // For each customer, create five rows in the Orders table.
            for (int i = 1; i <= numItems; i++)
            {
                for (int j = 1; j < 6; j++)
                {
                    newRow2 = tOrders.NewRow();
                    newRow2["CustID"] = i;
                    newRow2["orderDate"] = new DateTime(2001, Math.Max(i % 12, 1), j * 2);
                    newRow2["OrderAmount"] = i * 10 + j * .1;
                    // Add the row to the Orders table.
                    tOrders.Rows.Add(newRow2);
                }
            }
            #endregion
        }

        private void SetUpDataGrids()
        {
            dgExample.CaptionBackColor = dgCustom.CaptionBackColor = Color.Blue;
            dgExample.CaptionFont = dgCustom.CaptionFont = new Font(
                dgExample.Font.FontFamily, dgExample.Font.SizeInPoints + 2, FontStyle.Regular);
            dgExample.CaptionForeColor = dgCustom.CaptionForeColor = Color.White;
            dgExample.ReadOnly = dgCustom.ReadOnly = true;
            dgExample.RowHeadersVisible = dgCustom.RowHeadersVisible = true;

            dgExample.CaptionText = ".NET DataGrid Control (Read Only)";
            dgCustom.CaptionText = "CustomDataGrid Control (Read Only)";

            // Bind data to DGs
            dgExample.SetDataBinding(myDataSet, "Customers");
            dgCustom.SetDataBinding(myDataSet, "Customers");
        }

        private void AddCustomDataTableStyle(DataGrid dg)
        {
            #region Code from: http://msdn.microsoft.com/en-us/library/system.windows.forms.datagrid.aspx
            DataGridTableStyle ts1 = new DataGridTableStyle();
            ts1.MappingName = "Customers";
            // Set other properties.
            ts1.AlternatingBackColor = Color.LightGray;

            /* Add a GridColumnStyle and set its MappingName 
            to the name of a DataColumn in the DataTable. 
            Set the HeaderText and Width properties. */

            DataGridColumnStyle boolCol = new DataGridBoolColumn();
            boolCol.MappingName = "Current";
            boolCol.HeaderText = "IsCurrent Customer";
            boolCol.Width = 150;
            ts1.GridColumnStyles.Add(boolCol);

            // Add a second column style.
            DataGridColumnStyle TextCol = new DataGridTextBoxColumn();
            TextCol.MappingName = "custName";
            TextCol.HeaderText = "Customer Name";
            TextCol.Width = 250;
            ts1.GridColumnStyles.Add(TextCol);

            // Create the second table style with columns.
            DataGridTableStyle ts2 = new DataGridTableStyle();
            ts2.MappingName = "Orders";

            // Set other properties.
            ts2.AlternatingBackColor = Color.LightBlue;

            // Create new ColumnStyle objects
            DataGridColumnStyle cOrderDate =
            new DataGridTextBoxColumn();
            cOrderDate.MappingName = "OrderDate";
            cOrderDate.HeaderText = "Order Date";
            cOrderDate.Width = 100;
            ts2.GridColumnStyles.Add(cOrderDate);

            /* Use a PropertyDescriptor to create a formatted
            column. First get the PropertyDescriptorCollection
            for the data source and data member. */
            PropertyDescriptorCollection pcol = this.BindingContext
            [myDataSet, "Customers.custToOrders"].GetItemProperties();

            /* Create a formatted column using a PropertyDescriptor.
            The formatting character "c" specifies a currency format. */
            DataGridColumnStyle csOrderAmount =
            new DataGridTextBoxColumn(pcol["OrderAmount"], "c", true);
            csOrderAmount.MappingName = "OrderAmount";
            csOrderAmount.HeaderText = "Total";
            csOrderAmount.Width = 100;
            ts2.GridColumnStyles.Add(csOrderAmount);

            /* Add the DataGridTableStyle instances to 
            the GridTableStylesCollection. */
            dg.TableStyles.Add(ts1);
            dg.TableStyles.Add(ts2);
            #endregion
        }
    }
}
