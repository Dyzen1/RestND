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

        private string _Table_ID;

        public string Table_ID
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

        #region Table Status

        private bool _Table_Status;

        public bool Table_Status
        {
            get { return _Table_Status; }
            set { _Table_Status = value; }
        }

        #endregion

        #region Constructor

        public Table(int Table_Num)

        {
            Table_Number = Table_Num;
            Table_Status = true;
        }
        #endregion

        #region Default Constructor

        public Table()
        {
            
        }

        #endregion


    }
}
