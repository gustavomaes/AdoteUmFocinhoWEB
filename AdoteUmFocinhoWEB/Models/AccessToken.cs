using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AdoteUmFocinhoWEB.Models
{
    public class AccessToken
    {
        [Key]
        public string Token { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User UserData { get; set; }

        public DateTime Date { get; set; }

    }
}