using System.Drawing;
using System.Windows.Forms;

namespace epos
{
    partial class ProductsPanel
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelList;
        private Label lblTitle;
        private FlowLayoutPanel flowProducts;

        private Panel panelDetail;
        private Guna.UI2.WinForms.Guna2Button btnBack;
        private Label lblDetailTitle;
        private Guna.UI2.WinForms.Guna2TextBox txtDetailBarcode;
        private Guna.UI2.WinForms.Guna2TextBox txtDetailName;
        private Guna.UI2.WinForms.Guna2ComboBox cmbDetailCategory;
        private Guna.UI2.WinForms.Guna2TextBox txtDetailPrice;
        private Guna.UI2.WinForms.Guna2Button btnSaveDetail;
        private Guna.UI2.WinForms.Guna2Button btnDeleteDetail;
        private PictureBox picDetail;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panelList = new Panel();
            lblTitle = new Label();
            flowProducts = new FlowLayoutPanel();

            panelDetail = new Panel();
            btnBack = new Guna.UI2.WinForms.Guna2Button();
            lblDetailTitle = new Label();
            txtDetailBarcode = new Guna.UI2.WinForms.Guna2TextBox();
            txtDetailName = new Guna.UI2.WinForms.Guna2TextBox();
            cmbDetailCategory = new Guna.UI2.WinForms.Guna2ComboBox();
            txtDetailPrice = new Guna.UI2.WinForms.Guna2TextBox();
            btnSaveDetail = new Guna.UI2.WinForms.Guna2Button();
            btnDeleteDetail = new Guna.UI2.WinForms.Guna2Button();
            picDetail = new PictureBox();

            SuspendLayout();

            // ======= panelList =======
            panelList.Dock = DockStyle.Fill;
            panelList.BackColor = Color.Transparent;

            // lblTitle
            lblTitle.Text = "Produkty";
            lblTitle.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(70, 40);

            // flowProducts
            flowProducts.Location = new Point(70, 100);
            flowProducts.Size = new Size(900, 500);
            flowProducts.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            flowProducts.AutoScroll = true;
            flowProducts.WrapContents = true;

            panelList.Controls.Add(lblTitle);
            panelList.Controls.Add(flowProducts);

            // ======= panelDetail =======
            panelDetail.Dock = DockStyle.Fill;
            panelDetail.BackColor = Color.Transparent;
            panelDetail.Visible = false;

            // btnBack
            btnBack.Text = "←";
            btnBack.FillColor = Color.Transparent;
            btnBack.ForeColor = Color.White;
            btnBack.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            btnBack.BorderRadius = 8;
            btnBack.Size = new Size(50, 40);
            btnBack.Location = new Point(70, 40);
            btnBack.Click += btnBack_Click;

            // lblDetailTitle
            lblDetailTitle.Text = "Detail produktu";
            lblDetailTitle.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblDetailTitle.ForeColor = Color.White;
            lblDetailTitle.AutoSize = true;
            lblDetailTitle.Location = new Point(140, 40);

            int detailX = 140;
            int detailY = 110;
            int inputWidth = 320;
            int spacing = 60;

            // txtDetailBarcode
            txtDetailBarcode.PlaceholderText = "Bar kód";
            txtDetailBarcode.BorderRadius = 8;
            txtDetailBarcode.Size = new Size(inputWidth, 40);
            txtDetailBarcode.Location = new Point(detailX, detailY);

            // txtDetailName
            txtDetailName.PlaceholderText = "Názov";
            txtDetailName.BorderRadius = 8;
            txtDetailName.Size = new Size(inputWidth, 40);
            txtDetailName.Location = new Point(detailX, detailY + spacing);

            // cmbDetailCategory
            cmbDetailCategory.BorderRadius = 8;
            cmbDetailCategory.Size = new Size(inputWidth, 40);
            cmbDetailCategory.Location = new Point(detailX, detailY + spacing * 2);
            cmbDetailCategory.DrawMode = DrawMode.OwnerDrawFixed;
            cmbDetailCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDetailCategory.Font = new Font("Segoe UI", 10F);
            cmbDetailCategory.Items.AddRange(new object[]
            {
                "ks",
                "balenie",
                "krabica",
                "kg",
                "g",
                "l",
                "ml",
                "m",
                "cm"
            });
            cmbDetailCategory.SelectedIndex = 0;

            // txtDetailPrice
            txtDetailPrice.PlaceholderText = "Cena";
            txtDetailPrice.BorderRadius = 8;
            txtDetailPrice.Size = new Size(inputWidth, 40);
            txtDetailPrice.Location = new Point(detailX, detailY + spacing * 3);

            // btnSaveDetail
            btnSaveDetail.Text = "Upraviť";
            btnSaveDetail.BorderRadius = 8;
            btnSaveDetail.Size = new Size(140, 40);
            btnSaveDetail.Location = new Point(detailX, detailY + spacing * 4 + 10);
            btnSaveDetail.FillColor = Color.White;
            btnSaveDetail.ForeColor = Color.Black;
            btnSaveDetail.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSaveDetail.Click += btnSaveDetail_Click;

            // btnDeleteDetail
            btnDeleteDetail.Text = "Odstrániť";
            btnDeleteDetail.BorderRadius = 8;
            btnDeleteDetail.Size = new Size(140, 40);
            btnDeleteDetail.Location = new Point(detailX + 160, detailY + spacing * 4 + 10);
            btnDeleteDetail.FillColor = Color.FromArgb(255, 80, 80);
            btnDeleteDetail.ForeColor = Color.White;
            btnDeleteDetail.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnDeleteDetail.Click += btnDeleteDetail_Click;

            // picDetail
            picDetail.Size = new Size(300, 360);
            picDetail.Location = new Point(600, 130);
            picDetail.SizeMode = PictureBoxSizeMode.Zoom;
            picDetail.BackColor = Color.White;

            panelDetail.Controls.Add(btnBack);
            panelDetail.Controls.Add(lblDetailTitle);
            panelDetail.Controls.Add(txtDetailBarcode);
            panelDetail.Controls.Add(txtDetailName);
            panelDetail.Controls.Add(cmbDetailCategory);
            panelDetail.Controls.Add(txtDetailPrice);
            panelDetail.Controls.Add(btnSaveDetail);
            panelDetail.Controls.Add(btnDeleteDetail);
            panelDetail.Controls.Add(picDetail);

            // ======= ProductsPanel (UserControl) =======
            Controls.Add(panelList);
            Controls.Add(panelDetail);

            BackColor = Color.Transparent;
            Size = new Size(1000, 650);

            ResumeLayout(false);
        }
    }
}
