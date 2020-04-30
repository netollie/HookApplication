using System;
using System.Windows.Forms;

namespace HookApplication
{
    public partial class Main : Form
    {
        /// <summary>
        /// global hook
        /// </summary>
        private KeyboardHook kh;

        /// <summary>
        /// entry
        /// </summary>
        public Main()
        {
            InitializeComponent();
        }

        /// <summary>
        /// before
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Load(object sender, EventArgs e)
        {
            kh = new KeyboardHook();
            kh.SetHook();
            kh.OnKeyDownEvent += kh_OnKeyDownEvent;
        }

        /// <summary>
        /// after
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Closing(object sender, FormClosingEventArgs e)
        {
            kh.UnHook();
        }

        /// <summary>
        /// catch keyboard action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void kh_OnKeyDownEvent(object sender, KeyEventArgs e)
        {
            this.output.Text = e.KeyValue.ToString();

            if (e.KeyData == (Keys.S | Keys.Control)) {
                this.Show();
            }

            if (e.KeyData == (Keys.H | Keys.Control)) {
                this.Hide();
            }
        }
    }
}
