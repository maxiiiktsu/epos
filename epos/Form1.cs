using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Fonts;


namespace epos
{
    public partial class Form1 : Form
    {
        private SettingsPanel settingsPanel;
        private AddItemPanel addItemPanel;
        private ProductsPanel productsPanel;

        private Control currentPanel;

        // NAVBAR underline + active button
        private Panel underline;
        private Guna.UI2.WinForms.Guna2Button activeButton;

        private Timer underlineAnimTimer;
        private int targetUnderlineX;

        
        private BarcodeScanner scanner;

        
        private Guna.UI2.WinForms.Guna2TextBox receiptTextBox;

        
        private Guna.UI2.WinForms.Guna2Button btnPrintReceipt;

        // ====== BLOČEK ======
        private class ReceiptItem
        {
            public string Barcode;
            public string Name;
            public decimal UnitPrice;
            public int Quantity;
        }

        private readonly System.Collections.Generic.List<ReceiptItem> receiptItems =
            new System.Collections.Generic.List<ReceiptItem>();

        public Form1()
        {
            WindowsFontResolver.Apply();
            InitializeComponent();

            
            receiptTextBox = new Guna.UI2.WinForms.Guna2TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BorderThickness = 0,
                Font = new Font("Consolas", 9),
                BackColor = Color.White,
                ForeColor = Color.Black,
                Cursor = Cursors.Default
            };
            receiptPreview.Controls.Clear();
            receiptPreview.FillColor = Color.White;
            receiptPreview.Controls.Add(receiptTextBox);

            // print btn
            btnPrintReceipt = new Guna.UI2.WinForms.Guna2Button
            {
                Text = "Tlačiť bloček",
                BorderRadius = 8,
                Size = new Size(140, 40),
                FillColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Visible = true
            };
            btnPrintReceipt.Click += BtnPrintReceipt_Click;
            backgroundPanel.Controls.Add(btnPrintReceipt);
            btnPrintReceipt.BringToFront();

            
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

            
            SetActiveButton(btnHome);

            
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

        // pri zatvorení vypneme kameru
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

            // animácia
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
        // KAMERA SETTINGS
        // ===========================================================

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
        // BARCODE HANDLER
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

                                
                                AddItemToReceipt(code, name, price);
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

        // ===========================================================
        // BLOČEK – manipulácia s položkami
        // ===========================================================

        private void AddItemToReceipt(string barcode, string name, decimal price)
        {
            // ak už produkt v bločku je → zvýšime množstvo
            var existing = receiptItems.FirstOrDefault(i => i.Barcode == barcode);
            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                receiptItems.Add(new ReceiptItem
                {
                    Barcode = barcode,
                    Name = name,
                    UnitPrice = price,
                    Quantity = 1
                });
            }

            UpdateReceiptText();
        }

        private void UpdateReceiptText()
        {
            if (receiptTextBox == null)
                return;

            var sb = new StringBuilder();

            if (receiptItems.Count == 0)
            {
                sb.AppendLine("Bloček je prázdny.");
                receiptTextBox.Text = sb.ToString();
                return;
            }

            sb.AppendLine("Položky");
            sb.AppendLine(new string('-', 40));

            decimal subtotal = 0m;

            foreach (var item in receiptItems)
            {
                decimal lineTotal = item.UnitPrice * item.Quantity;
                subtotal += lineTotal;

                sb.AppendLine(item.Name);
                sb.AppendLine(
                    $"  {item.Quantity} x {item.UnitPrice:0.00} €   = {lineTotal:0.00} €"
                );
                sb.AppendLine();
            }

            sb.AppendLine(new string('-', 40));
            sb.AppendLine($"Spolu: {subtotal:0.00} €");

            receiptTextBox.Text = sb.ToString();
        }

        // ===========================================================
        // TLAČ BLOČKA PDF
        // ===========================================================

        private void BtnPrintReceipt_Click(object sender, EventArgs e)
        {
            if (receiptItems.Count == 0)
            {
                MessageBox.Show("Bloček je prázdny, nie je čo tlačiť.",
                    "Tlač", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var now = DateTime.Now;

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "PDF súbor (*.pdf)|*.pdf";
                sfd.FileName = $"EPOS_{now:yyyyMMdd_HHmmss}.pdf";

                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    var doc = new PdfDocument();
                    doc.Info.Title = "EPOS Bloček";

                    PdfPage page = doc.AddPage();
                    page.Size = PdfSharp.PageSize.A5;
                    page.Orientation = PdfSharp.PageOrientation.Portrait;

                    XGraphics gfx = XGraphics.FromPdfPage(page);

                    var fontHeader = new XFont("Arial", 20, XFontStyleEx.Bold);
                    var fontRegular = new XFont("Arial", 9, XFontStyleEx.Regular);
                    var fontMono = new XFont("Courier New", 9, XFontStyleEx.Regular); // monospace


                    double w = page.Width;
                    double y = 30;

                    var center = XStringFormats.Center;
                    var right = XStringFormats.TopRight;

                    // HEADER
                    gfx.DrawString("EPOS", fontHeader, XBrushes.Black,
                        new XRect(0, y, w, 30), center);
                    y += 40;

                    gfx.DrawString("EPOS", fontRegular, XBrushes.Black,
                        new XRect(0, y, w, 12), center);
                    y += 12;
                    gfx.DrawString("Hálova 16, 851 01 Bratislava", fontRegular, XBrushes.Black,
                        new XRect(0, y, w, 12), center);
                    y += 12;
                    gfx.DrawString("IČO: 12345678", fontRegular, XBrushes.Black,
                        new XRect(0, y, w, 12), center);
                    y += 12;
                    gfx.DrawString("DIČ: 1234567890", fontRegular, XBrushes.Black,
                        new XRect(0, y, w, 12), center);
                    y += 14;

                    gfx.DrawLine(XPens.Black, 20, y, w - 20, y);
                    y += 10;

                    // INFO O DOKLADE
                    gfx.DrawString($"Pokladňa: 69", fontMono, XBrushes.Black,
                        new XPoint(20, y));
                    gfx.DrawString($"Doklad: {now:yyyy-000000}", fontMono, XBrushes.Black,
                        new XPoint(w - 20, y), right);
                    y += 14;

                    gfx.DrawString($"Dátum: {now:dd.MM.yyyy}", fontMono, XBrushes.Black,
                        new XPoint(20, y));
                    gfx.DrawString($"Čas: {now:HH:mm:ss}", fontMono, XBrushes.Black,
                        new XPoint(w - 20, y), right);
                    y += 14;

                    gfx.DrawString("Pokladník:", fontMono, XBrushes.Black,
                        new XPoint(20, y));
                    // zatiaľ nevieme meno → prázdne
                    y += 16;

                    gfx.DrawLine(XPens.Black, 20, y, w - 20, y);
                    y += 10;

                    // POLOŽKY
                    decimal subtotal = 0m;

                    foreach (var item in receiptItems)
                    {
                        decimal lineTotal = item.UnitPrice * item.Quantity;
                        subtotal += lineTotal;

                        gfx.DrawString(item.Name, fontMono, XBrushes.Black,
                            new XPoint(20, y));
                        gfx.DrawString(lineTotal.ToString("0.00") + " €", fontMono, XBrushes.Black,
                            new XPoint(w - 20, y), right);
                        y += 14;

                        string qtyLine = $"{item.Quantity} x {item.UnitPrice:0.00} €";
                        gfx.DrawString(qtyLine, fontMono, XBrushes.Black,
                            new XPoint(30, y));
                        y += 14;

                        y += 4;
                    }

                    gfx.DrawLine(XPens.Black, 20, y, w - 20, y);
                    y += 10;

                    // JEDNODUCHÁ DPH 20 %
                    decimal taxBase = subtotal / 1.20m;
                    decimal tax = subtotal - taxBase;

                    gfx.DrawString("Medzisúčet:", fontMono, XBrushes.Black,
                        new XPoint(20, y));
                    gfx.DrawString(subtotal.ToString("0.00") + " €", fontMono, XBrushes.Black,
                        new XPoint(w - 20, y), right);
                    y += 14;

                    gfx.DrawString("Základ DPH:", fontMono, XBrushes.Black,
                        new XPoint(20, y));
                    gfx.DrawString(taxBase.ToString("0.00") + " €", fontMono, XBrushes.Black,
                        new XPoint(w - 20, y), right);
                    y += 14;

                    gfx.DrawString("DPH 20 %:", fontMono, XBrushes.Black,
                        new XPoint(20, y));
                    gfx.DrawString(tax.ToString("0.00") + " €", fontMono, XBrushes.Black,
                        new XPoint(w - 20, y), right);
                    y += 16;

                    gfx.DrawLine(XPens.Black, 20, y, w - 20, y);
                    y += 10;

                    gfx.DrawString("CELKOM NA ÚHRADU:", fontMono, XBrushes.Black,
                        new XPoint(20, y));
                    gfx.DrawString(subtotal.ToString("0.00") + " €", fontMono, XBrushes.Black,
                        new XPoint(w - 20, y), right);
                    y += 16;

                    gfx.DrawString("Zaplatené kartou:", fontMono, XBrushes.Black,
                        new XPoint(20, y));
                    gfx.DrawString(subtotal.ToString("0.00") + " €", fontMono, XBrushes.Black,
                        new XPoint(w - 20, y), right);
                    y += 20;

                    gfx.DrawLine(XPens.Black, 20, y, w - 20, y);
                    y += 20;

                    gfx.DrawString("Ďakujeme za nákup!", fontMono, XBrushes.Black,
                        new XRect(0, y, w, 12), center);
                    y += 14;
                    gfx.DrawString("Reklamácie len s týmto dokladom.", fontMono, XBrushes.Black,
                        new XRect(0, y, w, 12), center);

                    doc.Save(sfd.FileName);

                    MessageBox.Show("PDF bloček bol uložený:\n" + sfd.FileName,
                        "Tlač", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Chyba pri generovaní PDF:\n" + ex.Message,
                        "Tlač", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
            btnProducts.Location = new Point(380, topPadding); // Products na mieste RemoveItem

            btnLogout.Location = new Point(w - btnLogout.Width - 40, topPadding);

            // UPDATE underline position after resize
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
                receiptTextBox.Visible = true;
                btnPrintReceipt.Visible = true;

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

                // tlačidlo Tlačiť
                btnPrintReceipt.Location = new Point(70, baseY + 230);

                int receiptWidth = 340;
                int rightMargin = 80;

                receiptPreview.Size = new Size(receiptWidth, 460);
                receiptPreview.Location = new Point(
                    w - receiptWidth - rightMargin,
                    baseY - 30
                );

                UpdateReceiptText();
            }
            else
            {
                // schovať HOME prvky
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
                receiptTextBox.Visible = false;
                btnPrintReceipt.Visible = false;

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
