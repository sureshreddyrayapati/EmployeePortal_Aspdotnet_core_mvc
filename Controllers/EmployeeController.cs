using Microsoft.AspNetCore.Mvc;
using PracticeAspcoreMVC.Models;
using System.ComponentModel;

namespace PracticeAspcoreMVC.Controllers
{
    public class EmployeeController : Controller
    {
        public static List<Employee> employeList;

        static EmployeeController()
        {
            employeList = new List<Employee>()
            {
                new Employee() { EmployeeId = 1, EmployeeName = "Suresh Reddy", EmployeeAge = 24, JoingDate = new DateTime(2025, 07, 27), Location = "Bengaluru" },
                new Employee() { EmployeeId = 2, EmployeeName = "Priya Sharma", EmployeeAge = 29, JoingDate = new DateTime(2024, 12, 10), Location = "Hyderabad" },
                new Employee() { EmployeeId = 3, EmployeeName = "Kiran Kumar", EmployeeAge = 31, JoingDate = new DateTime(2023, 03, 15), Location = "Chennai" },
                new Employee() { EmployeeId = 4, EmployeeName = "Meena Joshi", EmployeeAge = 27, JoingDate = new DateTime(2022, 09, 05), Location = "Pune" },
                new Employee() { EmployeeId = 5, EmployeeName = "Rahul Verma", EmployeeAge = 26, JoingDate = new DateTime(2025, 01, 01), Location = "Mumbai" },
            };
        }
        public IActionResult Index(string? searchTerm)
        {
            var list = employeList.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var normalized = searchTerm.Trim().ToLower();
                list = list.Where(e =>
                    (!string.IsNullOrEmpty(e.EmployeeName) && e.EmployeeName.ToLower().Contains(normalized)) ||
                    (!string.IsNullOrEmpty(e.Location) && e.Location.ToLower().Contains(normalized)) ||
                    e.EmployeeId.ToString() == normalized ||
                    e.EmployeeAge.ToString() == normalized
                );
                ViewData["CurrentFilter"] = searchTerm;
            }

            return View(list.ToList());
        }
        [HttpGet]
        public IActionResult create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.EmployeeId=employeList.Max(e=>e.EmployeeId)+1;
                employeList.Add(employee);
                return RedirectToAction("Index");
            }
            return View(employee);
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var emp=employeList.FirstOrDefault(e=>e.EmployeeId==id);
            if (emp == null)
            {
                return NotFound();
            }
            return View(emp);
        }
        [HttpPost , ActionName("Delete")]
        public IActionResult DeleteConfirmation(int id)
        {
            var emp=employeList.FirstOrDefault(e=>e.EmployeeId == id);
            if (emp != null)
            {
                employeList.Remove(emp);
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var emp = employeList.FirstOrDefault(e => e.EmployeeId == id);
            if (emp == null) return NotFound();
            return View(emp);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit( Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return View(employee);
            }
            var existing =employeList.FirstOrDefault(e=>e.EmployeeId==employee.EmployeeId);
            if (existing == null)
            {
                return RedirectToAction("Index");
            }

            existing.EmployeeName = employee.EmployeeName;
            existing.EmployeeAge = employee.EmployeeAge;
            existing.JoingDate = employee.JoingDate;
            existing.Location = employee.Location;

            return RedirectToAction("Index");
        }
    }
}
