using EmployeesApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesApp.Application.Interfaces
{
    public interface IRepository
    {
        IEnumerable<Employee> GetAll();
        Employee Get(int id);
        Task<Employee> GetAsync(int id);
        IEnumerable<Employee> Find(Func<Employee, Boolean> predicate);
        public Task CreateRangeAsync(IEnumerable<Employee> employees);
        void Update(Employee employee);
        public void Save();
        public Task SaveAsync();
    }
}
