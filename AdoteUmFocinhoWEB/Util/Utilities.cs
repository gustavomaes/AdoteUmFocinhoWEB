using AdoteUmFocinhoWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace AdoteUmFocinhoWEB.Util
{
    public class Utilities
    {
        public const double RadioTierra = 6371;

        public static int MaxComplaints() { return 3; }

        public static string URLBase { get { return System.Configuration.ConfigurationManager.AppSettings["URLBase"]; } }

        public static string GET_URL(String URLComplemento)
        {

            String URL = URLBase + (!URLBase.EndsWith("/") ? "/" : "");

            if (URLComplemento.StartsWith("/"))
                URLComplemento = URLComplemento.Remove(0, 1);

            URL = URL + URLComplemento;

            return URL;

        }

        public static string HashPassword(string Password)
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte b in MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(Password)))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public static AccessToken GetToken(HttpRequestMessage Request)
        {
            if (Request.Headers.Any(hh => hh.Key == "Token"))
            {
                String Token = Request.Headers.First(hh => hh.Key == "Token").Value.First();

                AccessToken AT = null;
                using (DB Banco = new DB())
                    AT = Banco.AccessTokens.Where(hh => hh.Token == Token).FirstOrDefault();

                if (AT != null)
                    return AT;
            }

            return null;
        }

        public static int GetTokenUser(HttpRequestMessage Request)
        {
            AccessToken AT = GetToken(Request);

            if (AT != null)
                return AT.UserId;

            return -1;
        }

        public static int GetWidthScreen(HttpRequestMessage Request)
        {

            if (Request.Headers.Any(hh => hh.Key.ToLower() == "widthscreen"))
            {
                String Token = Request.Headers.First(hh => hh.Key.ToLower() == "widthscreen").Value.First();

                int RET = -1;

                int.TryParse(Token, out RET);

                return RET;
            }

            return -1;
        }

        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double distance = 0;
            double Lat = (lat2 - lat1) * (Math.PI / 180);
            double Lon = (lng2 - lng1) * (Math.PI / 180);
            double a = Math.Sin(Lat / 2) * Math.Sin(Lat / 2) + Math.Cos(lat1 * (Math.PI / 180)) * Math.Cos(lat2 * (Math.PI / 180)) * Math.Sin(Lon / 2) * Math.Sin(Lon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            distance = RadioTierra * c;
            return distance;
        }
    }
}