using RestND.Data;
using System.Collections.Generic;

namespace RestND.MVVM.Model
{
    public abstract class BaseService<T> : CrudServices<T>
    {
        protected readonly DatabaseOperations _db;

        public BaseService(DatabaseOperations db)
        {
            _db = db;
        }

        public abstract List<T> GetAll();
        public abstract bool Add(T item);
        public abstract bool Update(T item);
        public abstract bool Delete(int id);
    }
}