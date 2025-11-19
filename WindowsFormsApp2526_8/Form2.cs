using System;

using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;


namespace WindowsFormsApp2526_8
{
    public partial class Form2 : Form
    {



        public Form2()
        {
            InitializeComponent();
            comboBox1.Visible = false;
            comboBox1.Items.Add("Easy");
            comboBox1.Items.Add("Hard");

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            InitializeDatabase();
            this.Size = new Size(1472, 1023);
            this.MaximumSize = new Size(1472, 1023);
            this.MinimumSize = new Size(1472, 1023);
            richTextBox1.Visible = false;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            comboBox1.Visible = true;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select an option first!");
                return;
            }

            string selectedValue = comboBox1.SelectedItem.ToString();
            Form1 form1 = new Form1(selectedValue);
            form1.Show();
        }
        private void InitializeDatabase()
        {
            string connectionString = "Data source=scoress.db;Version=3";
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();

            string createTable = "CREATE TABLE IF NOT EXISTS Scores (" +
                                 "id INTEGER PRIMARY KEY AUTOINCREMENT," +
                                 "score INTEGER NOT NULL);";

            SQLiteCommand command = new SQLiteCommand(createTable, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Visible = true;
            string connectionString = "Data source=scoress.db;Version=3";
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();

            string select = "SELECT score FROM Scores ORDER BY score ASC LIMIT 5";
            SQLiteCommand command = new SQLiteCommand(select, connection);
            SQLiteDataReader reader = command.ExecuteReader();

            richTextBox1.Clear(); // show results here

            int rank = 1;
            while (reader.Read())
            {
                int score = reader.GetInt32(0);
                richTextBox1.AppendText(rank + ". " + score + "\n");
                rank++;
            }

            connection.Close();

        }
    }
}
