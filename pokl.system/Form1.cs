using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Media;



namespace pokl.system
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();


        }

        List<Product> products = new List<Product>();




        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice videoCaptureDevice;

        private void Form1_Load(object sender, EventArgs e)
        {
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in filterInfoCollection)
                comboBox1.Items.Add(device.Name);
            comboBox1.SelectedIndex = 0;

            videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[comboBox1.SelectedIndex].MonikerString);
            videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
            videoCaptureDevice.Start();



            tabControl1.Dock = DockStyle.Fill;
            try
            {
                FileStream file = new FileStream("database.json", FileMode.CreateNew);

            }
            catch
            {

            }



            button2_Click(this, EventArgs.Empty);
            this.Resize += new EventHandler(poklsystem_resize);

        }

        private void poklsystem_resize(object sender, EventArgs e)
        {
            CenterPanel();
        }

        private void CenterPanel()
        {

            panel1.Left = (this.ClientSize.Width - panel1.Width) / 2;
            panel1.Top = (this.ClientSize.Height - panel1.Height) / 2;
        }



        private void button1_Click(object sender, EventArgs e)
        {


        }

        private void VideoCaptureDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            BarcodeReader reader = new BarcodeReader();
            var result = reader.Decode(bitmap);
            if (result != null)
            {
                textBox1.Invoke(new MethodInvoker(delegate ()
                {
                    textBox1.Text = result.ToString();
                }));
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoCaptureDevice != null)
            {
                if (videoCaptureDevice.IsRunning)
                    videoCaptureDevice.Stop();
            }
        }

        private void button1_settings_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();

            settings.ShowDialog();



        }

        private void button_Click_1(object sender, EventArgs e)
        {

            bool isValid = true;

            Product product = new Product();
            if (textBox_id.Text != string.Empty)
            {
                product.Id = textBox_id.Text;
            }
            else
            {
                label5.Text = " Chyba v zadavani udajov";
                isValid = false;
            }

            if (textBox_name.Text != string.Empty)
            {
                product.Name = textBox_name.Text;
            }
            else
            {
                label5.Text = " Chyba v zadavani udajov";
                isValid = false;
            }

            double weight_validation;
            if (double.TryParse(textBox_weight.Text, out weight_validation))
            {
                product.Weight = weight_validation;
            }
            else
            {
                label5.Text = " Chyba v zadavani udajov";
                isValid = false;
            }

            double price_validation;
            if (double.TryParse(textBox_price.Text, out price_validation))
            {
                product.Price = price_validation;
            }
            else
            {
                label5.Text = " Chyba v zadavani udajov";
                isValid = false;
            }

            if (isValid)
            {
                products.Add(product);
                label5.Text = "Item bol pridany";
            }

            textBox_id.Text = string.Empty;
            textBox_name.Text = string.Empty;
            textBox_price.Text = string.Empty;
            textBox_weight.Text = string.Empty;


        }

        private void button2_Click(object sender, EventArgs e)
        {

            string dejson = File.ReadAllText("database.json");
            products = JsonSerializer.Deserialize<List<Product>>(dejson) ?? new List<Product>();
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.ColumnStyles.Clear();

            tableLayoutPanel1.RowCount = products.Count + 1;
            tableLayoutPanel1.ColumnCount = 4;

            for (int i = 0; i < tableLayoutPanel1.ColumnCount; i++)
            {
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            }

            tableLayoutPanel1.Controls.Add(new Label() { Text = "ID" }, 0, 0);
            tableLayoutPanel1.Controls.Add(new Label() { Text = "Name" }, 1, 0);
            tableLayoutPanel1.Controls.Add(new Label() { Text = "Weight" }, 2, 0);
            tableLayoutPanel1.Controls.Add(new Label() { Text = "Price" }, 3, 0);

            for (int row = 0; row < products.Count; row++)
            {
                Product product = products[row];
                tableLayoutPanel1.Controls.Add(new Label() { Text = product.Id }, 0, row + 1);
                tableLayoutPanel1.Controls.Add(new Label() { Text = product.Name }, 1, row + 1);
                tableLayoutPanel1.Controls.Add(new Label() { Text = product.Weight.ToString() }, 2, row + 1);
                tableLayoutPanel1.Controls.Add(new Label() { Text = product.Price.ToString("C") }, 3, row + 1);
            }

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button_Add_Click(object sender, EventArgs e)
        {
            bool isValid = true;

            Product product = new Product();
            if (textBox_id.Text != string.Empty)
            {
                product.Id = textBox_id.Text;
            }
            else
            {
                label5.Text = " Chyba v zadavani udajov";
                isValid = false;
            }

            if (textBox_name.Text != string.Empty)
            {
                product.Name = textBox_name.Text;
            }
            else
            {
                label5.Text = " Chyba v zadavani udajov";
                isValid = false;
            }

            double weight_validation;
            if (double.TryParse(textBox_weight.Text, out weight_validation))
            {
                product.Weight = weight_validation;
            }
            else
            {
                label5.Text = " Chyba v zadavani udajov";
                isValid = false;
            }

            double price_validation;
            if (double.TryParse(textBox_price.Text, out price_validation))
            {
                product.Price = price_validation;
            }
            else
            {
                label5.Text = " Chyba v zadavani udajov";
                isValid = false;
            }

            if (isValid)
            {
                products.Add(product);
                label5.Text = "Item bol pridany";
            }

            textBox_id.Text = string.Empty;
            textBox_name.Text = string.Empty;
            textBox_price.Text = string.Empty;
            textBox_weight.Text = string.Empty;

            string oldJson = File.ReadAllText("database.json");

            if (string.IsNullOrWhiteSpace(oldJson))
            {
                oldJson = "[]";
            }

            List<Product> de_products = JsonSerializer.Deserialize<List<Product>>(oldJson) ?? new List<Product>();

            de_products.Add(products.Last());

            string newJson = JsonSerializer.Serialize(de_products, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText("database.json", newJson);




        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string id = textBox1.Text;

            if (!string.IsNullOrEmpty(id))
            {


                Product foundproduct = products.FirstOrDefault(p => p.Id == textBox1.Text);

                if (foundproduct != null)
                {
                    textBox2.Text = foundproduct.Name;
                    textBox3.Text = foundproduct.Weight.ToString();
                    textBox4.Text = foundproduct.Price.ToString();
                }
                else
                {
                    textBox2.Text = string.Empty;
                    textBox3.Text = string.Empty;
                    textBox4.Text = string.Empty;
                }


            }
            else
            {
                textBox2.Text = string.Empty;
                textBox3.Text = string.Empty;
                textBox4.Text = string.Empty;
            }

            string remove_id = textBox8.Text;

            if (!string.IsNullOrEmpty(remove_id))
            {


                Product foundproduct = products.FirstOrDefault(p => p.Id == textBox8.Text);

                if (foundproduct != null)
                {
                    textBox7.Text = foundproduct.Name;
                    textBox6.Text = foundproduct.Weight.ToString();
                    textBox5.Text = foundproduct.Price.ToString();
                }
                else
                {
                    textBox7.Text = string.Empty;
                    textBox6.Text = string.Empty;
                    textBox5.Text = string.Empty;
                }


            }
            else
            {
                textBox7.Text = string.Empty;
                textBox6.Text = string.Empty;
                textBox5.Text = string.Empty;
            }
        }

        private void button3_remove_Click(object sender, EventArgs e)
        {
            Product foundproduct = products.FirstOrDefault(p => p.Id == textBox8.Text);
            products.Remove(foundproduct);
            string newJson = JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("database.json", newJson);

            textBox8.Text = string.Empty;
            textBox7.Text = string.Empty;
            textBox6.Text = string.Empty;
            textBox5.Text = string.Empty;

        }

        List<Product> bought_products = new List<Product>();

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            Product foundproduct = products.FirstOrDefault(p => p.Id == textBox1.Text);
            bought_products.Add(foundproduct);
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Produkt\tVaha\tCena");
            sb.AppendLine("-------------------------------------------");


            foreach (Product product in bought_products)
            {
                
                if (product != null)
                {
                    sb.AppendLine($"{product.Name}\t{product.Weight + "g"}\t{product.Price.ToString("C")}");
                }
                else
                {
                    label14.Text = "nieje nacitany ziadny produkt";
                }

            }

            richTextBox1.Text = sb.ToString();
            //SoundPlayer beep = new SoundPlayer();


            //richTextBox1.ScrollToCaret();


        }

        

        

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

            //SystemSounds.Asterisk.Play();
            

            /*richTextBox1.SelectionStart = richTextBox1.Text.Length;
            
            richTextBox1.ScrollToCaret();*/
            
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
}
