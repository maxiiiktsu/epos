using System;
using System.Windows.Forms;

namespace epos
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            // vycentrovanie panelu
            CenterCard();
            this.Resize += LoginForm_Resize;
        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            txtCode.Focus();
            CenterCard();
        }

        private void LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void LoginForm_Resize(object sender, EventArgs e)
        {
            CenterCard();
        }

        private void CenterCard()
        {
            if (cardPanel == null) return;

            cardPanel.Left = (this.ClientSize.Width - cardPanel.Width) / 2;
            cardPanel.Top = (this.ClientSize.Height - cardPanel.Height) / 2;
        }

        // ====== LOGIN CLICK (bez databázy) ======
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            var code = txtCode.Text.Trim();

            if (string.IsNullOrWhiteSpace(code))
            {
                lblError.Text = "Prosím, zadajte kód.";
                txtCode.Focus();
                return;
            }

            // žiadna validácia – hneď otvoríme Form1
            var main = new Form1();
            main.FormClosed += (_, __) => Application.Exit();

            Hide();
            main.Show();
        }
    }
}
