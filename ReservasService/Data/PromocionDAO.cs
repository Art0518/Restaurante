using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ReservasService.Data
{
    public class PromocionDAO
    {
        private readonly string _connectionString;

   public PromocionDAO(string connectionString)
        {
            _connectionString = connectionString;
      }

        // Listar todas las promociones activas
        public DataTable ListarPromocionesActivas()
     {
            DataTable dt = new DataTable();
         using (SqlConnection conn = new SqlConnection(_connectionString))
      {
           conn.Open();
                string query = @"
          SELECT * FROM [menu].[Promocion] 
  WHERE Estado = 'Activa' 
AND CAST(GETDATE() AS DATE) BETWEEN CAST(FechaInicio AS DATE) AND CAST(FechaFin AS DATE)
 ORDER BY FechaFin DESC";

         using (SqlCommand cmd = new SqlCommand(query, conn))
    {
 using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
 {
   adapter.Fill(dt);
                }
           }
            }
   return dt;
     }

        // Listar promociones válidas para carrito (si necesitas lógica específica)
        public DataTable ListarPromocionesValidasParaCarrito(int idUsuario)
        {
            // Por ahora retorna las mismas promociones activas
        // Puedes agregar lógica específica según tus necesidades
            return ListarPromocionesActivas();
        }
    }
}
