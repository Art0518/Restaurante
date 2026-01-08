using System;
using System.Data;
using System.Data.SqlClient;

namespace AccesoDatos.Conexion
{
    public class ConexionSQL
    {
        // 🔹 Cadena de conexión a MonsterASP
        private readonly string cadenaConexion =
            "Server=tcp:db31553.databaseasp.net,1433;" +
            "Database=db31553;" +
            "User Id=db31553;" +
            "Password=0520ARTU;" +
            "Encrypt=True;" +
            "TrustServerCertificate=True;" +
            "MultipleActiveResultSets=True;";

        // 🔹 Retorna un objeto SqlConnection
        public SqlConnection CrearConexion()
        {
            return new SqlConnection(cadenaConexion);
        }

        // 🔹 Método auxiliar: ejecuta comandos directos
        public void ProbarConexion()
        {
            using (SqlConnection cn = CrearConexion())
            {
                try
                {
                    cn.Open();
                    Console.WriteLine("Conexión exitosa a MonsterASP.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al conectar: " + ex.Message);
                }
            }
        }
    }
}
