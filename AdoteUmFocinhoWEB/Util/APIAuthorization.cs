﻿using AdoteUmFocinhoWEB.Models;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace AdoteUmFocinhoWEB.Util
{
    public class APIAuthorization : AuthorizationFilterAttribute
    {
        public override bool AllowMultiple
        {
            get { return false; }
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {

            //Validação
            if (actionContext.ControllerContext.Request.Headers.Any(hh => hh.Key == "Token"))
            {

                string TokenLogado = actionContext.ControllerContext.Request.Headers.First(hh => hh.Key == "Token").Value.First();

                using (DB Banco = new DB())
                {
                    AccessToken AT = Banco.AccessTokens.Find(TokenLogado);

                    //Token Existe
                    if (AT != null)
                        return;
                }

                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }
        }
    }
}