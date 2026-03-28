using System.ComponentModel.DataAnnotations;

namespace MyFirstApi.Entities
{
    public class Employee
    {
        [Key]
        public Guid Id { get; set; } 
        public string? Name { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateOnly? DOB { get; set; }
        public string? Position { get; set; }
        public string? Department { get; set; }
        public string? EmailAddress { get; set; }

    }
}
