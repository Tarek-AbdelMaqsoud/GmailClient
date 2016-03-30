using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        T Get(int ID);
        T Add(T entity);
        bool Delete(T entity);
        T Modify(T entity);
    }
}
