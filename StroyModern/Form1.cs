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

namespace StroyModern
{
    public partial class Form1 : Form
    {

        DataBase dataBase = new DataBase();

        private readonly CheckUser2 _user;


        enum RowState
        {
            Existed,
            New,
            Modfied,
            ModfiedNew,
            Deleted
        }

        public Form1(CheckUser2 user)
        {
            InitializeComponent();
            addOrders.Visible = false;
            _user = user;
        }

        public void CreateColumns()
        {
            dataGridView1.Columns.Add("name_item", "название товара"); //0
            dataGridView1.Columns.Add("number_plant", "номер цеха"); //1
            dataGridView1.Columns.Add("price_item", "цена товара"); //2
            dataGridView1.Columns.Add("article", "артикул");       //3
            dataGridView1.Columns.Add("photo", "изображение");     //4
            dataGridView1.Columns.Add("type_item", "категория товара"); //5
        }

        private void ReadSingleRow(DataGridView gridView, IDataRecord record)
        {
            gridView.Rows.Add(record.GetString(0), record.GetString(1), record.GetInt64(2), record.GetInt64(3), record.GetString(4), record.GetString(5), RowState.ModfiedNew);
        }

        private void RefreshDataGrid(DataGridView dataGrid)
        {
            dataGrid.Rows.Clear();
            string queryString = $"select name_item,number_plant,price_item,article,photo,type_item from products_tbl";

            NpgsqlCommand command = new NpgsqlCommand(queryString, dataBase.GetConnection());

            dataBase.OpenConnection();

            NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dataGrid, reader);
            }
            reader.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
            label4.Text = $"{_user.Login2}:{_user.Status}";
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                string imagePath = row.Cells[4].Value.ToString();

                if (!string.IsNullOrEmpty(imagePath))
                {

                    pictureBox1.ImageLocation = imagePath;
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    MessageBox.Show("Изображение не найдено");
                }
            }
        }

        private int clickedRowIndex = -1;

        private void addOrders_Click(object sender, EventArgs e)
        {
            if (clickedRowIndex >= 0 && clickedRowIndex < dataGridView1.Rows.Count)
            {
                string name = dataGridView1.Rows[clickedRowIndex].Cells[0].Value.ToString();
              
                string article = dataGridView1.Rows[clickedRowIndex].Cells[3].Value.ToString();
                decimal price_item = Convert.ToDecimal(dataGridView1.Rows[clickedRowIndex].Cells[2].Value);
              


                NpgsqlConnection npgsqlConnection = new NpgsqlConnection("Server = localhost; port = 5432;Database = StroyModern; User Id=postgres; Password = 123");

                FormCreateOrders formOrders = new FormCreateOrders(name, article, price_item, npgsqlConnection, _user);
                formOrders.Show();
                Hide();

            }
        }

      /*  private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Сохраняем индекс строки, на которую нажал пользователь правой кнопкой мыши
                clickedRowIndex = e.RowIndex;

                // Показываем panel1
                addOrders.Visible = true;

            }
        }*/


        private void Search(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string searchString = $"select name_item,number_plant,price_item,article,photo,type_item from products_tbl where concat (name_item, type_item) like '%" + textBox1.Text + "%'";

            NpgsqlCommand comm = new NpgsqlCommand(searchString, dataBase.GetConnection());

            dataBase.OpenConnection();

            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }

            read.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridView1);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == "название (по возрастанию)")
            {
                dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Ascending);
            }
            if (comboBox1.SelectedItem == "категория (по возрастанию)")
            {
                dataGridView1.Sort(dataGridView1.Columns[3], ListSortDirection.Ascending);
            }
            if (comboBox1.SelectedItem == "цена (по возрастанию)")
            {
                dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);
            }


            if (comboBox1.SelectedItem == "название (по убыванию)")
            {
                dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Descending);
            }
            if (comboBox1.SelectedItem == "категория (по убыванию)")
            {
                dataGridView1.Sort(dataGridView1.Columns[3], ListSortDirection.Descending);
            }
            if (comboBox1.SelectedItem == "цена (по убыванию)")
            {
                dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Descending);
            }
        }

        public void AllTypes(DataGridView dgw)
        {
            dgw.Rows.Clear();

            dataBase.OpenConnection();

            string query = $"select name_item,number_plant,price_item,article,photo,type_item from products_tbl";
            NpgsqlCommand comm = new NpgsqlCommand(query, dataBase.GetConnection());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
            dataBase.CloseConnection();
        }

        public void Garden(DataGridView dgw)
        {
            dgw.Rows.Clear();

            dataBase.OpenConnection();

            string query = $"select name_item,number_plant,price_item,article,photo,type_item from products_tbl where type_item LIKE 'Всё для сада'";
            NpgsqlCommand comm = new NpgsqlCommand(query, dataBase.GetConnection());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
            dataBase.CloseConnection();
        }

        public void Doors(DataGridView dgw)
        {
            dgw.Rows.Clear();

            dataBase.OpenConnection();

            string query = $"select name_item,number_plant,price_item,article,photo,type_item from products_tbl where type_item LIKE 'Двери и комплектующие'";
            NpgsqlCommand comm = new NpgsqlCommand(query, dataBase.GetConnection());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
            dataBase.CloseConnection();
        }

        public void Furniture(DataGridView dgw)
        {
            dgw.Rows.Clear();

            dataBase.OpenConnection();

            string query = $"select name_item,number_plant,price_item,article,photo,type_item from products_tbl where type_item LIKE 'Мебель'";
            NpgsqlCommand comm = new NpgsqlCommand(query, dataBase.GetConnection());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
            dataBase.CloseConnection();
        }

        public void Wallpaper(DataGridView dgw)
        {
            dgw.Rows.Clear();

            dataBase.OpenConnection();

            string query = $"select name_item,number_plant,price_item,article,photo,type_item from products_tbl where type_item LIKE 'Обои'";
            NpgsqlCommand comm = new NpgsqlCommand(query, dataBase.GetConnection());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
            dataBase.CloseConnection();
        }

        public void Photowallpaper(DataGridView dgw)
        {
            dgw.Rows.Clear();

            dataBase.OpenConnection();

            string query = $"select name_item,number_plant,price_item,article,photo,type_item from products_tbl where type_item LIKE 'Фотообои'";
            NpgsqlCommand comm = new NpgsqlCommand(query, dataBase.GetConnection());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
            dataBase.CloseConnection();
        }

        public void Cement(DataGridView dgw)
        {
            dgw.Rows.Clear();

            dataBase.OpenConnection();

            string query = $"select name_item,number_plant,price_item,article,photo,type_item from products_tbl where type_item LIKE 'Цементные и сыпучие материалы'";
            NpgsqlCommand comm = new NpgsqlCommand(query, dataBase.GetConnection());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
            dataBase.CloseConnection();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == "Все категории")
            {
                AllTypes(dataGridView1);
            }
            if (comboBox2.SelectedItem == "Всё для сада")
            {
                Garden(dataGridView1);
            }
            if (comboBox2.SelectedItem == "Двери и комплектующие")
            {
                Doors(dataGridView1);
            }
            if (comboBox2.SelectedItem == "Мебель")
            {
                Furniture(dataGridView1);
            }
            if (comboBox2.SelectedItem == "Обои")
            {
                Wallpaper(dataGridView1);
            }
            if (comboBox2.SelectedItem == "Фотообои")
            {
                Photowallpaper(dataGridView1);
            }
            if (comboBox2.SelectedItem == "Цементные и сыпучие материалы")
            {
                Cement(dataGridView1);
            }
        }

       
    }
}
