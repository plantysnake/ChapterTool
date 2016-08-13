using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ChapterTool
{

    public static class TaskAsync
    {
        public static async Task<string> RunProcessAsync(string fileName, string args, string workingDirectory = "")
        {
            using (var process = new Process
            {
                StartInfo =
                {
                    FileName = fileName, Arguments = args,
                    UseShellExecute = false, CreateNoWindow = true,
                    RedirectStandardOutput = true, RedirectStandardError = true
                },
                EnableRaisingEvents = true
            })
            {
                if (!string.IsNullOrEmpty(workingDirectory))
                {
                    process.StartInfo.WorkingDirectory = workingDirectory;
                }
                return await RunProcessAsync(process).ConfigureAwait(false);
            }
        }

        private static Task<string> RunProcessAsync(Process process)
        {
            var tcs = new TaskCompletionSource<string>();
            var ret = string.Empty;
            process.Exited += (sender, args) => tcs.SetResult(ret);
            process.OutputDataReceived += (sender, args) => ret += args.Data + "\r\n";
            //process.ErrorDataReceived += (s, ea) => Debug.WriteLine("ERR: " + ea.Data);

            if (!process.Start())
            {
                throw new InvalidOperationException("Could not start process: " + process);
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return tcs.Task;
        }
    }
}