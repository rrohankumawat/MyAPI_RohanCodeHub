using Microsoft.EntityFrameworkCore;
using MyFirstApi.Data;
using MyFirstApi.Dto;
using MyFirstApi.IService;

namespace MyFirstApi.Services
{
    public class EmployeeService(AppDbContext _context) : IEmployeeService
    {
        public async Task<Tuple<int, List<EmployeeDto>>> GetAllEmployeeAsync()
        {
            try
            {
                return new Tuple<int, List<EmployeeDto>>(1, await _context.Employees.AsNoTracking().Select(x => new EmployeeDto
                {
                    Id = x.Id,
                    CreatedDate = x.CreatedDate,
                    LastModifiedDate = x.LastModifiedDate,
                    Department = x.Department,
                    DOB = x.DOB,
                    Name = x.Name,
                    EmailAddress = x.EmailAddress,
                    Position = x.Position
                }).ToListAsync());

            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<Tuple<int, string>> CreateEmployee(EmployeeDto employee)
        {
            var existing = await _context.Employees.AnyAsync(x => x.EmailAddress == employee.EmailAddress);

            if (existing)
            {
                return new Tuple<int, string>(0, "Employee Already exist With same Email Id");
            }

            await _context.Employees.AddAsync(new Entities.Employee
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                LastModifiedDate = null,
                Department = employee.Department,
                DOB = employee.DOB,
                Name = employee.Name,
                EmailAddress = employee.EmailAddress,
                Position = employee.Position
            });

            await _context.SaveChangesAsync();

            return new Tuple<int, string>(1, "employee Created Successfully!");
        }



        public async Task<Tuple<int, string>> UpdateEmployee(EmployeeDto employee)
        {
            if (employee == null)
            {
                return new Tuple<int, string>(0, "Please fill All The Details");
            }
            var existing = await _context.Employees.FirstOrDefaultAsync(x => x.EmailAddress == employee.EmailAddress);

            if (existing == null)
            {
                return new Tuple<int, string>(0, "Employee Not Exist With This Email Id");
            }

            existing.Position = string.IsNullOrWhiteSpace(employee.Position) ? existing.Position : employee.Position;
            existing.DOB = employee.DOB ?? existing.DOB;
            existing.Name = string.IsNullOrWhiteSpace(employee.Name) ? existing.Name : employee.Name;
            existing.EmailAddress = string.IsNullOrWhiteSpace(employee.EmailAddress) ? existing.EmailAddress : employee.EmailAddress;
            existing.Department = string.IsNullOrWhiteSpace(employee.Department) ? existing.Department : employee.Department;

            _context.Employees.Update(existing);
            await _context.SaveChangesAsync();

            return new Tuple<int, string>(1, "employee Updated Successfully!");
        }


        public async Task<Tuple<int, string>> DeleteEmployee(Guid id)
        {
            var data = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);

            if (data == null)
            {
                return new Tuple<int, string>(0, "Employee Not Exist With This Id");
            }


            _context.Employees.Remove(data);
            await _context.SaveChangesAsync();

            return new Tuple<int, string>(0, "Employee Deleted Successfully!");
        }


        public async Task<Tuple<int, EmployeeDto>> GetEmployeeById(Guid id)
        {
            var data = await _context.Employees.AsNoTracking().Select(x => new EmployeeDto
            {
                Id = x.Id,
                CreatedDate = x.CreatedDate,
                LastModifiedDate = x.LastModifiedDate,
                Department = x.Department,
                DOB = x.DOB,
                Name = x.Name,
                EmailAddress = x.EmailAddress,
                Position = x.Position
            }).FirstOrDefaultAsync(x => x.Id == id);

            if (data == null)
            {
                return new Tuple<int, EmployeeDto>(0, null);
            }
            return new Tuple<int, EmployeeDto>(1, data);
        }
    }
}
