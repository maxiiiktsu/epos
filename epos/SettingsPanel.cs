using System;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using MySql.Data.MySqlClient;

namespace epos
{
    public partial class SettingsPanel : UserControl
    {
        private FilterInfoCollection cameras;

        
        public event Action<int> CameraChanged;

        public SettingsPanel()
        {
            InitializeComponent();

            LoadCameras();

            
            cmbCameras.SelectedIndexChanged += CmbCameras_SelectedIndexChanged;
        }

        private void LoadCameras()
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

        
        public void RefreshCameras()
        {
            LoadCameras();
        }

        private void CmbCameras_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cameras == null || cameras.Count == 0)
                return;

            
            if (cmbCameras.SelectedIndex <= 0)
                return;

            
            int cameraIndex = cmbCameras.SelectedIndex - 1;

            
            CameraChanged?.Invoke(cameraIndex);
        }

        // ================= ZMENA KÓDU POKLADNÍKA =================

        private void btnSavePassword_Click(object sender, EventArgs e)
        {
            string oldCode = txtOldCode.Text.Trim();
            string newCode = txtNewCode.Text.Trim();

            if (oldCode.Length == 0 || newCode.Length == 0)
            {
                MessageBox.Show("Vyplňte oba kódy.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();

                    
                    string checkSql = "SELECT id FROM cashiers WHERE code = @old LIMIT 1";
                    int? cashierId = null;

                    using (var checkCmd = new MySqlCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@old", oldCode);
                        object result = checkCmd.ExecuteScalar();

                        if (result == null || result == DBNull.Value)
                        {
                            MessageBox.Show("Pôvodný kód nesedí.", "Chyba",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        cashierId = Convert.ToInt32(result);
                    }

                    
                    string updateSql = "UPDATE cashiers SET code = @new WHERE id = @id";

                    using (var updateCmd = new MySqlCommand(updateSql, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@new", newCode);
                        updateCmd.Parameters.AddWithValue("@id", cashierId.Value);
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
    }
}
