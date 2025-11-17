using System;
using System.Drawing;
using System.Windows.Forms;

namespace epos
{
    public partial class Form1 : Form
    {
        private SettingsPanel settingsPanel;
        private AddItemPanel addItemPanel;
        private ProductsPanel productsPanel;

        private Control currentPanel;

        // NAVBAR underline + aktívny button
        private Panel underline;
        private Guna.UI2.WinForms.Guna2Button activeButton;

        private Timer underlineAnimTimer;
        private int targetUnderlineX;

        public Form1()
        {
            InitializeComponent();

            // skryť staré tlačidlo Remove Item (už ho nepoužívame)
            btnRemoveItem.Visible = false;

            // SETTINGS PANEL
            settingsPanel = new SettingsPanel();
            settingsPanel.Visible = false;
            backgroundPanel.Controls.Add(settingsPanel);
            settingsPanel.BringToFront();

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

            LayoutUi();
            Resize += (_, __) => LayoutUi();
            Shown += (_, __) => LayoutUi();
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
            btnProducts.Location = new Point(380, topPadding); // Products ide na miesto RemoveItem

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
