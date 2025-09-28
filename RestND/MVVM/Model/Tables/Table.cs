using System;

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

        #region Maximum table diners

        private int _Max_Diners;
        public int Max_Diners
        {
            get => _Max_Diners;
            set => _Max_Diners = value;
        }
        #endregion

        #region Columns (C)
        private int _C;
        public int C
        {
            get { return _C; }
            set { _C = value; }
        }
        #endregion

        #region Rows (R)
        private int _R;
        public int R
        {
            get { return _R; }
            set { _R = value; }
        }
        #endregion

        #region Static Table Count
        public static int Table_Count = 0;
        #endregion

        #region Max Table Number
        public const int MAX_TABLE_NUMBER = 25;
        #endregion

        #region Table Status (e.g. is it currently occupied?)
        private bool _Table_Status;
        public bool Table_Status
        {
            get { return _Table_Status; }
            set { _Table_Status = value; }
        }
        #endregion

        #region Is Active - was this table added by the user
        private bool _Is_Active;
        public bool Is_Active
        {
            get { return _Is_Active; }
            set { _Is_Active = value; }
        }
        #endregion

        #region Constructor
        public Table(int tableNumber, int c, int r)
        {
            this.C = c;
            this.R = r;
            this.Table_Number = tableNumber;
            this.Table_Status = true;
            this.Max_Diners = 2;
            this.Is_Active = false;
        }
        #endregion

        #region Default Constructor
        public Table()
        {
            this.Max_Diners = 2;
            this.Is_Active = false;
        }
        #endregion
    }
}
