using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace epos
{
    public partial class LoginForm
    {
        private Guna2GradientPanel backgroundPanel;
        private Guna2ShadowPanel cardPanel;
        private Guna2HtmlLabel lblTitle;
        private Label lblCodeCaption;
        private Guna2TextBox txtCode;
        private Guna2Button btnLogin;
        private Guna2HtmlLabel lblError;

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // ===== LOGIN FORM =====
            this.Text = "Epos – Login";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Black;
            this.ClientSize = new Size(1280, 720);
            this.KeyPreview = true;
            this.KeyDown += LoginForm_KeyDown;
            this.Shown += LoginForm_Shown;

            // ===== BACKGROUND PANEL =====
            this.backgroundPanel = new Guna2GradientPanel();
            this.backgroundPanel.Dock = DockStyle.Fill;
            this.backgroundPanel.GradientMode = LinearGradientMode.ForwardDiagonal;
            this.backgroundPanel.FillColor = Color.FromArgb(18, 18, 18);
            this.backgroundPanel.FillColor2 = Color.FromArgb(35, 35, 35);
            this.Controls.Add(this.backgroundPanel);

            // ===== CARD PANEL =====
            this.cardPanel = new Guna2ShadowPanel();
            this.cardPanel.Size = new Size(460, 280);
            this.cardPanel.BackColor = Color.Transparent;
            this.cardPanel.FillColor = Color.White;
            this.cardPanel.Radius = 30;
            this.cardPanel.ShadowColor = Color.FromArgb(50, 0, 0, 0);
            this.cardPanel.ShadowDepth = 10;
            this.cardPanel.ShadowShift = 3;
            // dizajnovo ho dáme zhruba doprostred, runtime ho dorovnáme
            this.cardPanel.Location = new Point(410, 220);
            this.backgroundPanel.Controls.Add(this.cardPanel);

            // ===== TITLE =====
            lblTitle = new Guna2HtmlLabel();
            lblTitle.Text = "Vitajte v Epos";
            lblTitle.AutoSize = true;
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblTitle.Location = new Point(40, 35);
            cardPanel.Controls.Add(lblTitle);

            // ===== CAPTION =====
            lblCodeCaption = new Label();
            lblCodeCaption.Text = "Kód pokladníka";
            lblCodeCaption.AutoSize = true;
            lblCodeCaption.Font = new Font("Segoe UI", 9);
            lblCodeCaption.ForeColor = Color.Black;
            lblCodeCaption.Location = new Point(40, 105);
            cardPanel.Controls.Add(lblCodeCaption);

            // ===== TEXTBOX =====
            txtCode = new Guna2TextBox();
            txtCode.PlaceholderText = "Zadajte kód pokladníka";
            txtCode.Location = new Point(40, 130);
            txtCode.Size = new Size(370, 38);
            txtCode.BorderRadius = 10;
            txtCode.BorderThickness = 1;
            txtCode.BorderColor = Color.FromArgb(230, 230, 230);
            txtCode.FillColor = Color.FromArgb(248, 248, 248);
            txtCode.Font = new Font("Segoe UI", 10);
            txtCode.PasswordChar = '•';
            cardPanel.Controls.Add(txtCode);

            // ===== ERROR LABEL =====
            lblError = new Guna2HtmlLabel();
            lblError.Text = "";
            lblError.AutoSize = true;
            lblError.BackColor = Color.Transparent;
            lblError.ForeColor = Color.FromArgb(220, 0, 0);
            lblError.Font = new Font("Segoe UI", 8);
            lblError.Location = new Point(40, 172);
            cardPanel.Controls.Add(lblError);

            // ===== LOGIN BUTTON =====
            btnLogin = new Guna2Button();
            btnLogin.Text = "Prihlásiť sa";
            btnLogin.Location = new Point(40, 200);
            btnLogin.Size = new Size(120, 40);
            btnLogin.BorderRadius = 8;
            btnLogin.FillColor = Color.Black;
            btnLogin.ForeColor = Color.White;
            btnLogin.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnLogin.Click += BtnLogin_Click;
            cardPanel.Controls.Add(btnLogin);

            // ENTER = login
            this.AcceptButton = btnLogin;
        }
    }
}
