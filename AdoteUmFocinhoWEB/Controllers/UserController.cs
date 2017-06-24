using AdoteUmFocinhoWEB.Models;
using AdoteUmFocinhoWEB.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AdoteUmFocinhoWEB.Controllers
{
    public class UserController : ApiController
    {
        private DB db = new DB();

        //Login
        [Route("api/user/login")]
        [HttpPost]
        public HttpResponseMessage Login(dynamic DadosLogin)
        {
            string Email = DadosLogin.Email.ToString();
            string Password = Utilities.HashPassword(DadosLogin.Password.ToString());

            User user = db.Users.FirstOrDefault(uu => uu.Email == Email &&
                                                      uu.Password == Password &&
                                                      uu.Block == false);

            if (user != null)
            {
                AccessToken AT = new AccessToken();
                AT.UserId = user.Id;
                AT.Token = Guid.NewGuid().ToString();
                AT.Date = DateTime.Now;

                db.AccessTokens.Add(AT);
                db.SaveChanges();

                HttpResponseMessage Response = Request.CreateResponse(HttpStatusCode.OK, user);
                Response.Headers.Add("Token", AT.Token);

                return Response;

            }
            else
                return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        //Register
        [Route("api/users")]
        [HttpPost]
        public HttpResponseMessage Register(dynamic user)
        {
            string Email = user.Email.ToString();

            User NewUser = db.Users.FirstOrDefault(uu => uu.Email == Email);
            if (NewUser != null)
                return Request.CreateResponse(HttpStatusCode.NotAcceptable, "Email já cadastrado!");

            NewUser = new User();

            NewUser.Name = user.Name.ToString();
            NewUser.Email = user.Email.ToString();
            NewUser.Password = Utilities.HashPassword(user.Password.ToString());
            NewUser.Date = DateTime.Now;

            db.Users.Add(NewUser);
            db.SaveChanges();

            AccessToken AT = new AccessToken();
            AT.UserId = NewUser.Id;
            AT.Token = Guid.NewGuid().ToString();
            AT.Date = DateTime.Now;

            db.AccessTokens.Add(AT);
            db.SaveChanges();

            HttpResponseMessage Response = Request.CreateResponse(HttpStatusCode.OK, NewUser);
            Response.Headers.Add("Token", AT.Token);

            return Response;
        }

        //Register Facebook
        [Route("api/users/facebook")]
        [HttpPost]
        public HttpResponseMessage RegisterFacebook(dynamic user)
        {
            string Email = user.Email.ToString();
            string IdSocial = user.IdSocial.ToString();
            string Name = user.Name.ToString();

            User NewUser = db.Users.FirstOrDefault(uu => uu.Email == Email && uu.IdSocial == IdSocial);
            if (NewUser == null)
            {
                NewUser = new User();

                NewUser.Name = Name;
                NewUser.Email = Email;
                NewUser.IdSocial = IdSocial;
                NewUser.Date = DateTime.Now;

                db.Users.Add(NewUser);
                db.SaveChanges();
            }

            AccessToken AT = new AccessToken();
            AT.UserId = NewUser.Id;
            AT.Token = Guid.NewGuid().ToString();
            AT.Date = DateTime.Now;

            db.AccessTokens.Add(AT);
            db.SaveChanges();

            HttpResponseMessage Response = Request.CreateResponse(HttpStatusCode.OK, NewUser);
            Response.Headers.Add("Token", AT.Token);

            return Response;
        }

        //Logoff
        [APIAuthorization]
        [Route("api/users/logoff")]
        [HttpPost]
        public HttpResponseMessage LogOff()
        {
            AccessToken AT = Utilities.GetToken(Request);

            if (AT != null)
            {

                db.Entry(AT).State = EntityState.Deleted;
                db.SaveChanges();

                HttpResponseMessage Response = Request.CreateResponse(HttpStatusCode.OK);
                Response.Headers.Add("Token", "");

                return Response;
            }
            else
                return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        //Push Notification
        [APIAuthorization]
        [Route("api/usuario/add_token_push")]
        [HttpPut]
        public HttpResponseMessage AddToken([FromBody]UserToken UT)
        {
            int UserId = Utilities.GetTokenUser(Request);

            User user = db.Users.Find(UserId);

            if (user != null)
            {
                UT.UserId = user.Id;
                UT.Date = DateTime.Now;

                db.UsersTokens.Add(UT);
                db.SaveChanges();

                HttpResponseMessage Response = Request.CreateResponse(HttpStatusCode.OK, UT);
                return Response;
            }
            else
                return Request.CreateResponse(HttpStatusCode.NotFound);

        }
    }
}
