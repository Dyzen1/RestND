using MySql.Data.MySqlClient;
using RestND.MVVM.Model.Orders;
using RestND.MVVM.Model.VAT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.Data
{
    public class VATservices()
    {
        #region Get current VAT rate
        private readonly DatabaseOperations _db = DatabaseOperations.Instance;

        /// Returns the current VAT (a single row).
        public Vat Get()
        {
            const string sql = "SELECT Vat_ID, Percentage FROM vat WHERE Vat_ID = 1 LIMIT 1";
            var rows = _db.ExecuteReader(sql);

            var row = rows.FirstOrDefault();

            if (row != null)
            {
                return new Vat
                {
                    Vat_ID = Convert.ToInt32(row["Vat_ID"]),
                    Percentage = Convert.ToDouble(row["Percentage"])
                };
            }
            return null;
        }
        #endregion

        #region Update Vat rate
        // Update only the percentage on the single row.
        public bool UpdateRate(double newPercentage)
        {
            const string sql = "UPDATE vat SET Percentage = @p WHERE Vat_ID = 1";
            var parameters = new List<MySqlParameter>
                {
                    new MySqlParameter("@p", newPercentage)
                };

            var ok = _db.ExecuteNonQuery(sql, parameters.ToArray()) > 0;

            return ok;
        }
        #endregion
    }
}
