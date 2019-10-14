using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using static System.Collections.Specialized.INotifyCollectionChanged;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ShipsOnWayToPier
{
    public class PierStock : Stack<Ship>, INotifyCollectionChanged
    {
        Stack<Ship> _stack = new Stack<Ship>();
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        /// <summary>
        /// Кассс сток Stack<Ship> наследуемый от интерфейсов INotifyCollectionChanged для динамического обновления коллекции
        /// </summary>
        public PierStock() : base() { }
        public new Ship Pop()
        {
            var ship = _stack.Pop();

            OnCollectionChanged(new NotifyCollectionChangedEventArgs
                (NotifyCollectionChangedAction.Remove, ship));

            return ship;
        }
        public new void Push(Ship ship)
        {
            if (ship != null)
                _stack.Push(ship);
            else throw new ArgumentNullException();

            OnCollectionChanged(new NotifyCollectionChangedEventArgs
                (NotifyCollectionChangedAction.Add, ship));
        }
        public void OnCollectionChanged(NotifyCollectionChangedEventArgs args) => CollectionChanged?.Invoke(this, args);
    }
    public class TonnelQueue : Queue<Ship>, INotifyCollectionChanged
    {
        Queue<Ship> _queue = new Queue<Ship>();
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        /// <summary>
        /// Кассс очередь Queue<Ship> наследуемый от интерфейсов INotifyCollectionChanged для динамического обновления коллекции
        /// </summary>
        public TonnelQueue() : base() { }
        public TonnelQueue(IEnumerable<Ship> collection) : base(collection) { }
        public new void Enqueue(Ship ship)
        {
            if (ship != null)
                _queue.Enqueue(ship);
            else throw new ArgumentNullException();

            OnCollectionChanged(new NotifyCollectionChangedEventArgs
                (NotifyCollectionChangedAction.Add, ship));
        }
        public new Ship Dequeue()
        {
            var ship = _queue.Dequeue();

            OnCollectionChanged(new NotifyCollectionChangedEventArgs
                (NotifyCollectionChangedAction.Remove, ship));

            return ship;
        }
        public void OnCollectionChanged(NotifyCollectionChangedEventArgs args) => CollectionChanged?.Invoke(this, args);
    }
    public class SeaList : List<Ship>, INotifyCollectionChanged
    {
        List<Ship> _list = new List<Ship>();
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        /// <summary>
        /// Кассс коллекция List<Ship> наследуемый от интерфейсов INotifyCollectionChanged для динамического обновления коллекции
        /// </summary>
        public SeaList() : base() { }
        public SeaList(IEnumerable<Ship> collection) : base(collection) { }

        public new void Add(Ship ship)
        {
            if (ship != null)
                _list.Add(ship);
            else throw new ArgumentNullException();

            if (CollectionChanged != null)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs
                    (NotifyCollectionChangedAction.Add, ship));
        }
        public new void Remove(Ship ship)
        {
            if (ship != null)
                _list.Remove(ship);
            else throw new ArgumentNullException();

            if(CollectionChanged != null)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs
                    (NotifyCollectionChangedAction.Remove, ship));
        }
        public void OnCollectionChanged(NotifyCollectionChangedEventArgs args) => CollectionChanged?.Invoke(this, args);
    }
}