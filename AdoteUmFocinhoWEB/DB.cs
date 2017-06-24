using AdoteUmFocinhoWEB.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace AdoteUmFocinhoWEB
{
    public class DB : DbContext
    {
        public DB() : base("SQLServer")
        {
        }

        public DbSet<AccessToken> AccessTokens { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Interaction> Interactions { get; set; }
        public DbSet<UserToken> UsersTokens { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}