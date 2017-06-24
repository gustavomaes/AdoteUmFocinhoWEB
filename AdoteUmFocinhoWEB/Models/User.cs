using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AdoteUmFocinhoWEB.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        
        [Index("Login", Order = -1)]
        public string Email { get; set; }

        [Index("Login", Order = -1)]
        [JsonIgnore]
        public string Password { get; set; }

        [Index("Login", Order = -1)]
        public bool Block { get; set; }

        public string IdSocial { get; set; }

        public DateTime Date { get; set; }
    }
}