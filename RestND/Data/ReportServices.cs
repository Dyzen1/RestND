using MySql.Data.MySqlClient;
using RestND.Data;
using System;
using System.Collections.Generic;

namespace RestND.Services
{
    public class ReportServices
    {
        private readonly DatabaseOperations _db = DatabaseOperations.Instance;

        public List<(string DishName, int QuantitySold)> GetDishSales(DateTime fromDate, DateTime toDate)
        {
            string query = @"
                SELECT d.Dish_Name, SUM(dio.Quantity) AS TotalSold
                FROM dishes_in_order dio
                JOIN dishes d ON d.Dish_ID = dio.Dish_ID
                JOIN orders o ON o.Order_ID = dio.Order_ID
                WHERE o.Order_Date BETWEEN @from AND @to
                GROUP BY d.Dish_Name;
            ";

            var results = new List<(string, int)>();
            var rows = _db.ExecuteReader(query,
                new MySqlParameter("@from", fromDate),
                new MySqlParameter("@to", toDate));

            foreach (var row in rows)
            {
                results.Add((row["Dish_Name"].ToString(), Convert.ToInt32(row["TotalSold"])));
            }

            return results;
        }
    }
}
