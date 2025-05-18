using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Employees;
using RestND.MVVM.Model.Orders;

namespace RestND.Data
{
    public class BillServices() : BaseService<Bill>(DatabaseOperations.Instance)
    {

        #region Get All Bills
        public override List<Bill> GetAll()
        {
            var bills = new List<Bill>();
            string query = "SELECT * FROM bill";
            var rows = _db.ExecuteReader(query);

            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                var bill = new Bill();

                bill.Bill_ID = row["Bill_ID"].ToString();
                bill.Price = Convert.ToDouble(row["Price"]);
                //bill.Bill_Date = Convert.ToDateTime(row["Bill_Date"]);
                bill.Order.assignedEmployee.Employee_Name = row["Employee_Name"].ToString();

                var discount = new Discount();
                discount.Discount_ID = row["Discount_ID"]?.ToString() ?? "";
                bill.Discount = discount;

                var order = new Order();
                order.Order_ID = row["Order_ID"].ToString();
                bill.Order = order;

                bills.Add(bill);
            }

            return bills;
        }
        #endregion

        #region Add Bill
        public override bool Add(Bill b)
        {
            string query = "INSERT INTO bill (Order_ID, Price, Discount_ID, Bill_Date, Employee_Name) " +
                           "VALUES (@orderId, @price, @discountId, @billDate, @employeeName)";

            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@orderId", b.Order.Order_ID),
                new MySqlParameter("@price", b.Price),
                new MySqlParameter("@discountId", b.Discount?.Discount_ID ?? (object)DBNull.Value),
                //new MySqlParameter("@billDate", b.Bill_Date),
                new MySqlParameter("@employeeName", b.Order.assignedEmployee.Employee_Name)) > 0;
        }
        #endregion

        #region Update Bill
        public override bool Update(Bill b)
        {
            string query = "UPDATE bill SET Price = @price, Discount_ID = @discountId, Bill_Date = @billDate, Employee_Name = @employeeName WHERE Bill_ID = @billId";

            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@price", b.Price),
                new MySqlParameter("@discountId", b.Discount?.Discount_ID ?? (object)DBNull.Value),
                //new MySqlParameter("@billDate", b.Bill_Date),
                new MySqlParameter("@employeeName", b.Order.assignedEmployee.Employee_Name),
                new MySqlParameter("@billId", b.Bill_ID)) > 0;
        }
        #endregion

        #region Delete Bill
        public override bool Delete(int billId)
        {
            string query = "DELETE FROM bill WHERE Bill_ID = @id";
            return _db.ExecuteNonQuery(query, new MySqlParameter("@id", billId)) > 0;
        }
        #endregion
        
 
    }
}
