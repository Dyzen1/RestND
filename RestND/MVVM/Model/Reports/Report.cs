using System;

namespace RestND.MVVM.Model.Reports
{
    public class Report
    {
        #region Dish Name

        private string _DishName;

        public string DishName
        {
            get { return _DishName; }
            set { _DishName = value; }
        }

        #endregion

        #region Times Ordered

        private int _TimesOrdered;

        public int TimesOrdered
        {
            get { return _TimesOrdered; }
            set { _TimesOrdered = value; }
        }

        #endregion

        #region Total Revenue

        private double _TotalRevenue;

        public double TotalRevenue
        {
            get { return _TotalRevenue; }
            set { _TotalRevenue = value; }
        }

        #endregion

        #region Constructor

        public Report(string dishName, int timesOrdered, double totalRevenue)
        {
            this.DishName = dishName;
            this.TimesOrdered = timesOrdered;
            this.TotalRevenue = totalRevenue;
        }
        #endregion

        #region Constructor without parameters

        public Report()
        {

        }
        #endregion


    }
}