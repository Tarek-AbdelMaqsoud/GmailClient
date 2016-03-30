using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Model;
using IRepository;

namespace Repository
{
    public class UserRepository : IRepository.IUserRepository
    {
        private readonly GmailClientContext _dbContext = new Data.GmailClientContext();

        public IEnumerable<UsersDb> GetAll()
        {
            var users = _dbContext.Users.ToList();
            foreach (var user in users)
            {
                user.GmailConfig = _dbContext.GmailConfigs.Where(g=>g.GmailConfigId == user.GmailConfigId).FirstOrDefault();
            }
            return users;
        }
        public UsersDb Get(int ID)
        {
            var user = _dbContext.Users.Where(u => u.UsersId == ID).FirstOrDefault();
            user.GmailConfig = _dbContext.GmailConfigs.Where(g => g.GmailConfigId == user.GmailConfigId).FirstOrDefault();

            return user;
        }
        public UsersDb Get(string username, string password)
        {
            var dc = new GmailClientContext();
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var user = dc.Users.Where(x => x.Username.ToLower() == username && x.Password.ToLower() == password).FirstOrDefault();
                if (user != null)
                {
                    user.GmailConfig = _dbContext.GmailConfigs.Where(g => g.GmailConfigId == user.GmailConfigId).FirstOrDefault();
                }

                return user;
            }
            return null;
        }
        public UsersDb Add(UsersDb user)
        {

            var dc = new GmailClientContext();
            UsersDb j = new UsersDb();
            j.FirstName = user.FirstName;
            j.LastName = user.LastName;
            j.Username = user.Username;
            j.Password = user.Password;

            dc.Users.Add(j);

            return (dc.SaveChanges() > 0) ? j : user;
        }
        public bool Delete(UsersDb user)
        {
            var dc = new GmailClientContext();
            dc.Users.Remove(user);

            return dc.SaveChanges() > 0;
        }
        public UsersDb Modify(UsersDb user)
        {
            var dc = new GmailClientContext();

            dc.Entry(user).State = System.Data.Entity.EntityState.Modified;
            dc.SaveChanges();

            return user;
        }
    }
}
