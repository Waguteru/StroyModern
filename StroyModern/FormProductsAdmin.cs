using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace StroyModern
{
    
    public partial class FormProductsAdmin : Form
    {
        DataBase dataBase = new DataBase();

        private readonly CheckUser _user;

        enum RowState
        {
            Exited,
            New,
            Modifided,
            ModifidedNew,
            Deleted
        }

        int selectedRow;

        public FormProductsAdmin(CheckUser user)
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
            dataGridView1.Columns.Add("id_item", "ID товара"); //6
            dataGridView1.Columns.Add("isNew", String.Empty); //7
            dataGridView1.Columns["isNew"].Visible = false;
        }

       /* private void ReadSingleRow(DataGridView gridView, IDataRecord record)
        {
            gridView.Rows.Add(record.GetString(0), record.GetString(1), record.GetInt64(2), record.GetInt64(3), record.GetString(4), record.GetString(5), record.GetInt32(6), RowState.ModfiedNew);
        }*/

        private void ReadSingleRow2(DataGridView gridView, IDataRecord record)
        {
            gridView.Rows.Add(record.GetString(0), record.GetString(1), record.GetInt64(2), record.GetInt64(3), record.GetString(4), record.GetString(5), record.GetInt32(6), RowState.ModifidedNew);
        }

        private void RefreshDataGrid(DataGridView dataGrid)
        {
            dataGrid.Rows.Clear();
            string queryString = $"select name_item,number_plant,price_item,article,photo,type_item,id_item from products_tbl";

            NpgsqlCommand command = new NpgsqlCommand(queryString, dataBase.GetConnection());

            dataBase.OpenConnection();

            NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow2(dataGrid, reader);
            }
            reader.Close();
        }
        private void addOrders_Click(object sender, EventArgs e)
        {
            if (clickedRowIndex >= 0 && clickedRowIndex < dataGridView1.Rows.Count)
            {
                string name = dataGridView1.Rows[clickedRowIndex].Cells[0].Value.ToString();

                string article = dataGridView1.Rows[clickedRowIndex].Cells[3].Value.ToString();
                decimal price_item = Convert.ToDecimal(dataGridView1.Rows[clickedRowIndex].Cells[2].Value);



                NpgsqlConnection npgsqlConnection = new NpgsqlConnection("Server = localhost; port = 5432;Database = StroyModern; User Id=postgres; Password = 123");

                FormAddOrders formOrders = new FormAddOrders(name, article, price_item, npgsqlConnection, _user);
                formOrders.Show();
                Hide();

            }
        }

        private void IsAdmin()
        {
            button3.Enabled = _user.IsAdmin;
            button4.Enabled = _user.IsAdmin;
            button5.Enabled = _user.IsAdmin;
            button6.Enabled = _user.IsAdmin;
            button1.Enabled = _user.IsAdmin;
        }

        private void FormProductsAdmin_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
            IsAdmin();
            rolesLB.Text = $"{_user.Login}:{_user.Status}";
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            selectedRow = e.RowIndex;

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

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBox2.Text = row.Cells[0].Value.ToString();
                textBox3.Text = row.Cells[1].Value.ToString();
                textBox4.Text = row.Cells[2].Value.ToString();
                textBox5.Text = row.Cells[3].Value.ToString();
                textBox6.Text = row.Cells[4].Value.ToString();
                textBox7.Text = row.Cells[5].Value.ToString();
                textBox8.Text = row.Cells[6].Value.ToString();
            }

        }

        private int clickedRowIndex = -1;

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Сохраняем индекс строки, на которую нажал пользователь правой кнопкой мыши
                clickedRowIndex = e.RowIndex;

                // Показываем panel1
                addOrders.Visible = true;

            }
        }

        private void Search(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string searchString = $"select name_item,number_plant,price_item,article,photo,type_item,id_item from products_tbl where concat (name_item, type_item) like '%" + textBox1.Text + "%'";

            NpgsqlCommand comm = new NpgsqlCommand(searchString, dataBase.GetConnection());

            dataBase.OpenConnection();

            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow2(dgw, read);
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

            string query = $"select name_item,number_plant,price_item,article,photo,type_item,id_item from products_tbl";
            NpgsqlCommand comm = new NpgsqlCommand(query, dataBase.GetConnection());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow2(dgw, read);
            }
            read.Close();
            dataBase.CloseConnection();
        }

        public void Garden(DataGridView dgw)
        {
            dgw.Rows.Clear();

            dataBase.OpenConnection();

            string query = $"select name_item,number_plant,price_item,article,photo,type_item,id_item from products_tbl where type_item LIKE 'Всё для сада'";
            NpgsqlCommand comm = new NpgsqlCommand(query, dataBase.GetConnection());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow2(dgw, read);
            }
            read.Close();
            dataBase.CloseConnection();
        }

        public void Doors(DataGridView dgw)
        {
            dgw.Rows.Clear();

            dataBase.OpenConnection();

            string query = $"select name_item,number_plant,price_item,article,photo,type_item,id_item from products_tbl where type_item LIKE 'Двери и комплектующие'";
            NpgsqlCommand comm = new NpgsqlCommand(query, dataBase.GetConnection());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow2(dgw, read);
            }
            read.Close();
            dataBase.CloseConnection();
        }

        public void Furniture(DataGridView dgw)
        {
            dgw.Rows.Clear();

            dataBase.OpenConnection();

            string query = $"select name_item,number_plant,price_item,article,photo,type_item,id_item from products_tbl where type_item LIKE 'Мебель'";
            NpgsqlCommand comm = new NpgsqlCommand(query, dataBase.GetConnection());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow2(dgw, read);
            }
            read.Close();
            dataBase.CloseConnection();
        }

        public void Wallpaper(DataGridView dgw)
        {
            dgw.Rows.Clear();

            dataBase.OpenConnection();

            string query = $"select name_item,number_plant,price_item,article,photo,type_item,id_item from products_tbl where type_item LIKE 'Обои'";
            NpgsqlCommand comm = new NpgsqlCommand(query, dataBase.GetConnection());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow2(dgw, read);
            }
            read.Close();
            dataBase.CloseConnection();
        }

        public void Photowallpaper(DataGridView dgw)
        {
            dgw.Rows.Clear();

            dataBase.OpenConnection();

            string query = $"select name_item,number_plant,price_item,article,photo,type_item,id_item from products_tbl where type_item LIKE 'Фотообои'";
            NpgsqlCommand comm = new NpgsqlCommand(query, dataBase.GetConnection());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow2(dgw, read);
            }
            read.Close();
            dataBase.CloseConnection();
        }

        public void Cement(DataGridView dgw)
        {
            dgw.Rows.Clear();

            dataBase.OpenConnection();

            string query = $"select name_item,number_plant,price_item,article,photo,type_item,id_item from products_tbl where type_item LIKE 'Цементные и сыпучие материалы'";
            NpgsqlCommand comm = new NpgsqlCommand(query, dataBase.GetConnection());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow2(dgw, read);
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

        private void button1_Click(object sender, EventArgs e)
        {
            FormAddItems formAddItems = new FormAddItems(_user);
            formAddItems.Show();
            Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormMailing formMailing = new FormMailing(_user);
            formMailing.Show();
            Hide();
        }


        private void Edit()
        {
            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;

            var name = textBox2.Text;
            var number_plant = textBox3.Text;
            var price_item = textBox4.Text;
            var article = textBox5.Text;
            var photo = textBox6.Text;
            var type_item = textBox7.Text;
            var id = textBox8.Text;

            if (dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString() != string.Empty)
            {
                dataGridView1.Rows[selectedRowIndex].SetValues(name, number_plant, price_item, article, photo, type_item, id);
                dataGridView1.Rows[selectedRowIndex].Cells[7].Value = RowState.Modifided;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Edit();
        }


        private void Update()
        {
            dataBase.OpenConnection();

            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView1.Rows[index].Cells[7].Value;

                if (rowState == RowState.Exited)
                    continue;


                if (rowState == RowState.Modifided)
                {
                    var name = dataGridView1.Rows[index].Cells[0].Value.ToString();
                    var number_plant = dataGridView1.Rows[index].Cells[1].Value.ToString();
                    var price_item = dataGridView1.Rows[index].Cells[2].Value.ToString();
                    var article = dataGridView1.Rows[index].Cells[3].Value.ToString();
                    var photo = dataGridView1.Rows[index].Cells[4].Value.ToString();
                    var type_item = dataGridView1.Rows[index].Cells[5].Value.ToString();
                    var id = dataGridView1.Rows[index].Cells[6].Value.ToString();

                    var changeQuery = $"update products_tbl set name_item = '{name}', number_plant = '{number_plant}', price_item = '{price_item}',article = '{article}',photo = '{photo}',type_item '{type_item}'  where id_item = '{id}'";

                    var comm = new NpgsqlCommand(changeQuery, dataBase.GetConnection());
                    comm.ExecuteNonQuery();
                }

            }
            dataBase.CloseConnection();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Update();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            dataBase.OpenConnection();

            var name = textBox2.Text;
            var number_plant = textBox3.Text;
            var price_item = Convert.ToInt64(textBox4.Text);
            var article = Convert.ToInt64(textBox5.Text);
            var photo = textBox6.Text;
            var type_item = textBox7.Text;
            var id = Convert.ToInt32(textBox8.Text);

            string query = $"UPDATE products_tbl SET article = '{article}' WHERE id_item = " + id;
            NpgsqlCommand command = new NpgsqlCommand(@query, dataBase.GetConnection());
            command.ExecuteNonQuery();
            dataBase.CloseConnection();
        }

        private void deleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;

            dataGridView1.Rows[index].Visible = false;

            if (dataGridView1.Rows[index].Cells[0].Value.ToString() == string.Empty)
            {
                dataGridView1.Rows[index].Cells[7].Value = RowState.Deleted;
                return;
            }
            dataGridView1.Rows[index].Cells[7].Value = RowState.Deleted;
        }


        private void button4_Click(object sender, EventArgs e)
        {
            deleteRow();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            dataBase.OpenConnection();

            var name = textBox2.Text;
            var number_plant = textBox3.Text;
            var price_item = Convert.ToInt64(textBox4.Text);
            var article = Convert.ToInt64(textBox5.Text);
            var photo = textBox6.Text;
            var type_item = textBox7.Text;
            var id = Convert.ToInt32(textBox8.Text);

            string query = $"DELETE FROM products_tbl WHERE id_item = " + id;
            NpgsqlCommand command = new NpgsqlCommand(@query, dataBase.GetConnection());
            command.ExecuteNonQuery();
            dataBase.CloseConnection();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            FormAddUser formAddUser = new FormAddUser(_user);
            formAddUser.Show();
            this.Close();
        }
    }
    
}
