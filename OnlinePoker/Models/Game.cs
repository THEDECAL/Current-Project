using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlinePoker.Models
{
    public class Game
    {
        const int CARDS_FOR_DIST = 5;
        Deck _deck = new Deck();
        public List<Player> Players { get; private set; } = new List<Player>();
        public int Bank { get; set; }
        public Game()
        {
            _deck.Shuffle();
        }
        public void CardDistribution()
        {
            for (int i = 0; i < CARDS_FOR_DIST; i++)
                Players.ForEach((p) => p.TakeCard(_deck.GetCard()));
        }
    }
}