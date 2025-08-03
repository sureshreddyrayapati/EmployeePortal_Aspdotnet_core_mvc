namespace PracticeAspcoreMVC.Models
{
    public class Employee
    {
        public required int EmployeeId { get; set; }
        public required string EmployeeName { get; set; }
        public int EmployeeAge { get; set; }
        public DateTime JoingDate { get; set; }
        public required string Location { get; set; }
    }
}
