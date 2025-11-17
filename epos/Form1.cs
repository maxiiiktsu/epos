using System;
using System.Windows.Forms;

namespace epos
{
    public partial class Form1 : Form
    {
        private readonly string _cashierName;
        private readonly string _cashierCode;

        // Tento konštruktor nechávame kvôli Designeru
        public Form1()
        {
            InitializeComponent();
        }

        // Tento používame z LoginForm
        public Form1(string cashierName, string cashierCode) : this()
        {
            _cashierName = cashierName;
            _cashierCode = cashierCode;

            // napr. titulok okna
            Text = $"Epos – {_cashierName}";

            // ak máš nejaký label v UI (napr. lblUserInfo), môžeš:
            // lblUserInfo.Text = $"{_cashierName} (kód: {_cashierCode})";
        }
    }
}
