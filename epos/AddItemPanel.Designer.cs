namespace epos
{
    partial class AddItemPanel
    {
        private System.ComponentModel.IContainer components = null;

        private Guna.UI2.WinForms.Guna2TextBox txtBarcode;
        private Guna.UI2.WinForms.Guna2TextBox txtName;
        private Guna.UI2.WinForms.Guna2TextBox txtPrice;
        private Guna.UI2.WinForms.Guna2TextBox txtImage;

        private Guna.UI2.WinForms.Guna2ComboBox cmbCategory;

        


        // NEW: DPH kategória
        private System.Windows.Forms.Label lblVatCategory;
        private Guna.UI2.WinForms.Guna2ComboBox cmbVatCategory;

        private Guna.UI2.WinForms.Guna2Button btnAddProduct;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }
        

        private void InitializeComponent()
        {
           

            this.txtBarcode = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtName = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtPrice = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtImage = new Guna.UI2.WinForms.Guna2TextBox();

            this.cmbCategory = new Guna.UI2.WinForms.Guna2ComboBox();

            // NEW
            this.lblVatCategory = new System.Windows.Forms.Label();
            this.cmbVatCategory = new Guna.UI2.WinForms.Guna2ComboBox();

            this.btnAddProduct = new Guna.UI2.WinForms.Guna2Button();

            this.SuspendLayout();

            // =============================
            // txtBarcode
            // =============================
            txtBarcode.PlaceholderText = "Čiarový kód";
            txtBarcode.BorderRadius = 8;
            txtBarcode.Size = new System.Drawing.Size(300, 40);
            txtBarcode.Location = new System.Drawing.Point(50, 50);

            // =============================
            // txtName
            // =============================
            txtName.PlaceholderText = "Názov produktu";
            txtName.BorderRadius = 8;
            txtName.Size = new System.Drawing.Size(300, 40);
            txtName.Location = new System.Drawing.Point(50, 110);

            // =============================
            // cmbCategory
            // =============================
            cmbCategory.BorderRadius = 8;
            cmbCategory.Size = new System.Drawing.Size(300, 40);
            cmbCategory.Location = new System.Drawing.Point(50, 170);
            cmbCategory.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbCategory.Font = new System.Drawing.Font("Segoe UI", 10F);

            cmbCategory.Items.AddRange(new object[]
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

            cmbCategory.SelectedIndex = 0;

            // =============================
            // lblVatCategory (NEW)
            // =============================
            lblVatCategory.Text = "DPH kategória";
            lblVatCategory.AutoSize = true;
            lblVatCategory.ForeColor = System.Drawing.Color.White;
            lblVatCategory.Font = new System.Drawing.Font("Segoe UI", 9F);
            lblVatCategory.Location = new System.Drawing.Point(50, 220);

            // =============================
            // cmbVatCategory (NEW)
            // =============================
            cmbVatCategory.BorderRadius = 8;
            cmbVatCategory.Size = new System.Drawing.Size(300, 40);
            cmbVatCategory.Location = new System.Drawing.Point(50, 245);
            cmbVatCategory.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cmbVatCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbVatCategory.Font = new System.Drawing.Font("Segoe UI", 10F);

            

            // =============================
            // txtPrice (posunuté nižšie)
            // =============================
            txtPrice.PlaceholderText = "Cena (bez DPH)";
            txtPrice.BorderRadius = 8;
            txtPrice.Size = new System.Drawing.Size(300, 40);
            txtPrice.Location = new System.Drawing.Point(50, 305);

            // =============================
            // txtImage (posunuté nižšie)
            // =============================
            txtImage.PlaceholderText = "Link na obrázok (URL alebo cesta)";
            txtImage.BorderRadius = 8;
            txtImage.Size = new System.Drawing.Size(300, 40);
            txtImage.Location = new System.Drawing.Point(50, 365);

            // =============================
            // btnAddProduct (posunuté nižšie)
            // =============================
            btnAddProduct.Text = "Pridať produkt";
            btnAddProduct.BorderRadius = 8;
            btnAddProduct.Size = new System.Drawing.Size(200, 45);
            btnAddProduct.Location = new System.Drawing.Point(50, 430);
            btnAddProduct.FillColor = System.Drawing.Color.Black;
            btnAddProduct.ForeColor = System.Drawing.Color.White;
            btnAddProduct.Click += new System.EventHandler(this.btnAddProduct_Click);

            // =============================
            // AddItemPanel
            // =============================
            this.Controls.Add(txtBarcode);
            this.Controls.Add(txtName);
            this.Controls.Add(cmbCategory);

            // NEW
            this.Controls.Add(lblVatCategory);
            this.Controls.Add(cmbVatCategory);

            this.Controls.Add(txtPrice);
            this.Controls.Add(txtImage);
            this.Controls.Add(btnAddProduct);

            this.BackColor = System.Drawing.Color.Transparent;
            this.Size = new System.Drawing.Size(800, 600);

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
