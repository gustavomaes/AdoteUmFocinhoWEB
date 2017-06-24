using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoteUmFocinhoWEB.Models
{
    public class UserToken
    {
        [Key]
        public int Id { get; set; }

        [Index]
        public int UserId { get; set; }

        public enum Plataform
        {
            Android = 1,
            iOS = 2
        }

        [Index("Search", 1)]
        public Plataform PlataformPush { get; set; }

        [Index("Search", 2)]
        [StringLength(100)]
        public string Token { get; set; }

        public DateTime Date { get; set; }

    }
}