using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace SRTF
{
    public partial class frmSJF : MetroForm
    {
        public frmSJF()
        {
            InitializeComponent();
        }
        List<process> processes = new List<process>();
        private void txtname_Enter(object sender, EventArgs e)
        {

            TextBox txt = (TextBox)sender;
            if (txt.Text == txt.DefaultText())
            {
                txt.Text = "";
                txt.ForeColor = Color.Black;
            }
        }

        private void txtname_Leave(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            if (txt.Text == "")
            {
                txt.Text = txt.DefaultText();
                txt.ForeColor = Color.LightGray;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                processes.Add(new process { name = txtname.Text.Trim(), burst = int.Parse(txtburst.Text), arrival = int.Parse(txtarrival.Text), remaining = int.Parse(txtburst.Text) });
                dgvProcesses.Rows.Add(txtname.Text.Trim(), txtburst.Text.Trim(), txtarrival.Text.Trim());
                lblMsg.ForeColor = Color.Green; lblMsg.Text = "succeed";
            }
            catch (Exception) { lblMsg.ForeColor = Color.PaleVioletRed; lblMsg.Text = "invalid input";  }
            foreach (TextBox txt in this.Controls.OfType<TextBox>())
                txt.Text = "";
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            int count = 0;
            foreach (TextBox txt in this.Controls.OfType<TextBox>())
            {
                if (txt.Text != txt.DefaultText() && txt.Text != "" && ++count > 2)
                btnAdd.Enabled = true;
                else  btnAdd.Enabled = false;
                //
                //
                //
                if (processes.Count == 0)
                    btnSchdle.Enabled = false;
                else btnSchdle.Enabled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //test samples
            List<process> processes1 = new List<process> {
                new process { name = "p2", burst = 4, arrival = 2, remaining = 4 },
                new process { name = "p4", burst = 4, arrival = 5, remaining = 4 },
                new process { name = "p1", burst = 7, arrival = 0, remaining = 7 },
                new process { name = "p3", burst = 1, arrival = 4 , remaining = 1}
                //
                //
                //new process {name = "p1", burst = 12, arrival = 0 },
                //new process {name = "p2", burst = 19, arrival = 5 },
                //new process {name = "p3", burst = 21, arrival = 8 },
                //new process {name = "p4", burst = 13, arrival = 11 },
                //new process {name = "p5", burst = 15, arrival = 15 }
                //1  4  5  2  3
                };

            List<process>  processes2 = new List<process> {
                new process { name = "p1", burst = 6, arrival = 0, remaining = 6 },
                new process { name = "p2", burst = 2, arrival = 1, remaining = 2 },
                new process { name = "p3", burst = 8, arrival = 2, remaining = 8 },
                new process { name = "p4", burst = 4, arrival = 3, remaining = 4 }

                };
            //processes = processes1;
            processes = new List<process>();
        }

        void fillTable()
        {
            int avWait = 0, avTA = 0;
            dgvProcesses.Rows.Clear();
            dgvProcesses.Width = 400;
            dgvProcesses.Columns.AddRange(new DataGridViewTextBoxColumn { Name = "start" }, new DataGridViewTextBoxColumn { Name = "Wait" }, new DataGridViewTextBoxColumn { Name = "finish" }, new DataGridViewTextBoxColumn { Name = "TA" });
            foreach (process p in process.Terminated)
            {
                if (p.isClone)
                    continue;

                int wait = - p.arrival + (p.finish - p.burst); //  (p.finish - p.burst) for preempted processes
                int turnAround = (p.finish - p.arrival);
                dgvProcesses.Rows.Add(p.name, p.burst, p.arrival, p.start, wait, p.finish, turnAround);
                avWait += wait;
                avTA += turnAround;

            }
            dgvProcesses.Sort(dgvProcesses.Columns.GetFirstColumn(DataGridViewElementStates.None), ListSortDirection.Ascending);
            int n = process.Terminated.Count(p => !p.isClone);
            lblW.Text = ($"av(Wait) = {avWait/ n}");
            lblTA.Text = ($"av(TA) = {avTA / n}");
        }
        void drawGanttChart()
        {
            foreach (process p in process.Terminated.Reverse())
            {
                DataGridViewColumn c = new DataGridViewTextBoxColumn { Name = p.name, Width = Math.Max(20 + p.burst * 10, 30) };
                dgvGanttChart.Columns.Add(c);
                dgvGanttChart.Width += c.Width;
            }
            dgvGanttChart.Visible = true;
            this.Paint += new PaintEventHandler((object sender, PaintEventArgs e) =>
            {
                Graphics timeGrpcs = e.Graphics;
                foreach (process p in process.Terminated.Reverse())
                {
                    if (p == process.Terminated.Reverse().First())
                    {
                        timeGrpcs.DrawString(p.start.ToString(), new Font("consolas", 10), new SolidBrush(Color.Gray), 270, 381);
                        timeGrpcs.DrawString(p.finish.ToString(), new Font("consolas", 10), new SolidBrush(Color.Gray), 270 + (20 + p.burst * 10), 381);
                    }
                    else
                        timeGrpcs.DrawString(p.finish.ToString(), new Font("consolas", 10), new SolidBrush(Color.Gray), 270 + (p.finish * 10 + 20 + p.burst * 10), 381);
                }
            });
        }

        private void btnSchdle_Click(object sender, EventArgs e)
        {
            process.scheduleSJF(processes, tglPree.Checked );

            fillTable();
            drawGanttChart();
            lblMsg.Text = "Click To Reset";
        }

        private void lblMsg_Click(object sender, EventArgs e)
        {
            processes?.Clear();
            process.Terminated?.Clear();
            lblMsg.Text = "Ready";
            lblW.Text = "";
            lblTA.Text = "";
            dgvProcesses.Columns.Clear();
            dgvProcesses.Columns.AddRange(new DataGridViewTextBoxColumn { Name = "name" }, new DataGridViewTextBoxColumn { Name = "burst" }, new DataGridViewTextBoxColumn { Name = "arrival" });
            dgvProcesses.Width -= 200;
            dgvGanttChart.Columns.Clear();
            this.CreateGraphics().Clear(Color.White);
            //Size = new Size(width:500,height:490);
        }
    }
}
