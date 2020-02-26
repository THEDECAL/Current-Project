using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlinePoker.Models.Data
{
    public class Initializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager)
        {
            if(await userManager.FindByEmailAsync("thedecal1@gmail.com") == null)
            {
                var user1 = new User
                    {
                        UserName = "thedecal1@gmail.com",
                        Email = "thedecal1@gmail.com",
                        NickName = "THE DECAL!",
                        CoinsAmount = Models.User.STARTING_COINS,
                        EmailConfirmed = true,
                        LockoutEnabled = true
                    };
                await userManager.CreateAsync(user1, @"AQeT.5*gehWqeAh");
                userManager.UpdateAsync(user1).Wait();
            }
            if (await userManager.FindByEmailAsync("thedecal2@gmail.com") == null)
            {
                var user2 = new User
                {
                    UserName = "thedecal2@gmail.com",
                    Email = "thedecal2@gmail.com",
                    NickName = "TESTIK",
                    CoinsAmount = Models.User.STARTING_COINS,
                    EmailConfirmed = true,
                    LockoutEnabled = true
                };
                await userManager.CreateAsync(user2, @"AQeT.5*gehWqeAh");
                userManager.UpdateAsync(user2).Wait();
            }
        }
    }
}
