using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gorevtakipv2
{
    public class sqlbaglanti
    {
        public MySqlConnection Connection()
        {
            string server = "localhost";
            string database = "takipsema";
            string username = "root";
            string password = "12345678";
            string connstring = $"Server={server};Database={database};User Id={username};Password={password};";
            return new MySqlConnection(connstring);
        }
    }
}
