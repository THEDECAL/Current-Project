using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using OnlinePoker.Data;
using OnlinePoker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlinePoker.Hubs
{
    /// <summary>
    /// SignalR хаб для игры в покер
    /// </summary>
    public class PokerHub : Hub
    {
        static private List<Game> _games;
        static public List<Game> Games { get => _games = _games ?? new List<Game>(); }
        //private readonly UserManager<User> _userManager;

        //public PokerHub(UserManager<User> userManager)
        //{
        //    _userManager = userManager;
        //}

        public override Task OnConnectedAsync() => base.OnConnectedAsync();
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var game = Games.FirstOrDefault(g => g.Players.Exists(p => p.UserId == Context.UserIdentifier));
            game.DelConnection(Context.UserIdentifier, Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }
        /// <summary>
        /// Подключение игрока в игре
        /// </summary>
        /// <param name="gameId">Иидентификатор игры</param>
        public void ConnectToGame(string gameId)
        {
            if (gameId != null)
            {
                try
                {
                    var game = Games.FirstOrDefault(g => g.Id == gameId);
                    if (game != null)
                    {
                        //Если в игре есть место для игрока
                        if (game.IsPlacePlayer)
                        {
                            var userConns = game.GetConnections(Context.UserIdentifier);

                            game.AddConnection(Context.UserIdentifier, Context.ConnectionId);
                            Clients.Clients(userConns).SendAsync("Wait").Wait();
                            Clients.Clients(game.Connections).SendAsync("AddPlayer", game.GetPlayersNickNames()).Wait();

                            //Если все игроки подключены начинаем игру
                            if (!game.IsPlacePlayer)
                                StartGame(game);
                        }

                        //Если пользователь уже был в игре переподключаем его
                        if (game.IsUserExists(Context.UserIdentifier))
                            ReconnectToGame(game);
                    }
                }
                catch (Exception ex) { }
            }
            else throw new NullReferenceException();
        }
        /// <summary>
        /// Переподключение игрока к игре
        /// </summary>
        /// <param name="game">Объект игры</param>
        protected void ReconnectToGame(Game game)
        {
            if (game != null)
            {
                var userConns = game.GetConnections(Context.UserIdentifier);
                if (game.IsStarted)
                {
                    Clients.Clients(userConns).SendAsync("CloseWait").Wait();
                    Clients.Clients(userConns).SendAsync("AddPlayer", game.GetPlayersNickNames()).Wait();
                }
                else
                {
                    Clients.Clients(userConns).SendAsync("Wait").Wait();
                    Clients.Clients(userConns).SendAsync("AddPlayer", game.GetPlayersNickNames()).Wait();
                }
            }
            else throw new NullReferenceException();
        }
        /// <summary>
        /// Запуск начала игры
        /// </summary>
        /// <param name="game">Объект игры</param>
        protected async void StartGame(Game game)
        {
            if (game != null)
            {
                await Task.Run(() =>
                {
                    game.CardDistribution();
                    Clients.Clients(game.Connections).SendAsync("CloseWait").Wait();
                });
            }
            else throw new NullReferenceException();
        }
    }
}
 