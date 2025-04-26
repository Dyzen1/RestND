using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model
{
    public class Table
    {
        #region Table Number

        private int _Table_ID;

        public int Table_ID
        {
            get { return _Table_ID; }
            set { _Table_ID = value; }
        }

        #endregion

        #region X cordinates

        private double _X;

        public double X
        {
            get { return X; }
            set { _X = value; }
        }


        #endregion

        #region Y cordinates
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
            this.Table_ID = Table_Num;
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
