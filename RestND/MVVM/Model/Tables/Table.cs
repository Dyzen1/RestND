using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model.Tables
{
    public class Table
    {
        #region Table ID

        private int _Table_ID;

        public int Table_ID
        {
            get { return _Table_ID; }
            set { _Table_ID = value; }
        }

        #endregion
        
        #region Table Number

        private int _Table_Number;

        public int Table_Number
        {
             get { return _Table_Number; }
            set { _Table_Number = value; }
        }

        #endregion

        #region X coordinates

        private double _X;

        public double X
        {
            get { return X; }
            set { _X = value; }
        }


        #endregion

        #region Y coordinates
        private double _Y;
        public double Y
        {
            get { return Y; }
            set { _Y = value; }
        }
        #endregion

        #region Table Status - for tables in order

        private bool _Table_Status;

        public bool Table_Status
        {
            get { return _Table_Status; }
            set { _Table_Status = value; }
        }

        #endregion

        #region Is Active - indicates wheather a table had beed added by user 

        private bool _Is_Active;

        public bool Is_Active
        {
            get { return _Is_Active; }
            set { _Is_Active = value; }
        }

        #endregion

        #region Constructor for a new table
        public Table(int Table_Num,double x, double y)
        {
            this.X = x;
            this.Y = y;
            this.Table_Number = Table_Num;
            this.Table_Status = true;
            this.Is_Active = false;
        }

        #endregion

        #region Default Constructor

        public Table()
        {
            
        }

        #endregion

    }
}
