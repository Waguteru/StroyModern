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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace StroyModern
{
    public partial class FormAuten : Form
    {

        DataBase dataBase = new DataBase();

        private int failedAttempts = 0;
        private DateTime blockedUntil = DateTime.MinValue;
        private bool closed = false;
        private bool captchaRequired  = false;
        private string correctCaptcha = "";


        public FormAuten()
        {
            InitializeComponent();
            txtcapcha.Visible = false;
            label5.Visible = false;
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


        private void FormAuten_Load(object sender, EventArgs e)
        {
            /*Random rand = new Random();
            int num = rand.Next(6,8);
            string captcha = "";
            int totl = 0;
            do
            {
                int chr = rand.Next(48, 128);
                if((chr >= 48 && chr <= 57) || (chr >= 65 && chr <= 90) || ( chr >= 97 && chr <= 122))
                {
                    captcha = captcha + (char)chr;
                    totl++;
                    if(totl == num)
                        break;
                   
                }

            }while(true);
            lbcaptha.Text = captcha;*/
        }

        private void button1_Click(object sender, EventArgs e)
        {

          /*  dataBase.OpenConnection();

            var login = textBox2.Text;
            var password = textBox3.Text;

           

            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter();
            DataTable table = new DataTable();

            var query = $"select id_user, login_user, password_user, id_role from user_tbl where login_user = '{login}' and password_user = '{password}'";

            NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, dataBase.GetConnection());

            adapter.SelectCommand = npgsqlCommand;
            adapter.Fill(table);

            if (table.Rows.Count == 1)
            {
                if (lbcaptha.Text == txtcapcha.Text)
                {
                    MessageBox.Show("Капча верна");
                }
                var user = new CheckUser2(table.Rows[0].ItemArray[1].ToString(), Convert.ToBoolean(table.Rows[0].ItemArray[3]));

                MessageBox.Show("Вы успешно вошли!", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Form1 product = new Form1(user);
                product.Show();
                this.Hide();

            }
            else
            {
                 MessageBox.Show("Такого аккаунта не существует!", "Аккаунта не существует!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                 MessageBox.Show("Капча неверна.\nВведите ещё раз");
                  this.OnLoad(e);

            }
            dataBase.CloseConnection();

            */









            /*if (lbcaptha.Text==txtcapcha.Text)
            {
                MessageBox.Show("Капча верна");

                Form1 product = new Form1();
                product.Show();
                Hide();
            }
            else
            {
                MessageBox.Show("Капча неверна.\nВведите ещё раз");
                this.OnLoad(e);
            }
            */
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormAutenAdmin form = new FormAutenAdmin();
            form.Show();
            Hide();
        }

        

        private void button3_Click(object sender, EventArgs e)
        {
            dataBase.OpenConnection();

            var login = textBox2.Text;
            var password = textBox3.Text;

            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter();
            DataTable table = new DataTable();

            var query = $"select id_user, login_user, password_user, id_role from user_tbl where login_user = '{login}' and password_user = '{password}'";

            NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, dataBase.GetConnection());

            adapter.SelectCommand = npgsqlCommand;
            adapter.Fill(table);

            if(closed)
            {
                return;
            }
            else if(DateTime.Now < blockedUntil)
            {
                MessageBox.Show($"Попробуйте ещё раз через {blockedUntil.Subtract(DateTime.Now).TotalSeconds} секунд");

                 return;
            }
            else if(captchaRequired)
            {
                if(txtcapcha.Text != correctCaptcha)
                {
                    MessageBox.Show("превышено максимально количество попыток");

                    closed = true;
                    this.Close();
                    return;
                }

                failedAttempts = 0;
                captchaRequired = false;
                txtcapcha.Text = "";
                var user = new CheckUser2(table.Rows[0].ItemArray[1].ToString(), Convert.ToBoolean(table.Rows[0].ItemArray[3]));
                Form1 form1 = new Form1(user);
                form1.Show();
                this.Hide();
            }
            else if(table.Rows.Count == 1)
            {
                MessageBox.Show("Вы успешно вошли!", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                var user = new CheckUser2(table.Rows[0].ItemArray[1].ToString(), Convert.ToBoolean(table.Rows[0].ItemArray[3]));
                Form1 form1 = new Form1(user);
                form1.Show();
                this.Hide();
            }
            else
            {
                failedAttempts++;
                if(failedAttempts ==3)
                {
                    blockedUntil = DateTime.Now.AddSeconds(30);
                    MessageBox.Show($"Вы ввели неправильно в 3 раз.Попробуйте через 30 сек");
                }
            }

            if(failedAttempts >= 4)
            {
                failedAttempts++;
                capLB.Visible = true;
                txtcapcha.Visible = true;
                label5.Visible = true;
                panel1.Visible = true;
                correctCaptcha = GenerateCaptcha();
                capLB.Text = $"Captcha: {correctCaptcha}";
                captchaRequired = true;
                MessageBox.Show($"Вы ввели в 4 раз неправильно.Код капчи: {correctCaptcha}");
            }

            if(failedAttempts >= 6)
            {
                closed = true;
                this.Close();
            }
        }
    }
}
