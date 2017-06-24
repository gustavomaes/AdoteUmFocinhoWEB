using AdoteUmFocinhoWEB.Models;
using AdoteUmFocinhoWEB.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Web.Http;

namespace AdoteUmFocinhoWEB.Controllers
{
    public class PetController : ApiController
    {
        private DB db = new DB();

        //Feed
        [APIAuthorization]
        [Route("api/pets/feed")]
        [HttpPost]
        public IHttpActionResult Feed(dynamic Filters)
        {

            int UserId = Utilities.GetTokenUser(Request);

            int Radius = Filters.Radius;
            double UserLat = Filters.Latitude;
            double UserLong = Filters.Longitude;

            List<int> Gender = JsonConvert.DeserializeObject<List<int>>(Filters.Genders.ToString());
            List<int> Specie = JsonConvert.DeserializeObject<List<int>>(Filters.Specie.ToString()); 
            List<int> LifeStage = JsonConvert.DeserializeObject<List<int>>(Filters.LifeStage.ToString());

            List<Pet> pets = db.Pets
                .Where(pp => pp.Block == false &&
                       pp.UserId != UserId)
                .OrderByDescending(pp => pp.Date).ToList();

            foreach (var p in pets.ToArray())
            {
                p.RefreshDataPhoto(Utilities.GetWidthScreen(Request));

                p.Favorite = db.Interactions.Any(ii => ii.PetId== p.Id && ii.Type == Interaction.TypeInteraction.Favorite && ii.UserId == UserId);
                
                if (!Gender.Contains((int)p.Specie))
                    pets.Remove(p);

                if (!Specie.Contains((int)p.Specie))
                    pets.Remove(p);

                if (!LifeStage.Contains((int)p.type))
                    pets.Remove(p);

                if (pets.Contains(p) && Utilities.GetDistance(UserLat, UserLong, p.Latitude, p.Longitude) > Radius)
                    pets.Remove(p);

            }

            return Ok(pets);
        }

        //My Feed
        [APIAuthorization]
        [Route("api/pets/myfeed")]
        [HttpGet]
        public IHttpActionResult MyFeed()
        {

            int UserId = Utilities.GetTokenUser(Request);

            //ADD FILTERS
            List<Pet> pets = db.Pets
                .Where(pp => pp.Block == false &&
                       pp.UserId == UserId)
                .OrderByDescending(pp => pp.Date).ToList();

            foreach (var p in pets)
            {
                p.MyPet = true;
                p.RefreshDataPhoto(Utilities.GetWidthScreen(Request));
            }
            return Ok(pets);
        }

        //My Favorites
        [APIAuthorization]
        [Route("api/pets/favorite")]
        [HttpGet]
        public IHttpActionResult FeedFavorite()
        {

            int UserId = Utilities.GetTokenUser(Request);
            
            List<Pet> pets = db.Pets
                .Where(pp => pp.Block == false)
                .OrderByDescending(pp => pp.Date).ToList();

            foreach (var p in pets.ToArray())
            { 
                p.RefreshDataPhoto(Utilities.GetWidthScreen(Request));

                p.Favorite = db.Interactions.Any(ii => ii.PetId == p.Id && ii.Type == Interaction.TypeInteraction.Favorite && ii.UserId == UserId);
                if (!p.Favorite)
                    pets.Remove(p);
            }
            return Ok(pets);
        }

        //Get Photo
        [APIAuthorization]
        [Route("api/pets/{PetID}/photo/{WidthScreen}")]
        [HttpGet]
        public HttpResponseMessage Photo(int PetID, int WidthScreen)
        {
            Pet pet = db.Pets.Find(PetID);

            if (pet != null && pet.Photo != null)
            {

                pet.Photo = pet.Photo.ProportionalResize(WidthScreen);

                HttpResponseMessage Result = new HttpResponseMessage(HttpStatusCode.OK);
                Result.Content = new StreamContent(new MemoryStream(pet.Photo));
                Result.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Image.Jpeg);

                return Result;
            }

            return new HttpResponseMessage(HttpStatusCode.NotAcceptable);
        }

        //Register Pet
        [APIAuthorization]
        [Route("api/pets")]
        [HttpPost]
        public IHttpActionResult Register(Pet pet)
        {
            pet.Date = DateTime.Now;

            db.Pets.Add(pet);
            db.SaveChanges();

            pet.RefreshDataPhoto(Utilities.GetWidthScreen(Request));
            pet.MyPet = true;

            return Ok(pet);
        }

        //Delete Pet
        [APIAuthorization]
        [Route("api/pets/{PetId}")]
        [HttpDelete]
        public IHttpActionResult DeletePet(int PetId)
        {

            Pet pet = db.Pets.Find(PetId);

            int UserId = Utilities.GetTokenUser(Request);

            if (pet == null || pet.UserId!= UserId)
            {
                return NotFound();
            }

            db.Pets.Remove(pet);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        //Report
        [APIAuthorization]
        [Route("api/pets/report/{PetId}")]
        [HttpPut]
        public IHttpActionResult Report(int PetId)
        {

            int UserId = Utilities.GetTokenUser(Request);
            
            Interaction petInteraction = db.Interactions.FirstOrDefault(pp => pp.PetId == PetId &&
                                                                       pp.UserId == UserId &&
                                                                       pp.Type == Interaction.TypeInteraction.Report);

            if (petInteraction == null)
            {
                petInteraction = new Interaction();
                petInteraction.PetId = PetId;
                petInteraction.UserId = Utilities.GetTokenUser(Request);
                petInteraction.Type = Interaction.TypeInteraction.Report;
                petInteraction.Date= DateTime.Now;

                db.Interactions.Add(petInteraction);

                Pet pet= db.Pets.Find(PetId);
                pet.AmountReports++;

                if (pet.AmountReports > Utilities.MaxComplaints())
                    pet.Block = true;

                db.SaveChanges();
            }

            return Ok();
        }

        //Favorite
        [APIAuthorization]
        [Route("api/pets/favorite/{PetId}")]
        [HttpPut]
        public IHttpActionResult Favorite(int PetId, bool Favorite)
        {

            int UserId = Utilities.GetTokenUser(Request);

            Interaction petInteraction = db.Interactions.FirstOrDefault(pp => pp.PetId == PetId &&
                                                                        pp.UserId == UserId &&
                                                                        pp.Type == Interaction.TypeInteraction.Favorite);
            
            if (petInteraction == null)
            {

                if (Favorite)
                {
                    petInteraction = new Interaction();

                    petInteraction.PetId = PetId;
                    petInteraction.UserId = UserId;
                    petInteraction.Type = Interaction.TypeInteraction.Favorite;
                    petInteraction.Date = DateTime.Now;
                    db.Interactions.Add(petInteraction);
                }
            }
            else
            {
                if (!Favorite)
                {

                    db.Interactions.Remove(petInteraction);

                }
            }

            db.SaveChanges();

            return Ok();
        }

    }
}
