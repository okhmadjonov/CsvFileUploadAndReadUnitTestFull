using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesApp.Application.Abstract
{
    public interface ILoadCSVService
    {
        Task<int> LoadFromCSVAsync(StreamReader stream);
    }
}
