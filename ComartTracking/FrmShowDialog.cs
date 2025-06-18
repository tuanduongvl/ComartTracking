namespace ComartTracking
{
    public partial class FrmShowDialog : Form
    {
        public string sInfor = string.Empty;
        public FrmShowDialog(string infor)
        {
            InitializeComponent();
            sInfor = infor;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!lb_infor.Created) return;
            Color back_color;
            Color fore_color;
            if (lb_infor.BackColor == Color.Red)
            {
                back_color = Color.White;
                fore_color = Color.Black;
            }
            else
            {
                back_color = Color.Red;
                fore_color = Color.White;
            }
            if (lb_infor.InvokeRequired)
            {
                lb_infor.Invoke((MethodInvoker)delegate
                {
                    lb_infor.BackColor = back_color;
                    lb_infor.ForeColor = fore_color;
                });
            }
            else
            {
                lb_infor.BackColor = back_color;
                lb_infor.ForeColor = fore_color;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }
        private void CloseForm()
        {
            timer1.Stop();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            CloseForm();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            CloseForm();
        }

        private void FrmShowDialog_Shown(object sender, EventArgs e)
        {
            lb_infor.Text = sInfor;
        }
    }
}
