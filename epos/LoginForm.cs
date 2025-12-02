using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace epos
{
    public partial class LoginForm : Form
    {
        private Color defaultBorderColor;
        private readonly Color errorColor = Color.FromArgb(240, 80, 80);

        public LoginForm()
        {
            InitializeComponent();

            
            defaultBorderColor = txtCode.BorderColor;

           
            CenterCard();
            this.Resize += LoginForm_Resize;
            this.Shown += LoginForm_Shown;
            this.KeyDown += LoginForm_KeyDown;

            
            txtCode.TextChanged += (_, __) => ClearError();

            
            ClearError();
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

        // ====== UI helpery pre error stav ======

        private void ShowError(string message)
        {
            // text
            lblError.Text = message;
            lblError.ForeColor = errorColor;
            lblError.Visible = true;

            // červený rámček okolo textboxu (aj fokus/hover)
            txtCode.BorderColor = errorColor;
            txtCode.FocusedState.BorderColor = errorColor;
            txtCode.HoverState.BorderColor = errorColor;
        }

        private void ClearError()
        {
            lblError.Text = "";
            lblError.Visible = false;

            txtCode.BorderColor = defaultBorderColor;
            txtCode.FocusedState.BorderColor = defaultBorderColor;
            txtCode.HoverState.BorderColor = defaultBorderColor;
        }

        // ====== LOGIN CLICK – napojené na DB ======
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            ClearError();

            var code = txtCode.Text.Trim();

            if (string.IsNullOrWhiteSpace(code))
            {
                ShowError("Prosím, zadajte kód.");
                txtCode.Focus();
                return;
            }

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();

                    string sql = "SELECT id FROM cashiers WHERE code = @code LIMIT 1";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@code", code);

                        object result = cmd.ExecuteScalar();

                        if (result == null || result == DBNull.Value)
                        {
                            // kód neexistuje
                            ShowError("Zadali ste nesprávny kód");
                            txtCode.SelectAll();
                            txtCode.Focus();
                            return;
                        }
                    }
                }

                
                var main = new Form1();
                main.FormClosed += (_, __) => Application.Exit();

                Hide();
                main.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Nastala chyba pri overovaní kódu:\n" + ex.Message,
                    "Chyba databázy",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
