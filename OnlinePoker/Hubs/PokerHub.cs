using Microsoft.AspNetCore.SignalR;
using OnlinePoker.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        static public readonly int THROW_TIME_ANIM = 700;

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
                        var user = Data.DBCRUD.GetUserById(Context.UserIdentifier).Result;

                        if (user.CoinsAmount >= Game.STARTING_BET)
                        {
                            game.AddConnection(Context.UserIdentifier, Context.ConnectionId);

                            var userConns = game.GetConnections(Context.UserIdentifier);
                            Clients.Clients(userConns).SendAsync("WaitWindow");
                            Clients.Clients(game.Connections).SendAsync("AddPlayer", game.GetPlayersNickNames());
                        }
                    }

                    StartGame(game);
                }
                else throw new NullReferenceException();
            }
            catch (Exception ex) { }
        }
        /// <summary>
        /// Выполнение ставки игрока
        /// </summary>
        /// <param name="amountBet">Сумма ставки</param>
        /// <param name="gameId">Идентификатор игры</param>
        public void Bet(int? amountBet, string gameId)
        {
            var game = Games.FirstOrDefault(g => g.Id == gameId);

            if (game != null && amountBet != null)
            {
                var user = game.Players.FirstOrDefault(p => p.UserId == Context.ConnectionId);
                lock(game)
                {
                    user.Coins -= amountBet.Value;
                    game.Bank += amountBet.Value;
                }

                Clients.Clients(game.Connections).SendAsync("AddCoinsToBank", game.Bank);
            }
        }
        /// <summary>
        /// Завершения партии
        /// </summary>
        /// <param name="gameId">Идентификатор игры</param>
        public void GameOver(string gameId)
        {
            var game = Games.FirstOrDefault(g => g.Id == gameId);

            if (game != null)
            {

            }
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
                    var user = game.Players.FirstOrDefault(p => p.UserId == Context.UserIdentifier);

                    Clients.Clients(userConns).SendAsync("CloseWaitWindow");
                    Clients.Clients(userConns).SendAsync("AddPlayer", game.GetPlayersNickNames());
                    Clients.Clients(userConns).SendAsync("AddCoinsToBank", game.Bank);
                    Clients.Clients(userConns).SendAsync("AddCombName", user.GetCombination());

                    var cardVersion = "v1/";
                    var plNum = 0;
                    var cards = game.Players.Select(p =>
                    {
                        plNum = game.Players.IndexOf(p) + 1;

                        return (p.UserId == Context.UserIdentifier)
                                ? p.Cards.Select(c => cardVersion + c.GetNumericString()).ToList()
                                : Game.GetEmptyCards(cardVersion);
                    });

                    Clients.Clients(userConns).SendAsync("QuickCardDist", cards);
                }
                else
                {
                    Clients.Clients(userConns).SendAsync("WaitWindow").Wait();
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
                    game.Bet();
                    game.CardDist();
                    Clients.Clients(game.Connections).SendAsync("CloseWaitWindow");
                    Clients.Clients(game.Connections).SendAsync("AddCoinsToBank", game.Bank);

                    game.Players.ForEach(p =>
                    {
                        var currPlNum = game.Players.IndexOf(p) + 1;
                        var userConns = game.GetConnections(p.UserId);
                        var cardVersion = "v1/";
                        var cards = game.Players.Select(pp => (currPlNum == game.Players.IndexOf(pp) + 1)
                            ? p.Cards.Select(c => cardVersion + c.GetNumericString()).ToList()
                            : Game.GetEmptyCards(cardVersion));

                        Clients.Clients(userConns).SendAsync("CardDist", cards);
                        Clients.Clients(userConns).SendAsync("AddCombName", p.GetCombination());
                    });
                }
            }
            else throw new NullReferenceException(); 
        }
    }
}