using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace OnlinePoker.Models
{
    public class User : IdentityUser
    {
        public string NickName { get; set; }
        public int CoinsAmount { get; set; }
    }
}