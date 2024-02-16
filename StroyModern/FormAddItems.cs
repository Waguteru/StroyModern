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
    public partial class FormAddItems : Form
    {

        DataBase dataBase = new DataBase();
        private readonly CheckUser _user;

        public FormAddItems(CheckUser user)
        {
            InitializeComponent();
            _user = user;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataBase.OpenConnection();

            var name_item = name_tb.Text;
            var plant = plany_tb.Text;
            var price = price_tb.Text;
            var acticle = article_tb.Text;
            var photo = photo_tb.Text;
            var type = type_tb.Text;

            string query = $"insert into productS_tbl (name_item,number_plant,price_item,article,photo,type_item) values ('{name_item}','{plant}','{price}','{acticle}','{photo}','{type}')";

            NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, dataBase.GetConnection());
            npgsqlCommand.ExecuteNonQuery();

            MessageBox.Show("данные успешно добавлены!", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            dataBase.CloseConnection();

            FormProductsAdmin form1 = new FormProductsAdmin(_user);
            form1.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormProductsAdmin form1 = new FormProductsAdmin(_user);
            form1.Show();
            this.Close();
        }

        private void FormAddItems_Load(object sender, EventArgs e)
        {
            label8.Text = $"{_user.Login}:{_user.Status}";
        }
    }
}
