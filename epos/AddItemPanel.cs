using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace epos
{
    public partial class AddItemPanel : UserControl
    {
        public AddItemPanel()
        {
            InitializeComponent();

            // načítaj DPH kľúče hneď po vytvorení panelu
            LoadVatKeys();
        }

        // =========================
        // DPH (vat_settings) -> combobox
        // Display: "STANDARD (20%)"
        // Value: "STANDARD"
        // =========================
        private void LoadVatKeys()
        {
            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();

                    string sql = "SELECT vat_key, vat_rate FROM vat_settings ORDER BY vat_key";

                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);

                        // pomocný stĺpec pre DisplayText
                        if (!dt.Columns.Contains("DisplayText"))
                            dt.Columns.Add("DisplayText", typeof(string));

                        foreach (DataRow r in dt.Rows)
                        {
                            string key = r["vat_key"].ToString();
                            decimal rate = Convert.ToDecimal(r["vat_rate"], CultureInfo.InvariantCulture);
                            r["DisplayText"] = $"{key} ({rate:0.##}%)";
                        }

                        cmbVatCategory.DisplayMember = "DisplayText";
                        cmbVatCategory.ValueMember = "vat_key";
                        cmbVatCategory.DataSource = dt;

                        // default vyber STANDARD ak existuje, inak prvý
                        int idx = -1;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (dt.Rows[i]["vat_key"].ToString() == "STANDARD")
                            {
                                idx = i;
                                break;
                            }
                        }
                        cmbVatCategory.SelectedIndex = (idx >= 0) ? idx : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nepodarilo sa načítať DPH sadzby: " + ex.Message,
                    "DPH", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // fallback
                cmbVatCategory.DataSource = null;
                cmbVatCategory.Items.Clear();
                cmbVatCategory.Items.Add("STANDARD (fallback)");
                cmbVatCategory.SelectedIndex = 0;
            }
        }

        // =========================
        // ADD PRODUCT
        // =========================
        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            string barcode = txtBarcode.Text.Trim();
            string name = txtName.Text.Trim();
            string category = cmbCategory.SelectedItem?.ToString() ?? "";
            string priceText = txtPrice.Text.Trim();
            string image = txtImage.Text.Trim();

            if (string.IsNullOrWhiteSpace(barcode) ||
                string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(category) ||
                string.IsNullOrWhiteSpace(priceText))
            {
                MessageBox.Show("Vyplňte všetky povinné polia.", "Chyba",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!TryParseDecimal(priceText, out decimal price))
            {
                MessageBox.Show("Cena musí byť číslo.", "Chyba",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // vat_key z comboboxu
            string vatKey = null;

            // ak je DataSource, SelectedValue bude string (vat_key)
            if (cmbVatCategory.SelectedValue is string keyFromDs)
                vatKey = keyFromDs;

            // fallback ak by combobox nebol bindnutý
            if (string.IsNullOrWhiteSpace(vatKey))
                vatKey = cmbVatCategory.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(vatKey))
            {
                MessageBox.Show("Vyberte DPH sadzbu.", "Chyba",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();

                    string sql = @"
                        INSERT INTO products (barcode, name, category, price, image, vat_key)
                        VALUES (@b, @n, @c, @p, @i, @vatKey)";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@b", barcode);
                        cmd.Parameters.AddWithValue("@n", name);
                        cmd.Parameters.AddWithValue("@c", category);
                        cmd.Parameters.AddWithValue("@p", price);
                        cmd.Parameters.AddWithValue("@i",
                            string.IsNullOrWhiteSpace(image) ? (object)DBNull.Value : image);
                        cmd.Parameters.AddWithValue("@vatKey", vatKey);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Produkt bol úspešne pridaný.", "Hotovo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // reset
                txtBarcode.Text = "";
                txtName.Text = "";
                cmbCategory.SelectedIndex = 0;
                txtPrice.Text = "";
                txtImage.Text = "";

                // nechaj DPH na STANDARD (alebo aktuálny výber)
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

        private static bool TryParseDecimal(string s, out decimal value)
        {
            s = (s ?? "").Trim().Replace(',', '.');
            return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }
    }
}
