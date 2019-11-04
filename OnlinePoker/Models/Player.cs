using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace OnlinePoker.Models
{
    public enum Combination
    {
        HighCard,
        Pair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    };
    public class Player
    {
        List<Card> _cards = new List<Card>();
        public string PlayerName { get; set; }
        public int Tokens { get; set; }
        /// <summary>
        /// Метод получения карты игроком
        /// </summary>
        /// <param name="card"></param>
        public void TakeCard(Card card) => _cards.Add(card);
        /// <summary>
        /// Метод получения карт игрока
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<Card> GetCards() => _cards.AsReadOnly();
        /// <summary>
        /// Метод получения комбинации из текущих карт
        /// </summary>
        /// <returns></returns>
        public Combination GetCombination()
        {
            var sortCrd = _cards.OrderBy((c) => (int)c.Rank).ToArray();
            bool isOneSuit = _cards.All((c) => _cards[0].Suit == c.Suit);
            //RoyalFlush
            if (isOneSuit && sortCrd[0].Rank == Rank._10 && sortCrd[4].Rank == Rank.Ace)
                return Combination.RoyalFlush;
            //StraightFlush
            if (isOneSuit &&
                (((int)sortCrd[4].Rank - (int)sortCrd[0].Rank == 4) ||
                (sortCrd[0].Rank == Rank.Ace && (int)sortCrd[4].Rank - (int)sortCrd[1].Rank == 3) ||
                (sortCrd[4].Rank == Rank.Ace && (int)sortCrd[3].Rank - (int)sortCrd[0].Rank == 3)))
                return Combination.StraightFlush;
            //FourOfAKind
            if (sortCrd[0].Rank == sortCrd[3].Rank || sortCrd[1].Rank == sortCrd[4].Rank)
                return Combination.FourOfAKind;
            //FullHouse
            if ((sortCrd[0].Rank == sortCrd[1].Rank && sortCrd[2].Rank == sortCrd[4].Rank) ||
                (sortCrd[0].Rank == sortCrd[2].Rank && sortCrd[3].Rank == sortCrd[4].Rank))
                return Combination.FullHouse;
            //Flush
            if (isOneSuit)
                return Combination.Flush;
            //Straight
            if ((sortCrd[0].Rank == Rank.Ace && (int)sortCrd[4].Rank - (int)sortCrd[1].Rank == 3) ||
                (sortCrd[4].Rank == Rank.Ace && (int)sortCrd[3].Rank - (int)sortCrd[0].Rank == 3))
                return Combination.Straight;
            //ThreeOfAKind
            if (sortCrd[0].Rank == sortCrd[2].Rank ||
                sortCrd[1].Rank == sortCrd[3].Rank ||
                sortCrd[2].Rank == sortCrd[4].Rank)
                return Combination.ThreeOfAKind;
            //TwoPair
            if (sortCrd[0].Rank == sortCrd[1].Rank && sortCrd[2].Rank == sortCrd[3].Rank ||
                sortCrd[1].Rank == sortCrd[2].Rank && sortCrd[3].Rank == sortCrd[4].Rank)
                return Combination.TwoPair;
            //Pair
            if (sortCrd[0].Rank == sortCrd[1].Rank ||
                sortCrd[1].Rank == sortCrd[2].Rank ||
                sortCrd[2].Rank == sortCrd[3].Rank ||
                sortCrd[3].Rank == sortCrd[4].Rank)
                return Combination.Pair;
            //HighCard
            return Combination.HighCard;
        }
        /// <summary>
        /// Метод получения первой самой старшей карты среди карт
        /// </summary>
        /// <returns></returns>
        public Card GetHighCard() => _cards.FirstOrDefault((cc) => cc.Rank == _cards.Max((c) => c.Rank));
    }
}