//using RestND.MVVM.Model.Employees;
//using RestND.MVVM.Model;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Mysqlx.Crud;
//using MySql.Data.MySqlClient;
//using System.Windows.Documents;

//namespace RestND.Data
//{
//    {
//                        Table_Number = Convert.ToInt32(row["Table_Number"])
//}
//                });

//            }

//            return orders;
//        }
//        #endregion

//        #region Add Employee
//        public override bool Add(Employee e)
//        {
//            string query = "INSERT INTO Employee (Employee_ID, Employee_Name, Employee_Role) VALUES (@id, @name, @role)";
//            return _db.ExecuteNonQuery(query,
//                new MySqlParameter("@id", e.Employee_ID),
//                new MySqlParameter("@role", e.Employee_Role),
//                new MySqlParameter("@name", e.Employee_Name)) > 0;
//        }
//        #endregion

//        #region Update Product
//        public override bool Update(Employee e)
//        {
//            string query = "UPDATE Employee SET Employee_Name = @name, Employee_ID = @id, Employee_Role = @role WHERE Employee_ID = @id";
//            return _db.ExecuteNonQuery(query,
//                new MySqlParameter("@name", e.Employee_Name),
//                new MySqlParameter("@role", e.Employee_Role),
//                new MySqlParameter("@id", e.Employee_ID)) > 0;
//        }
//        #endregion

//        #region Delete Product
//        public override bool Delete(int employeeID)
//        {
//            string query = "DELETE FROM Employee WHERE Employee_ID = @id";
//            return _db.ExecuteNonQuery(query, new MySqlParameter("@id", employeeID)) > 0;
//        }
//        #endregion
//    }
//}
