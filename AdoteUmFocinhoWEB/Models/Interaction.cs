using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AdoteUmFocinhoWEB.Models
{
    public class Interaction
    {
        [Key]
        public int Id { get; set; }

        [Index]
        public int UserId { get; set; }

        [Index]
        public int PetId { get; set; }

        public enum TypeInteraction
        {
            Favorite = 1,
            Report = 2
        }

        [Index]
        public TypeInteraction Type { get; set; }

        public DateTime Date { get; set; }
    }
}