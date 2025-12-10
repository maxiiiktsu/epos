using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;

namespace epos
{
    public class ManualAddForm : Form
    {
        
        public class ManualProductResult
        {
            public string Barcode { get; set; }
            public string Name { get; set; }
            public decimal UnitPrice { get; set; }
            public int Quantity { get; set; }
            public string ImageUrl { get; set; }
        }

        public ManualProductResult SelectedProduct { get; private set; }

        private Guna2TextBox txtBarcode;
        private Guna2TextBox txtName;
        private Guna2TextBox txtQuantity;
        private Label lblPriceValue;
        private PictureBox picPreview;
        private Guna2Button btnAdd;
        private ListBox lstSuggestions;

        private bool _updatingFields;

        private class Suggestion
        {
            public string Barcode { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public string ImageUrl { get; set; }

            public override string ToString()
            {
                return $"{Barcode} – {Name}";
            }
        }

        public ManualAddForm()
        {
            BuildUi();
        }

        private void BuildUi()
        {
            Text = "Pridať produkt manuálne";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;
            ClientSize = new Size(620, 360);

            var lblTitle = new Label
            {
                Text = "Vyhľadaj pomocou bar kódu alebo názvu",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            Controls.Add(lblTitle);

            int leftX = 20;
            int inputWidth = 260;

            // Bar kód
            int y = 70;
            Controls.Add(new Label
            {
                Text = "Bar kód",
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(leftX, y)
            });

            txtBarcode = new Guna2TextBox
            {
                PlaceholderText = "Zadajte bar kód",
                Location = new Point(leftX, y + 18),
                Size = new Size(inputWidth, 32),
                BorderRadius = 8
            };
            txtBarcode.TextChanged += (_, __) => OnSearchTextChanged();
            txtBarcode.KeyDown += TextBox_KeyDownForSuggestions;
            Controls.Add(txtBarcode);

            // Názov
            y = 140;
            Controls.Add(new Label
            {
                Text = "Názov",
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(leftX, y)
            });

            txtName = new Guna2TextBox
            {
                PlaceholderText = "Zadajte názov",
                Location = new Point(leftX, y + 18),
                Size = new Size(inputWidth, 32),
                BorderRadius = 8
            };
            txtName.TextChanged += (_, __) => OnSearchTextChanged();
            txtName.KeyDown += TextBox_KeyDownForSuggestions;
            Controls.Add(txtName);

            
            int suggestionsTop = y + 18 + 32 + 4;
            lstSuggestions = new ListBox
            {
                Location = new Point(leftX, suggestionsTop),
                Size = new Size(inputWidth, 110),
                Visible = false
            };
            lstSuggestions.Click += LstSuggestions_Click;
            lstSuggestions.KeyDown += LstSuggestions_KeyDown;
            Controls.Add(lstSuggestions);

            // Počet
            y = 210;
            Controls.Add(new Label
            {
                Text = "Počet",
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(leftX, y)
            });

            txtQuantity = new Guna2TextBox
            {
                PlaceholderText = "Zadajte počet",
                Location = new Point(leftX, y + 18),
                Size = new Size(inputWidth, 32),
                BorderRadius = 8
            };
            Controls.Add(txtQuantity);

            // Cena
            y = 260;
            Controls.Add(new Label
            {
                Text = "Cena",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(leftX, y)
            });

            lblPriceValue = new Label
            {
                Text = "-",
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(leftX + 50, y)
            };
            Controls.Add(lblPriceValue);

            
            picPreview = new PictureBox
            {
                BackColor = Color.FromArgb(230, 230, 230),
                Size = new Size(230, 230),
                Location = new Point(330, 90),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            Controls.Add(picPreview);

            var lblImg = new Label
            {
                Text = "obrázok",
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray
            };
            lblImg.Location = new Point(
                picPreview.Left + (picPreview.Width - lblImg.Width) / 2,
                picPreview.Top + (picPreview.Height - lblImg.Height) / 2
            );
            Controls.Add(lblImg);

            // Button Pridať
            btnAdd = new Guna2Button
            {
                Text = "Pridať",
                BorderRadius = 8,
                Size = new Size(90, 36),
                FillColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(leftX, ClientSize.Height - 56)
            };
            btnAdd.Click += BtnAdd_Click;
            Controls.Add(btnAdd);

            AcceptButton = btnAdd;
        }

        // ======================= SUGGESTIONS =======================

        private void OnSearchTextChanged()
        {
            if (_updatingFields) return;

            string term = txtBarcode.Text.Trim();
            if (string.IsNullOrWhiteSpace(term))
                term = txtName.Text.Trim();

            if (term.Length < 2)
            {
                lstSuggestions.Visible = false;
                lstSuggestions.Items.Clear();
                return;
            }

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();

                    string sql =
                        @"SELECT barcode, name, price, image
                          FROM products
                          WHERE barcode LIKE @t OR name LIKE @t
                          ORDER BY name
                          LIMIT 10";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@t", "%" + term + "%");

                        using (var rd = cmd.ExecuteReader())
                        {
                            lstSuggestions.Items.Clear();

                            while (rd.Read())
                            {
                                lstSuggestions.Items.Add(new Suggestion
                                {
                                    Barcode = rd.GetString("barcode"),
                                    Name = rd.GetString("name"),
                                    Price = rd.GetDecimal("price"),
                                    ImageUrl = rd["image"] as string
                                });
                            }
                        }
                    }
                }

                lstSuggestions.Visible = lstSuggestions.Items.Count > 0;
            }
            catch (Exception ex)
            {
                
                MessageBox.Show("Chyba pri vyhľadávaní:\n" + ex.Message,
                    "DB chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lstSuggestions.Visible = false;
                lstSuggestions.Items.Clear();
            }
        }

        private void TextBox_KeyDownForSuggestions(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down && lstSuggestions.Visible && lstSuggestions.Items.Count > 0)
            {
                lstSuggestions.Focus();
                lstSuggestions.SelectedIndex = 0;
                e.Handled = true;
            }
        }

        private void LstSuggestions_Click(object sender, EventArgs e)
        {
            ApplySelectedSuggestion();
        }

        private void LstSuggestions_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ApplySelectedSuggestion();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                lstSuggestions.Visible = false;
                e.Handled = true;
            }
        }

        private void ApplySelectedSuggestion()
        {
            if (lstSuggestions.SelectedItem is Suggestion s)
            {
                _updatingFields = true;

                txtBarcode.Text = s.Barcode;
                txtName.Text = s.Name;
                lblPriceValue.Text = s.Price.ToString("0.00") + " €";

                if (!string.IsNullOrWhiteSpace(s.ImageUrl))
                {
                    try { picPreview.Load(s.ImageUrl); }
                    catch { picPreview.Image = null; }
                }
                else
                {
                    picPreview.Image = null;
                }

                _updatingFields = false;
                lstSuggestions.Visible = false;
            }
        }

        // ======================= PRIDAŤ PRODUKT =======================

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string barcode = txtBarcode.Text.Trim();
            string name = txtName.Text.Trim();
            string qtyText = txtQuantity.Text.Trim();

            if (string.IsNullOrWhiteSpace(barcode) && string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Zadajte bar kód alebo názov.", "Chyba",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int qty = 1;
            if (!string.IsNullOrWhiteSpace(qtyText) && !int.TryParse(qtyText, out qty))
            {
                MessageBox.Show("Počet musí byť celé číslo.", "Chyba",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (qty <= 0) qty = 1;

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();

                    string sql =
                        @"SELECT barcode, name, price, image 
                          FROM products
                          WHERE (@bc <> '' AND barcode = @bc)
                             OR (@bc = '' AND @nm <> '' AND name LIKE @nm)
                          LIMIT 1";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@bc", barcode);
                        cmd.Parameters.AddWithValue("@nm",
                            string.IsNullOrWhiteSpace(name) ? "" : "%" + name + "%");

                        using (var rd = cmd.ExecuteReader())
                        {
                            if (!rd.Read())
                            {
                                MessageBox.Show("Produkt sa nenašiel.", "Info",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }

                            string dbBarcode = rd.GetString("barcode");
                            string dbName = rd.GetString("name");
                            decimal price = rd.GetDecimal("price");
                            string imgUrl = rd["image"] as string;

                            lblPriceValue.Text = price.ToString("0.00") + " €";

                            if (!string.IsNullOrWhiteSpace(imgUrl))
                            {
                                try { picPreview.Load(imgUrl); }
                                catch { picPreview.Image = null; }
                            }
                            else
                            {
                                picPreview.Image = null;
                            }

                            SelectedProduct = new ManualProductResult
                            {
                                Barcode = dbBarcode,
                                Name = dbName,
                                UnitPrice = price,
                                Quantity = qty,
                                ImageUrl = imgUrl
                            };
                        }
                    }
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba pri práci s databázou:\n" + ex.Message,
                    "DB chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
