using ClosedXML.Excel;
using LibVLCSharp.Shared;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
namespace ComartTracking
{
    public partial class MainForm : Form
    {
        CodeReader reader = new CodeReader();
        LotRecord currentLot = new LotRecord("", 0, "","", DateTime.Now);
        MySqlConnection conn;
        string mySQLConnectionString;
        string LotID;
        LotState lotState;
        RunState runState;
        BoxState boxState;
        Camera camera = new Camera("192.168.1.64", 8000, "admin", "legion@25");
        string filePath = "";
        int count = 0;
        Conveyor conveyor;
        private bool bAutoCloseLot = false;
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
            if (IsLastLotEndTimeNull())
            {
                string info = "Chưa kết thúc Lot trước, Bạn có muốn tiếp tục không?";
                using (FrmShowDialog frm = new FrmShowDialog(info))
                {
                    frm.ShowDialog();
                    if (frm.DialogResult == DialogResult.Yes)
                    {
                        LotID = GetLastLotID();
                        int defincount = int.Parse(GetDefineCountLotID(LotID));
                        if (LotID == null)
                        {
                            MessageBox.Show("Không tìm thấy Lot trước, vui lòng tạo Lot mới");
                            runState = RunState.STOP;
                            lotState = LotState.END;
                            btn_NewLot.Text = "Tạo Lot mới";
                            tb_LotNo.Enabled = true;
                            tb_ProductID.Enabled = true;
                            num_BoxCount.Enabled = true;
                            generateNewLot();
                        }
                        else
                        {
                            count = GetLastBoxNoForLot(LotID);
                            if (count < 0) count = 0; //if no box found, set count to 0 
                            tb_LotNo.Text = LotID;
                            UpdateLabel(label5, "Count: " + count + "/" + defincount);
                            lotState = LotState.RUNNING;
                            currentLot = new LotRecord(LotID, (int)num_BoxCount.Value, tb_ProductID.Text,txtCustomer.Text, DateTime.Now);
                            btn_NewLot.Text = "Kết thúc Lot";
                            tb_LotNo.Enabled = false;
                            tb_ProductID.Enabled = false;
                            num_BoxCount.Value = defincount;
                            num_BoxCount.Enabled = false;
                        }
                    }
                    else
                    {
                        LotID = GetLastLotID();
                        count = GetLastBoxNoForLot(LotID);
                        if (count < 0) count = 0;
                        DateTime dateTime = DateTime.Now;
                        UpdateDateTimeEnd(LotID, dateTime, count);
                    }
                }
            }
        }
        #region Code read event handler
        private void Reader_OnCodeRead(object? sender, string e)
        {
            processCode(e);
        }
        void saveLotwithoutEndtime()
        {
            currentLot.realCount = count;
            // Save the current lot to the database
            MySqlCommand cmd = new MySqlCommand("INSERT INTO LotRecord (LotID, PartID, DateTimeStart, DateTimeEnd, DefinedCount, RealCount) VALUES (@LotID, @PartID, @DateTimeStart, @DateTimeEnd, @DefinedCount, @RealCount)", conn);
            cmd.Parameters.AddWithValue("@LotID", currentLot.LotID);
            cmd.Parameters.AddWithValue("@PartID", currentLot.PartID);
            cmd.Parameters.AddWithValue("@DateTimeStart", currentLot.startTime);
            cmd.Parameters.AddWithValue("@DateTimeEnd", null);
            cmd.Parameters.AddWithValue("@DefinedCount", currentLot.boxCount);
            cmd.Parameters.AddWithValue("@RealCount", currentLot.realCount);
            cmd.ExecuteNonQuery();
        }
        public bool IsLastLotEndTimeNull()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT DateTimeEnd FROM LotRecord ORDER BY DateTimeStart DESC, LotID DESC LIMIT 1", conn);
            object result = cmd.ExecuteScalar();

            

            if (result == DBNull.Value || result == null)
            {
                return true; // DateTimeEnd là NULL
            }
            else
            {
                return false; // DateTimeEnd không phải là NULL
            }
        }
        public string GetLastLotID()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT LotID FROM LotRecord ORDER BY DateTimeStart DESC, LotID DESC LIMIT 1", conn);
            object result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                return result.ToString(); // Trả về LotID nếu không phải NULL
            }
            else
            {
                return string.Empty; // Trả về chuỗi rỗng nếu không có LotID
            }
        }
        public string GetDefineCountLotID(string lotId)
        {
            try
            {
                string query = "SELECT DefinedCount FROM comart.lotrecord WHERE LotID = @LotID";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@LotID", lotId); // Truyền giá trị LotID vào tham số

                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return result.ToString(); // Trả về LotID nếu không phải NULL
                }
                else
                {
                    return string.Empty; // Trả về chuỗi rỗng nếu không có LotID
                }
            }
            catch (Exception)
            {
                return string.Empty; // Trả về chuỗi rỗng nếu không có LotID
            }
            
        }
        public int GetLastBoxNoForLot(string lotId) // Giả sử LotID là string, nếu là int thì đổi kiểu dữ liệu
        {
            int lastBoxNo = -1; // Giá trị mặc định nếu không tìm thấy hoặc có lỗi
            try
            {

                // Câu lệnh SQL đã sửa đổi để lọc theo LotID
                string query = "SELECT BoxNo FROM BoxRecord WHERE LotID = @LotID ORDER BY DateTime DESC, BoxID DESC LIMIT 1;";
                // Ghi chú: Nếu BoxID không phải là cột ID tự tăng, hãy bỏ "BoxID DESC" khỏi ORDER BY.

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@LotID", lotId); // Truyền giá trị LotID vào tham số

                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    lastBoxNo = Convert.ToInt32(result); // Chuyển đổi kết quả sang kiểu int
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi hoặc xử lý theo cách phù hợp với ứng dụng của bạn
                Console.WriteLine("Lỗi khi lấy BoxNo cuối cùng: " + ex.Message);
            }
            return lastBoxNo;
        }
        public void UpdateDateTimeEnd(string lotId, DateTime newDateTimeEnd, int count)
        {
            try
            {
                string query = "UPDATE LotRecord SET DateTimeEnd = @NewDateTime, RealCount = @RealCount WHERE LotID = @LotID;";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@LotID", lotId);
                cmd.Parameters.AddWithValue("@NewDateTime", newDateTimeEnd);
                cmd.Parameters.AddWithValue("@RealCount", count);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi cập nhật DateTimeEnd: " + ex.Message);
            }
        }

        void processCode(string e)
        {
            if (runState == RunState.STOP)
                return;
            if (e == "ERROR\r")
            {
                //UpdateButton(btn_Start, Color.Red);
                runState = RunState.STOP;
                boxState = BoxState.ERROR;
                conveyor.turnOff();
                tb_ReEnterCode.Focus();
                return;
            }
            else
            {
                boxState = BoxState.OK;
                //UpdateButton(btn_Start, Color.Green);
            }

            count++;
            //using the delegate to update the label
            UpdateLabel(label4, "Code: " + e);
            UpdateLabel(label5, "Count: " + count + "/" + currentLot.boxCount);
            //if the lot is running, save the box
            if (lotState == LotState.RUNNING)
            {
                //write the box code to camera overlay
                string[] overlap = new string[3];
                overlap[0] = "Customer: " + currentLot.CustomerID;
                overlap[1] = "Part ID: " + currentLot.PartID; 
                overlap[2] = "Code: " + e + ", Count: " + count + "/" + currentLot.boxCount;

                if(!camera.setOSD(overlap))
                {
                    //UpdateButton(btn_Start, Color.Red);
                    runState = RunState.STOP;
                    boxState = BoxState.ERROR;
                    conveyor.turnOff();
                    tb_ReEnterCode.Focus();
                    count--;
                    return;
                }    
               //camera.setText(e);
               //save the box to the database
               saveBox(e, currentLot.LotID, currentLot.PartID);
            }
            if(bAutoCloseLot)
            {
                if(count == currentLot.boxCount)
                {
                    runState = RunState.STOP;
                    endLot();
                    saveLot();
                    lotState = LotState.END;
                    btn_NewLot.Text = "Tạo Lot mới";
                    tb_LotNo.Enabled = true;
                    tb_ProductID.Enabled = true;
                    num_BoxCount.Enabled = true;
                    generateNewLot();
                    camera.setOSD(new string[1] { "Idel" });
                    conveyor.turnOff();
                }    
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
                    UpdateDateTimeEnd(LotID, currentLot.endTime, (int)num_BoxCount.Value);
                    lotState = LotState.END;
                    btn_NewLot.Text = "Tạo Lot mới";
                    tb_LotNo.Enabled = true;
                    tb_ProductID.Enabled = true;
                    num_BoxCount.Enabled = true;
                    generateNewLot();
                    camera.setOSD(new string[1] {"Idel"});
                    break;
                case LotState.END:
                    if ((int)num_BoxCount.Value == 0)
                    {
                        MessageBox.Show("Số lượng thùng cần lớn hơn 0!!!!");
                        return;
                    }
                    count = 0;
                    lotState = LotState.RUNNING;
                    currentLot = new LotRecord(LotID, (int)num_BoxCount.Value, tb_ProductID.Text,txtCustomer.Text, DateTime.Now);
                    label4.Text = "Code:";
                    label5.Text = $"Count: 0/{(int)num_BoxCount.Value}";
                    btn_NewLot.Text = "Kết thúc Lot";
                    tb_LotNo.Enabled = false;
                    //tb_ProductID.Enabled = false;
                    num_BoxCount.Enabled = false;
                    saveLotwithoutEndtime();
                    break;
            }
        }

        void generateNewLot()
        {
            LotID = "";
            tb_LotNo.Text = DateTime.Now.ToString("yyyyMMddHHmmss");
            LotID = DateTime.Now.ToString("yyyyMMddHHmmss");
        }
        private void btn_Start_Click(object sender, EventArgs e)
        {
            if(lotState != LotState.RUNNING)
            {
                MessageBox.Show("Bạn cần tạo lot mới trước khi chạy băng tải.");
                return;
            }    
            if (runState == RunState.START)
            {
                runState = RunState.STOP;
                conveyor.turnOff();
            }
            else
            {
                runState = RunState.START;
                conveyor.turnOn();
            }
        }
        #endregion


        #region Flow control

        void init()
        {
            conveyor = new Conveyor("COM4", 9600);
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
                conveyor.turnOn();
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
            }
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
                {
                    int percent = CHCNetSDK.NET_DVR_GetDownloadPos(0);
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

        private void btn_exportExcel_Click(object sender, EventArgs e)
        {
            //export the box list in current lot to an excel file
            if (dg_Boxes.Rows.Count == 0)
            {
                MessageBox.Show("Chưa có Box nào được chọn");
                return;
            }
            //pop up a save file dialog to save the excel file
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            saveFileDialog.Title = "Save Excel File";
            saveFileDialog.FileName = "BoxList_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            saveFileDialog.FilterIndex = 0;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                //create a new excel file
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Box List");
                    //add the header
                    for (int i = 0; i < dg_Boxes.Columns.Count; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = dg_Boxes.Columns[i].HeaderText;
                    }
                    //add the data
                    for (int i = 0; i < dg_Boxes.Rows.Count; i++)
                    {
                        for (int j = 0; j < dg_Boxes.Columns.Count; j++)
                        {
                            var cellValue = dg_Boxes.Rows[i].Cells[j].Value;
                            worksheet.Cell(i + 2, j + 1).Value = cellValue != null ? cellValue.ToString() : string.Empty;
                        }
                    }
                    workbook.SaveAs(saveFileDialog.FileName);
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            camera.setOSD( new string[1] {"Idel"});
        }
    }
}
