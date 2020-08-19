using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Domain;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class Seed
    {
        public static async Task SeedData(DataContext dataContext, UserManager<AppUser> userManager)
        {

            if (!dataContext.Users.Any())
            {
                var users = new AppUser[] {
                    new AppUser{
                        DisplayName = "Bob",
                        UserName = "bob",
                        Email = "bob@test.com"
                    },
                    new AppUser{
                        DisplayName = "Alice",
                        UserName = "alice",
                        Email = "alice@test.com"
                    },
                    new AppUser{
                        DisplayName = "Jane",
                        UserName = "jane",
                        Email = "jane@test.com"
                    },
                };

                foreach (var item in users)
                    await userManager.CreateAsync(item, "Pa$$w0rd");
            }

            if (!dataContext.Activities.Any())
            {
                var toAdd = new List<Activity>
                {
                    new Activity
                    { 
                        Title= "Past activity 1",
                        Date= DateTime.Now.AddDays(-3),
                        Description= "Three days ago",
                        Category="Drinks",
                        City="London",
                        Venue="Pub"
                    },
                    new Activity
                    {
                        Title= "Past activity 2",
                        Date= DateTime.Now.AddDays(-1),
                        Description= "Yesterday",
                        Category="Culture",
                        City="Paris",
                        Venue="Louvre"
                    },
                    new Activity
                    {
                        Title= "Future activity 1",
                        Date= DateTime.Now.AddDays(2),
                        Description= "In Two days",
                        Category="Drinks",
                        City="Rome",
                        Venue="Coliseum"
                    },
                    new Activity
                    {
                        Title= "Future activity 2",
                        Date= DateTime.Now.AddMonths(3),
                        Description= "In three months",
                        Category="Music",
                        City="London",
                        Venue="Thamesis river"
                    },
                    new Activity
                    {
                        Title= "Past activity 1",
                        Date= DateTime.Now.AddDays(4),
                        Description= "In four months",
                        Category="Film",
                        City="London",
                        Venue="Cinema"
                    }
                };
                dataContext.Activities.AddRange(toAdd);
                dataContext.SaveChanges();
            }
        }
    }
}
