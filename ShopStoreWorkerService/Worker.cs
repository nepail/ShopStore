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

        // ���Ն��ӕr
        public override async Task StartAsync(CancellationToken stoppingToken)
        {
            cpuLogger = new StreamWriter(logPath, true);
            //connection = new HubConnectionBuilder().WithUrl(@"http://localhost:6372/chatHub").Build();
            await connection.StartAsync();
            _logger.LogInformation("connection successful");
            _logger.LogInformation("Service started");

            //Log("Service started");
            // ����e BackgroundService �� StartAsync() ���� ExecuteAsync��
            // �� StopAsync() �r���� stoppingToken.Cancel() ���ŽY��
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
                

                // ʹ�� ThreadPool ���У������xȡ CPU �ٷֱȵĺ��Õr�g�ɔ_ Task.Delay �g��
                // https://docs.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads
                // �@�e���fʽ ThreadPool ����������� Task ȡ�� 
                // ������https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-cancel-a-task-and-its-children
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

        // ����ֹͣ�r
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
