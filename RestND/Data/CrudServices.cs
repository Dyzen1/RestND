using System.Collections.Generic;

namespace RestND.Data
{
    public interface CrudServices<T>
    {
        //T = dynamic type
        List<T> GetAll();
        bool Add(T item);
        bool Update(T item);
        bool Delete(T item);
    }
}
