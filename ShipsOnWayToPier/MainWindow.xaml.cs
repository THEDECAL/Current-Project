using System;
using System.IO;
using System.Text;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using System.Collections.Generic;

namespace ShipsOnWayToPier
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    partial class MainWindow : Window
    {
        Queue<string> _shipNames;
        readonly Random _rand = new Random();
        public SeaList ShipsInSea { get; private set; } = new SeaList() { new Ship("test", Capacity._10, Cargo.Banana)};
        //public ObservableCollection<Ship> ShipsInSea { get; private set; } = new ObservableCollection<Ship>() { new Ship("test", Capacity._10, Cargo.Banana) };
        public TonnelQueue ShipsInChannel { get; private set; } = new TonnelQueue();
        public PierStock ShipsInPierForBreeds { get; private set; } = new PierStock();
        public PierStock ShipsInPierForBananas { get; private set; } = new PierStock();
        public PierStock ShipsInPierForClothes { get; private set; } = new PierStock();
        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
            _shipNames = new Queue<string>(Resource.ResourceManager.GetString("ShipNames")?.Split('\n'))
                ?? throw new NullReferenceException();
        }
        int GetRandomNumber(int min, int max) => _rand.Next(min, max + 1);

        void btnStart_Click(object sender, RoutedEventArgs e)
        {
            var threadOfGenerateShips = new Thread(new ThreadStart(() =>
            {
                for (int i = 0; i < Enum.GetNames(typeof(Capacity)).Length; i++)
                {
                    for (int j = 0; j < Enum.GetNames(typeof(Cargo)).Length; j++)
                    {
                        ShipsInSea.Add(new Ship(GetShipName(), (Capacity)i, (Cargo)j));
                    }
                    //Перемешивание
                    //ShipsInSea.Move(GetRandomNumber(0, ShipsInSea.Count), GetRandomNumber(0, ShipsInSea.Count));
                }
            }
            ));
            threadOfGenerateShips.Start();
            //threadOfGenerateShips.Join();
        }
        Ship GetRandomShip()
        {
            var capacity = (Capacity)_rand.Next((int)Capacity._10, (int)Capacity._100 + 1);
            var cargo = (Cargo)_rand.Next((int)Cargo.Breed, (int)Cargo.Clothes + 1);

            return new Ship(GetShipName(), capacity, cargo);
        }
        string GetShipName()
        {
            var topName = _shipNames.Dequeue();
            _shipNames.Enqueue(topName);

            return topName;
        }

        private void lbShipsInPier(object sender, DependencyPropertyChangedEventArgs e)
        {
            var listBox = sender as ListBox;

            if (listBox != null)
            {
                const string lbPrefix = "lb";
                const string lblPrefix = "lbl";

                var lblName = listBox.Name.Replace(lbPrefix, lblPrefix);
                var lblObj = this.FindName(lblName) as Label;
                var pierStock = listBox.ItemsSource as PierStock;

                if (lblObj != null && pierStock != null)
                {
                    if (pierStock.Count > 0)
                        lblObj.Content = pierStock.Peek();
                }
            }
        }
    }
}
