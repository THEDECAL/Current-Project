using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlinePoker.Models
{
    public enum Suit { NULL, Hearts, Diamonds, Clubs, Spades }
    public enum Rank { NULL, _2 = 2, _3, _4, _5, _6, _7, _8, _9, _10, Jack, Queen, King, Ace }
    public class Card
    {
        public Suit Suit { get; private set; } = Suit.NULL;
        public Rank Rank { get; private set; } = Rank.NULL;
        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }
        public override string ToString() => $"{(Rank < Rank.Jack ? $"{(int)Rank}": $"{Rank.ToString()[0]}")}{Suit.ToString()}";
    }

    public class Deck
    {
        Random _random = new Random();
        public Stack<Card> _cards = new Stack<Card>();
        public Deck()
        {
            for (int suit = (int)Suit.Hearts; suit <= (int)Suit.Spades; suit++)
            {
                for (int rank = (int)Rank._2; rank <= (int)Rank.Ace; rank++)
                {
                    _cards.Push(new Card((Suit)suit, (Rank)rank ));
                }
            }
        }
        /// <summary>
        /// Метод перемешивания карт в колоде
        /// </summary>
        public void Shuffle() => _cards = new Stack<Card>(_cards.OrderBy((c) => _random.Next()));
        /// <summary>
        /// Метод получения карты сверху колоды
        /// </summary>
        /// <returns></returns>
        public Card GetCard() => _cards.Pop();
    }
}