using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using OnlinePoker.Data;
using OnlinePoker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            if(game != null)
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
                        //Если пользователь уже был в игре переподключаем его
                        if (game.IsUserExists(Context.UserIdentifier))
                            ReconnectToGame(game);
                        //Если в игре есть место для игрока
                        else if (game.IsPlacePlayer)
                        {
                            game.AddConnection(Context.UserIdentifier, Context.ConnectionId);

                            var userConns = game.GetConnections(Context.UserIdentifier);
                            Clients.Clients(userConns).SendAsync("Wait").Wait();
                            Clients.Clients(game.Connections).SendAsync("AddPlayer", game.GetPlayersNickNames()).Wait();
                        }

                        StartGame(game);
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
                game.AddConnection(Context.UserIdentifier, Context.ConnectionId);
                var userConns = game.GetConnections(Context.UserIdentifier);
                if (game.IsStarted)
                {
                    var cardVersion = "v1/";
                    var playerNum = 0;
                    var cards = game.Players.Select(p =>
                    {
                        playerNum = game.Players.IndexOf(p) + 1;
                        return (p.UserId == Context.UserIdentifier)
                           ? p.Cards.Select(c => cardVersion + c.GetNumericString()).ToList()
                           : Game.GetEmptyCards(cardVersion);
                    });

                    Clients.Clients(userConns).SendAsync("CloseWait").Wait();
                    Clients.Clients(userConns).SendAsync("AddPlayer", game.GetPlayersNickNames()).Wait();
                    Clients.Clients(userConns).SendAsync("CardDistribution", cards, playerNum).Wait();
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
        protected void StartGame(Game game)
        {
            if (game != null)
            {
                //Начинать игру если все игроки подключены и игра ещё не разу не стартовала
                if (!game.IsPlacePlayer && !game.IsStarted)
                {
                    game.CardDistribution();
                    Clients.Clients(game.Connections).SendAsync("CloseWait").Wait();

                    game.Players.ForEach(p =>
                    {
                        var currPlayerNumber = game.Players.IndexOf(p) + 1;
                        var userConns = game.GetConnections(p.UserId);
                        var cardVersion = "v1/";
                        var cards = game.Players.Select(pp => (currPlayerNumber == game.Players.IndexOf(pp) + 1)
                            ? p.Cards.Select(c => cardVersion + c.GetNumericString()).ToList()
                            : Game.GetEmptyCards(cardVersion));

                        Clients.Clients(userConns).SendAsync("CardDistribution", cards, currPlayerNumber).Wait();
                    });
                }
            }
            else throw new NullReferenceException(); 
        }
    }
}