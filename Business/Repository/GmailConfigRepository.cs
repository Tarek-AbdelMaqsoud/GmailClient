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
    public class GmailConfigRepository : IRepository.IGmailConfigRepository
    {
        private readonly GmailClientContext _dbContext = new Data.GmailClientContext();

        public IEnumerable<GmailConfigDb> GetAll()
        {
            return _dbContext.GmailConfigs.ToList();
        }
        public GmailConfigDb Get(int ID)
        {
            return _dbContext.GmailConfigs.Where(gmailConfig => gmailConfig.GmailConfigId == ID).FirstOrDefault();
        }
        public GmailConfigDb Add(GmailConfigDb gmailConfig)
        {
            var dc = new GmailClientContext();
            GmailConfigDb j = new GmailConfigDb();
            j.GmailId = gmailConfig.GmailId;
            j.GmailPassword = gmailConfig.GmailPassword;

            dc.GmailConfigs.Add(j);

            return (dc.SaveChanges() > 0) ? j : gmailConfig;
        }
        public GmailConfigDb Get(string GmailId)
        {
            return _dbContext.GmailConfigs.Where(gmailConfig => gmailConfig.GmailId.ToLower() == GmailId.ToLower()).FirstOrDefault();
        }
        public bool Delete(GmailConfigDb gmail)
        {
            var dc = new GmailClientContext();
            dc.GmailConfigs.Remove(gmail);

            return dc.SaveChanges() > 0;
        }
        public GmailConfigDb Modify(GmailConfigDb gmail)
        {
            var dc = new GmailClientContext();

            dc.Entry(gmail).State = System.Data.Entity.EntityState.Modified;
            dc.SaveChanges();

            return gmail;
        }
    }
}
