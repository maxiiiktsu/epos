using MySql.Data.MySqlClient;

namespace epos
{
    public static class Database
    {
        // podľa tvojho lokálneho setupu
        private static string host = "localhost";
        private static string user = "root";
        private static string password = ""; // ak máš heslo, doplň sem
        private static string database = "epos";

        // ŽIADNE SslMode, žiadne extra parametre
        private static string connString =
            $"Server={host};Database={database};Uid={user};Pwd={password};";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connString);
        }
    }
}
