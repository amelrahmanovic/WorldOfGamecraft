using CombatService.Models;

namespace CombatService.DataAccessObject
{
    public class ApplicationUserDAO
    {
        private AppDbContex _contex;
        public ApplicationUserDAO(AppDbContex contex)
        {
            _contex = contex;
        }
        public List<ApplicationUser> GetAll()
        {
            return _contex.ApplicationUser.ToList();
        }
        public void Save(ApplicationUser applicationUser)
        {
            if(GetById(applicationUser.Id)==null)
            {
                _contex.ApplicationUser.Add(applicationUser);
                _contex.SaveChanges();
            }
        }
        public ApplicationUser? GetById(string id)
        {
            return _contex.ApplicationUser.SingleOrDefault(x => x.Id == id);
        }
    }
}
