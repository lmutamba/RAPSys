using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string JobTitle { get; set; }
        public string Office { get; set; }
        public int EmployeeRoleId { get; set; }
        public int Role { get; set; }
        public string PersonType { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string DateOfBirthString { get; set; }
        public string PlaceOfBirth { get; set; }
        public int? Father { get; set; }
        public string FatherName { get; set; }
        public int? Mother { get; set; }
        public string MotherName { get; set; }
        public int VulnerabilityTypeId { get; set; }
        public string VulnerabilityDetail { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    }
}
