using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using OnlinePoker.Models;

namespace OnlinePoker.Hubs
{
    public class PokerHub : Hub
    {
        private readonly List<Game> _games = new List<Game>();
        //Словарь подключенных пользователей где ключ id пользователя из БД, а значение список id подключений
        private readonly Dictionary<string, List<string>> _users = new Dictionary<string, List<string>>();
        /// <summary>
        /// Метод добавления новой игры
        /// </summary>
        /// <param name="playerCount">Принимает кол-во пользователей</param>
        public void AddGame(int playerCount) => _games.Add(new Game(playerCount));
        public async void Connect(int gameId)
        {
            await Task.Run(() => {
                var currUser = GetCurrentUser();
                //var game = _games.

                AddConnection(currUser, Context.ConnectionId);
                
            });
        }
        /// <summary>
        /// Метод добавления нового подключения в словарь
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="connId">ID подключения</param>
        private void AddConnection(ApplicationUser user, string connId)
        {
            if (user != null)
            {
                lock (_users)
                {
                    var userKey = _users.Keys.FirstOrDefault(key => key == user.Id);

                    if (userKey != null)
                    {
                        if (!_users[userKey].Exists(id => id == connId))
                            _users[userKey].Add(connId);
                    }
                    else
                    {
                        _users.Add(user.Id, new List<string>() { connId });
                    }
                }
            }
            throw new NullReferenceException();
        }
        /// <summary>
        /// Метод удаления подключения из словаря
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="connId">ID подключения</param>
        private void DelConnection(ApplicationUser user, string connId)
        {

        }
        private ApplicationUser GetCurrentUser() => ApplicationDbContext.DbContext.Users
                .FirstOrDefault(u => u.Email == Context.User.Identity.Name);
        public override Task OnDisconnected(bool stopCalled)
        {
            var currUser = GetCurrentUser();



            return base.OnDisconnected(stopCalled);
        }
    }
}