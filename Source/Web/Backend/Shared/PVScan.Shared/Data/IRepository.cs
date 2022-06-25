using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Shared.Data
{
    // Todo: cancellation tokens?
    public interface IRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);
        IQueryable<T> Query();
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(T entity);
    }
}
