using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThreadsExam
{
    public partial class Form1 : Form
    {
        bool _isPause = false;
        readonly char[] TRIM_SYMBOLS = { ' ', '\t' };
        string _pathToLog = "";
        Mutex _logMutex = new Mutex();
        List<Thread> _threadPool = new List<Thread>();
        Mutex _threadPoolMutex = new Mutex();
        public Form1()
        {
            InitializeComponent();

            tbCurrentPath.Text = Directory.GetCurrentDirectory();
            //pbProgress.Minimum = 0;
            //pbProgress.Maximum = 100;
            //pbProgress.Value = 0;
            initDisks();
        }
        private void initDisks()
        {
            var listDisks = DriveInfo.GetDrives();

            lbDisks.Items.Clear();
            foreach (var item in listDisks)
            {
                lbDisks.Items.Add(item.Name);
                lbDisks.SelectedItem = item.Name;
            }
        }
        private void btnOpenFileDialog_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Текстовые файлы (*.txt)|*.txt";
            ofd.InitialDirectory = tbCurrentPath.Text;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var fileName = ofd.FileName;
                var strings = File.ReadAllLines(fileName);

                if (strings.Count() > 0)
                {
                    foreach (var item in strings)
                    {
                        var word = item.Trim(TRIM_SYMBOLS);

                        if (word.Count() > 0)
                            lbWords.Items.Add(word);
                    }
                }
                else
                {
                    MessageBox.Show("Файл пустой.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void btnAddWord_Click(object sender, EventArgs e)
        {
            var word = tbWord.Text.Trim(TRIM_SYMBOLS);

            if (word.Count() > 0)
            {
                lbWords.Items.Add(word);
                tbWord.Text = "";
            }
        }
        private void btnSelectPath_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                var folderPath = fbd.SelectedPath;
                if(checkPermission(folderPath))
                    tbCurrentPath.Text = folderPath;
            }
        }
        /// <summary>
        /// 
        /// Проверка прав пути на создание и запись файлов
        /// </summary>
        /// <param name="path">Принимает путь для проверки</param>
        private bool checkPermission(string path)
        {
            try
            {
                var fileName = Guid.NewGuid().ToString();
                var pathToFile = $"{path}\\{ fileName}";

                using (var fs = File.Create(pathToFile)) { }
                File.WriteAllText(pathToFile, "Test On Write");
                File.Delete(pathToFile);

                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("В указанном месте нет прав на создание и запись файлов.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }
        }
        private void btnRefresh_Click(object sender, EventArgs e) => initDisks();
        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            fbd.SelectedPath = tbCurrentPath.Text;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                var folderPath = fbd.SelectedPath;
                if(checkPermission(folderPath))
                    lbFolders.Items.Add(folderPath);
            }
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (lbWords.Items.Count > 0)
            {
                if (lbDisks.SelectedItems.Count > 0 || lbFolders.Items.Count > 0)
                {
                    ClearResult();
                    LockUnlockButtons();

                    //Делаем один массив из дисков и путей поиска
                    var allListToSearch = new List<string>();
                    foreach (var item in lbDisks.SelectedItems)
                        allListToSearch.Add(item.ToString());
                    foreach (var item in lbFolders.Items)
                        allListToSearch.Add(item.ToString());

                    //Выясняем кол-во подпапок для установки границы прогрессбара
                    //Task.Run(() =>
                    //{
                    //    var dirCount = 0;
                    //    foreach (var item in allListToSearch)
                    //    {
                    //        var dirs = Directory.GetDirectories(item, "*", SearchOption.AllDirectories);
                    //        dirCount += dirs.Count();
                    //    }
                    //    pbProgress.Maximum = dirCount;
                    //}).Wait();

                    //Запускаем потоки для поиска
                    pbProgress.Maximum = allListToSearch.Count + 1;
                    foreach (var item in allListToSearch)
                    {
                        pbProgress.Value += 1;
                        var searchThread = new Thread(new ThreadStart(() => Search(item)));
                        _threadPoolMutex.WaitOne();
                        _threadPool.Add(searchThread);
                        _threadPoolMutex.ReleaseMutex();
                        searchThread.Start();
                    }
                }
                else MessageBox.Show("Не выбрано ни одного места для поиска.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else MessageBox.Show("Не добавлено ни одного слова для поиска.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private void Search(string path)
        {
            var searchThread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    var files = Directory.GetFiles(path, "*.txt");
                    var directories = new List<string>(Directory.GetDirectories(path));//.Where(d => !d.Contains("$")).ToList();

                    var changePBThread = new Thread(new ThreadStart(() =>
                    {
                        Thread.Sleep(1000);
                        pbProgress.Invoke(new Action(() =>
                        {
                            pbProgress.Maximum += directories.Count();
                            pbProgress.Value += 1;
                        }));
                    }));
                    _threadPoolMutex.WaitOne();
                    _threadPool.Add(changePBThread);
                    changePBThread.Start();
                    _threadPoolMutex.ReleaseMutex();

                    //if (files.Count() > 0)
                    //{

                    //}

                    if (directories.Count() > 0)
                    {
                        foreach (var item in directories)
                            Search(item);
                    }
                }
                catch (UnauthorizedAccessException) { }
            }));
            _threadPoolMutex.WaitOne();
            _threadPool.Add(searchThread);
            searchThread.Start();
            _threadPoolMutex.ReleaseMutex();
        }
        /// <summary>
        /// Чистка результатов поиска
        /// </summary>
        private void ClearResult()
        {
            lbFindFiles.Items.Clear();
            lblFilesCount.Text = "0";
            lblFindCount.Text = "0";
            pbProgress.Value = 0;
        }
        /// <summary>
        /// Блокировка и разблокировка кнопок во время выполнения поиска
        /// </summary>
        private void LockUnlockButtons()
        {
            btnSearch.Enabled = !btnSearch.Enabled;
            btnAddWord.Enabled = !btnAddWord.Enabled;
            btnRefresh.Enabled = !btnRefresh.Enabled;
            btnAddFolder.Enabled = !btnAddFolder.Enabled;
            btnOpenFileDialog.Enabled = !btnOpenFileDialog.Enabled;
            tbWord.Enabled = !tbWord.Enabled;
            btnSelectPath.Enabled = !btnSelectPath.Enabled;
            btnPause.Enabled = !btnPause.Enabled;
            btnStop.Enabled = !btnStop.Enabled;
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            _threadPoolMutex.WaitOne();

            foreach (var item in _threadPool)
                if(item.ThreadState == ThreadState.Running)
                    item.Abort();

            ClearResult();
            LockUnlockButtons();

            _threadPoolMutex.ReleaseMutex();
        }
        private void btnPause_Click(object sender, EventArgs e)
        {
            _isPause = !_isPause;
            btnPause.Invoke(new Action(() => btnPause.Text = (_isPause) ? "Возобновить" : "Приостановить"));

            _threadPoolMutex.WaitOne();

            foreach (var item in _threadPool)
            {
                if (item.ThreadState == ThreadState.Running)
                    item.Suspend();
                else if (item.ThreadState == ThreadState.Suspended)
                    item.Resume();
            }

            _threadPoolMutex.ReleaseMutex();
        }
    }
}
