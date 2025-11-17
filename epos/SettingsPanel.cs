using System;
using System.Windows.Forms;

// SAFE optional import – if AForge is missing, app still compiles
#if AFORGE
using AForge.Video.DirectShow;
#endif

namespace epos
{
    public partial class SettingsPanel : UserControl
    {
#if AFORGE
        private FilterInfoCollection cameras;
#endif

        public SettingsPanel()
        {
            InitializeComponent();

            // try loading cameras but do not crash if AForge is missing
            SafeLoadCameras();
        }

        private void SafeLoadCameras()
        {
            // If AForge is not installed → handle gracefully
#if !AFORGE
            cmbCameras.Items.Clear();
            cmbCameras.Items.Add("Kamera modul nie je nainštalovaný");
            cmbCameras.SelectedIndex = 0;
            return;
#else
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
#endif
        }

        private void btnSavePassword_Click(object sender, EventArgs e)
        {
            string oldCode = txtOldCode.Text.Trim();
            string newCode = txtNewCode.Text.Trim();

            if (oldCode.Length == 0 || newCode.Length == 0)
            {
                MessageBox.Show("Vyplňte oba kódy.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // === predpríprava na databázu ===
            // tu neskôr doplníme DB logiku:
            // 1. check if oldCode matches DB
            // 2. update new code

            MessageBox.Show("Kód pokladníka bol zmenený (demo).",
                "Hotovo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
