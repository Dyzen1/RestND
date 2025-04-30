using MySql.Data.MySqlClient;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Tables;
using System;
using System.Collections.Generic;

namespace RestND.Data
{

<<<<<<< HEAD
    public class TableServices : BaseService<Table>
    {   
        #region Constructor
        public TableServices() : base(new DatabaseOperations("127.0.0.1", "restnd", "root", "D123456N!")) { }
        #endregion
        
=======
    public class TableServices() : BaseService<Table>(DatabaseOperations.Instance)
    {

>>>>>>> 5ead172fc20c05b8cefaf649f8c4749d4aebdaca
        #region Get All Tables
        public override List<Table> GetAll()
        {
            List<Table> tables = new List<Table>();
            string query = "SELECT * FROM 'table'";

            var rows = _db.ExecuteReader(query);

            foreach (var row in rows)
            {
                tables.Add(new Table
                {
                    Table_ID = Convert.ToInt32(row["Table_ID"]),
                    Table_Number = Convert.ToInt32(row["Table_Number"]),
                    Table_Status = Convert.ToBoolean(row["Table_Status"])
                });
            }

            return tables;
        }
        #endregion

        #region Add Table
        public  override bool Add(Table t){
         string query = "INSERT INTO table (Table_ID, Table_Number ,Y , X) VALUES (@id,@tablenum ,@y , @x)";

            return _db.ExecuteNonQuery(query,
                        new MySqlParameter("@id", t.Table_ID),
                        new MySqlParameter("@tablenum", t.Table_Number),
                        new MySqlParameter("@y", t.Y)  ,
                        new MySqlParameter("@x", t.X)) > 0;

        }
        #endregion

        #region Update Table
        public override bool Update(Table t){
            if(t.Table_Status != true) return false;

            string query = "UPDATE table SET Table_Number =@tablenum , Y = @y , X = @x WHERE Table_ID = @id";

            return _db.ExecuteNonQuery(query,
                        new MySqlParameter("@id", t.Table_ID),
                        new MySqlParameter("@tablenum", t.Table_Number),
                        new MySqlParameter("@y", t.Y),
                        new MySqlParameter("@x", t.X)) > 0;


        }
        #endregion

        #region Delete Table
        public override bool Delete(int tableId){

            string query = "DELETE FROM table WHERE Table_ID = @id";
            return _db.ExecuteNonQuery(query, new MySqlParameter("@id", tableId)) > 0;
        }
        #endregion


    }

}
