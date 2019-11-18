using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using OnlinePoker.Models;

namespace OnlinePoker.Hubs
{
    public class GameHub : Hub
    {
        static public List<Game> Games { get; private set; } = new List<Game>();
        //Словарь подключенных пользователей где ключ id пользователя из БД, а значение список id подключений
        private readonly Dictionary<string, List<string>> _users = new Dictionary<string, List<string>>();
        public void Connect(string gameId)
        {
            var currUser = GetCurrentUser();
            var game = Games.FirstOrDefault(g => g.Id == gameId);

            if (currUser != null && game != null)
            {
                game.Players.Add(new Player(currUser.Id, currUser.NickName));
                AddConnection(currUser, Context.ConnectionId);

                var userIndex = _users.Keys.ToList().IndexOf(currUser.Id);

                //Если все игроки подключены начинаем игру
                if (game.PlayerCount == game.Players.Count)
                    StartGame();
            }
            else throw new NullReferenceException();
        }
        /// <summary>
        /// Метод старта игры
        /// </summary>
        private void StartGame()
        {
            
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
            else throw new NullReferenceException();
        }
        /// <summary>
        /// Метод удаления подключения из словаря
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="connId">ID подключения</param>
        private void DelConnection(ApplicationUser user, string connId)
        {
            var userKey = _users.Keys.FirstOrDefault(key => key == user.Id);

            if (userKey != null)
            {
                if (_users[userKey].Count == 1) _users.Remove(userKey);
                else _users[userKey].Remove(connId);
            }
        }
        /// <summary>
        /// Метод который получает текущего пользователя
        /// </summary>
        /// <returns></returns>
        private ApplicationUser GetCurrentUser() => ApplicationDbContext.DbContext.Users
                .FirstOrDefault(u => u.Email == Context.User.Identity.Name);
        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            var currUser = GetCurrentUser();

            if (currUser != null)
                DelConnection(currUser, Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }
    }
}