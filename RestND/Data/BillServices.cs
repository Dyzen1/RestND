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

            foreach (var row in rows)
            {
                if (row.TryGetValue("Is_Active", out var isActive) && Convert.ToBoolean(isActive) == false)
                    continue; // Skip inactive bills
                var bill = new Bill
                {
                    Bill_ID = Convert.ToInt32(row["Bill_ID"]),
                    Price = Convert.ToDouble(row["Price"]),
                    Bill_Date = Convert.ToDateTime(row["Bill_Date"]),
                    Order = new Order
                    {
                        Order_ID = Convert.ToInt32(row["Order_ID"]),
                        assignedEmployee = new Employee
                        {
                            Employee_Name = row["Employee_Name"].ToString()
                        }
                    },
                    Discount = new Discount
                    {
                        Discount_ID = Convert.ToInt32(row["Discount_ID"] ?? "")
                    }
                };
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

        #region Delete Bill (not really deleting, just marking as inactive)
        public override bool Delete(Bill bill)
        {
            bill.Is_Active = false;
            string query = "UPDATE bill SET Is_Active = @active WHERE Bill_ID = @id";
            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@active", bill.Is_Active),
                new MySqlParameter("@id", bill.Bill_ID)) > 0;
        }
        #endregion
        
 
    }
}
