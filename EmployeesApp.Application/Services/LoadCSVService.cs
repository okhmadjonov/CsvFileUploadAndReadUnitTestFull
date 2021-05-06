using CsvHelper;
using CsvHelper.Configuration;
using EmployeesApp.Application.Abstract;
using EmployeesApp.Application.CsvMapper;
using EmployeesApp.Application.Interfaces;
using EmployeesApp.Domain.Models;
using EmployeesApp.Infrastructure;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesApp.Application.Services
{
    public class LoadCSVService : ILoadCSVService
    {
        private readonly IRepository _repo;

        public LoadCSVService(IRepository repo)
        {
            _repo = repo;
        }

        public async Task<int> LoadFromCSVAsync(StreamReader stream)
        {
            // this is for .csv formats
            var employees = GetEmployeesCSV(stream);

            // Add data and save database
            await _repo.CreateRangeAsync(employees);
            await _repo.SaveAsync();

            return employees.Count();

            // this is for .xlsx formats(Excel)
            //var employees = await GetEmployeesExcel(stream);
        }

        private List<Employee> GetEmployeesCSV(StreamReader stream)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
            };

            using var csv = new CsvReader(stream, config);

            csv.Context.RegisterClassMap<EmployeeMap>();

            var employees = csv.GetRecords<Employee>().ToList();

            return employees;
        }

        private async Task<List<Employee>> GetEmployeesExcel(Stream stream)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(stream);

            await package.LoadAsync(stream);

            var ws = package.Workbook.Worksheets[0];

            // check if the worksheet is completely empty
            if (ws.Dimension == null)
            {
                // return result
            }

            List<Employee> employees = new List<Employee>();

            // loop and get contents
            for (int i = 2; i <= ws.Dimension.End.Row; i++)
            {
                var row = ws.Cells[i, 1, i, ws.Dimension.End.Column];

                var col = 1;
                Employee employee = new Employee();

                // loop all cells in the row
                employee.PayrollNumber = row[i, col].Value.ToString();
                employee.ForeNames = row[i, col + 1].Value.ToString();
                employee.Surname = row[i, col + 2].Value.ToString();
                employee.DateOfBirth = DateTime.Parse(row[i, col + 3].Value.ToString());
                employee.Telephone = row[i, col + 4].Value.ToString();
                employee.Mobile = row[i, col + 5].Value.ToString();
                employee.Address = row[i, col + 6].Value.ToString();
                employee.Address2 = row[i, col + 7].Value.ToString();
                employee.Postcode = row[i, col + 8].Value.ToString();
                employee.EmailHome = row[i, col + 9].Value.ToString();
                employee.StartDate = DateTime.Parse(row[i, col + 10].Value.ToString());

                employees.Add(employee);
            }

            return employees;
        }
    }
}
