using System.ComponentModel;
using System.Windows.Forms;


namespace Presentation
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.ServersDataGridView = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.PlayersDataGridView = new System.Windows.Forms.DataGridView();
            this.PlayersTableUpdateButton = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            ((System.ComponentModel.ISupportInitialize)(this.ServersDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PlayersDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // ServersDataGridView
            // 
            this.ServersDataGridView.AllowUserToAddRows = false;
            this.ServersDataGridView.AllowUserToResizeRows = false;
            this.ServersDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.ServersDataGridView.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.ServersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ServersDataGridView.Location = new System.Drawing.Point(12, 28);
            this.ServersDataGridView.Name = "ServersDataGridView";
            this.ServersDataGridView.Size = new System.Drawing.Size(1027, 280);
            this.ServersDataGridView.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Servers:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 325);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Players:";
            // 
            // PlayersDataGridView
            // 
            this.PlayersDataGridView.AllowUserToAddRows = false;
            this.PlayersDataGridView.AllowUserToResizeRows = false;
            this.PlayersDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.PlayersDataGridView.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.PlayersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.PlayersDataGridView.Location = new System.Drawing.Point(12, 344);
            this.PlayersDataGridView.Name = "PlayersDataGridView";
            this.PlayersDataGridView.Size = new System.Drawing.Size(1027, 266);
            this.PlayersDataGridView.TabIndex = 3;
            this.PlayersDataGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.PlayersDataGridView_CellBeginEdit);
            this.PlayersDataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.PlayersDataGridView_CellEndEdit);
            // 
            // PlayersTableUpdateButton
            // 
            this.PlayersTableUpdateButton.Enabled = false;
            this.PlayersTableUpdateButton.Location = new System.Drawing.Point(74, 317);
            this.PlayersTableUpdateButton.Name = "PlayersTableUpdateButton";
            this.PlayersTableUpdateButton.Size = new System.Drawing.Size(75, 23);
            this.PlayersTableUpdateButton.TabIndex = 5;
            this.PlayersTableUpdateButton.Text = "Update";
            this.PlayersTableUpdateButton.UseVisualStyleBackColor = true;
            this.PlayersTableUpdateButton.Click += new System.EventHandler(this.PlayersTableUpdateButton_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 622);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1051, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1051, 644);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.PlayersTableUpdateButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.PlayersDataGridView);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ServersDataGridView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 350);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Master Server - MongoDB";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MongoDBForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.ServersDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PlayersDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DataGridView ServersDataGridView;
        private Label label1;
        private Label label3;
        private DataGridView PlayersDataGridView;
        private Button PlayersTableUpdateButton;
        private StatusStrip statusStrip1;
    }
}