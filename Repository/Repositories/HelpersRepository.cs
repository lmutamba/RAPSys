using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using RapModel.Model;
using RapModel.ViewModel;

namespace Repository
{
    public class HelpersRepository
    {
        private readonly PersonViewModel Person = new PersonViewModel();
        private RAPSystemEntities db = new RAPSystemEntities();

        public HelpersRepository()
        {
            db.Database.Connection.Open();
        }

        public PersonViewModel GetLogInPerson()
        {
            string login = HttpContext.Current.User.Identity.Name;
            login = login.Split('\\')[1] + "@tfm.cmoc.com";
            var person = db.T_Person.FirstOrDefault(p => p.Email == login);
            var PersonView = new PersonViewModel() { 
                EmployeeId = person.T_Employee.FirstOrDefault().EmployeeId,
                FirstName = person.FirstName,
                LastName = person.LastName,
                MiddleName = person.MiddleName
            };

            return PersonView;// person;
        }
    }
}
