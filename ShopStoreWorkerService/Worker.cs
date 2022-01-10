using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ShopStoreWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly string logPath;
        private StreamWriter cpuLogger = null!;
        private HubConnection connection;

        public Worker(ILogger<Worker> logger, IConfiguration config)
        {
            _logger = logger;
            logPath = Path.Combine(config.GetValue<string>("LogPath") ??
                AppContext.BaseDirectory!,
                "cpu.log");

            connection = new HubConnectionBuilder().WithUrl(@"http://localhost:6372/chatHub").Build();
            //connection = new HubConnectionBuilder().WithUrl(@"http://192.168.6.4:8083/chatHub").Build();
        }

        void Log(string message)
        {
            if (cpuLogger == null) return;
            cpuLogger.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}");
            cpuLogger.Flush();
        }

        // 服務啟動時
        public override async Task StartAsync(CancellationToken stoppingToken)
        {
            cpuLogger = new StreamWriter(logPath, true);
            //connection = new HubConnectionBuilder().WithUrl(@"http://localhost:6372/chatHub").Build();
            await connection.StartAsync();
            _logger.LogInformation("connection successful");
            _logger.LogInformation("Service started");

            //Log("Service started");
            // 基底類別 BackgroundService 在 StartAsync() 呼叫 ExecuteAsync、
            // 在 StopAsync() 時呼叫 stoppingToken.Cancel() 優雅結束
            await base.StartAsync(stoppingToken);
        }

        int GetCpuLoad()
        {
            using var p = new Process();
            p.StartInfo.FileName = "wmic.exe";
            p.StartInfo.Arguments = "cpu get loadpercentage";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            int load = -1;
            var m = System.Text.RegularExpressions.Regex.Match(
                p.StandardOutput.ReadToEnd(), @"\d+");
            if (m.Success) load = int.Parse(m.Value);
            p.WaitForExit();
            return load;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                //_logger.LogInformation($"CPU: {GetCpuLoad()}%");
                await Task.Delay(1000, stoppingToken);
                

                // 使用 ThreadPool 執行，避免讀取 CPU 百分比的耗用時間干擾 Task.Delay 間隔
                // https://docs.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads
                // 這裡用舊式 ThreadPool 寫法，亦可用 Task 取代 
                // 參考：https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-cancel-a-task-and-its-children
                ThreadPool.QueueUserWorkItem(
                    (obj) =>
                    {
                        try
                        {
                            var cancelToken = (CancellationToken)obj!;
                            if (!stoppingToken.IsCancellationRequested)
                            {
                                
                                var cpuValue = GetCpuLoad();

                                Log($"CPU: {cpuValue}%");
                                _logger.LogInformation($"Logging CPU load");
                                _logger.LogInformation($"CPU: {cpuValue}%");
                                //connection.InvokeAsync("SendMessage", "CPU:", cpuValue);
                                connection.InvokeAsync("SendMessage", "CPU:", cpuValue.ToString());

                                //SendCPUUsage(cpuValue);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.ToString());
                            throw;
                        }
                    }, stoppingToken);
                await Task.Delay(5000, stoppingToken);
            }
        }

        private async void SendCPUUsage(int cpu)
        {
            await connection.InvokeAsync("SendMessage", "CPU:", cpu.ToString());
        }

        // 服務停止時
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service stopped");
            Log("Service stopped");
            cpuLogger.Dispose();
            cpuLogger = null!;
            await base.StopAsync(stoppingToken);
        }
    }
}
