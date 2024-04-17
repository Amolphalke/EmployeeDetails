using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeDetails.Models
{
    public class Employee
    {

        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Language { get; set; }
        [Required]
        public string gender { get; set; }
        [Required]
        public string MobileNumber { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public decimal MonthlySalary { get; set; }
        [Required]
        public decimal AnnualSalary { get; set; }
        [Required]
        public string? ResumePath { get; set; }
        [Required]
        public int City_id { get; set; }

        public City? City { get; set; }
    }
}
