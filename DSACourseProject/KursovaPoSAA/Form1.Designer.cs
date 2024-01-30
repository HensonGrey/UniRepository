namespace KursovaPoSAA
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            listView1 = new ListView();
            contextMenuStrip1 = new ContextMenuStrip(components);
            saveToolStripMenuItem = new ToolStripMenuItem();
            deleteToolStripMenuItem = new ToolStripMenuItem();
            dataGridView1 = new DataGridView();
            CommandTextBox = new TextBox();
            RunCommandBtn = new Button();
            TableDataLbl = new Label();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            SaveChangesBtn = new ToolStripMenuItem();
            SaveAllBtn = new ToolStripMenuItem();
            DeleteAllBtn = new ToolStripMenuItem();
            OpenFileBtn = new ToolStripMenuItem();
            customizeToolStripMenuItem = new ToolStripMenuItem();
            optionsToolStripMenuItem = new ToolStripMenuItem();
            contentsToolStripMenuItem = new ToolStripMenuItem();
            indexToolStripMenuItem = new ToolStripMenuItem();
            searchToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            SaveTableBtn = new ContextMenuStrip(components);
            toolStripMenuItem1 = new ToolStripMenuItem();
            contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            menuStrip1.SuspendLayout();
            SaveTableBtn.SuspendLayout();
            SuspendLayout();
            // 
            // listView1
            // 
            listView1.GridLines = true;
            listView1.Location = new Point(12, 31);
            listView1.Name = "listView1";
            listView1.Size = new Size(112, 547);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.List;
            listView1.MouseClick += listView1_MouseClick;
            listView1.MouseDoubleClick += listView1_MouseDoubleClick;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(20, 20);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { saveToolStripMenuItem, deleteToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(211, 80);
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(210, 24);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new Size(210, 24);
            deleteToolStripMenuItem.Text = "Delete";
            deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(144, 196);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.RowTemplate.Height = 29;
            dataGridView1.Size = new Size(669, 382);
            dataGridView1.TabIndex = 1;
            // 
            // CommandTextBox
            // 
            CommandTextBox.Location = new Point(143, 31);
            CommandTextBox.Multiline = true;
            CommandTextBox.Name = "CommandTextBox";
            CommandTextBox.Size = new Size(670, 126);
            CommandTextBox.TabIndex = 2;
            // 
            // RunCommandBtn
            // 
            RunCommandBtn.Location = new Point(819, 75);
            RunCommandBtn.Name = "RunCommandBtn";
            RunCommandBtn.Size = new Size(30, 38);
            RunCommandBtn.TabIndex = 3;
            RunCommandBtn.UseVisualStyleBackColor = true;
            RunCommandBtn.Click += RunCommandBtn_Click;
            // 
            // TableDataLbl
            // 
            TableDataLbl.AutoSize = true;
            TableDataLbl.Location = new Point(12, 581);
            TableDataLbl.Name = "TableDataLbl";
            TableDataLbl.Size = new Size(0, 20);
            TableDataLbl.TabIndex = 4;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(868, 28);
            menuStrip1.TabIndex = 6;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { SaveChangesBtn, SaveAllBtn, DeleteAllBtn, OpenFileBtn });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(46, 24);
            fileToolStripMenuItem.Text = "&File";
            // 
            // SaveChangesBtn
            // 
            SaveChangesBtn.Name = "SaveChangesBtn";
            SaveChangesBtn.Size = new Size(183, 26);
            SaveChangesBtn.Text = "Save Changes";
            SaveChangesBtn.Click += toolStripMenuItem4_Click;
            // 
            // SaveAllBtn
            // 
            SaveAllBtn.Name = "SaveAllBtn";
            SaveAllBtn.Size = new Size(183, 26);
            SaveAllBtn.Text = "Save All";
            SaveAllBtn.Click += toolStripMenuItem1_Click;
            // 
            // DeleteAllBtn
            // 
            DeleteAllBtn.Name = "DeleteAllBtn";
            DeleteAllBtn.Size = new Size(183, 26);
            DeleteAllBtn.Text = "Delete All";
            DeleteAllBtn.Click += toolStripMenuItem2_Click;
            // 
            // OpenFileBtn
            // 
            OpenFileBtn.Name = "OpenFileBtn";
            OpenFileBtn.Size = new Size(183, 26);
            OpenFileBtn.Text = "Open File";
            OpenFileBtn.Click += toolStripMenuItem3_Click;
            // 
            // customizeToolStripMenuItem
            // 
            customizeToolStripMenuItem.Name = "customizeToolStripMenuItem";
            customizeToolStripMenuItem.Size = new Size(161, 26);
            customizeToolStripMenuItem.Text = "&Customize";
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new Size(161, 26);
            optionsToolStripMenuItem.Text = "&Options";
            // 
            // contentsToolStripMenuItem
            // 
            contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
            contentsToolStripMenuItem.Size = new Size(150, 26);
            contentsToolStripMenuItem.Text = "&Contents";
            // 
            // indexToolStripMenuItem
            // 
            indexToolStripMenuItem.Name = "indexToolStripMenuItem";
            indexToolStripMenuItem.Size = new Size(150, 26);
            indexToolStripMenuItem.Text = "&Index";
            // 
            // searchToolStripMenuItem
            // 
            searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            searchToolStripMenuItem.Size = new Size(150, 26);
            searchToolStripMenuItem.Text = "&Search";
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(147, 6);
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(150, 26);
            aboutToolStripMenuItem.Text = "&About...";
            // 
            // SaveTableBtn
            // 
            SaveTableBtn.ImageScalingSize = new Size(20, 20);
            SaveTableBtn.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1 });
            SaveTableBtn.Name = "contextMenuStrip1";
            SaveTableBtn.Size = new Size(212, 28);
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(211, 24);
            toolStripMenuItem1.Text = "toolStripMenuItem1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(868, 609);
            Controls.Add(TableDataLbl);
            Controls.Add(RunCommandBtn);
            Controls.Add(CommandTextBox);
            Controls.Add(dataGridView1);
            Controls.Add(listView1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Simple DataBase";
            contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            SaveTableBtn.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListView listView1;
        private DataGridView dataGridView1;
        private TextBox CommandTextBox;
        private Button RunCommandBtn;
        private Label TableDataLbl;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem customizeToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem contentsToolStripMenuItem;
        private ToolStripMenuItem indexToolStripMenuItem;
        private ToolStripMenuItem searchToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem SaveAllBtn;
        private ToolStripMenuItem DeleteAllBtn;
        private ToolStripMenuItem OpenFileBtn;
        private ToolStripMenuItem SaveChangesBtn;
        private ToolStripMenuItem toolStripMenuItem4;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ContextMenuStrip SaveTableBtn;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
    }
}