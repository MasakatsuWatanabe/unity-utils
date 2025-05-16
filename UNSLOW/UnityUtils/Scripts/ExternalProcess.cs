using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static UnityEngine.Debug;

namespace UNSLOW.UnityUtils
{
    /// <summary>
    /// 外部プロセス呼び出し機能のラッパー
    /// </summary>
    public sealed class ExternalProcess : IDisposable
    {
        private Process _process;
        private StreamReader _standardOutput;
        private StreamReader _standardError;

        private CancellationTokenSource _cancellationTokenSource;
        private Task _logTask;
        private Task _errorTask;

        public delegate void LogFunc(string txt);

        private static async void Redirector(TextReader reader, LogFunc func, CancellationToken token )
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var t = await reader.ReadLineAsync();
                    if (t != null)
                        func(t);
                }
            }
            catch (Exception e)
            {
                Log(e);
            }
        }

        public void Call(string fileName, string args, bool isInProject = true, LogFunc logFunc = null, LogFunc errorFunc = null, bool noWindow = false)
        {
            var info = new ProcessStartInfo(fileName, args);

            if (isInProject)
                info.WorkingDirectory = Directory.GetCurrentDirectory();

            logFunc ??= Log;
            errorFunc ??= LogError;

            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.CreateNoWindow = noWindow;

            _process = Process.Start(info);
            
            if(_process == null)
                throw new Exception("Process could not start.");

            _standardOutput = _process.StandardOutput;
            _standardError = _process.StandardError;

            _cancellationTokenSource = new CancellationTokenSource();

            var token = _cancellationTokenSource.Token;

            _logTask = Task.Run(() => Redirector(_standardOutput, logFunc, token), token);
            _errorTask = Task.Run(() => Redirector(_standardError, errorFunc, token), token);
        }

        public void Call(ProcessStartInfo info, LogFunc logFunc = null, LogFunc errorFunc = null)
        {
            if (_process != null)
                throw new Exception("Process already running.");

            logFunc ??= Log;
            errorFunc ??= LogError;

            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.UseShellExecute = false;

            _process = Process.Start(info);
            
            if(_process == null)
                throw new Exception("Process could not start.");

            _standardOutput = _process.StandardOutput;
            _standardError = _process.StandardError;

            _cancellationTokenSource = new CancellationTokenSource();

            var token = _cancellationTokenSource.Token;

            _logTask = Task.Run(() => Redirector(_standardOutput, logFunc, token), token);
            _errorTask = Task.Run(() => Redirector(_standardError, errorFunc, token), token);
        }

        public int WaitForExit()
        {
            _process.WaitForExit();
            var code = _process.ExitCode;

            _process.Dispose();
            _process = null;

            EndTasks();

            return code;
        }

        public int WaitForExit(int millSec)
        {
            _process.WaitForExit(millSec);
            var code = _process.ExitCode;
            _process = null;

            EndTasks();

            return code;
        }

        public void Kill()
        {
            if (_process == null)
                return;

            _process.Kill();
            _process.Dispose();
            _process = null;

            EndTasks();
        }

        private void EndTasks()
        {
            _cancellationTokenSource?.Cancel();
            _logTask = null;
            _errorTask = null;
        }

        #region IDisposable Support
        private bool _disposedValue;

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                    Kill();

                _disposedValue = true;
            }

            _cancellationTokenSource?.Cancel();
            _logTask?.Wait();
            _errorTask?.Wait();
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
