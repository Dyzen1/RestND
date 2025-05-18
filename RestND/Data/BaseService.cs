using RestND.Data;
using System.Collections.Generic;

namespace RestND.MVVM.Model
{
    public abstract class BaseService<T> : CrudServices<T>
    {
        protected readonly DatabaseOperations _db;
        #region VAT
        private double _Vat;
        public double Vat
        {
            get { return _Vat; }
            set { _Vat = value; }
        }
        #endregion

        public BaseService(DatabaseOperations db)
        {
            _db = db;
            this.Vat = 0.18;
        }

        public abstract List<T> GetAll();
        public abstract bool Add(T item);
        public abstract bool Update(T item);
        public abstract bool Delete(int id);
        

    }
}