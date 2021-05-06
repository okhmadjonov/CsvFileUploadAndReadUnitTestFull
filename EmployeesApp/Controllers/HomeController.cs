using EmployeesApp.Application.Abstract;
using EmployeesApp.Application.Interfaces;
using EmployeesApp.Domain.Models;
using EmployeesApp.Infrastructure;
using EmployeesApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeesApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRepository _repo;
        private readonly ILoadCSVService _loadCSVService;

        public HomeController(
            ILogger<HomeController> logger,
            IRepository repo,
            ILoadCSVService loadCSVService
        )
        {
            _logger = logger;
            _repo = repo;
            _loadCSVService = loadCSVService;
        }

        public IActionResult Index()
        {
            var employees = _repo.GetAll().OrderBy(e => e.Surname).ToList();
            return View(employees);
        }

        [HttpPost]
        public async Task<JsonResult> LoadCSV([FromForm] IFormFile file)
        {
            int addedRows = 0;
            using (var streamReader = new StreamReader(file.OpenReadStream()))
            {
                 addedRows = await _loadCSVService.LoadFromCSVAsync(streamReader);
            }

            return Json(addedRows);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id.HasValue)
            {
                var employee = await _repo.GetAsync(id.Value);
                if (employee != null)
                    return View(employee);

                return NotFound();
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Employee employee)
        {
            _repo.Update(employee);
            await _repo.SaveAsync();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
