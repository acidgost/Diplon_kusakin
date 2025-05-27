using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Diplon_kusakin
{
    public class Connection
    {
        public static MySqlDataReader SqlConnection(string query, List<MySqlParameter> parameters = null)
        {
            string connectString = "server=127.0.0.1;port=3306;database=dip;uid=root;pwd=;";
            MySqlConnection mySqlConnection = new MySqlConnection(connectString);
            mySqlConnection.Open();
            MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConnection);

            if (parameters != null)
            {
                mySqlCommand.Parameters.AddRange(parameters.ToArray());
            }
            return mySqlCommand.ExecuteReader();
        }


        public static void MySqlClose(MySqlConnection connection)
        {
            connection.Close();

        }

        public static object ExecuteScalar(string query, List<MySqlParameter> parameters = null)
        {
            string connectString = "server=127.0.0.1;port=3306;database=dip;uid=root;pwd=;";
            using (MySqlConnection connection = new MySqlConnection(connectString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(query, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters.ToArray());
                }
                return command.ExecuteScalar();
            }
        }

        public static DataTable ExecuteDataTable(string query, List<MySqlParameter> parameters = null)
        {
            string connectString = "server=127.0.0.1;port=3306;database=dip;uid=root;pwd=;";
            using (MySqlConnection connection = new MySqlConnection(connectString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(query, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters.ToArray());
                }

                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }


    }
}
