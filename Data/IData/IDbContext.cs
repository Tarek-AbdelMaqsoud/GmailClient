using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
namespace IData
{
    public interface IDbContext
    {
        IDbSet<UsersDb> Users { get; }
        IDbSet<GmailConfigDb> GmailConfigs { get; }
    }
}
