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
    [Table("GmailConfig")]
    public class GmailConfigDb : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long GmailConfigId { get; set; }

        [Required]
        public string GmailId { get; set; }

        [Required]
        public string GmailPassword { get; set; }

    }
}
