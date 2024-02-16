using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StroyModern
{
    public class TestData
    {
        DataBase dataBase = new DataBase();

        public TestData() { }

        public void AddUsers(string login,string password) 
        {
            dataBase.OpenConnection();

            var querystring = $"insert into user_tbl (login_user,password_user) values ('{login}','{password}')";

            var comm = new NpgsqlCommand(querystring, dataBase.GetConnection());
            comm.ExecuteNonQuery();

            MessageBox.Show("данные успешно добавлены!", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            dataBase.CloseConnection();
        }

        public void Authorization(string login,string password)
        {
            dataBase.OpenConnection();

            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter();
            DataTable table = new DataTable();

            var query = $"select id_user, login_user, password_user, id_role from roles_tbl where login_user = '{login}' and password_user = '{password}'";

            NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, dataBase.GetConnection());

            adapter.SelectCommand = npgsqlCommand;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                MessageBox.Show("Пользователь уже существует!");

            }

            dataBase.CloseConnection();
        }


        public void UPDATEitem(string acticle)
        {
            dataBase.OpenConnection();

            var id = Convert.ToInt32(1);

            string query = $"UPDATE products_tbl SET article = '{acticle}' WHERE id_item = " + id;
            NpgsqlCommand command = new NpgsqlCommand(@query, dataBase.GetConnection());
            command.ExecuteNonQuery();
            dataBase.CloseConnection();
        }
        public void Deleteitem(string name_item)
        {
            dataBase.OpenConnection();

            string query = $"DELETE FROM products_tbl WHERE name_item = '{name_item}'";
            NpgsqlCommand command = new NpgsqlCommand(@query, dataBase.GetConnection());
            command.ExecuteNonQuery();
            dataBase.CloseConnection();
        }
    }
}

