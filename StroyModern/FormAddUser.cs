using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StroyModern
{
    public partial class FormAddUser : Form
    {

        DataBase dataBase = new DataBase();
        private readonly CheckUser _user;

        public FormAddUser(CheckUser user)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            _user = user;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataBase.OpenConnection();

            var login = textBox1.Text;
            var password = textBox2.Text;
           

            string query = $"insert into user_tbl (login_user,password_user) values ('{login}','{password}')";

            NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, dataBase.GetConnection());
            npgsqlCommand.ExecuteNonQuery();

            MessageBox.Show("данные успешно добавлены!", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            dataBase.CloseConnection();

            FormProductsAdmin formProducts = new FormProductsAdmin(_user);
            formProducts.Show();
            this.Close();
        }

        private void FormAddUser_Load(object sender, EventArgs e)
        {
            label6.Text = $"{_user.Login}:{_user.Status}";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FormProductsAdmin formProductsAdmin = new FormProductsAdmin(_user);
            formProductsAdmin.Show();
            this.Close();
        }
    }
}
