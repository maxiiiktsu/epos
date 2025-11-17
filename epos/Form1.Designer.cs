using System.Drawing;
using System.Windows.Forms;

namespace epos
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private Guna.UI2.WinForms.Guna2GradientPanel backgroundPanel;

        private Guna.UI2.WinForms.Guna2Button btnHome;
        private Guna.UI2.WinForms.Guna2Button btnSettings;
        private Guna.UI2.WinForms.Guna2Button btnAddItem;
        private Guna.UI2.WinForms.Guna2Button btnRemoveItem;
        private Guna.UI2.WinForms.Guna2Button btnProducts;
        private Guna.UI2.WinForms.Guna2Button btnLogout;

        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Label lblBarcode;
        private Label lblBarcodeValue;
        private Label lblName;
        private Label lblNameValue;
        private Label lblCount;
        private Label lblCountValue;
        private Label lblPrice;
        private Label lblPriceValue;

        private Guna.UI2.WinForms.Guna2Button btnAddManual;
        private Guna.UI2.WinForms.Guna2Panel receiptPreview;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }


        // ========================================================================
        // DESIGNER
        // ========================================================================

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // BACKGROUND PANEL ====================================================
            backgroundPanel = new Guna.UI2.WinForms.Guna2GradientPanel();
            backgroundPanel.Dock = DockStyle.Fill;
            backgroundPanel.FillColor = Color.FromArgb(15, 15, 15);
            backgroundPanel.FillColor2 = Color.FromArgb(30, 30, 30);
            backgroundPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            backgroundPanel.BackColor = Color.Black; // gradient parent is fine
            this.Controls.Add(backgroundPanel);


            // TOP MENU — all transparent ==========================================
            btnHome = CreateNavBtn("Home", 40);
            btnSettings = CreateNavBtn("Settings", 150);
            btnAddItem = CreateNavBtn("Add Item", 260);
            btnRemoveItem = CreateNavBtn("Remove Item", 380);
            btnProducts = CreateNavBtn("Products", 520);

            // HOME underline
            btnHome.Font = new Font("Segoe UI", 10, FontStyle.Bold | FontStyle.Underline);

            // LOGOUT BUTTON (transparent)
            btnLogout = new Guna.UI2.WinForms.Guna2Button();
            btnLogout.Text = "Odhlásiť sa";
            btnLogout.Size = new Size(120, 36);
            btnLogout.Font = new Font("Segoe UI", 9);
            btnLogout.BorderRadius = 6;
            btnLogout.FillColor = Color.Transparent;
            btnLogout.ForeColor = Color.White;
            btnLogout.BorderColor = Color.White;
            btnLogout.BorderThickness = 1;
            btnLogout.BackColor = Color.Transparent;
            btnLogout.UseTransparentBackground = true;
            btnLogout.Cursor = Cursors.Hand;
            btnLogout.Location = new Point(1100, 32);

            backgroundPanel.Controls.Add(btnHome);
            backgroundPanel.Controls.Add(btnSettings);
            backgroundPanel.Controls.Add(btnAddItem);
            backgroundPanel.Controls.Add(btnRemoveItem);
            backgroundPanel.Controls.Add(btnProducts);
            backgroundPanel.Controls.Add(btnLogout);


            // PRODUCT TITLE =======================================================
            lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblTitle.Text = "Informácie o produkte";
            lblTitle.AutoSize = true;
            lblTitle.ForeColor = Color.White;
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblTitle.Location = new Point(70, 140);
            backgroundPanel.Controls.Add(lblTitle);


            // PRODUCT INFO ========================================================
            int y = 230;

            lblBarcode = CreateLabel("Bar kód", 70, y);
            lblBarcodeValue = CreateValue("-", 180, y);

            lblName = CreateLabel("Názov", 70, y + 40);
            lblNameValue = CreateValue("-", 180, y + 40);

            lblCount = CreateLabel("Počet", 70, y + 80);
            lblCountValue = CreateValue("-", 180, y + 80);

            lblPrice = CreateLabel("Cena", 70, y + 120);
            lblPriceValue = CreateValue("-", 180, y + 120);

            backgroundPanel.Controls.Add(lblBarcode);
            backgroundPanel.Controls.Add(lblBarcodeValue);
            backgroundPanel.Controls.Add(lblName);
            backgroundPanel.Controls.Add(lblNameValue);
            backgroundPanel.Controls.Add(lblCount);
            backgroundPanel.Controls.Add(lblCountValue);
            backgroundPanel.Controls.Add(lblPrice);
            backgroundPanel.Controls.Add(lblPriceValue);


            // ADD MANUAL BUTTON ===================================================
            btnAddManual = new Guna.UI2.WinForms.Guna2Button();
            btnAddManual.Text = "Pridať produkt manuálne";
            btnAddManual.Size = new Size(240, 45);
            btnAddManual.FillColor = Color.White;
            btnAddManual.BorderRadius = 8;
            btnAddManual.ForeColor = Color.Black;
            btnAddManual.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnAddManual.Location = new Point(70, y + 180);
            backgroundPanel.Controls.Add(btnAddManual);


            // RECEIPT PANEL =======================================================
            receiptPreview = new Guna.UI2.WinForms.Guna2Panel();
            receiptPreview.Size = new Size(340, 460);
            receiptPreview.FillColor = Color.FromArgb(235, 235, 235); // intentionally light
            receiptPreview.Location = new Point(800, 200);

            Label prev = new Label();
            prev.Text = "-";
            prev.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            prev.AutoSize = true;
            prev.Location = new Point(150, 200);

            receiptPreview.Controls.Add(prev);
            backgroundPanel.Controls.Add(receiptPreview);


            // FORM SETTINGS =======================================================
            this.Text = "Epos – Home";
            this.ClientSize = new Size(1280, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.BackColor = Color.Black;
        }


        // ========================================================================
        // HELPERS
        // ========================================================================

        private Guna.UI2.WinForms.Guna2Button CreateNavBtn(string text, int x)
        {
            var b = new Guna.UI2.WinForms.Guna2Button();

            b.Text = text;
            b.Font = new Font("Segoe UI", 10);
            b.FillColor = Color.Transparent;
            b.BackColor = Color.Transparent;
            b.UseTransparentBackground = true;
            b.ForeColor = Color.White;
            b.BorderThickness = 0;
            b.AutoSize = true;
            b.Location = new Point(x, 35);
            b.Cursor = Cursors.Hand;

            // absolutely ensure no shadow or effects
            b.ShadowDecoration.Enabled = false;

            return b;
        }

        private Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.Transparent,
                Location = new Point(x, y)
            };
        }

        private Label CreateValue(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                ForeColor = Color.WhiteSmoke,
                Font = new Font("Segoe UI", 12),
                BackColor = Color.Transparent,
                Location = new Point(x, y)
            };
        }
    }
}
