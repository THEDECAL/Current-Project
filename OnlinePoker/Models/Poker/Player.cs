using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
        public List<Card> Cards { get; } = new List<Card>();
        public string UserId { get; private set; }
        public string NickName { get; private set; }
        public int Coins { get; set; }
        public bool IsGameOver { get; set; } = false;

        public Player(string userId)
        {
            if (userId != null)
            {
                var user = Data.DBCRUD.GetUserById(userId).Result;
                UserId = userId;
                NickName = user.NickName;
                Coins = user.CoinsAmount;
            }
            else throw new NullReferenceException();
        }
        /// <summary>
        /// Метод получения первой самой старшей карты среди карт
        /// </summary>
        /// <returns></returns>
        private Card GetHighCard() => Cards.FirstOrDefault((cc) => cc.Rank == Cards.Max((c) => c.Rank));
        /// <summary>
        /// Метод получения карты игроком
        /// </summary>
        /// <param name="card"></param>
        public void TakeCard(Card card) => Cards.Add(card);
        /// <summary>
        /// Метод получения карт игрока
        /// </summary>
        /// <returns></returns>
        //public ReadOnlyCollection<Card> GetCards() => Cards.AsReadOnly();
        /// <summary>
        /// Метод получения комбинации из текущих карт
        /// </summary>
        /// <returns></returns>
        public Combination GetCombination()
        {
            var sortCrd = Cards.OrderBy((c) => (int)c.Rank).ToArray();
            bool isOneSuit = Cards.All((c) => Cards[0].Suit == c.Suit);
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

        public static bool operator ==(Player p1, Player p2) => p1.UserId == p2.UserId;
        public static bool operator !=(Player p1, Player p2) => !(p1 == p2);

        public override bool Equals(object obj) => (obj as Player) == this;
        public override int GetHashCode() => UserId.GetHashCode();
        public override string ToString() => $"{NickName} ({UserId})";
    }
}