using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace epos
{
    public partial class Form1 : Form
    {
        private SettingsPanel settingsPanel;
        private AddItemPanel addItemPanel;
        private ProductsPanel productsPanel;

        private Control currentPanel;

        
        private Panel underline;
        private Guna.UI2.WinForms.Guna2Button activeButton;

        private Timer underlineAnimTimer;
        private int targetUnderlineX;

        
        private BarcodeScanner scanner;

        public Form1()
        {
            InitializeComponent();

            
            btnRemoveItem.Visible = false;

            // SETTINGS PANEL
            settingsPanel = new SettingsPanel();
            settingsPanel.Visible = false;
            backgroundPanel.Controls.Add(settingsPanel);
            settingsPanel.BringToFront();

            
            settingsPanel.CameraChanged += OnCameraChanged;


            // ADD ITEM PANEL
            addItemPanel = new AddItemPanel();
            addItemPanel.Visible = false;
            backgroundPanel.Controls.Add(addItemPanel);
            addItemPanel.BringToFront();

            // PRODUCTS PANEL
            productsPanel = new ProductsPanel();
            productsPanel.Visible = false;
            backgroundPanel.Controls.Add(productsPanel);
            productsPanel.BringToFront();

            // UNDERLINE panel
            underline = new Panel
            {
                Height = 2,
                Width = 40,
                BackColor = Color.White,
                Visible = false
            };
            backgroundPanel.Controls.Add(underline);

            // ANIMATION TIMER
            underlineAnimTimer = new Timer();
            underlineAnimTimer.Interval = 10;
            underlineAnimTimer.Tick += AnimateUnderline;

            // NAVBAR EVENTS
            HookNavbarEvents();

            // START WITH HOME ACTIVE
            SetActiveButton(btnHome);

            // layout
            LayoutUi();
            Resize += (_, __) => LayoutUi();
            Shown += (_, __) => LayoutUi();

            // ====== SCANNER ======
            try
            {
                scanner = new BarcodeScanner(null); 
                scanner.BarcodeDetected += OnBarcodeDetected;
                scanner.Start(0); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nepodarilo sa spustiť kameru: " + ex.Message,
                    "Kamera", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            scanner?.Dispose();
            base.OnFormClosing(e);
        }

        // ===========================================================
        // NAVBAR EVENTS
        // ===========================================================


        private void HookNavbarEvents()
        {
            btnHome.Click += (_, __) =>
            {
                ShowHome();
                SetActiveButton(btnHome);
            };

            btnSettings.Click += (_, __) =>
            {
                ShowPanel(settingsPanel);
                SetActiveButton(btnSettings);
            };

            btnAddItem.Click += (_, __) =>
            {
                ShowPanel(addItemPanel);
                SetActiveButton(btnAddItem);
            };

            btnProducts.Click += (_, __) =>
            {
                productsPanel.RefreshProducts();
                ShowPanel(productsPanel);
                SetActiveButton(btnProducts);
            };
        }

        // ===========================================================
        // UNDERLINE LOGIC
        // ===========================================================

        private void SetActiveButton(Guna.UI2.WinForms.Guna2Button btn)
        {
            activeButton = btn;

            underline.Visible = true;

            // underline = 45 % šírky tlačidla
            underline.Width = (int)(btn.Width * 0.45);

            // centrovanie
            targetUnderlineX = btn.Left + (btn.Width - underline.Width) / 2;

            
            underlineAnimTimer.Start();
        }

        private void AnimateUnderline(object sender, EventArgs e)
        {
            int currentX = underline.Left;
            int speed = 12;

            if (Math.Abs(currentX - targetUnderlineX) <= speed)
            {
                underline.Left = targetUnderlineX;
                underlineAnimTimer.Stop();
            }
            else
            {
                underline.Left += (currentX < targetUnderlineX) ? speed : -speed;
            }

            underline.Top = activeButton.Bottom + 2;
        }

        // ===========================================================
        // BARCODE HANDLER (pozadie)
        // ===========================================================

        private void OnBarcodeDetected(string code)
        {
            
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnBarcodeDetected(code)));
                return;
            }

            
            if (currentPanel != null)
                return;

            lblBarcodeValue.Text = code;

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();

                    string sql = "SELECT name, price FROM products WHERE barcode = @code LIMIT 1";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@code", code);

                        using (var rd = cmd.ExecuteReader())
                        {
                            if (rd.Read())
                            {
                                string name = rd.GetString("name");
                                decimal price = rd.GetDecimal("price");

                                lblNameValue.Text = name;
                                lblPriceValue.Text = price.ToString("0.00") + " €";
                                lblCountValue.Text = "1";
                            }
                            else
                            {
                                lblNameValue.Text = "Neznámy produkt";
                                lblPriceValue.Text = "-";
                                lblCountValue.Text = "-";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba databázy pri hľadaní produktu: " + ex.Message,
                    "DB chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnCameraChanged(int cameraIndex)
        {
            try
            {
                scanner?.Start(cameraIndex);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nepodarilo sa prepnúť kameru: " + ex.Message,
                    "Kamera", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        // ===========================================================
        // PANEL SWITCHING
        // ===========================================================

        private void ShowHome()
        {
            if (currentPanel != null)
                currentPanel.Visible = false;

            currentPanel = null;
            LayoutUi();
        }

        private void ShowPanel(Control panel)
        {
            if (currentPanel != null)
                currentPanel.Visible = false;

            currentPanel = panel;
            panel.Visible = true;
            panel.BringToFront();

            LayoutUi();
        }

        // ===========================================================
        // RESPONSIVE LAYOUT
        // ===========================================================

        private void LayoutUi()
        {
            if (backgroundPanel == null) return;

            int w = ClientSize.Width;
            int h = ClientSize.Height;
            int topPadding = 35;

            // === NAVBAR ===
            btnHome.Location = new Point(40, topPadding);
            btnSettings.Location = new Point(150, topPadding);
            btnAddItem.Location = new Point(260, topPadding);
            btnProducts.Location = new Point(380, topPadding); 

            btnLogout.Location = new Point(w - btnLogout.Width - 40, topPadding);

            
            if (activeButton != null)
            {
                underline.Width = (int)(activeButton.Width * 0.45);
                underline.Left = activeButton.Left + (activeButton.Width - underline.Width) / 2;
                underline.Top = activeButton.Bottom + 2;
            }

            // === HOME PANEL ===
            if (currentPanel == null)
            {
                lblTitle.Visible = true;
                lblBarcode.Visible = true;
                lblBarcodeValue.Visible = true;
                lblName.Visible = true;
                lblNameValue.Visible = true;
                lblCount.Visible = true;
                lblCountValue.Visible = true;
                lblPrice.Visible = true;
                lblPriceValue.Visible = true;
                btnAddManual.Visible = true;
                receiptPreview.Visible = true;

                int baseY = topPadding + 180;

                lblTitle.Location = new Point(70, topPadding + 100);
                lblBarcode.Location = new Point(70, baseY);
                lblBarcodeValue.Location = new Point(180, baseY);
                lblName.Location = new Point(70, baseY + 40);
                lblNameValue.Location = new Point(180, baseY + 40);
                lblCount.Location = new Point(70, baseY + 80);
                lblCountValue.Location = new Point(180, baseY + 80);
                lblPrice.Location = new Point(70, baseY + 120);
                lblPriceValue.Location = new Point(180, baseY + 120);
                btnAddManual.Location = new Point(70, baseY + 180);

                int receiptWidth = 340;
                int rightMargin = 80;

                receiptPreview.Size = new Size(receiptWidth, 460);
                receiptPreview.Location = new Point(
                    w - receiptWidth - rightMargin,
                    baseY - 30
                );
            }
            else
            {
                
                lblTitle.Visible = false;
                lblBarcode.Visible = false;
                lblBarcodeValue.Visible = false;
                lblName.Visible = false;
                lblNameValue.Visible = false;
                lblCount.Visible = false;
                lblCountValue.Visible = false;
                lblPrice.Visible = false;
                lblPriceValue.Visible = false;
                btnAddManual.Visible = false;
                receiptPreview.Visible = false;

                int panelTop = topPadding + 60;
                int panelHeight = h - (topPadding + 60);

                settingsPanel.Location = new Point(0, panelTop);
                settingsPanel.Size = new Size(w, panelHeight);

                addItemPanel.Location = new Point(0, panelTop);
                addItemPanel.Size = new Size(w, panelHeight);

                productsPanel.Location = new Point(0, panelTop);
                productsPanel.Size = new Size(w, panelHeight);
            }
        }
    }
}
