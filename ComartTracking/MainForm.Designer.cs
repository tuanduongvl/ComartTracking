namespace ComartTracking
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            btn_NewLot = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            statusStrip1 = new StatusStrip();
            statusDB = new ToolStripStatusLabel();
            statusCamera = new ToolStripStatusLabel();
            statusRun = new ToolStripStatusLabel();
            statusReadError = new ToolStripStatusLabel();
            btn_Start = new Button();
            tb_LotNo = new TextBox();
            label1 = new Label();
            label2 = new Label();
            tb_ProductID = new TextBox();
            label3 = new Label();
            num_BoxCount = new NumericUpDown();
            label4 = new Label();
            label5 = new Label();
            tb_ReEnterCode = new TextBox();
            label6 = new Label();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            vidPlayer = new AxWMPLib.AxWindowsMediaPlayer();
            lbl_percent = new Label();
            btn_ExportVideo = new Button();
            vid_progress = new ProgressBar();
            dg_Boxes = new DataGridView();
            label8 = new Label();
            label7 = new Label();
            dt_To = new DateTimePicker();
            dt_From = new DateTimePicker();
            btn_SearchLot = new Button();
            btn_ListLot = new Button();
            dg_Lot = new DataGridView();
            pictureBox1 = new PictureBox();
            btn_exportExcel = new Button();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)num_BoxCount).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)vidPlayer).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dg_Boxes).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dg_Lot).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // btn_NewLot
            // 
            btn_NewLot.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_NewLot.Font = new Font("Segoe UI", 15.75F);
            btn_NewLot.Location = new Point(633, 45);
            btn_NewLot.Name = "btn_NewLot";
            btn_NewLot.Size = new Size(185, 82);
            btn_NewLot.TabIndex = 0;
            btn_NewLot.Text = "Tạo Lot mới";
            btn_NewLot.UseVisualStyleBackColor = true;
            btn_NewLot.Click += btn_NewLot_Click;
            // 
            // timer1
            // 
            timer1.Tick += timer1_Tick;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { statusDB, statusCamera, statusRun, statusReadError });
            statusStrip1.Location = new Point(0, 1019);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1790, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // statusDB
            // 
            statusDB.Name = "statusDB";
            statusDB.Size = new Size(89, 17);
            statusDB.Text = "Database status";
            // 
            // statusCamera
            // 
            statusCamera.Name = "statusCamera";
            statusCamera.Size = new Size(48, 17);
            statusCamera.Text = "Camera";
            // 
            // statusRun
            // 
            statusRun.Name = "statusRun";
            statusRun.Size = new Size(63, 17);
            statusRun.Text = "Đang chạy";
            // 
            // statusReadError
            // 
            statusReadError.Name = "statusReadError";
            statusReadError.Size = new Size(75, 17);
            statusReadError.Text = "Lỗi đọc code";
            // 
            // btn_Start
            // 
            btn_Start.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_Start.Font = new Font("Segoe UI", 15.75F);
            btn_Start.Location = new Point(633, 184);
            btn_Start.Name = "btn_Start";
            btn_Start.Size = new Size(185, 84);
            btn_Start.TabIndex = 2;
            btn_Start.Text = "Bắt đầu";
            btn_Start.UseVisualStyleBackColor = true;
            btn_Start.Click += btn_Start_Click;
            // 
            // tb_LotNo
            // 
            tb_LotNo.Location = new Point(104, 33);
            tb_LotNo.Name = "tb_LotNo";
            tb_LotNo.Size = new Size(236, 35);
            tb_LotNo.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(8, 36);
            label1.Name = "label1";
            label1.Size = new Size(78, 30);
            label1.TabIndex = 4;
            label1.Text = "Mã Lot";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(8, 80);
            label2.Name = "label2";
            label2.Size = new Size(140, 30);
            label2.TabIndex = 6;
            label2.Text = "Mã sản phẩm";
            // 
            // tb_ProductID
            // 
            tb_ProductID.Location = new Point(154, 80);
            tb_ProductID.Name = "tb_ProductID";
            tb_ProductID.Size = new Size(455, 35);
            tb_ProductID.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(346, 35);
            label3.Name = "label3";
            label3.Size = new Size(157, 30);
            label3.TabIndex = 8;
            label3.Text = "Số lượng thùng";
            // 
            // num_BoxCount
            // 
            num_BoxCount.Location = new Point(509, 33);
            num_BoxCount.Name = "num_BoxCount";
            num_BoxCount.Size = new Size(100, 35);
            num_BoxCount.TabIndex = 9;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(8, 172);
            label4.Name = "label4";
            label4.Size = new Size(48, 30);
            label4.TabIndex = 10;
            label4.Text = "S/N";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(272, 172);
            label5.Name = "label5";
            label5.Size = new Size(96, 30);
            label5.TabIndex = 11;
            label5.Text = "Số lượng";
            // 
            // tb_ReEnterCode
            // 
            tb_ReEnterCode.Location = new Point(143, 210);
            tb_ReEnterCode.Name = "tb_ReEnterCode";
            tb_ReEnterCode.Size = new Size(317, 35);
            tb_ReEnterCode.TabIndex = 12;
            tb_ReEnterCode.KeyDown += tb_ReEnterCode_KeyDown;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(6, 213);
            label6.Name = "label6";
            label6.Size = new Size(131, 30);
            label6.TabIndex = 13;
            label6.Text = "Nhập lại mã:";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(num_BoxCount);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(tb_LotNo);
            groupBox1.Controls.Add(tb_ReEnterCode);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(tb_ProductID);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label3);
            groupBox1.Font = new Font("Segoe UI", 15.75F);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(615, 256);
            groupBox1.TabIndex = 14;
            groupBox1.TabStop = false;
            groupBox1.Text = "Vận hành";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btn_exportExcel);
            groupBox2.Controls.Add(vidPlayer);
            groupBox2.Controls.Add(lbl_percent);
            groupBox2.Controls.Add(btn_ExportVideo);
            groupBox2.Controls.Add(vid_progress);
            groupBox2.Controls.Add(dg_Boxes);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(dt_To);
            groupBox2.Controls.Add(dt_From);
            groupBox2.Controls.Add(btn_SearchLot);
            groupBox2.Controls.Add(btn_ListLot);
            groupBox2.Controls.Add(dg_Lot);
            groupBox2.Location = new Point(12, 274);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(1765, 701);
            groupBox2.TabIndex = 15;
            groupBox2.TabStop = false;
            groupBox2.Text = "Lịch sử";
            // 
            // vidPlayer
            // 
            vidPlayer.Enabled = true;
            vidPlayer.Location = new Point(1194, 22);
            vidPlayer.Name = "vidPlayer";
            vidPlayer.OcxState = (AxHost.State)resources.GetObject("vidPlayer.OcxState");
            vidPlayer.Size = new Size(556, 366);
            vidPlayer.TabIndex = 15;
            // 
            // lbl_percent
            // 
            lbl_percent.AutoSize = true;
            lbl_percent.Location = new Point(1192, 477);
            lbl_percent.Name = "lbl_percent";
            lbl_percent.Size = new Size(10, 15);
            lbl_percent.TabIndex = 14;
            lbl_percent.Text = " ";
            // 
            // btn_ExportVideo
            // 
            btn_ExportVideo.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btn_ExportVideo.Location = new Point(1191, 423);
            btn_ExportVideo.Name = "btn_ExportVideo";
            btn_ExportVideo.Size = new Size(235, 51);
            btn_ExportVideo.TabIndex = 11;
            btn_ExportVideo.Text = "Xuất video của lot";
            btn_ExportVideo.UseVisualStyleBackColor = true;
            btn_ExportVideo.Click += btn_ExportVideo_Click;
            // 
            // vid_progress
            // 
            vid_progress.Location = new Point(1192, 394);
            vid_progress.Name = "vid_progress";
            vid_progress.Size = new Size(558, 23);
            vid_progress.TabIndex = 9;
            // 
            // dg_Boxes
            // 
            dg_Boxes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dg_Boxes.Location = new Point(720, 22);
            dg_Boxes.Name = "dg_Boxes";
            dg_Boxes.Size = new Size(465, 660);
            dg_Boxes.TabIndex = 7;
            dg_Boxes.CellClick += dg_Boxes_CellClick;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(479, 151);
            label8.Name = "label8";
            label8.Size = new Size(22, 15);
            label8.TabIndex = 6;
            label8.Text = "To:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(479, 95);
            label7.Name = "label7";
            label7.Size = new Size(38, 15);
            label7.TabIndex = 5;
            label7.Text = "From:";
            // 
            // dt_To
            // 
            dt_To.CalendarFont = new Font("Segoe UI", 15.75F);
            dt_To.Location = new Point(479, 169);
            dt_To.Name = "dt_To";
            dt_To.Size = new Size(200, 23);
            dt_To.TabIndex = 4;
            // 
            // dt_From
            // 
            dt_From.CalendarFont = new Font("Segoe UI", 15.75F);
            dt_From.Location = new Point(479, 113);
            dt_From.Name = "dt_From";
            dt_From.Size = new Size(200, 23);
            dt_From.TabIndex = 3;
            // 
            // btn_SearchLot
            // 
            btn_SearchLot.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btn_SearchLot.Location = new Point(479, 265);
            btn_SearchLot.Name = "btn_SearchLot";
            btn_SearchLot.Size = new Size(235, 51);
            btn_SearchLot.TabIndex = 2;
            btn_SearchLot.Text = "Tìm kiếm";
            btn_SearchLot.UseVisualStyleBackColor = true;
            btn_SearchLot.Click += btn_SearchLot_Click;
            // 
            // btn_ListLot
            // 
            btn_ListLot.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btn_ListLot.Location = new Point(479, 22);
            btn_ListLot.Name = "btn_ListLot";
            btn_ListLot.Size = new Size(235, 51);
            btn_ListLot.TabIndex = 1;
            btn_ListLot.Text = "Lấy danh sách Lot";
            btn_ListLot.UseVisualStyleBackColor = true;
            btn_ListLot.Click += btn_ListLot_Click;
            // 
            // dg_Lot
            // 
            dg_Lot.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dg_Lot.Location = new Point(8, 22);
            dg_Lot.Name = "dg_Lot";
            dg_Lot.Size = new Size(465, 660);
            dg_Lot.TabIndex = 0;
            dg_Lot.CellClick += dg_Lot_CellClick;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.Logo;
            pictureBox1.Location = new Point(1303, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(459, 256);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 16;
            pictureBox1.TabStop = false;
            // 
            // btn_exportExcel
            // 
            btn_exportExcel.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btn_exportExcel.Location = new Point(1515, 423);
            btn_exportExcel.Name = "btn_exportExcel";
            btn_exportExcel.Size = new Size(235, 51);
            btn_exportExcel.TabIndex = 16;
            btn_exportExcel.Text = "Xuất lịch sử ra Excel";
            btn_exportExcel.UseVisualStyleBackColor = true;
            btn_exportExcel.Click += btn_exportExcel_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1790, 1041);
            Controls.Add(pictureBox1);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(btn_Start);
            Controls.Add(statusStrip1);
            Controls.Add(btn_NewLot);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Name = "MainForm";
            Text = "Comart - Hệ thống theo dõi lịch sử xuất bán";
            FormClosing += MainForm_FormClosing;
            KeyDown += MainForm_KeyDown;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)num_BoxCount).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)vidPlayer).EndInit();
            ((System.ComponentModel.ISupportInitialize)dg_Boxes).EndInit();
            ((System.ComponentModel.ISupportInitialize)dg_Lot).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btn_NewLot;
        private System.Windows.Forms.Timer timer1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel statusDB;
        private Button btn_Start;
        private TextBox tb_LotNo;
        private Label label1;
        private Label label2;
        private TextBox tb_ProductID;
        private Label label3;
        private NumericUpDown num_BoxCount;
        private ToolStripStatusLabel statusCamera;
        private Label label4;
        private Label label5;
        private ToolStripStatusLabel statusRun;
        private ToolStripStatusLabel statusReadError;
        private TextBox tb_ReEnterCode;
        private Label label6;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Button btn_ListLot;
        private DataGridView dg_Lot;
        private Button btn_SearchLot;
        private Label label8;
        private Label label7;
        private DateTimePicker dt_To;
        private DateTimePicker dt_From;
        private DataGridView dg_Boxes;
        private ProgressBar vid_progress;
        private PictureBox pictureBox1;
        private Button btn_ExportVideo;
        private Label lbl_percent;
        private AxWMPLib.AxWindowsMediaPlayer vidPlayer;
        private Button btn_exportExcel;
    }
}
