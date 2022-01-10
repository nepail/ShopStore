using System;
using System.Windows.Forms;
using Microsoft.AspNetCore.SignalR.Client;

namespace singalRClient
{
    public partial class Form1 : Form
    {
        private HubConnection connection;
        //private IHubConnectionBuilder connectionBuilder;

        public Form1()
        {
            InitializeComponent();

            //var connection = connectionBuilder.WithUrl(@"http://192.168.6.4:8083/chatHub").WithAutomaticReconnect().Build();

            //connection = new HubConnectionBuilder().WithUrl(@"http://192.168.6.4:8083/chatHub").Build();

            connection = new HubConnectionBuilder().WithUrl(@"http://localhost:6372/chatHub").Build();

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await connection.StartAsync();
            richTextBox1.Text = "Connection started";
        }

        private async void send_Click(object sender, EventArgs e)
        {
            await connection.InvokeAsync("SendMessage", textBox1.Text, textBox2.Text);
        }
    }
}
