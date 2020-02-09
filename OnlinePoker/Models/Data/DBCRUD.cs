using Microsoft.AspNetCore.Identity;
using OnlinePoker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlinePoker.Data
{
    public class DBCRUD
    {
        static private OnlinePokerContext Ctx { get => new OnlinePokerContext(new Microsoft.EntityFrameworkCore.DbContextOptions<OnlinePokerContext>()); }

        static public async Task<User> GetUserById(string id)
        {
            return await Task.Run(() =>
            {
                using (var ctx = Ctx)
                    return ctx.Users.FirstOrDefault(u => u.Id == id);
            });
        }
    }
}
