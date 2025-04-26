using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.Data
{
    public interface CrudServices<T>
    {
        List<T> GetAll();
        bool Add(T item);
        bool Update(T item);
        bool Delete(int id);


    }
}
