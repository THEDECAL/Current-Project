using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBooter.Models
{
    abstract class DownloadState
    {
        private readonly DownloadFile _downloadFile;
        public DownloadState(DownloadFile downloadFile)
        {
            _downloadFile = downloadFile;
        }
        public void Stop() => _downloadFile.State = new StopedState(_downloadFile);
        public void Pause() => _downloadFile.State = new PausedState(_downloadFile);
        public void Resume() => _downloadFile.State = new RunningState(_downloadFile);
    }

    class PausedState : DownloadState
    {
        public PausedState(DownloadFile downloadFile) : base(downloadFile) { }
    }
    class RunningState : DownloadState
    {
        public RunningState(DownloadFile downloadFile) : base(downloadFile) { }
    }
    class StopedState : DownloadState
    {
        public StopedState(DownloadFile downloadFile) : base(downloadFile) { }
    }
}
