using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using EmployeesApp.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EmployeesApp.Application.CsvMapper
{
    public class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap()
        {
            Map(prop => prop.PayrollNumber).Index(0);
            Map(prop => prop.ForeNames).Index(1);
            Map(prop => prop.Surname).Index(2);
            Map(prop => prop.DateOfBirth).TypeConverter<CustomDateTimeConverter>().Index(3);
            Map(prop => prop.Telephone).Index(4);
            Map(prop => prop.Mobile).Index(5);
            Map(prop => prop.Address).Index(6);
            Map(prop => prop.Address2).Index(7);
            Map(prop => prop.Postcode).Index(8);
            Map(prop => prop.EmailHome).Index(9);
            Map(prop => prop.StartDate).TypeConverter<CustomDateTimeConverter>().Index(10);
        }
    }

    public class CustomDateTimeConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string value, IReaderRow row, MemberMapData memberMapData)
        {
            CultureInfo enUS = new CultureInfo("en-US");

            var formatStrings = new string[] { "dd/MM/yyyy", "dd/M/yyyy" };
            if (DateTime.TryParseExact(value, formatStrings, enUS, DateTimeStyles.None, out var dateValue))
                return dateValue;

            return new DateTime();
        }
    }
}
