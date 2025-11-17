using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace epos
{
    public partial class ProductsPanel : UserControl
    {
        private int? selectedProductId = null;

        public ProductsPanel()
        {
            InitializeComponent();
        }

        // volaj z Form1 pri otvorení Products
        public void RefreshProducts()
        {
            ShowListView();
            LoadProducts();
        }

        // ================= LIST VIEW =================

        private void LoadProducts()
        {
            flowProducts.Controls.Clear();

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT id, name, barcode, category, price, image FROM products ORDER BY name";
                    using (var cmd = new MySqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32("id");
                            string name = reader.GetString("name");
                            string image = reader.IsDBNull(reader.GetOrdinal("image"))
                                ? null
                                : reader.GetString("image");

                            var card = CreateProductCard(id, name, image);
                            flowProducts.Controls.Add(card);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba pri načítavaní produktov: " + ex.Message);
            }
        }

        private Control CreateProductCard(int id, string name, string imagePath)
        {
            var card = new Guna.UI2.WinForms.Guna2Panel
            {
                Width = 220,
                Height = 260,
                BorderRadius = 12,
                FillColor = Color.FromArgb(240, 240, 240),
                Margin = new Padding(15),
                Cursor = Cursors.Hand,
                Tag = id
            };

            var pic = new PictureBox
            {
                Width = card.Width,
                Height = 180,
                Dock = DockStyle.Top,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };

            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                try
                {
                    pic.LoadAsync(imagePath);
                }
                catch
                {
                    // ak sa nepodarí načítať, necháme prázdne
                }
            }

            var nameLabel = new Label
            {
                Text = name,
                Dock = DockStyle.Bottom,
                Height = 40,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Padding = new Padding(10, 0, 0, 0)
            };

            var deleteBtn = new Guna.UI2.WinForms.Guna2Button
            {
                Text = "🗑",
                Width = 32,
                Height = 32,
                BorderRadius = 16,
                FillColor = Color.FromArgb(255, 80, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(card.Width - 40, 8),
                Cursor = Cursors.Hand
            };

            deleteBtn.Click += (s, e) =>
            {
                e = null;
                OnDeleteProduct(id, name);
            };

            card.Controls.Add(nameLabel);
            card.Controls.Add(pic);
            card.Controls.Add(deleteBtn);

            card.Click += (_, __) => OpenDetail(id);
            pic.Click += (_, __) => OpenDetail(id);
            nameLabel.Click += (_, __) => OpenDetail(id);

            return card;
        }

        private void OnDeleteProduct(int id, string name)
        {
            var res = MessageBox.Show(
                $"Naozaj chcete odstrániť produkt:\n{name}?",
                "Odstrániť produkt",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (res != DialogResult.Yes)
                return;

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();
                    string sql = "DELETE FROM products WHERE id = @id";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }

                RefreshProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba pri mazaní produktu: " + ex.Message);
            }
        }

        // ================= DETAIL VIEW =================

        private void OpenDetail(int id)
        {
            selectedProductId = id;
            LoadProductDetail(id);
            ShowDetailView();
        }

        private void LoadProductDetail(int id)
        {
            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT barcode, name, category, price, image FROM products WHERE id = @id";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtDetailBarcode.Text = reader.GetString("barcode");
                                txtDetailName.Text = reader.GetString("name");

                                string category = reader.GetString("category");
                                if (cmbDetailCategory.Items.Contains(category))
                                    cmbDetailCategory.SelectedItem = category;
                                else
                                    cmbDetailCategory.SelectedIndex = 0;

                                txtDetailPrice.Text = reader.GetDecimal("price").ToString("0.##");

                                string image = reader.IsDBNull(reader.GetOrdinal("image"))
                                    ? null
                                    : reader.GetString("image");

                                if (!string.IsNullOrWhiteSpace(image))
                                {
                                    try
                                    {
                                        picDetail.LoadAsync(image);
                                    }
                                    catch
                                    {
                                        picDetail.Image = null;
                                    }
                                }
                                else
                                {
                                    picDetail.Image = null;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba pri načítaní detailu produktu: " + ex.Message);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            ShowListView();
        }

        private void btnSaveDetail_Click(object sender, EventArgs e)
        {
            if (selectedProductId == null)
                return;

            string barcode = txtDetailBarcode.Text.Trim();
            string name = txtDetailName.Text.Trim();
            string category = cmbDetailCategory.SelectedItem?.ToString() ?? "";
            string priceText = txtDetailPrice.Text.Trim();

            if (barcode == "" || name == "" || category == "" || priceText == "")
            {
                MessageBox.Show("Vyplňte všetky polia.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(priceText, out decimal price))
            {
                MessageBox.Show("Cena musí byť číslo.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();
                    string sql = @"UPDATE products 
                                   SET barcode = @b, name = @n, category = @c, price = @p
                                   WHERE id = @id";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@b", barcode);
                        cmd.Parameters.AddWithValue("@n", name);
                        cmd.Parameters.AddWithValue("@c", category);
                        cmd.Parameters.AddWithValue("@p", price);
                        cmd.Parameters.AddWithValue("@id", selectedProductId.Value);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Produkt bol upravený.", "Hotovo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshProducts();
                ShowListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba pri ukladaní produktu: " + ex.Message);
            }
        }

        private void btnDeleteDetail_Click(object sender, EventArgs e)
        {
            if (selectedProductId == null)
                return;

            string name = txtDetailName.Text.Trim();

            var res = MessageBox.Show(
                $"Naozaj chcete odstrániť produkt:\n{name}?",
                "Odstrániť produkt",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (res != DialogResult.Yes)
                return;

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();
                    string sql = "DELETE FROM products WHERE id = @id";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", selectedProductId.Value);
                        cmd.ExecuteNonQuery();
                    }
                }

                selectedProductId = null;
                RefreshProducts();   // zároveň prepne na list view
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba pri mazaní produktu: " + ex.Message);
            }
        }

        // ================= VIEW TOGGLING =================

        private void ShowListView()
        {
            panelList.Visible = true;
            panelDetail.Visible = false;
        }

        private void ShowDetailView()
        {
            panelList.Visible = false;
            panelDetail.Visible = true;
        }
    }
}
