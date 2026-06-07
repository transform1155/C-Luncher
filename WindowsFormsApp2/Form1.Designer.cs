namespace WindowsFormsApp2
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.headerPanel = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabVersions = new System.Windows.Forms.TabPage();
            this.tabDownloads = new System.Windows.Forms.TabPage();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.tabAbout = new System.Windows.Forms.TabPage();

            // Version tab controls
            this.panelVersionFilter = new System.Windows.Forms.Panel();
            this.btnFilterAll = new ModernButton();
            this.btnFilterRelease = new ModernButton();
            this.btnFilterSnapshot = new ModernButton();
            this.btnFilterAprilFool = new ModernButton();
            this.panelVersionList = new System.Windows.Forms.FlowLayoutPanel();
            this.panelVersionInfo = new System.Windows.Forms.Panel();
            this.lblVersionTitle = new System.Windows.Forms.Label();
            this.lblVersionType = new System.Windows.Forms.Label();
            this.txtVersionInfo = new System.Windows.Forms.RichTextBox();
            this.btnDownloadVersion = new ModernButton();
            this.btnLaunchVersion = new ModernButton();
            this.panelLaunchSettings = new System.Windows.Forms.Panel();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblGameDir = new System.Windows.Forms.Label();
            this.txtGameDir = new System.Windows.Forms.TextBox();
            this.btnBrowse = new ModernButton();

            // Download tab controls
            this.panelDownloadControls = new System.Windows.Forms.Panel();
            this.lblActiveDownloads = new System.Windows.Forms.Label();
            this.lblSpeedLimit = new System.Windows.Forms.Label();
            this.cmbSpeedLimit = new System.Windows.Forms.ComboBox();
            this.lblConcurrentDownloads = new System.Windows.Forms.Label();
            this.cmbConcurrentDownloads = new System.Windows.Forms.ComboBox();
            this.panelDownloadList = new System.Windows.Forms.FlowLayoutPanel();
            this.overallProgress = new ModernProgressBar();
            this.lblOverallStatus = new System.Windows.Forms.Label();

            // Settings tab controls
            this.panelSettings = new System.Windows.Forms.Panel();
            this.chkAutoUpdate = new System.Windows.Forms.CheckBox();
            this.chkKeepLauncherOpen = new System.Windows.Forms.CheckBox();
            this.lblJavaPath = new System.Windows.Forms.Label();
            this.txtJavaPath = new System.Windows.Forms.TextBox();
            this.btnBrowseJava = new ModernButton();
            this.lblMemory = new System.Windows.Forms.Label();
            this.cmbMemory = new System.Windows.Forms.ComboBox();
            this.btnSaveSettings = new ModernButton();

            // About tab controls
            this.lblAboutTitle = new System.Windows.Forms.Label();
            this.txtAbout = new System.Windows.Forms.RichTextBox();

            this.headerPanel.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabVersions.SuspendLayout();
            this.tabDownloads.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.tabAbout.SuspendLayout();
            this.panelVersionFilter.SuspendLayout();
            this.panelVersionInfo.SuspendLayout();
            this.panelLaunchSettings.SuspendLayout();
            this.panelDownloadControls.SuspendLayout();
            this.panelSettings.SuspendLayout();
            this.SuspendLayout();

            //
            // headerPanel
            //
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.headerPanel.Controls.Add(this.lblTitle);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(900, 50);
            this.headerPanel.TabIndex = 0;

            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(200, 30);
            this.lblTitle.Text = "我的世界启动器";
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;

            //
            // tabMain
            //
            this.tabMain.Controls.Add(this.tabVersions);
            this.tabMain.Controls.Add(this.tabDownloads);
            this.tabMain.Controls.Add(this.tabSettings);
            this.tabMain.Controls.Add(this.tabAbout);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tabMain.Location = new System.Drawing.Point(0, 50);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(900, 550);
            this.tabMain.TabIndex = 1;

            //
            // tabVersions
            //
            this.tabVersions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.tabVersions.Controls.Add(this.panelVersionInfo);
            this.tabVersions.Controls.Add(this.panelVersionFilter);
            this.tabVersions.Controls.Add(this.panelVersionList);
            this.tabVersions.Controls.Add(this.panelLaunchSettings);
            this.tabVersions.Location = new System.Drawing.Point(4, 24);
            this.tabVersions.Name = "tabVersions";
            this.tabVersions.Padding = new System.Windows.Forms.Padding(10);
            this.tabVersions.Size = new System.Drawing.Size(892, 522);
            this.tabVersions.TabIndex = 0;
            this.tabVersions.Text = "版本";

            //
            // panelVersionFilter
            //
            this.panelVersionFilter.BackColor = System.Drawing.Color.White;
            this.panelVersionFilter.Controls.Add(this.btnFilterAll);
            this.panelVersionFilter.Controls.Add(this.btnFilterRelease);
            this.panelVersionFilter.Controls.Add(this.btnFilterSnapshot);
            this.panelVersionFilter.Controls.Add(this.btnFilterAprilFool);
            this.panelVersionFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelVersionFilter.Location = new System.Drawing.Point(10, 10);
            this.panelVersionFilter.Name = "panelVersionFilter";
            this.panelVersionFilter.Padding = new System.Windows.Forms.Padding(10);
            this.panelVersionFilter.Size = new System.Drawing.Size(872, 50);
            this.panelVersionFilter.TabIndex = 0;

            //
            // btnFilterAll
            //
            this.btnFilterAll.BackColor = ColorSchemes.Release.Primary;
            this.btnFilterAll.ForeColor = System.Drawing.Color.White;
            this.btnFilterAll.Location = new System.Drawing.Point(13, 12);
            this.btnFilterAll.Name = "btnFilterAll";
            this.btnFilterAll.Size = new System.Drawing.Size(80, 26);
            this.btnFilterAll.TabIndex = 0;
            this.btnFilterAll.Text = "全部";
            this.btnFilterAll.BorderRadius = 13;
            this.btnFilterAll.Click += new System.EventHandler(this.btnFilter_Click);

            //
            // btnFilterRelease
            //
            this.btnFilterRelease.BackColor = ColorSchemes.Release.Primary;
            this.btnFilterRelease.ForeColor = System.Drawing.Color.White;
            this.btnFilterRelease.Location = new System.Drawing.Point(99, 12);
            this.btnFilterRelease.Name = "btnFilterRelease";
            this.btnFilterRelease.Size = new System.Drawing.Size(100, 26);
            this.btnFilterRelease.TabIndex = 1;
            this.btnFilterRelease.Text = "正式版";
            this.btnFilterRelease.BorderRadius = 13;
            this.btnFilterRelease.Click += new System.EventHandler(this.btnFilter_Click);

            //
            // btnFilterSnapshot
            //
            this.btnFilterSnapshot.BackColor = ColorSchemes.Snapshot.Primary;
            this.btnFilterSnapshot.ForeColor = System.Drawing.Color.White;
            this.btnFilterSnapshot.Location = new System.Drawing.Point(205, 12);
            this.btnFilterSnapshot.Name = "btnFilterSnapshot";
            this.btnFilterSnapshot.Size = new System.Drawing.Size(100, 26);
            this.btnFilterSnapshot.TabIndex = 2;
            this.btnFilterSnapshot.Text = "快照版";
            this.btnFilterSnapshot.BorderRadius = 13;
            this.btnFilterSnapshot.Click += new System.EventHandler(this.btnFilter_Click);

            //
            // btnFilterAprilFool
            //
            this.btnFilterAprilFool.BackColor = ColorSchemes.AprilFool.Primary;
            this.btnFilterAprilFool.ForeColor = System.Drawing.Color.White;
            this.btnFilterAprilFool.Location = new System.Drawing.Point(311, 12);
            this.btnFilterAprilFool.Name = "btnFilterAprilFool";
            this.btnFilterAprilFool.Size = new System.Drawing.Size(100, 26);
            this.btnFilterAprilFool.TabIndex = 3;
            this.btnFilterAprilFool.Text = "愚人节";
            this.btnFilterAprilFool.BorderRadius = 13;
            this.btnFilterAprilFool.Click += new System.EventHandler(this.btnFilter_Click);

            //
            // panelVersionList
            //
            this.panelVersionList.AutoScroll = true;
            this.panelVersionList.BackColor = System.Drawing.Color.Transparent;
            this.panelVersionList.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelVersionList.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.panelVersionList.Location = new System.Drawing.Point(10, 60);
            this.panelVersionList.Name = "panelVersionList";
            this.panelVersionList.Padding = new System.Windows.Forms.Padding(5);
            this.panelVersionList.Size = new System.Drawing.Size(350, 452);
            this.panelVersionList.TabIndex = 1;
            this.panelVersionList.WrapContents = false;

            //
            // panelVersionInfo
            //
            this.panelVersionInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.panelVersionInfo.Controls.Add(this.lblVersionTitle);
            this.panelVersionInfo.Controls.Add(this.lblVersionType);
            this.panelVersionInfo.Controls.Add(this.txtVersionInfo);
            this.panelVersionInfo.Controls.Add(this.btnDownloadVersion);
            this.panelVersionInfo.Controls.Add(this.btnLaunchVersion);
            this.panelVersionInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelVersionInfo.Location = new System.Drawing.Point(360, 60);
            this.panelVersionInfo.Name = "panelVersionInfo";
            this.panelVersionInfo.Padding = new System.Windows.Forms.Padding(15);
            this.panelVersionInfo.Size = new System.Drawing.Size(522, 452);
            this.panelVersionInfo.TabIndex = 2;

            //
            // lblVersionTitle
            //
            this.lblVersionTitle.AutoSize = true;
            this.lblVersionTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblVersionTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lblVersionTitle.Location = new System.Drawing.Point(18, 15);
            this.lblVersionTitle.Name = "lblVersionTitle";
            this.lblVersionTitle.Size = new System.Drawing.Size(150, 25);
            this.lblVersionTitle.TabIndex = 0;
            this.lblVersionTitle.Text = "请选择一个版本";
            this.lblVersionTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));

            //
            // lblVersionType
            //
            this.lblVersionType.AutoSize = true;
            this.lblVersionType.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblVersionType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblVersionType.Location = new System.Drawing.Point(18, 45);
            this.lblVersionType.Name = "lblVersionType";
            this.lblVersionType.Size = new System.Drawing.Size(0, 15);
            this.lblVersionType.TabIndex = 1;
            this.lblVersionType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));

            //
            // txtVersionInfo
            //
            this.txtVersionInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtVersionInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtVersionInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtVersionInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.txtVersionInfo.Location = new System.Drawing.Point(18, 70);
            this.txtVersionInfo.Name = "txtVersionInfo";
            this.txtVersionInfo.ReadOnly = true;
            this.txtVersionInfo.Size = new System.Drawing.Size(486, 130);
            this.txtVersionInfo.TabIndex = 2;
            this.txtVersionInfo.Text = "从列表中选择一个版本查看详情";
            this.txtVersionInfo.WordWrap = true;
            this.txtVersionInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));

            //
            // btnDownloadVersion
            //
            this.btnDownloadVersion.BackColor = ColorSchemes.Info;
            this.btnDownloadVersion.ForeColor = System.Drawing.Color.White;
            this.btnDownloadVersion.Location = new System.Drawing.Point(18, 215);
            this.btnDownloadVersion.Name = "btnDownloadVersion";
            this.btnDownloadVersion.Size = new System.Drawing.Size(120, 40);
            this.btnDownloadVersion.TabIndex = 3;
            this.btnDownloadVersion.Text = "下载";
            this.btnDownloadVersion.BorderRadius = 8;
            this.btnDownloadVersion.Click += new System.EventHandler(this.btnDownload_Click);
            this.btnDownloadVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));

            //
            // btnLaunchVersion
            //
            this.btnLaunchVersion.BackColor = ColorSchemes.Success;
            this.btnLaunchVersion.ForeColor = System.Drawing.Color.White;
            this.btnLaunchVersion.Location = new System.Drawing.Point(150, 215);
            this.btnLaunchVersion.Name = "btnLaunchVersion";
            this.btnLaunchVersion.Size = new System.Drawing.Size(120, 40);
            this.btnLaunchVersion.TabIndex = 4;
            this.btnLaunchVersion.Text = "启动";
            this.btnLaunchVersion.BorderRadius = 8;
            this.btnLaunchVersion.Click += new System.EventHandler(this.btnLaunch_Click);
            this.btnLaunchVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));

            //
            // panelLaunchSettings
            //
            this.panelLaunchSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelLaunchSettings.Controls.Add(this.lblUsername);
            this.panelLaunchSettings.Controls.Add(this.txtUsername);
            this.panelLaunchSettings.Controls.Add(this.lblGameDir);
            this.panelLaunchSettings.Controls.Add(this.txtGameDir);
            this.panelLaunchSettings.Controls.Add(this.btnBrowse);
            this.panelLaunchSettings.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelLaunchSettings.Location = new System.Drawing.Point(10, 412);
            this.panelLaunchSettings.Name = "panelLaunchSettings";
            this.panelLaunchSettings.Padding = new System.Windows.Forms.Padding(10);
            this.panelLaunchSettings.Size = new System.Drawing.Size(872, 100);
            this.panelLaunchSettings.TabIndex = 3;
            this.panelLaunchSettings.Visible = false;

            //
            // lblUsername
            //
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(13, 15);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(60, 15);
            this.lblUsername.Text = "用户名：";

            //
            // txtUsername
            //
            this.txtUsername.Location = new System.Drawing.Point(80, 12);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(150, 23);
            this.txtUsername.TabIndex = 1;
            this.txtUsername.Text = "玩家";

            //
            // lblGameDir
            //
            this.lblGameDir.AutoSize = true;
            this.lblGameDir.Location = new System.Drawing.Point(13, 50);
            this.lblGameDir.Name = "lblGameDir";
            this.lblGameDir.Size = new System.Drawing.Size(60, 15);
            this.lblGameDir.Text = "游戏目录：";

            //
            // txtGameDir
            //
            this.txtGameDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGameDir.Location = new System.Drawing.Point(80, 47);
            this.txtGameDir.Name = "txtGameDir";
            this.txtGameDir.Size = new System.Drawing.Size(650, 23);
            this.txtGameDir.TabIndex = 3;

            //
            // btnBrowse
            //
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.BackColor = ColorSchemes.Release.Primary;
            this.btnBrowse.ForeColor = System.Drawing.Color.White;
            this.btnBrowse.Location = new System.Drawing.Point(740, 45);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(80, 26);
            this.btnBrowse.TabIndex = 4;
            this.btnBrowse.Text = "浏览";
            this.btnBrowse.BorderRadius = 6;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);

            //
            // tabDownloads
            //
            this.tabDownloads.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.tabDownloads.Controls.Add(this.panelDownloadControls);
            this.tabDownloads.Controls.Add(this.panelDownloadList);
            this.tabDownloads.Controls.Add(this.overallProgress);
            this.tabDownloads.Controls.Add(this.lblOverallStatus);
            this.tabDownloads.Location = new System.Drawing.Point(4, 24);
            this.tabDownloads.Name = "tabDownloads";
            this.tabDownloads.Padding = new System.Windows.Forms.Padding(10);
            this.tabDownloads.Size = new System.Drawing.Size(892, 522);
            this.tabDownloads.TabIndex = 1;
            this.tabDownloads.Text = "下载管理";

            //
            // panelDownloadControls
            //
            this.panelDownloadControls.BackColor = System.Drawing.Color.White;
            this.panelDownloadControls.Controls.Add(this.lblActiveDownloads);
            this.panelDownloadControls.Controls.Add(this.lblSpeedLimit);
            this.panelDownloadControls.Controls.Add(this.cmbSpeedLimit);
            this.panelDownloadControls.Controls.Add(this.lblConcurrentDownloads);
            this.panelDownloadControls.Controls.Add(this.cmbConcurrentDownloads);
            this.panelDownloadControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelDownloadControls.Location = new System.Drawing.Point(10, 10);
            this.panelDownloadControls.Name = "panelDownloadControls";
            this.panelDownloadControls.Padding = new System.Windows.Forms.Padding(10);
            this.panelDownloadControls.Size = new System.Drawing.Size(872, 60);
            this.panelDownloadControls.TabIndex = 0;

            //
            // lblActiveDownloads
            //
            this.lblActiveDownloads.AutoSize = true;
            this.lblActiveDownloads.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblActiveDownloads.Location = new System.Drawing.Point(13, 20);
            this.lblActiveDownloads.Name = "lblActiveDownloads";
            this.lblActiveDownloads.Size = new System.Drawing.Size(150, 19);
            this.lblActiveDownloads.Text = "活跃: 0 | 队列: 0";

            //
            // lblSpeedLimit
            //
            this.lblSpeedLimit.AutoSize = true;
            this.lblSpeedLimit.Location = new System.Drawing.Point(500, 22);
            this.lblSpeedLimit.Name = "lblSpeedLimit";
            this.lblSpeedLimit.Size = new System.Drawing.Size(70, 15);
            this.lblSpeedLimit.Text = "速度限制：";

            //
            // cmbSpeedLimit
            //
            this.cmbSpeedLimit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSpeedLimit.FormattingEnabled = true;
            this.cmbSpeedLimit.Items.AddRange(new object[] { "Unlimited", "1 MB/s", "2 MB/s", "5 MB/s", "10 MB/s" });
            this.cmbSpeedLimit.Location = new System.Drawing.Point(580, 19);
            this.cmbSpeedLimit.Name = "cmbSpeedLimit";
            this.cmbSpeedLimit.Size = new System.Drawing.Size(100, 23);
            this.cmbSpeedLimit.TabIndex = 2;
            this.cmbSpeedLimit.SelectedIndex = 0;
            this.cmbSpeedLimit.SelectedIndexChanged += new System.EventHandler(this.cmbSpeedLimit_SelectedIndexChanged);

            //
            // lblConcurrentDownloads
            //
            this.lblConcurrentDownloads.AutoSize = true;
            this.lblConcurrentDownloads.Location = new System.Drawing.Point(700, 22);
            this.lblConcurrentDownloads.Name = "lblConcurrentDownloads";
            this.lblConcurrentDownloads.Size = new System.Drawing.Size(60, 15);
            this.lblConcurrentDownloads.Text = "并行数：";

            //
            // cmbConcurrentDownloads
            //
            this.cmbConcurrentDownloads.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbConcurrentDownloads.FormattingEnabled = true;
            this.cmbConcurrentDownloads.Items.AddRange(new object[] { "1", "2", "3", "4", "6", "8" });
            this.cmbConcurrentDownloads.Location = new System.Drawing.Point(770, 19);
            this.cmbConcurrentDownloads.Name = "cmbConcurrentDownloads";
            this.cmbConcurrentDownloads.Size = new System.Drawing.Size(60, 23);
            this.cmbConcurrentDownloads.TabIndex = 4;
            this.cmbConcurrentDownloads.SelectedIndex = 3;
            this.cmbConcurrentDownloads.SelectedIndexChanged += new System.EventHandler(this.cmbConcurrentDownloads_SelectedIndexChanged);

            //
            // panelDownloadList
            //
            this.panelDownloadList.AutoScroll = true;
            this.panelDownloadList.BackColor = System.Drawing.Color.Transparent;
            this.panelDownloadList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDownloadList.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.panelDownloadList.Location = new System.Drawing.Point(10, 110);
            this.panelDownloadList.Name = "panelDownloadList";
            this.panelDownloadList.Padding = new System.Windows.Forms.Padding(5);
            this.panelDownloadList.Size = new System.Drawing.Size(872, 362);
            this.panelDownloadList.TabIndex = 1;
            this.panelDownloadList.WrapContents = false;

            //
            // overallProgress
            //
            this.overallProgress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.overallProgress.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.overallProgress.Location = new System.Drawing.Point(10, 472);
            this.overallProgress.Name = "overallProgress";
            this.overallProgress.Size = new System.Drawing.Size(872, 30);
            this.overallProgress.TabIndex = 2;
            this.overallProgress.BorderRadius = 6;
            this.overallProgress.ShowPercentage = true;

            //
            // lblOverallStatus
            //
            this.lblOverallStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblOverallStatus.Location = new System.Drawing.Point(10, 502);
            this.lblOverallStatus.Name = "lblOverallStatus";
            this.lblOverallStatus.Size = new System.Drawing.Size(872, 15);
            this.lblOverallStatus.TabIndex = 3;
            this.lblOverallStatus.Text = "暂无下载任务";

            //
            // tabSettings
            //
            this.tabSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.tabSettings.Controls.Add(this.panelSettings);
            this.tabSettings.Location = new System.Drawing.Point(4, 24);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(10);
            this.tabSettings.Size = new System.Drawing.Size(892, 522);
            this.tabSettings.TabIndex = 2;
            this.tabSettings.Text = "设置";

            //
            // panelSettings
            //
            this.panelSettings.BackColor = System.Drawing.Color.White;
            this.panelSettings.Controls.Add(this.chkAutoUpdate);
            this.panelSettings.Controls.Add(this.chkKeepLauncherOpen);
            this.panelSettings.Controls.Add(this.lblJavaPath);
            this.panelSettings.Controls.Add(this.txtJavaPath);
            this.panelSettings.Controls.Add(this.btnBrowseJava);
            this.panelSettings.Controls.Add(this.lblMemory);
            this.panelSettings.Controls.Add(this.cmbMemory);
            this.panelSettings.Controls.Add(this.btnSaveSettings);
            this.panelSettings.Location = new System.Drawing.Point(10, 10);
            this.panelSettings.Name = "panelSettings";
            this.panelSettings.Padding = new System.Windows.Forms.Padding(20);
            this.panelSettings.Size = new System.Drawing.Size(500, 350);
            this.panelSettings.TabIndex = 0;

            //
            // chkAutoUpdate
            //
            this.chkAutoUpdate.AutoSize = true;
            this.chkAutoUpdate.Location = new System.Drawing.Point(23, 23);
            this.chkAutoUpdate.Name = "chkAutoUpdate";
            this.chkAutoUpdate.Size = new System.Drawing.Size(180, 19);
            this.chkAutoUpdate.TabIndex = 0;
            this.chkAutoUpdate.Text = "启动时检查更新";
            this.chkAutoUpdate.UseVisualStyleBackColor = true;

            //
            // chkKeepLauncherOpen
            //
            this.chkKeepLauncherOpen.AutoSize = true;
            this.chkKeepLauncherOpen.Location = new System.Drawing.Point(23, 48);
            this.chkKeepLauncherOpen.Name = "chkKeepLauncherOpen";
            this.chkKeepLauncherOpen.Size = new System.Drawing.Size(200, 19);
            this.chkKeepLauncherOpen.TabIndex = 1;
            this.chkKeepLauncherOpen.Text = "游戏启动后保持启动器开启";
            this.chkKeepLauncherOpen.UseVisualStyleBackColor = true;

            //
            // lblJavaPath
            //
            this.lblJavaPath.AutoSize = true;
            this.lblJavaPath.Location = new System.Drawing.Point(23, 90);
            this.lblJavaPath.Name = "lblJavaPath";
            this.lblJavaPath.Size = new System.Drawing.Size(60, 15);
            this.lblJavaPath.Text = "Java 路径：";

            //
            // txtJavaPath
            //
            this.txtJavaPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtJavaPath.Location = new System.Drawing.Point(90, 87);
            this.txtJavaPath.Name = "txtJavaPath";
            this.txtJavaPath.Size = new System.Drawing.Size(300, 23);
            this.txtJavaPath.TabIndex = 3;

            //
            // btnBrowseJava
            //
            this.btnBrowseJava.BackColor = ColorSchemes.Release.Primary;
            this.btnBrowseJava.ForeColor = System.Drawing.Color.White;
            this.btnBrowseJava.Location = new System.Drawing.Point(400, 85);
            this.btnBrowseJava.Name = "btnBrowseJava";
            this.btnBrowseJava.Size = new System.Drawing.Size(60, 26);
            this.btnBrowseJava.TabIndex = 4;
            this.btnBrowseJava.Text = "Browse";
            this.btnBrowseJava.BorderRadius = 6;
            this.btnBrowseJava.Click += new System.EventHandler(this.btnBrowseJava_Click);

            //
            // lblMemory
            //
            this.lblMemory.AutoSize = true;
            this.lblMemory.Location = new System.Drawing.Point(23, 130);
            this.lblMemory.Name = "lblMemory";
            this.lblMemory.Size = new System.Drawing.Size(50, 15);
            this.lblMemory.Text = "内存：";

            //
            // cmbMemory
            //
            this.cmbMemory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMemory.FormattingEnabled = true;
            this.cmbMemory.Items.AddRange(new object[] { "1 GB", "2 GB", "4 GB", "6 GB", "8 GB", "12 GB", "16 GB" });
            this.cmbMemory.Location = new System.Drawing.Point(90, 127);
            this.cmbMemory.Name = "cmbMemory";
            this.cmbMemory.Size = new System.Drawing.Size(120, 23);
            this.cmbMemory.TabIndex = 6;
            this.cmbMemory.SelectedIndex = 1;

            //
            // btnSaveSettings
            //
            this.btnSaveSettings.BackColor = ColorSchemes.Success;
            this.btnSaveSettings.ForeColor = System.Drawing.Color.White;
            this.btnSaveSettings.Location = new System.Drawing.Point(23, 280);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(120, 35);
            this.btnSaveSettings.TabIndex = 7;
            this.btnSaveSettings.Text = "保存设置";
            this.btnSaveSettings.BorderRadius = 6;
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);

            //
            // tabAbout
            //
            this.tabAbout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.tabAbout.Controls.Add(this.lblAboutTitle);
            this.tabAbout.Controls.Add(this.txtAbout);
            this.tabAbout.Location = new System.Drawing.Point(4, 24);
            this.tabAbout.Name = "tabAbout";
            this.tabAbout.Padding = new System.Windows.Forms.Padding(20);
            this.tabAbout.Size = new System.Drawing.Size(892, 522);
            this.tabAbout.TabIndex = 3;
            this.tabAbout.Text = "关于";

            //
            // lblAboutTitle
            //
            this.lblAboutTitle.AutoSize = true;
            this.lblAboutTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblAboutTitle.Location = new System.Drawing.Point(23, 23);
            this.lblAboutTitle.Name = "lblAboutTitle";
            this.lblAboutTitle.Size = new System.Drawing.Size(180, 30);
            this.lblAboutTitle.Text = "我的世界启动器";

            //
            // txtAbout
            //
            this.txtAbout.BackColor = System.Drawing.Color.White;
            this.txtAbout.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAbout.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtAbout.Location = new System.Drawing.Point(23, 60);
            this.txtAbout.Name = "txtAbout";
            this.txtAbout.ReadOnly = true;
            this.txtAbout.Size = new System.Drawing.Size(846, 400);
            this.txtAbout.TabIndex = 1;
            this.txtAbout.Text = "一款现代化的我的世界启动器，支持版本管理、并行下载等功能。\r\n\r\n功能特性：\r\n- 版本筛选（正式版/快照版/愚人节版）\r\n- 并行下载管理器\r\n- 带宽限制控制\r\n- 文件自动校验\r\n- 现代化界面设计\r\n\r\n版本：1.0.0\r\n\r\n本启动器使用 BMCLAPI 获取版本列表和下载文件。";

            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.headerPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "我的世界启动器";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.tabMain.ResumeLayout(false);
            this.tabVersions.ResumeLayout(false);
            this.tabDownloads.ResumeLayout(false);
            this.tabSettings.ResumeLayout(false);
            this.tabAbout.ResumeLayout(false);
            this.tabAbout.PerformLayout();
            this.panelVersionFilter.ResumeLayout(false);
            this.panelVersionInfo.ResumeLayout(false);
            this.panelVersionInfo.PerformLayout();
            this.panelLaunchSettings.ResumeLayout(false);
            this.panelLaunchSettings.PerformLayout();
            this.panelDownloadControls.ResumeLayout(false);
            this.panelDownloadControls.PerformLayout();
            this.panelSettings.ResumeLayout(false);
            this.panelSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabVersions;
        private System.Windows.Forms.TabPage tabDownloads;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.TabPage tabAbout;

        // Version tab
        private System.Windows.Forms.Panel panelVersionFilter;
        private ModernButton btnFilterAll;
        private ModernButton btnFilterRelease;
        private ModernButton btnFilterSnapshot;
        private ModernButton btnFilterAprilFool;
        private System.Windows.Forms.FlowLayoutPanel panelVersionList;
        private System.Windows.Forms.Panel panelVersionInfo;
        private System.Windows.Forms.Label lblVersionTitle;
        private System.Windows.Forms.Label lblVersionType;
        private System.Windows.Forms.RichTextBox txtVersionInfo;
        private ModernButton btnDownloadVersion;
        private ModernButton btnLaunchVersion;
        private System.Windows.Forms.Panel panelLaunchSettings;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblGameDir;
        private System.Windows.Forms.TextBox txtGameDir;
        private ModernButton btnBrowse;

        // Download tab
        private System.Windows.Forms.Panel panelDownloadControls;
        private System.Windows.Forms.Label lblActiveDownloads;
        private System.Windows.Forms.Label lblSpeedLimit;
        private System.Windows.Forms.ComboBox cmbSpeedLimit;
        private System.Windows.Forms.Label lblConcurrentDownloads;
        private System.Windows.Forms.ComboBox cmbConcurrentDownloads;
        private System.Windows.Forms.FlowLayoutPanel panelDownloadList;
        private ModernProgressBar overallProgress;
        private System.Windows.Forms.Label lblOverallStatus;

        // Settings tab
        private System.Windows.Forms.Panel panelSettings;
        private System.Windows.Forms.CheckBox chkAutoUpdate;
        private System.Windows.Forms.CheckBox chkKeepLauncherOpen;
        private System.Windows.Forms.Label lblJavaPath;
        private System.Windows.Forms.TextBox txtJavaPath;
        private ModernButton btnBrowseJava;
        private System.Windows.Forms.Label lblMemory;
        private System.Windows.Forms.ComboBox cmbMemory;
        private ModernButton btnSaveSettings;

        // About tab
        private System.Windows.Forms.Label lblAboutTitle;
        private System.Windows.Forms.RichTextBox txtAbout;
    }
}
