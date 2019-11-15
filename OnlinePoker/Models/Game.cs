using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlinePoker.Models
{
    public class Game
    {
        const int MAX_PLAYERS = 8;
        const int CARDS_FOR_DIST = 5;
        Deck _deck = new Deck();
        public List<Player> Players { get; private set; } = new List<Player>();
        public int PlayerCount { get; private set; }
        public int Bank { get; set; }
        public string Id { get; private set; } = Guid.NewGuid().ToString();
        public Game(int playerCount)
        {
            if (playerCount % 2 == 0 && playerCount <= MAX_PLAYERS)
            {
                PlayerCount = playerCount;

                _deck.Shuffle();
            }
            throw new ArgumentOutOfRangeException();
        }
        public void CardDistribution()
        {
            for (int i = 0; i < CARDS_FOR_DIST; i++)
                Players.ForEach(p => p.TakeCard(_deck.GetCard()));
        }
        public void AddPlayer(Player player)
        {
            if (Players.Count < PlayerCount)
            {
                if(!Players.Exists(p => p.UserId == player.UserId))
                    Players.Add(player);
            }
        }
    }
}