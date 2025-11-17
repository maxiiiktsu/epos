using System.Drawing;
using System.Windows.Forms;

namespace epos
{
    partial class SettingsPanel
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblTitle;
        private Label lblCamera;
        private Guna.UI2.WinForms.Guna2ComboBox cmbCameras;

        private Label lblCodeChange;
        private Guna.UI2.WinForms.Guna2TextBox txtOldCode;
        private Guna.UI2.WinForms.Guna2TextBox txtNewCode;

        private Guna.UI2.WinForms.Guna2Button btnSavePassword;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.BackColor = Color.Transparent;

            // TITLE
            lblTitle = new Label();
            lblTitle.Text = "Settings";
            lblTitle.Font = new Font("Segoe UI", 26, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(70, 40);

            // CAMERA LABEL
            lblCamera = new Label();
            lblCamera.Text = "Kamera";
            lblCamera.ForeColor = Color.White;
            lblCamera.AutoSize = true;
            lblCamera.Font = new Font("Segoe UI", 11);
            lblCamera.Location = new Point(70, 110);

            // CAMERA COMBOBOX
            cmbCameras = new Guna.UI2.WinForms.Guna2ComboBox();
            cmbCameras.Size = new Size(300, 40);
            cmbCameras.Location = new Point(70, 140);
            cmbCameras.BorderRadius = 8;
            cmbCameras.Font = new Font("Segoe UI", 10);
            cmbCameras.DrawMode = DrawMode.OwnerDrawFixed;
            cmbCameras.DropDownStyle = ComboBoxStyle.DropDownList;

            // CODE CHANGE LABEL
            lblCodeChange = new Label();
            lblCodeChange.Text = "Zmena kódu pokladníka";
            lblCodeChange.ForeColor = Color.White;
            lblCodeChange.AutoSize = true;
            lblCodeChange.Font = new Font("Segoe UI", 11);
            lblCodeChange.Location = new Point(70, 210);

            // OLD CODE
            txtOldCode = new Guna.UI2.WinForms.Guna2TextBox();
            txtOldCode.PlaceholderText = "Starý kód";
            txtOldCode.Size = new Size(300, 40);
            txtOldCode.Location = new Point(70, 240);
            txtOldCode.BorderRadius = 8;

            // NEW CODE
            txtNewCode = new Guna.UI2.WinForms.Guna2TextBox();
            txtNewCode.PlaceholderText = "Nový kód";
            txtNewCode.Size = new Size(300, 40);
            txtNewCode.Location = new Point(70, 290);
            txtNewCode.BorderRadius = 8;

            // SAVE BUTTON
            btnSavePassword = new Guna.UI2.WinForms.Guna2Button();
            btnSavePassword.Text = "Zmeniť";
            btnSavePassword.Size = new Size(120, 40);
            btnSavePassword.Location = new Point(70, 350);
            btnSavePassword.BorderRadius = 8;
            btnSavePassword.FillColor = Color.White;
            btnSavePassword.ForeColor = Color.Black;
            btnSavePassword.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSavePassword.Click += btnSavePassword_Click;

            // ADD TO PANEL
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblCamera);
            this.Controls.Add(cmbCameras);
            this.Controls.Add(lblCodeChange);
            this.Controls.Add(txtOldCode);
            this.Controls.Add(txtNewCode);
            this.Controls.Add(btnSavePassword);
        }
    }
}
