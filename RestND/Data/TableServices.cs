using MySql.Data.MySqlClient;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Tables;
using System;
using System.Collections.Generic;

namespace RestND.Data
{
    public class TableServices() : BaseService<Table>(DatabaseOperations.Instance)
    {
        #region Get All Tables
        public override List<Table> GetAll()
        {
            List<Table> tables = new List<Table>();
            string query = "SELECT * FROM `tables`";

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
         string query = "INSERT INTO `tables` (Table_ID, Table_Number ,Y , X) VALUES (@id,@tablenum ,@y , @x)";

            return _db.ExecuteNonQuery(query,
                        //new MySqlParameter("@id", t.Table_ID),
                        new MySqlParameter("@tablenum", t.Table_Number),
                        new MySqlParameter("@y", t.Y)  ,
                        new MySqlParameter("@x", t.X)) > 0;

        }

        #endregion

        #region Update Table
        public override bool Update(Table t){
            if(t.Table_Status != true) return false;

            string query = "UPDATE `tables` SET Table_Number = @tablenum , Y = @y , X = @x WHERE Table_ID = @id";

            return _db.ExecuteNonQuery(query,
                        new MySqlParameter("@id", t.Table_ID),
                        new MySqlParameter("@tablenum", t.Table_Number),
                        new MySqlParameter("@y", t.Y),
                        new MySqlParameter("@x", t.X)) > 0;


        }
        #endregion

        #region Delete Table
        public override bool Delete(Table d){

            if(d.Table_ID <= 0) return false;

            string query = "DELETE FROM `tables` WHERE Table_ID = @id";
            return _db.ExecuteNonQuery(query, new MySqlParameter("@id", d.Table_ID)) > 0;
        }
        #endregion

    }

}
