using System;
using System.Windows.Forms;
using App.Common;
using App.HttpServerScripts;
using Dal;
using Dal.Wrappers;
// ReSharper disable InconsistentNaming
// ReSharper disable LocalizableElement


namespace Presentation
{
    public partial class MainForm : Form
    {
        private readonly IServersProvider serversProvider;
        private readonly IPlayersProvider playersProvider;
        private readonly HttpServer masterServerHttpServer;
        private readonly IStripMessenger stripMessenger;

        private readonly IMongoDbProvider dbProvider;

        private delegate void UpdateServersFunc();

        private Timer serversRefreshTimer;
        private const int serversRefreshInterval = 5; // [sec]

        public MainForm(IServersProvider serversProvider,
            IPlayersProvider playersProvider,
            IMongoDbProvider dbProvider,
            HttpServer masterServerHttpServer,
            IStripMessenger stripMessenger)
        {
            this.serversProvider = serversProvider;
            this.playersProvider = playersProvider;
            this.masterServerHttpServer = masterServerHttpServer;
            this.stripMessenger = stripMessenger;
            this.dbProvider = dbProvider;
            InitializeComponent();

            this.serversProvider.OnUpdateServers += UpdateServersInvoker;
            this.stripMessenger.OnNewMessageReceived += OnNewMessageReceived;

            ConnectToMongo();
        }

        private async void ConnectToMongo()
        {
            stripMessenger.StripMessage = "Connecting to MongoDB...";

            try
            {
                await dbProvider.Client.ListDatabasesAsync();
            }
            catch (TimeoutException)
            {
                stripMessenger.StripMessage = "Timeout: Mongo connection failed";
                MessageBox.Show("Timeout: Mongo connection failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            stripMessenger.StripMessage = "Connected to MongoDB";

            //TestFill();

            PlayersTableUpdateButton.Enabled = true;

            masterServerHttpServer.StartHttpServer();

            UpdateServersInvoker(false);

            serversRefreshTimer = new Timer {Interval = serversRefreshInterval * 1000};
            serversRefreshTimer.Tick += ServersRefreshTimerTick;
            serversRefreshTimer.Enabled = true;
        }

        private void UpdateServersInvoker(bool withClearing)
        {
            Invoke(new UpdateServersFunc(() => UpdateServers(withClearing)));
        }

        private async void UpdateServers(bool withClearing)
        {
            if (withClearing)
                ServersDataGridView.DataSource = null;
            var servers = await serversProvider.LoadServers();
            if (servers.Count > 0) // без проверки будет IndexOutOfRangeException при клике на DataGridView после первого добавления
            // строки в таблицу ( http://stackoverflow.com/questions/15160707/indexoutofrangeexception-error-when-datagridview-is-clicked )
                ServersDataGridView.DataSource = servers;
            ServersDataGridView.Invalidate();
        }

        private async void ServersRefreshTimerTick(object sender, EventArgs e)
        {
            var deleteCount = await serversProvider.DeleteUnactiveServers();
            if(deleteCount > 0)
            {
                stripMessenger.StripMessage = "Deleted unactive servers from DB: " + deleteCount;
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void TestFill()
        {
            serversProvider.TestFillDB();
            playersProvider.TestFillDB();
        }

        private object oldValue;

        private void PlayersDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            oldValue = PlayersDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
        }

        private async void PlayersDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var newValue = PlayersDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            if(oldValue == newValue)
            {
                return;
            }

            stripMessenger.StripMessage = "Players updating in progress...";

            var playerId = (int)PlayersDataGridView.Rows[e.RowIndex].Cells[0].Value;
            var parameterToUpdate = PlayersDataGridView.Columns[e.ColumnIndex].HeaderText.Replace("String", "");
            if (await playersProvider.EditPlayer(playerId, parameterToUpdate, newValue))
            {
                MessageBox.Show("[PlayersCollection]:\n" +
                                $"Player: [{playerId}]\nValue of [{parameterToUpdate}] changed to [{newValue}]",
                    "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            var players = await playersProvider.LoadPlayers();
            if (players.Count > 0)
                PlayersDataGridView.DataSource = players;
            PlayersDataGridView.Invalidate();

            stripMessenger.StripMessage = "Players updated";
        }

        private async void PlayersTableUpdateButton_Click(object sender, EventArgs e)
        {
            stripMessenger.StripMessage = "Players updating in progress...";
            PlayersTableUpdateButton.Enabled = false;

            PlayersDataGridView.DataSource = null;
            var players = await playersProvider.LoadPlayers();
            if (players.Count > 0)
                PlayersDataGridView.DataSource = players;
            PlayersDataGridView.Invalidate();

            PlayersTableUpdateButton.Enabled = true;
            stripMessenger.StripMessage = "Players updated";
        }

        private void MongoDBForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            masterServerHttpServer.StopHttpServer();
        }

        private void SetStatusStripMessage(string message)
        {
            message = $"[{DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")}] {message}";
            statusStrip1.Items.Clear();
            statusStrip1.Items.Add(message);
        }

        private void OnNewMessageReceived()
        {
            SetStatusStripMessage(stripMessenger.StripMessage);
        }
    }
}
