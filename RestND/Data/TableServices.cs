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
            string query = "SELECT * FROM `tables` WHERE Is_Active = true";

            var rows = _db.ExecuteReader(query);

            foreach (var row in rows)
            {
                tables.Add(new Table
                {
                    Table_ID = Convert.ToInt32(row["Table_ID"]),
                    Table_Number = Convert.ToInt32(row["Table_Number"]),
                    C = Convert.ToInt32(row["C"]),
                    R = Convert.ToInt32(row["R"]),
                    Table_Status = Convert.ToBoolean(row["Table_Status"]),
                    Is_Active = Convert.ToBoolean(row["Is_Active"])
                });
            }

            return tables;
        }
        #endregion

        #region Add Table
        public override bool Add(Table t)
        {
            string query = "INSERT INTO `tables` (Table_Number, C, R, Table_Status, Is_Active) VALUES (@tablenum, @c, @r, @status, @active)";
            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@tablenum", t.Table_Number),
                new MySqlParameter("@c", t.C),
                new MySqlParameter("@r", t.R),
                new MySqlParameter("@status", t.Table_Status),
                new MySqlParameter("@active", t.Is_Active)) > 0;
        }
        #endregion

        #region Update Table
        public override bool Update(Table t)
        {
            string query = "UPDATE `tables` SET Table_Number = @tablenum, C = @c, R = @r, Table_Status = @status, Is_Active = @active WHERE Table_ID = @id";
            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@id", t.Table_ID),
                new MySqlParameter("@tablenum", t.Table_Number),
                new MySqlParameter("@c", t.C),
                new MySqlParameter("@r", t.R),
                new MySqlParameter("@status", t.Table_Status),
                new MySqlParameter("@active", t.Is_Active)) > 0;
        }
        #endregion

        #region Delete Table (Soft Delete)
        public override bool Delete(Table table)
        {
                
            

            string query = "UPDATE `tables` SET Is_Active = false WHERE Table_ID = @id";
            return _db.ExecuteNonQuery(query, new MySqlParameter("@id", table.Table_ID)) > 0;
        }
        #endregion
    }
}
