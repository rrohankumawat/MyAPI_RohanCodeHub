using System.ComponentModel.DataAnnotations;

namespace MyFirstApi.Entities
{
    public class Salary
    {
        [Key]
        public int SalaryBandId { get; set; }
        public decimal MinSalary { get; set; }
        public decimal MaxSalary
        {
            get; set;

        }
    }
}
