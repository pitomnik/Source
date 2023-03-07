using System;
using System.Windows.Forms;

using InterProcessObject;

namespace InterProcessObjectDemo
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			NextNumber();
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			TopMost = checkBox1.Checked;
		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e)
		{
			timer1.Enabled = checkBox2.Checked;
			button1.Enabled = !checkBox2.Checked;
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			NextNumber();
		}

		private void NextNumber()
		{
			using (InstanceWrapper<Counter> counter = InterProcessSingleton<Counter>.Instance.GetWrapper())
			{
				int number = counter.Instance.GetNumber();

				textBox1.Text = number.ToString();
			}
		}
	}
}