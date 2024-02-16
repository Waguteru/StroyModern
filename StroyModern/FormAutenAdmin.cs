using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace StroyModern
{
    public partial class FormAutenAdmin : Form
    {

        DataBase dataBase = new DataBase();

        private int failedAttempts = 0;
        private DateTime blockedUntil = DateTime.MinValue;
        private bool closed = false;
        private bool captchaRequired = false;
        private string correctCaptcha = "";

        public FormAutenAdmin()
        {
            InitializeComponent();
            textBox1.Visible = false;
            label1.Visible = false;
            panel1.Visible = false;
        }


        private string GenerateCaptcha()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder captcha = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < 6; i++)
            {
                captcha.Append(chars[random.Next(chars.Length)]);
            }
            return captcha.ToString();
        }

 

        private void button1_Click(object sender, EventArgs e)
        {
            dataBase.OpenConnection();

            var login = loginTXT.Text;
            var password = passTXT.Text;

            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter();
            DataTable table = new DataTable();

            var query = $"select id_user, login_user, password_user, id_role from roles_tbl where login_user = '{login}' and password_user = '{password}'";

            NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, dataBase.GetConnection());

            adapter.SelectCommand = npgsqlCommand;
            adapter.Fill(table);

            if (closed)
            {
                return;
            }
            else if (DateTime.Now < blockedUntil)
            {
                MessageBox.Show($"Попробуйте ещё раз через {blockedUntil.Subtract(DateTime.Now).TotalSeconds} секунд");

                return;
            }
            else if (captchaRequired)
            {
                if (textBox1.Text != correctCaptcha)
                {
                    MessageBox.Show("превышено максимально количество попыток");

                    closed = true;
                    this.Close();
                    return;
                }

                failedAttempts = 0;
                captchaRequired = false;
                textBox1.Text = "";
                var user = new CheckUser(table.Rows[0].ItemArray[1].ToString(), Convert.ToBoolean(table.Rows[0].ItemArray[3]));
                FormProductsAdmin form1 = new FormProductsAdmin(user);
                form1.Show();
                this.Hide();
            }
            else if (table.Rows.Count == 1)
            {
                MessageBox.Show("Вы успешно вошли!", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                var user = new CheckUser(table.Rows[0].ItemArray[1].ToString(), Convert.ToBoolean(table.Rows[0].ItemArray[3]));
                FormProductsAdmin form1 = new FormProductsAdmin(user);
                form1.Show();
                this.Hide();
            }
            else
            {
                failedAttempts++;
                if (failedAttempts == 3)
                {
                    blockedUntil = DateTime.Now.AddSeconds(30);
                    MessageBox.Show($"Вы ввели неправильно в 3 раз.Попробуйте через 30 сек");
                }
            }

            if (failedAttempts >= 4)
            {
                failedAttempts++;
                capLB.Visible = true;
                textBox1.Visible = true;
                label1.Visible = true;
                panel1.Visible = true;
                correctCaptcha = GenerateCaptcha();
                capLB.Text = $"Captcha: {correctCaptcha}";
                captchaRequired = true;
                MessageBox.Show($"Вы ввели в 4 раз неправильно.Код капчи: {correctCaptcha}");
            }

            if (failedAttempts >= 6)
            {
                closed = true;
                this.Close();
            }
        }

        private void FormAutenAdmin_Load(object sender, EventArgs e)
        {

        }
    }
}

 