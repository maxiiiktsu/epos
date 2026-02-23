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
        private string cashierCode;

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
            public decimal VatRate;
        }

        private readonly System.Collections.Generic.List<ReceiptItem> receiptItems =
            new System.Collections.Generic.List<ReceiptItem>();

        // ====== IMAGE PREVIEW ======
        private PictureBox productPicture;

        private string lastScannedCode = null;
        private DateTime lastScanAt = DateTime.MinValue;

        // ===== HOME GRID =====
        private TableLayoutPanel homeRoot;
        private BorderPanel infoCell;
        private BorderPanel imageCell;
        private BorderPanel receiptCell;

        public Form1(string cashierCode)
        {
            this.cashierCode = cashierCode;

            WindowsFontResolver.Apply();
            InitializeComponent();

            // ===== LOGOUT (fix) =====
            btnLogout.Click -= BtnLogout_Click;
            btnLogout.Click += BtnLogout_Click;

            btnRemoveItem.Visible = false;

            // ===== receipt textbox (do receiptPreview) =====
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

            // ===== print button =====
            btnPrintReceipt = new Guna.UI2.WinForms.Guna2Button
            {
                Text = "Tlačiť bloček",
                BorderRadius = 8,
                Size = new Size(220, 44),
                FillColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Visible = true
            };
            btnPrintReceipt.Click += BtnPrintReceipt_Click;

            // ===== product image preview =====
            productPicture = new PictureBox
            {
                BackColor = Color.FromArgb(25, 25, 25),
                SizeMode = PictureBoxSizeMode.Zoom,
                Dock = DockStyle.Fill,
                Margin = new Padding(12)
            };

            // ===== PANELY =====
            settingsPanel = new SettingsPanel { Visible = false };
            backgroundPanel.Controls.Add(settingsPanel);
            settingsPanel.BringToFront();
            settingsPanel.CameraChanged += OnCameraChanged;

            addItemPanel = new AddItemPanel { Visible = false };
            backgroundPanel.Controls.Add(addItemPanel);
            addItemPanel.BringToFront();

            productsPanel = new ProductsPanel { Visible = false };
            backgroundPanel.Controls.Add(productsPanel);
            productsPanel.BringToFront();

            // ===== UNDERLINE =====
            underline = new Panel
            {
                Height = 2,
                Width = 40,
                BackColor = Color.White,
                Visible = false
            };
            backgroundPanel.Controls.Add(underline);
            underline.BringToFront();

            underlineAnimTimer = new Timer { Interval = 10 };
            underlineAnimTimer.Tick += AnimateUnderline;

            HookNavbarEvents();
            SetActiveButton(btnHome);

            BuildHomeGrid();

            // manuálne pridanie
            btnAddManual.Click += btnAddManual_Click;

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
            try
            {
                scanner?.Dispose();
                scanner = null;
            }
            catch { }

            base.OnFormClosing(e);
        }

        // ===== LOGOUT HANDLER =====
        private void BtnLogout_Click(object sender, EventArgs e)
        {
            
            try
            {
                scanner?.Dispose();
                scanner = null;
            }
            catch { }

            Close(); 
        }

        // ===========================================================
        // HOME GRID BUILD
        // ===========================================================

        private class BorderPanel : Panel
        {
            public Color BorderColor { get; set; } = Color.FromArgb(55, 255, 255, 255);
            public int BorderThickness { get; set; } = 1;

            public BorderPanel()
            {
                DoubleBuffered = true;
                BackColor = Color.Transparent;
                Padding = new Padding(14);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                using (var pen = new Pen(BorderColor, BorderThickness))
                {
                    var r = ClientRectangle;
                    r.Width -= 1;
                    r.Height -= 1;
                    e.Graphics.DrawRectangle(pen, r);
                }
            }
        }

        private void BuildHomeGrid()
        {
            if (homeRoot != null) return;

            homeRoot = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 1,
                Dock = DockStyle.None,
                BackColor = Color.Transparent
            };

            homeRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34f));
            homeRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28f));
            homeRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38f));
            homeRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            infoCell = new BorderPanel();
            imageCell = new BorderPanel
            {
                BorderColor = Color.FromArgb(40, 255, 255, 255),
                BackColor = Color.FromArgb(18, 18, 18)
            };
            receiptCell = new BorderPanel();

            homeRoot.Controls.Add(infoCell, 0, 0);
            homeRoot.Controls.Add(imageCell, 1, 0);
            homeRoot.Controls.Add(receiptCell, 2, 0);

            infoCell.Dock = DockStyle.Fill;
            imageCell.Dock = DockStyle.Fill;
            receiptCell.Dock = DockStyle.Fill;

            // ---- INFO CELL (table) ----
            var infoTable = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 7,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            infoTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            infoTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            infoTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            infoTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 14));
            infoTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            infoTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            infoTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            infoTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            infoTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            lblTitle.Dock = DockStyle.Top;
            lblTitle.Margin = new Padding(0, 0, 0, 0);

            infoTable.Controls.Add(lblTitle, 0, 0);
            infoTable.SetColumnSpan(lblTitle, 2);

            lblBarcode.Dock = DockStyle.Top;
            lblBarcodeValue.Dock = DockStyle.Top;
            lblBarcodeValue.Margin = new Padding(0, 0, 0, 10);
            infoTable.Controls.Add(lblBarcode, 0, 2);
            infoTable.Controls.Add(lblBarcodeValue, 1, 2);

            lblName.Dock = DockStyle.Top;
            lblNameValue.Dock = DockStyle.Top;
            lblNameValue.Margin = new Padding(0, 0, 0, 10);
            infoTable.Controls.Add(lblName, 0, 3);
            infoTable.Controls.Add(lblNameValue, 1, 3);

            lblCount.Dock = DockStyle.Top;
            lblCountValue.Dock = DockStyle.Top;
            lblCountValue.Margin = new Padding(0, 0, 0, 10);
            infoTable.Controls.Add(lblCount, 0, 4);
            infoTable.Controls.Add(lblCountValue, 1, 4);

            lblPrice.Dock = DockStyle.Top;
            lblPriceValue.Dock = DockStyle.Top;
            lblPriceValue.Margin = new Padding(0, 0, 0, 10);
            infoTable.Controls.Add(lblPrice, 0, 5);
            infoTable.Controls.Add(lblPriceValue, 1, 5);

            var buttonsFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0, 12, 0, 0)
            };

            btnAddManual.Width = 220;
            btnAddManual.Height = 44;

            buttonsFlow.Controls.Add(btnAddManual);
            buttonsFlow.Controls.Add(btnPrintReceipt);

            var infoHost = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            infoHost.Controls.Add(infoTable);
            infoHost.Controls.Add(buttonsFlow);

            infoCell.Controls.Add(infoHost);

            imageCell.Controls.Add(productPicture);

            receiptPreview.Dock = DockStyle.Fill;
            receiptCell.Controls.Add(receiptPreview);

            backgroundPanel.Controls.Add(homeRoot);
            homeRoot.BringToFront();

            btnHome.BringToFront();
            btnSettings.BringToFront();
            btnAddItem.BringToFront();
            btnProducts.BringToFront();
            btnLogout.BringToFront();
            underline.BringToFront();
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

            underline.Width = (int)(btn.Width * 0.45);
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
        // MANUAL ADD
        // ===========================================================

        private void btnAddManual_Click(object sender, EventArgs e)
        {
            using (var dlg = new ManualAddForm())
            {
                if (dlg.ShowDialog(this) == DialogResult.OK && dlg.SelectedProduct != null)
                {
                    var p = dlg.SelectedProduct;

                    AddItemToReceipt(p.Barcode, p.Name, p.UnitPrice, p.Quantity, p.VatRate);

                    lblBarcodeValue.Text = p.Barcode;
                    lblNameValue.Text = p.Name;
                    lblCountValue.Text = GetQuantityForBarcode(p.Barcode).ToString();
                    lblPriceValue.Text = p.UnitPrice.ToString("0.00") + " €";

                    string img = TryGetSelectedProductImage(p);
                    SetProductImage(img);
                }
            }
        }

        private string TryGetSelectedProductImage(object selectedProduct)
        {
            try
            {
                var t = selectedProduct.GetType();

                var p1 = t.GetProperty("ImageUrl");
                if (p1 != null)
                {
                    var v = p1.GetValue(selectedProduct) as string;
                    if (!string.IsNullOrWhiteSpace(v)) return v;
                }

                var p2 = t.GetProperty("Image");
                if (p2 != null)
                {
                    var v = p2.GetValue(selectedProduct) as string;
                    if (!string.IsNullOrWhiteSpace(v)) return v;
                }
            }
            catch { }

            return null;
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

            var now = DateTime.Now;
            if (code == lastScannedCode && (now - lastScanAt).TotalMilliseconds < 700)
                return;

            lastScannedCode = code;
            lastScanAt = now;

            if (currentPanel != null)
                return;

            lblBarcodeValue.Text = code;

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();

                    string sql = @"
                                SELECT 
                                    p.name, 
                                    p.price, 
                                    p.image, 
                                    COALESCE(s.vat_rate, 20.00) AS vat_rate
                                FROM products p
                                LEFT JOIN vat_settings s ON s.vat_key = p.vat_key
                                WHERE p.barcode = @code
                                LIMIT 1";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@code", code);

                        using (var rd = cmd.ExecuteReader())
                        {
                            if (rd.Read())
                            {
                                string name = rd.GetString("name");
                                decimal price = rd.GetDecimal("price");
                                decimal vatRate = rd.GetDecimal(rd.GetOrdinal("vat_rate"));
                                string img = rd["image"] as string;

                                lblNameValue.Text = name;
                                lblPriceValue.Text = price.ToString("0.00") + " €";

                                AddItemToReceipt(code, name, price, 1, vatRate);
                                lblCountValue.Text = GetQuantityForBarcode(code).ToString();

                                SetProductImage(img);
                            }
                            else
                            {
                                lblNameValue.Text = "Neznámy produkt";
                                lblPriceValue.Text = "-";
                                lblCountValue.Text = "-";
                                SetProductImage(null);
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
        // RECEIPT
        // ===========================================================

        private void AddItemToReceipt(string barcode, string name, decimal price)
        {
            AddItemToReceipt(barcode, name, price, 1, 20m);
        }

        private void AddItemToReceipt(string barcode, string name, decimal price, int quantity)
        {
            AddItemToReceipt(barcode, name, price, quantity, 20m);
        }

        private void AddItemToReceipt(string barcode, string name, decimal price, int quantity, decimal vatRate)
        {
            if (quantity <= 0) quantity = 1;

            var existing = receiptItems.FirstOrDefault(i => i.Barcode == barcode);
            if (existing != null)
            {
                existing.Quantity += quantity;
                existing.UnitPrice = price;
                existing.Name = name;
                existing.VatRate = vatRate;
            }
            else
            {
                receiptItems.Add(new ReceiptItem
                {
                    Barcode = barcode,
                    Name = name,
                    UnitPrice = price, // bez DPH
                    Quantity = quantity,
                    VatRate = vatRate
                });
            }

            UpdateReceiptText();
        }

        private int GetQuantityForBarcode(string barcode)
        {
            var it = receiptItems.FirstOrDefault(x => x.Barcode == barcode);
            return it?.Quantity ?? 0;
        }

        private void SetProductImage(string imageValue)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageValue))
                {
                    productPicture.Image = null;
                    return;
                }

                productPicture.Image = null;
                productPicture.LoadAsync(imageValue);
            }
            catch
            {
                productPicture.Image = null;
            }
        }

        private static decimal Round2(decimal v) => Math.Round(v, 2, MidpointRounding.AwayFromZero);

        private class VatTotals
        {
            public decimal Net;
            public decimal Vat;
            public decimal Gross;
        }

        private static VatTotals CalcLineNet(decimal netUnitPrice, int qty, decimal vatRate)
        {
            var net = netUnitPrice * qty;
            var vat = net * (vatRate / 100m);
            var gross = net + vat;

            return new VatTotals
            {
                Net = Round2(net),
                Vat = Round2(vat),
                Gross = Round2(gross)
            };
        }

        private void UpdateReceiptText()
        {
            if (receiptTextBox == null) return;

            var sb = new StringBuilder();

            if (receiptItems.Count == 0)
            {
                sb.AppendLine("Bloček je prázdny.");
                receiptTextBox.Text = sb.ToString();
                return;
            }

            sb.AppendLine("Položky");
            sb.AppendLine(new string('-', 42));

            decimal totalNet = 0m;

            foreach (var item in receiptItems)
            {
                decimal lineNet = item.UnitPrice * item.Quantity;
                totalNet += lineNet;

                sb.AppendLine(item.Name);
                sb.AppendLine($"  {item.Quantity} x {item.UnitPrice:0.00} € = {lineNet:0.00} €  (DPH {item.VatRate:0}%)");
                sb.AppendLine();
            }

            var groups = receiptItems
                .GroupBy(x => x.VatRate)
                .Select(g => new
                {
                    VatRate = g.Key,
                    Net = g.Sum(i => i.UnitPrice * i.Quantity)
                })
                .OrderBy(g => g.VatRate)
                .ToList();

            sb.AppendLine(new string('-', 42));
            sb.AppendLine($"Základ spolu: {totalNet:0.00} €");

            decimal totalVat = 0m;
            foreach (var g in groups)
            {
                decimal vat = Math.Round(g.Net * (g.VatRate / 100m), 2, MidpointRounding.AwayFromZero);
                totalVat += vat;

                sb.AppendLine($"DPH {g.VatRate:0}%:  základ {g.Net:0.00} €  DPH {vat:0.00} €");
            }

            sb.AppendLine(new string('-', 42));
            sb.AppendLine($"Spolu na úhradu: {(totalNet + totalVat):0.00} €");

            receiptTextBox.Text = sb.ToString();
        }

        // ===========================================================
        // PDF PRINT
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

                    var page = doc.AddPage();
                    page.Size = PdfSharp.PageSize.A5;

                    var gfx = XGraphics.FromPdfPage(page);

                    var fontMono = new XFont("Courier New", 9);

                    double pw = page.Width;
                    double y = 25;

                    var right = XStringFormats.TopRight;
                    var center = XStringFormats.Center;

                    decimal totalNet = 0m;
                    decimal totalVat = 0m;
                    decimal totalGross = 0m;

                    // ===== HEADER =====
                    gfx.DrawString("EPOS", new XFont("Arial", 20), XBrushes.Black,
                        new XRect(0, y, pw, 30), center);
                    y += 40;

                    gfx.DrawString("EPOS", fontMono, XBrushes.Black, new XRect(0, y, pw, 12), center); y += 12;
                    gfx.DrawString("Hálova 16, 851 01 Bratislava", fontMono, XBrushes.Black, new XRect(0, y, pw, 12), center); y += 12;
                    gfx.DrawString("IČO: 12345678", fontMono, XBrushes.Black, new XRect(0, y, pw, 12), center); y += 12;
                    gfx.DrawString("DIČ: 1234567890", fontMono, XBrushes.Black, new XRect(0, y, pw, 12), center); y += 12;
                    gfx.DrawString("IČ DPH: SK1234567890", fontMono, XBrushes.Black, new XRect(0, y, pw, 12), center); y += 18;

                    gfx.DrawLine(XPens.Black, 20, y, pw - 20, y);
                    y += 14;

                    // ===== INFO RIADKY =====
                    string docNumber = now.ToString("yyyyMMddHHmmss");

                    gfx.DrawString($"Pokladňa: 69", fontMono, XBrushes.Black, new XPoint(20, y));
                    gfx.DrawString($"Doklad: {docNumber}", fontMono, XBrushes.Black, new XPoint(pw - 20, y), right);
                    y += 14;

                    gfx.DrawString($"Dátum: {now:dd.MM.yyyy}", fontMono, XBrushes.Black, new XPoint(20, y));
                    gfx.DrawString($"Čas: {now:HH:mm:ss}", fontMono, XBrushes.Black, new XPoint(pw - 20, y), right);
                    y += 14;

                    gfx.DrawString($"Pokladník: {cashierCode}", fontMono, XBrushes.Black, new XPoint(20, y));
                    y += 18;

                    gfx.DrawLine(XPens.Black, 20, y, pw - 20, y);
                    y += 14;

                    // ===== ITEMS =====
                    foreach (var item in receiptItems)
                    {
                        var line = CalcLineNet(item.UnitPrice, item.Quantity, item.VatRate);

                        totalNet += line.Net;
                        totalVat += line.Vat;
                        totalGross += line.Gross;

                        gfx.DrawString(item.Name, fontMono, XBrushes.Black, new XPoint(20, y));
                        gfx.DrawString(line.Gross.ToString("0.00") + " €", fontMono, XBrushes.Black, new XPoint(pw - 20, y), right);
                        y += 12;

                        gfx.DrawString($"{item.Quantity} x {item.UnitPrice:0.00} €  (DPH {item.VatRate}%)",
                            fontMono, XBrushes.Black, new XPoint(30, y));
                        y += 18;
                    }

                    gfx.DrawLine(XPens.Black, 20, y, pw - 20, y);
                    y += 14;

                    // ===== SUMMARY =====
                    gfx.DrawString("Medzisúčet (netto):", fontMono, XBrushes.Black, new XPoint(20, y));
                    gfx.DrawString(totalNet.ToString("0.00") + " €", fontMono, XBrushes.Black, new XPoint(pw - 20, y), right);
                    y += 12;

                    gfx.DrawString("DPH spolu:", fontMono, XBrushes.Black, new XPoint(20, y));
                    gfx.DrawString(totalVat.ToString("0.00") + " €", fontMono, XBrushes.Black, new XPoint(pw - 20, y), right);
                    y += 12;

                    gfx.DrawString("CELKOM NA ÚHRADU:", fontMono, XBrushes.Black, new XPoint(20, y));
                    gfx.DrawString(totalGross.ToString("0.00") + " €", fontMono, XBrushes.Black, new XPoint(pw - 20, y), right);

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

            // NAVBAR
            btnHome.Location = new Point(40, topPadding);
            btnSettings.Location = new Point(150, topPadding);
            btnAddItem.Location = new Point(260, topPadding);
            btnProducts.Location = new Point(380, topPadding);
            btnLogout.Location = new Point(w - btnLogout.Width - 40, topPadding);

            // underline
            if (activeButton != null)
            {
                underline.Width = (int)(activeButton.Width * 0.45);
                underline.Left = activeButton.Left + (activeButton.Width - underline.Width) / 2;
                underline.Top = activeButton.Bottom + 2;
            }

            // HOME vs PANELS
            if (currentPanel == null)
            {
                homeRoot.Visible = true;

                settingsPanel.Visible = false;
                addItemPanel.Visible = false;
                productsPanel.Visible = false;

                int marginX = 50;
                int top = topPadding + 85;
                int marginBottom = 45;

                homeRoot.Location = new Point(marginX, top);
                homeRoot.Size = new Size(
                    Math.Max(600, w - (marginX * 2)),
                    Math.Max(420, h - top - marginBottom)
                );

                UpdateReceiptText();
            }
            else
            {
                homeRoot.Visible = false;

                int panelTop = topPadding + 60;
                int panelHeight = h - panelTop;

                currentPanel.Location = new Point(0, panelTop);
                currentPanel.Size = new Size(w, panelHeight);
                currentPanel.Visible = true;
                currentPanel.BringToFront();

                btnHome.BringToFront();
                btnSettings.BringToFront();
                btnAddItem.BringToFront();
                btnProducts.BringToFront();
                btnLogout.BringToFront();
                underline.BringToFront();
            }
        }
    }
}