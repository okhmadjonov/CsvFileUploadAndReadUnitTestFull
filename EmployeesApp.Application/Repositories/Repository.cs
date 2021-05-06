using EmployeesApp.Application.Interfaces;
using EmployeesApp.Domain.Models;
using EmployeesApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesApp.Application.Repositories
{
    public class Repository : IRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Employee> GetAll()
        {
            return _dbContext.Employees;
        }

        public Employee Get(int id)
        {
            return _dbContext.Employees.Find(id);
        }

        public async Task<Employee> GetAsync(int id)
        {
            return await _dbContext.Employees.FindAsync(id);
        }

        public IEnumerable<Employee> Find(Func<Employee, bool> predicate)
        {
            return _dbContext.Employees.Where(predicate).ToList();
        }

        public async Task CreateRangeAsync(IEnumerable<Employee> employees)
        {
            await _dbContext.Employees.AddRangeAsync(employees);
        }

        public void Update(Employee employee)
        {
            _dbContext.Entry(employee).State = EntityState.Modified;
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
