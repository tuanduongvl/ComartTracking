using LibVLCSharp.Shared;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
namespace ComartTracking
{
    public partial class MainForm : Form
    {
        CodeReader reader = new CodeReader();
        LotRecord currentLot = new LotRecord("", 0, "", DateTime.Now);
        MySqlConnection conn;
        string mySQLConnectionString;
        LotState lotState;
        RunState runState;
        BoxState boxState;
        Camera camera = new Camera("192.168.1.64", 8000, "admin", "legion@25");
        string filePath = "";
        int count = 0;

        enum LotState
        {
            RUNNING,
            END
        }

        enum RunState
        {
            START,
            STOP
        }

        enum BoxState
        {
            OK,
            ERROR
        }


        public MainForm()
        {
            InitializeComponent();
            init();

        }
        #region Code read event handler
        private void Reader_OnCodeRead(object? sender, string e)
        {
            processCode(e);
        }

        void processCode(string e)
        {
            if (runState == RunState.STOP)
                return;
            count++;
            //using the delegate to update the label
            UpdateLabel(label4, "Code: " + e);
            UpdateLabel(label5, "Count: " + count + "/" + currentLot.boxCount);

            //if the lot is running, save the box
            if (lotState == LotState.RUNNING)
            {
                //write the box code to camera overlay
                camera.setOSD("Code: " + e + ", Count: " + count + "/" + currentLot.boxCount);
                //camera.setText(e);
                //save the box to the database
                saveBox(e, currentLot.LotID, currentLot.PartID);
            }

            if (e == "ERROR\r")
            {
                //UpdateButton(btn_Start, Color.Red);
                runState = RunState.STOP;
                boxState = BoxState.ERROR;
            }
            else
            {
                boxState = BoxState.OK;
                //UpdateButton(btn_Start, Color.Green);
            }
        }

        #endregion

        #region UI delegate


        // Delegate for updating UI label from other threads
        private delegate void UpdateLabelDelegate(Label label, string text);

        private void UpdateLabel(Label label, string text)
        {
            // Check if the request is from a different thread
            if (label.InvokeRequired)
            {
                // If so, invoke the delegate on the UI thread
                label.Invoke(new UpdateLabelDelegate(UpdateLabel), new object[] { label, text });
            }
            else
            {
                // Update the label directly if we're on the UI thread
                label.Text = text;
            }
        }
        // Delegate for updating button color from other threads
        private delegate void UpdateButtonDelegate(Button button, Color color);

        private void UpdateButton(Button button, Color color)
        {
            // Check if the request is from a different thread
            if (button.InvokeRequired)
            {
                // If so, invoke the delegate on the UI thread
                button.Invoke(new UpdateButtonDelegate(UpdateButton), new object[] { button, color });
            }
            else
            {
                // Update the button directly if we're on the UI thread
                button.BackColor = color;
            }
        }


        #endregion

        #region UI event handlers
        private void btn_NewLot_Click(object sender, EventArgs e)
        {
            switch (lotState)
            {
                //if the lot is running, the button allow to end it, and then allow user to type in text
                //if the lot is ended, the button allow to create a new lot
                case LotState.RUNNING:
                    runState = RunState.STOP;
                    endLot();
                    saveLot();
                    lotState = LotState.END;
                    btn_NewLot.Text = "Tạo Lot mới";
                    tb_LotNo.Enabled = true;
                    tb_ProductID.Enabled = true;
                    num_BoxCount.Enabled = true;
                    generateNewLot();
                    break;
                case LotState.END:
                    count = 0;
                    lotState = LotState.RUNNING;
                    currentLot = new LotRecord(tb_LotNo.Text, (int)num_BoxCount.Value, tb_ProductID.Text, DateTime.Now);
                    btn_NewLot.Text = "Kết thúc Lot";
                    tb_LotNo.Enabled = false;
                    tb_ProductID.Enabled = false;
                    num_BoxCount.Enabled = false;
                    break;



            }
        }

        void generateNewLot()
        {
            tb_LotNo.Text = DateTime.Now.ToString("yyyyMMddHHmmss");
        }
        private void btn_Start_Click(object sender, EventArgs e)
        {
            if (runState == RunState.START)
            {
                runState = RunState.STOP;
            }
            else
            {
                runState = RunState.START;
            }
        }
        #endregion


        #region Flow control

        void init()
        {
            mySQLConnectionString = "server=localhost;user id=dev;password=dev@123;database=comart";
            conn = new MySqlConnection(mySQLConnectionString);
            lotState = LotState.END;
            runState = RunState.STOP;
            btn_NewLot.Text = "Tạo Lot mới";
            camera.connect();
            camera.syncTime();
            reader.OnCodeRead += Reader_OnCodeRead;
            conn.Open();
            timer1.Start();
            generateNewLot();
            camera.PlaybackprogressBar = vid_progress;
            camera.OnDownloadCompleted += () =>
            {
                vidPlayer.URL = filePath;
            };
        }
        void endLot()
        {
            if (currentLot != null)
                currentLot.endTime = DateTime.Now;

        }

        void saveLot()
        {
            currentLot.realCount = count;
            // Save the current lot to the database
            MySqlCommand cmd = new MySqlCommand("INSERT INTO LotRecord (LotID, PartID, DateTimeStart, DateTimeEnd, DefinedCount, RealCount) VALUES (@LotID, @PartID, @DateTimeStart, @DateTimeEnd, @DefinedCount, @RealCount)", conn);
            cmd.Parameters.AddWithValue("@LotID", currentLot.LotID);
            cmd.Parameters.AddWithValue("@PartID", currentLot.PartID);
            cmd.Parameters.AddWithValue("@DateTimeStart", currentLot.startTime);
            cmd.Parameters.AddWithValue("@DateTimeEnd", currentLot.endTime);
            cmd.Parameters.AddWithValue("@DefinedCount", currentLot.boxCount);
            cmd.Parameters.AddWithValue("@RealCount", currentLot.realCount);
            cmd.ExecuteNonQuery();
        }

        void saveBox(string boxID, string lotID, string partID)
        {
            // Save the current box to the database
            BoxRecord box = new BoxRecord(boxID, lotID, partID, DateTime.Now, count);
            MySqlCommand cmd = new MySqlCommand("INSERT INTO BoxRecord (BoxID, LotID, PartID, DateTime, BoxNo) VALUES (@BoxID, @LotID, @PartID, @DateTime, @BoxNo)", conn);
            cmd.Parameters.AddWithValue("@BoxID", box.BoxID);
            cmd.Parameters.AddWithValue("@LotID", box.LotID);
            cmd.Parameters.AddWithValue("@PartID", box.PartID);
            cmd.Parameters.AddWithValue("@DateTime", box.DateTime);
            cmd.Parameters.AddWithValue("@BoxNo", box.BoxNo);
            cmd.ExecuteNonQuery();
        }

        #endregion

        #region timer event handler
        private void timer1_Tick(object sender, EventArgs e)
        {
            //check if connection to MySQL is not dead
            if (conn.State == ConnectionState.Closed)
            {
                statusDB.Text = "Database connection lost";
                statusDB.ForeColor = Color.Red;
                conn.Open();
            }
            else
            {
                statusDB.Text = "Database connection OK";
                statusDB.ForeColor = Color.Green;
            }

            //check if runstate is Run or Stop
            if (runState == RunState.START)
            {
                btn_Start.Text = "Stop";
                statusRun.Text = "Đang chạy";
                statusRun.ForeColor = Color.Green;
            }
            else
            {
                btn_Start.Text = "Start";
                statusRun.Text = "Đang dừng";
                statusRun.ForeColor = Color.Red;
            }

            if (boxState == BoxState.OK)
            {
                statusReadError.Text = "Đọc mã OK";
                statusReadError.ForeColor = Color.Green;
            }
            else
            {
                statusReadError.Text = "Lỗi đọc mã, bấm F10 để bắn mã bằng tay";
                statusReadError.ForeColor = Color.Red;
            }
            #endregion

        }


        private void tb_ReEnterCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                runState = RunState.START;
                processCode(tb_ReEnterCode.Text);
                tb_ReEnterCode.Text = "";
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F10)
            {
                tb_ReEnterCode.Focus();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if the lot is created then save it, but if not, just return
            if (currentLot.realCount > 0)
                {
                    endLot();
                    saveLot();
                };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            camera.tryfeature();
        }

        private void btn_ListLot_Click(object sender, EventArgs e)
        {
            //read the list of lots from the database and show on dg_lot
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM LotRecord", conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dg_Lot.DataSource = dt;
            dg_Lot.Columns["LotID"].HeaderText = "Lot ID";
            dg_Lot.Columns["PartID"].HeaderText = "Part ID";
            dg_Lot.Columns["DateTimeStart"].HeaderText = "Start Time";
            dg_Lot.Columns["DateTimeEnd"].HeaderText = "End Time";
            dg_Lot.Columns["DefinedCount"].HeaderText = "Defined Count";
            dg_Lot.Columns["RealCount"].HeaderText = "Real Count";
            //auto size all columns
            dg_Lot.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }

        private void btn_SearchLot_Click(object sender, EventArgs e)
        {
            //Search the lot by the time range between dt_From and dt_To
            DateTime from = dt_From.Value;
            DateTime to = dt_To.Value;
            //set from and to to the start and end of the day
            from = from.Date;
            to = to.Date.AddDays(1).AddTicks(-1);
            //check if from is greater than to
            if (from > to)
            {
                MessageBox.Show("Thời gian bắt đầu không được lớn hơn thời gian kết thúc");
                return;
            }
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM LotRecord WHERE DateTimeStart BETWEEN @from AND @to", conn);
            cmd.Parameters.AddWithValue("@from", from);
            cmd.Parameters.AddWithValue("@to", to);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dg_Lot.DataSource = dt;
            dg_Lot.Columns["LotID"].HeaderText = "Lot ID";
            dg_Lot.Columns["PartID"].HeaderText = "Part ID";
            dg_Lot.Columns["DateTimeStart"].HeaderText = "Start Time";
            dg_Lot.Columns["DateTimeEnd"].HeaderText = "End Time";
            dg_Lot.Columns["DefinedCount"].HeaderText = "Defined Count";
            dg_Lot.Columns["RealCount"].HeaderText = "Real Count";
            //auto size all columns
            dg_Lot.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void dg_Lot_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //list all boxes in the lot into dg_Box
            //get the LotID from the selected row
            string lotID = dg_Lot.Rows[e.RowIndex].Cells["LotID"].Value.ToString();
            //get the PartID from the selected row
            string partID = dg_Lot.Rows[e.RowIndex].Cells["PartID"].Value.ToString();
            //get the start time from the selected row
            DateTime startTime = DateTime.Parse(dg_Lot.Rows[e.RowIndex].Cells["DateTimeStart"].Value.ToString());
            //get the end time from the selected row
            DateTime endTime = DateTime.Parse(dg_Lot.Rows[e.RowIndex].Cells["DateTimeEnd"].Value.ToString());
            //get the real count from the selected row
            int realCount = int.Parse(dg_Lot.Rows[e.RowIndex].Cells["RealCount"].Value.ToString());
            //get the defined count from the selected row
            int definedCount = int.Parse(dg_Lot.Rows[e.RowIndex].Cells["DefinedCount"].Value.ToString());
            //get the box list from the database
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM BoxRecord WHERE LotID = @lotID AND PartID = @partID AND DateTime BETWEEN @startTime AND @endTime", conn);
            cmd.Parameters.AddWithValue("@lotID", lotID);
            cmd.Parameters.AddWithValue("@partID", partID);
            cmd.Parameters.AddWithValue("@startTime", startTime);
            cmd.Parameters.AddWithValue("@endTime", endTime);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dg_Boxes.DataSource = dt;
            dg_Boxes.Columns["BoxID"].HeaderText = "Box ID";
            dg_Boxes.Columns["LotID"].HeaderText = "Lot ID";
            dg_Boxes.Columns["PartID"].HeaderText = "Part ID";
            dg_Boxes.Columns["DateTime"].HeaderText = "Date Time";
            dg_Boxes.Columns["BoxNo"].HeaderText = "Box No";
            //auto size all columns
            dg_Boxes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }

        private void dg_Boxes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DateTime startTime = DateTime.Parse(dg_Boxes.Rows[e.RowIndex].Cells["DateTime"].Value.ToString());
            DateTime endTime = startTime.AddSeconds(5);
            camera.downloadByTime(startTime, endTime, @"D:\temp.mp4");
            filePath = @"D:\temp.mp4";

        }

        private void btn_Play_Click(object sender, EventArgs e)
        {
            //vidPlayer.URL = @"D:\temp.mp4";
        }

        private void btn_ExportVideo_Click(object sender, EventArgs e)
        {
            if (dg_Lot.CurrentCell == null)
            {
                MessageBox.Show("Chưa có Lot nào được chọn");
                return;
            }
            //pop up a save file dialog to save the video file
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "MP4 files (*.mp4)|*.mp4|All files (*.*)|*.*";
            saveFileDialog.Title = "Save Video File";
            saveFileDialog.FileName = "video_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".mp4";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {

                DateTime startTime = DateTime.Parse(dg_Lot.Rows[dg_Lot.CurrentCell.RowIndex].Cells["DateTimeStart"].Value.ToString());
                DateTime endTime = DateTime.Parse(dg_Lot.Rows[dg_Lot.CurrentCell.RowIndex].Cells["DateTimeEnd"].Value.ToString());
                camera.downloadByTime(startTime, endTime, saveFileDialog.FileName);
            }
            Task task = new Task(() =>
            {
                while (true)
                { int percent = CHCNetSDK.NET_DVR_GetDownloadPos(0); 
                //if (percent == -1)
                //        break;
                    UpdateLabel(lbl_percent, "Download progress: " + percent.ToString() + "%");
                    if (percent == 100)
                    {
                        filePath = saveFileDialog.FileName;
                        break;
                    }
                }
            });
            task.Start();

        }
    }
}
