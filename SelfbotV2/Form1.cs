using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Discord;
using Discord.Commands;
using Discord.Net.Providers.WS4Net;
using Discord.WebSocket;
using SelfbotV2.Properties;

namespace SelfbotV2
{
    public partial class Form1 : Form
    {
        private bool _baloonShow;
        public static CommandService Commands;
        private static DiscordSocketClient _client;
        private static readonly string[] PossiblePaths =
        {
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"\\discord\\Local Storage\\https_discordapp.com_0.localstorage" ,
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"\\discordptb\\Local Storage\\https_ptb.discordapp.com_0.localstorage"
        };

        public Form1()
        {
            InitializeComponent();
            InitializeNotifyTray();
            prefixTB.Text = Settings.Default.prefix;
        }

        private void InitializeNotifyTray()
        {
            var show = new ToolStripMenuItem { Text = "Show window" };
            var exit = new ToolStripMenuItem { Text = "Exit" };
            show.Click += delegate { Mynotifyicon_MouseClick(null, null); };
            exit.Click += delegate { Application.Exit(); };
            var rightClickMenu = new ContextMenuStrip();
            rightClickMenu.Items.AddRange(new ToolStripItem[]
            {
                show,
                exit
            });
            mynotifyicon.ContextMenuStrip = rightClickMenu;
        }
        private async void Form1_LoadAsync(object sender, EventArgs e)
        {
            string asd = Environment.SpecialFolder.ApplicationData.ToString();

            try { await RunAsync(); }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
                Application.Exit();
            }
            prefixTB.Enabled = true;
            connected.Visible = false;
            timer1.Enabled = false;
        }

        public async Task RunAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                WebSocketProvider = WS4NetProvider.Instance,
                MessageCacheSize = 100,
                LogLevel = LogSeverity.Info
            });

            _client.Log += async message =>
            {
                logBox.BeginInvoke(new Action(() => logBox.Items.Insert(0, message)));
                await Task.CompletedTask;
            };

            Commands = new CommandService();

            await InstallCommandsAsync();
            await _client.LoginAsync(TokenType.User, await GetTokenAsync());
            await _client.StartAsync();
        }
        
        public async Task<string> GetTokenAsync()
        {
            if (!string.IsNullOrEmpty(Settings.Default.token)) return Settings.Default.token;
            var dbPath = "";
            foreach (var path in PossiblePaths)
                if (File.Exists(path))
                {
                    dbPath = path;
                    break;
                }
                    
            if (dbPath == "" && MessageBox.Show("Are you using web discord?", "Discord not found", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Settings.Default.token =
                    Microsoft.VisualBasic.Interaction.InputBox(
                        "Go to web discord press Ctrl+Shift+I, go to Application tab, open Local Storage -> https://discordapp.com and copy the Settings.Default.token here",
                        "Discord token").Trim('"');
            else if(dbPath=="")
                throw new InvalidOperationException("Discord not found");

            if (string.IsNullOrEmpty(Settings.Default.token))
                using (var conn = new SQLiteConnection($"Data Source={dbPath};"))
                {
                    await conn.OpenAsync();
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT value FROM ItemTable WHERE key=""token""";
                        Settings.Default.token = Encoding.ASCII.GetString((byte[])await command.ExecuteScalarAsync()).Replace("\0", "").Trim('"');
                    }
                }
            if (string.IsNullOrEmpty(Settings.Default.token)) throw new InvalidOperationException("token not found");
            Settings.Default.Save();
            return Settings.Default.token;
        }

        public async Task InstallCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            _client.MessageUpdated += (oldmsg, newmsg,task) => HandleCommandAsync(newmsg);
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }
        public async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null || message.Author.Id != _client.CurrentUser.Id) return;
            var argPos = 0;
            if (!message.HasStringPrefix(Settings.Default.prefix, ref argPos)) return;
            await Commands.ExecuteAsync(new CommandContext(_client, message), argPos);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (connected.Text == "Connecting....") connected.Text = "Connecting";
            connected.Text += ".";
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case FormWindowState.Minimized:
                    mynotifyicon.Visible = true;
                    if (!_baloonShow)
                    {
                        mynotifyicon.ShowBalloonTip(500);
                        _baloonShow = true;
                    }
                    Hide();
                    break;

                case FormWindowState.Normal: mynotifyicon.Visible = false; break;
            }
        }

        private void Mynotifyicon_MouseClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }
        private void PrefixChanged(object sender, EventArgs e)
        {
            Settings.Default.prefix = prefixTB.Text;
            Settings.Default.Save();
            help.Text = Settings.Default.prefix + "help     for commands";
        }
    }
}
