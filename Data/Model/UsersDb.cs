using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Main;

namespace Model
{
    [Table("Users")]
    public class UsersDb : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UsersId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public Nullable<long> GmailConfigId { get; set; }

        [ForeignKey("GmailConfigId")]
        public GmailConfigDb GmailConfig { get; set; }
    }
}
