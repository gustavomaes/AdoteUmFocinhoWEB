using AdoteUmFocinhoWEB.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AdoteUmFocinhoWEB.Models
{
    public class Pet
    {
        [Key]
        public int Id { get; set; }

        [Index]
        public int UserId { get; set; }

        [Index]
        public bool Block { get; set; }

        public enum LifeStages
        {
            Puppy = 1,
            Teenager = 2,
            Adult = 3,
            Senior = 4
        }

        [Index]
        public LifeStages type { get; set; }

        public enum SpecieAnimals
        {
            Dog = 1,
            Cat = 2
        }

        [Index]
        public SpecieAnimals Specie { get; set; }

        public byte[] Photo { get; set; }

        [NotMapped]
        public string PhotoUrl { get; set; }

        public void RefreshDataPhoto(int WidthScreen)
        {
            PhotoUrl = Utilities.GET_URL("api/pets/" + Id + "/photo/" + WidthScreen.ToString("0"));
            Photo = null;
        }
        public string Description { get; set; }

        public string Phone { get; set; }

        public string Whatsapp { get; set; }

        public string Email { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime Date { get; set; }

        public int AmountReports { get; set; }

        [NotMapped]
        public bool Favorite { get; set; }

        [NotMapped]
        public bool MyPet { get; set; }
    }
}