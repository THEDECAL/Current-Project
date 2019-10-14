using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipsOnWayToPier
{
    public enum Cargo { Breed = 0, Banana = 1, Clothes = 2};
    public enum Capacity { _10 = 10, _50 = 50, _100 = 100 };
    public class Ship : INotifyPropertyChanged
    {
        public delegate void PropertyChanged();
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => throw new NotImplementedException();
            remove => throw new NotImplementedException();
        }

        public string Name { get; private set; }
        public Capacity Capacity { get; private set; }
        public Cargo Cargo { get; private set; }
        public int Congestion { get; set; } = 0;
        public bool IsFullCongestion { get => (Congestion == (int)Capacity); }
        public Ship(string name, Capacity capacity, Cargo cargo)
        {
            Name = name;
            Capacity = capacity;
            Cargo = cargo;
        }
    }
}
