using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace epos
{
    public partial class AddItemPanel : UserControl
    {
        public AddItemPanel()
        {
            InitializeComponent();
        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            string barcode = txtBarcode.Text.Trim();
            string name = txtName.Text.Trim();
            string category = cmbCategory.SelectedItem?.ToString() ?? "";
            string priceText = txtPrice.Text.Trim();
            string image = txtImage.Text.Trim();

            if (barcode == "" || name == "" || category == "" || priceText == "")
            {
                MessageBox.Show("Vyplňte všetky povinné polia.", "Chyba",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(priceText, out decimal price))
            {
                MessageBox.Show("Cena musí byť číslo.", "Chyba",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();

                    string sql = @"INSERT INTO products (barcode, name, category, price, image)
                                   VALUES (@b, @n, @c, @p, @i)";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@b", barcode);
                        cmd.Parameters.AddWithValue("@n", name);
                        cmd.Parameters.AddWithValue("@c", category);
                        cmd.Parameters.AddWithValue("@p", price);
                        cmd.Parameters.AddWithValue("@i", image);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Produkt bol úspešne pridaný.", "Hotovo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtBarcode.Text = "";
                txtName.Text = "";
                cmbCategory.SelectedIndex = 0;
                txtPrice.Text = "";
                txtImage.Text = "";
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                {
                    MessageBox.Show("Produkt s týmto čiarovým kódom už existuje.",
                        "Duplicitný záznam", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Chyba databázy: " + ex.Message,
                        "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
