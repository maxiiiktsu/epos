using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using AForge.Video.DirectShow;

namespace epos
{
    public partial class SettingsPanel : UserControl
    {
        private FilterInfoCollection cameras;

        // ===== layout karty =====
        private Panel leftCard;
        private Panel rightCard;

        // ===== DPH UI (Guna2) =====
        private Label lblVatTitle;
        private Label lblStd;
        private Label lblRed;

        private Guna2TextBox txtVatStandard;
        private Guna2TextBox txtVatReduced;
        private Guna2Button btnSaveVat;

        public event Action<int> CameraChanged;

        public SettingsPanel()
        {
            InitializeComponent();

            BuildLayout();
            ApplyStyleToExistingControls();

            
            cmbCameras.SelectedIndexChanged -= CmbCameras_SelectedIndexChanged;
            cmbCameras.SelectedIndexChanged += CmbCameras_SelectedIndexChanged;

            LoadCamerasSafe();
            LoadVatFromDbSafe();

            Resize += (_, __) => LayoutCards();
        }

        // =========================================================
        // UI – Layout
        // =========================================================

        private void BuildLayout()
        {
            // card panely
            leftCard = CreateCard();
            rightCard = CreateCard();

            
            MoveIfNotNull(leftCard, lblCamera);
            MoveIfNotNull(leftCard, cmbCameras);
            MoveIfNotNull(leftCard, lblCodeChange);
            MoveIfNotNull(leftCard, txtOldCode);
            MoveIfNotNull(leftCard, txtNewCode);
            MoveIfNotNull(leftCard, btnSavePassword);

            Controls.Add(leftCard);
            Controls.Add(rightCard);

            // ===== DPH controls (pravá karta) =====
            lblVatTitle = new Label
            {
                Text = "DPH sadzby",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true
            };

            lblStd = new Label
            {
                Text = "Štandardná (%)",
                ForeColor = Color.Gainsboro,
                Font = new Font("Segoe UI", 9),
                AutoSize = true
            };

            txtVatStandard = CreateGunaInput("20.00");

            lblRed = new Label
            {
                Text = "Znížená (%)",
                ForeColor = Color.Gainsboro,
                Font = new Font("Segoe UI", 9),
                AutoSize = true
            };

            txtVatReduced = CreateGunaInput("10.00");

            btnSaveVat = new Guna2Button
            {
                Text = "Uložiť DPH",
                BorderRadius = 8,
                Size = new Size(160, 40),
                FillColor = Color.White,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSaveVat.Click += (_, __) => SaveVatToDbSafe();

            rightCard.Controls.Add(lblVatTitle);
            rightCard.Controls.Add(lblStd);
            rightCard.Controls.Add(txtVatStandard);
            rightCard.Controls.Add(lblRed);
            rightCard.Controls.Add(txtVatReduced);
            rightCard.Controls.Add(btnSaveVat);

            LayoutCards();
        }

        private Panel CreateCard()
        {
            var p = new Panel
            {
                BackColor = Color.FromArgb(20, 20, 20),
                Padding = new Padding(20)
            };

            // jemný border (C# 7.3 safe)
            p.Paint += (_, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(60, 255, 255, 255), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
                }
            };

            return p;
        }

        private Guna2TextBox CreateGunaInput(string value)
        {
            return new Guna2TextBox
            {
                Text = value,
                BorderRadius = 8,
                Size = new Size(180, 38),
                Font = new Font("Segoe UI", 10),
                FillColor = Color.White,
                ForeColor = Color.Black
            };
        }

        private void ApplyStyleToExistingControls()
        {
            if (cmbCameras != null)
            {
                cmbCameras.BorderRadius = 8;
                cmbCameras.FillColor = Color.White;
                cmbCameras.ForeColor = Color.Black;
            }

            if (txtOldCode != null)
            {
                txtOldCode.BorderRadius = 8;
                txtOldCode.FillColor = Color.White;
                txtOldCode.ForeColor = Color.Black;
            }

            if (txtNewCode != null)
            {
                txtNewCode.BorderRadius = 8;
                txtNewCode.FillColor = Color.White;
                txtNewCode.ForeColor = Color.Black;
            }

            if (btnSavePassword != null)
            {
                btnSavePassword.BorderRadius = 8;
                btnSavePassword.FillColor = Color.White;
                btnSavePassword.ForeColor = Color.Black;
                btnSavePassword.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                btnSavePassword.Cursor = Cursors.Hand;
            }
        }

        private void LayoutCards()
        {
            int paddingX = 70;
            int gap = 24;

            
            int topY = 110 + 55; 

            int contentW = ClientSize.Width - paddingX * 2;
            if (contentW < 320) contentW = 320;

            bool twoCols = contentW >= 900;
            int cardW = twoCols ? (contentW - gap) / 2 : contentW;

            leftCard.Location = new Point(paddingX, topY);
            leftCard.Size = new Size(cardW, 320);

            rightCard.Location = twoCols
                ? new Point(paddingX + cardW + gap, topY)
                : new Point(paddingX, topY + leftCard.Height + gap);

            rightCard.Size = new Size(cardW, 260);

            // ===== LEFT CARD inner layout =====
            int x = 18;
            int y = 18;
            int w = leftCard.ClientSize.Width - 36;

            if (lblCamera != null)
            {
                lblCamera.Location = new Point(x, y);
                y += 26;
            }

            if (cmbCameras != null)
            {
                cmbCameras.Location = new Point(x, y);
                cmbCameras.Width = w;
                y += 58;
            }

            if (lblCodeChange != null)
            {
                lblCodeChange.Location = new Point(x, y);
                y += 30;
            }

            if (txtOldCode != null)
            {
                txtOldCode.Location = new Point(x, y);
                txtOldCode.Width = w;
                y += 52;
            }

            if (txtNewCode != null)
            {
                txtNewCode.Location = new Point(x, y);
                txtNewCode.Width = w;
                y += 60;
            }

            if (btnSavePassword != null)
            {
                btnSavePassword.Location = new Point(x, y);
                btnSavePassword.Width = Math.Min(160, w);
            }

            // ===== RIGHT CARD inner layout =====
            int rx = 18;
            int ry = 18;

            lblVatTitle.Location = new Point(rx, ry);
            ry += 34;

            lblStd.Location = new Point(rx, ry);
            ry += 22;

            txtVatStandard.Location = new Point(rx, ry);
            ry += 52;

            lblRed.Location = new Point(rx, ry);
            ry += 22;

            txtVatReduced.Location = new Point(rx, ry);
            ry += 60;

            btnSaveVat.Location = new Point(rx, ry);
        }

        private static void MoveIfNotNull(Control newParent, Control c)
        {
            if (c == null) return;

            if (c.Parent != null)
                c.Parent.Controls.Remove(c);

            newParent.Controls.Add(c);
        }

        // =========================================================
        // KAMERY
        // =========================================================

        private void LoadCamerasSafe()
        {
            try
            {
                cameras = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                cmbCameras.Items.Clear();
                cmbCameras.Items.Add("Vyberte kameru");

                foreach (FilterInfo cam in cameras)
                    cmbCameras.Items.Add(cam.Name);

                cmbCameras.SelectedIndex = 0;
            }
            catch
            {
                cmbCameras.Items.Clear();
                cmbCameras.Items.Add("Žiadne kamery");
                cmbCameras.SelectedIndex = 0;
            }
        }

        private void CmbCameras_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cameras == null || cameras.Count == 0) return;
            if (cmbCameras.SelectedIndex <= 0) return;

            int cameraIndex = cmbCameras.SelectedIndex - 1;
            CameraChanged?.Invoke(cameraIndex);
        }

        // =========================================================
        // ZMENA KÓDU POKLADNÍKA
        // =========================================================

        private void btnSavePassword_Click(object sender, EventArgs e)
        {
            string oldCode = txtOldCode.Text.Trim();
            string newCode = txtNewCode.Text.Trim();

            if (oldCode.Length == 0 || newCode.Length == 0)
            {
                MessageBox.Show("Vyplňte oba kódy.", "Chyba",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();

                    string checkSql = "SELECT id FROM cashiers WHERE code = @old LIMIT 1";
                    object result;

                    using (var checkCmd = new MySqlCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@old", oldCode);
                        result = checkCmd.ExecuteScalar();
                    }

                    if (result == null || result == DBNull.Value)
                    {
                        MessageBox.Show("Pôvodný kód nesedí.", "Chyba",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int cashierId = Convert.ToInt32(result);

                    string updateSql = "UPDATE cashiers SET code = @new WHERE id = @id";
                    using (var updateCmd = new MySqlCommand(updateSql, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@new", newCode);
                        updateCmd.Parameters.AddWithValue("@id", cashierId);
                        updateCmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Kód pokladníka bol zmenený.", "Hotovo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtOldCode.Text = "";
                txtNewCode.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba pri práci s databázou: " + ex.Message,
                    "DB chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // DPH – DB
        // =========================================================

        private void LoadVatFromDbSafe()
        {
            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand("SELECT vat_key, vat_rate FROM vat_settings", conn))
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            string key = rd.GetString("vat_key");
                            decimal rate = rd.GetDecimal("vat_rate");

                            if (key == "STANDARD")
                                txtVatStandard.Text = rate.ToString("0.00", CultureInfo.InvariantCulture);

                            if (key == "REDUCED")
                                txtVatReduced.Text = rate.ToString("0.00", CultureInfo.InvariantCulture);
                        }
                    }
                }
            }
            catch
            {
                
            }
        }

        private void SaveVatToDbSafe()
        {
            if (!TryParseVat(txtVatStandard.Text, out decimal std) ||
                !TryParseVat(txtVatReduced.Text, out decimal red))
            {
                MessageBox.Show("Zadajte platné čísla (napr. 20 alebo 10.00).",
                    "DPH", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();

                    using (var tx = conn.BeginTransaction())
                    {
                        UpsertVat(conn, tx, "STANDARD", std);
                        UpsertVat(conn, tx, "REDUCED", red);
                        tx.Commit();
                    }
                }

                MessageBox.Show("DPH sadzby uložené.", "DPH",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nepodarilo sa uložiť DPH sadzby: " + ex.Message,
                    "DPH", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void UpsertVat(MySqlConnection conn, MySqlTransaction tx, string key, decimal value)
        {
            string sql = @"
                INSERT INTO vat_settings (vat_key, vat_rate)
                VALUES (@k, @r)
                ON DUPLICATE KEY UPDATE vat_rate = @r;";

            using (var cmd = new MySqlCommand(sql, conn, tx))
            {
                cmd.Parameters.AddWithValue("@k", key);
                cmd.Parameters.AddWithValue("@r", value);
                cmd.ExecuteNonQuery();
            }
        }

        private static bool TryParseVat(string s, out decimal value)
        {
            s = (s ?? "").Trim().Replace(',', '.');

            return decimal.TryParse(
                s,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out value
            );
        }
    }
}
