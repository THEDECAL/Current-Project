using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FileBooter.Models
{
    class DownloadFile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        int _downloadProgress = 50;
        bool _isRunning = false;
        int _size;
        int _alreadyLoaded;
        public DownloadState State { get; set; }
        public int Size { get; private set; }
        public bool IsRunning
        {
            get => _isRunning;
            private set
            {
                _isRunning = value;
                OnPropertyChanged("IsRunning");
            }
        }
        public int DownloadProgress
        {
            get => _downloadProgress;
            private set
            {
                _downloadProgress = value;
                OnPropertyChanged("DownloadProgress");
            }
        }
        public string PathToSave { get; private set; }
        public DownloadFile(string pathToSave)
        {
            State = new RunningState(this);
            PathToSave = pathToSave;
        }
        public void Stop() => State.Stop();
        public void Pause() => State.Pause();
        public void Resume() => State.Resume();
        public void IncrementDownloadProgress() => _downloadProgress = (_downloadProgress < 100) ? _downloadProgress++ : 100;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if(PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
