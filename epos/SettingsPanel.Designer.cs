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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblCamera = new System.Windows.Forms.Label();
            this.cmbCameras = new Guna.UI2.WinForms.Guna2ComboBox();
            this.lblCodeChange = new System.Windows.Forms.Label();
            this.txtOldCode = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtNewCode = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnSavePassword = new Guna.UI2.WinForms.Guna2Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 26F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(70, 40);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(155, 47);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Settings";
            // 
            // lblCamera
            // 
            this.lblCamera.AutoSize = true;
            this.lblCamera.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblCamera.ForeColor = System.Drawing.Color.White;
            this.lblCamera.Location = new System.Drawing.Point(70, 110);
            this.lblCamera.Name = "lblCamera";
            this.lblCamera.Size = new System.Drawing.Size(60, 20);
            this.lblCamera.TabIndex = 1;
            this.lblCamera.Text = "Kamera";
            // 
            // cmbCameras
            // 
            this.cmbCameras.BackColor = System.Drawing.Color.Transparent;
            this.cmbCameras.BorderRadius = 8;
            this.cmbCameras.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbCameras.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCameras.FocusedColor = System.Drawing.Color.Empty;
            this.cmbCameras.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbCameras.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmbCameras.ItemHeight = 30;
            this.cmbCameras.Location = new System.Drawing.Point(70, 140);
            this.cmbCameras.Name = "cmbCameras";
            this.cmbCameras.Size = new System.Drawing.Size(300, 36);
            this.cmbCameras.TabIndex = 2;
            // 
            // lblCodeChange
            // 
            this.lblCodeChange.AutoSize = true;
            this.lblCodeChange.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblCodeChange.ForeColor = System.Drawing.Color.White;
            this.lblCodeChange.Location = new System.Drawing.Point(70, 210);
            this.lblCodeChange.Name = "lblCodeChange";
            this.lblCodeChange.Size = new System.Drawing.Size(169, 20);
            this.lblCodeChange.TabIndex = 3;
            this.lblCodeChange.Text = "Zmena kódu pokladníka";
            // 
            // txtOldCode
            // 
            this.txtOldCode.BorderRadius = 8;
            this.txtOldCode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtOldCode.DefaultText = "";
            this.txtOldCode.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtOldCode.Location = new System.Drawing.Point(70, 240);
            this.txtOldCode.Name = "txtOldCode";
            this.txtOldCode.PlaceholderText = "Starý kód";
            this.txtOldCode.SelectedText = "";
            this.txtOldCode.Size = new System.Drawing.Size(300, 40);
            this.txtOldCode.TabIndex = 4;
            // 
            // txtNewCode
            // 
            this.txtNewCode.BorderRadius = 8;
            this.txtNewCode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtNewCode.DefaultText = "";
            this.txtNewCode.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtNewCode.Location = new System.Drawing.Point(70, 290);
            this.txtNewCode.Name = "txtNewCode";
            this.txtNewCode.PlaceholderText = "Nový kód";
            this.txtNewCode.SelectedText = "";
            this.txtNewCode.Size = new System.Drawing.Size(300, 40);
            this.txtNewCode.TabIndex = 5;
            // 
            // btnSavePassword
            // 
            this.btnSavePassword.BorderRadius = 8;
            this.btnSavePassword.FillColor = System.Drawing.Color.White;
            this.btnSavePassword.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSavePassword.ForeColor = System.Drawing.Color.Black;
            this.btnSavePassword.Location = new System.Drawing.Point(70, 350);
            this.btnSavePassword.Name = "btnSavePassword";
            this.btnSavePassword.Size = new System.Drawing.Size(120, 40);
            this.btnSavePassword.TabIndex = 6;
            this.btnSavePassword.Text = "Zmeniť";
            // 
            // SettingsPanel
            // 
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblCamera);
            this.Controls.Add(this.cmbCameras);
            this.Controls.Add(this.lblCodeChange);
            this.Controls.Add(this.txtOldCode);
            this.Controls.Add(this.txtNewCode);
            this.Controls.Add(this.btnSavePassword);
            this.Name = "SettingsPanel";
            this.Size = new System.Drawing.Size(1583, 656);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
