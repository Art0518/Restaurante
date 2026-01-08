using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MenuService.Models;

namespace MenuService.Data
{
    public class PromocionDAO
    {
    private readonly string _connectionString;

   public PromocionDAO(string connectionString)
  {
 _connectionString = connectionString;
        }

        // Crear promoción
        public Promocion CrearPromocion(Promocion promocion)
 {
   using (SqlConnection conn = new SqlConnection(_connectionString))
 {
     conn.Open();
  string query = @"
           INSERT INTO [menu].[Promocion] 
        (IdRestaurante, Nombre, Descuento, FechaInicio, FechaFin, Estado)
   VALUES (@IdRestaurante, @Nombre, @Descuento, @FechaInicio, @FechaFin, @Estado);
    SELECT SCOPE_IDENTITY();";

  using (SqlCommand cmd = new SqlCommand(query, conn))
      {
   cmd.Parameters.AddWithValue("@IdRestaurante", promocion.IdRestaurante);
   cmd.Parameters.AddWithValue("@Nombre", promocion.Nombre);
 cmd.Parameters.AddWithValue("@Descuento", promocion.Descuento);
          cmd.Parameters.AddWithValue("@FechaInicio", promocion.FechaInicio);
           cmd.Parameters.AddWithValue("@FechaFin", promocion.FechaFin);
  cmd.Parameters.AddWithValue("@Estado", promocion.Estado ?? "Activa");

    promocion.IdPromocion = (int)(decimal)cmd.ExecuteScalar();
    }
       }
    return promocion;
        }

        // Obtener promoción por ID
     public Promocion ObtenerPromocionById(int idPromocion)
     {
    using (SqlConnection conn = new SqlConnection(_connectionString))
            {
       conn.Open();
 string query = "SELECT * FROM [menu].[Promocion] WHERE IdPromocion = @IdPromocion";

           using (SqlCommand cmd = new SqlCommand(query, conn))
        {
 cmd.Parameters.AddWithValue("@IdPromocion", idPromocion);
using (SqlDataReader reader = cmd.ExecuteReader())
    {
 if (reader.Read())
          {
   return MapearPromocion(reader);
     }
   }
      }
  }
       return null;
        }

  // Listar todas las promociones activas
        public List<Promocion> ListarPromocionesActivas()
        {
    List<Promocion> promociones = new List<Promocion>();
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
        using (SqlDataReader reader = cmd.ExecuteReader())
         {
       while (reader.Read())
  {
 promociones.Add(MapearPromocion(reader));
      }
       }
  }
   }
    return promociones;
        }

        // Listar todas las promociones
        public List<Promocion> ListarTodasPromociones()
        {
     List<Promocion> promociones = new List<Promocion>();
    using (SqlConnection conn = new SqlConnection(_connectionString))
 {
     conn.Open();
   string query = "SELECT * FROM [menu].[Promocion] ORDER BY FechaFin DESC";

   using (SqlCommand cmd = new SqlCommand(query, conn))
 {
     using (SqlDataReader reader = cmd.ExecuteReader())
      {
   while (reader.Read())
      {
   promociones.Add(MapearPromocion(reader));
      }
            }
   }
       }
        return promociones;
        }

        // Listar promociones por estado
      public List<Promocion> ListarPromocionesPorEstado(string estado)
        {
            List<Promocion> promociones = new List<Promocion>();
       using (SqlConnection conn = new SqlConnection(_connectionString))
    {
        conn.Open();
    string query = "SELECT * FROM [menu].[Promocion] WHERE Estado = @Estado ORDER BY FechaFin DESC";

   using (SqlCommand cmd = new SqlCommand(query, conn))
          {
          cmd.Parameters.AddWithValue("@Estado", estado);
   using (SqlDataReader reader = cmd.ExecuteReader())
               {
    while (reader.Read())
               {
                 promociones.Add(MapearPromocion(reader));
         }
          }
     }
            }
            return promociones;
        }

     // Actualizar promoción
    public bool ActualizarPromocion(Promocion promocion)
      {
    using (SqlConnection conn = new SqlConnection(_connectionString))
  {
   conn.Open();
    string query = @"
       UPDATE [menu].[Promocion] 
   SET Nombre = @Nombre, Descuento = @Descuento,
    FechaInicio = @FechaInicio, FechaFin = @FechaFin, Estado = @Estado
        WHERE IdPromocion = @IdPromocion";

   using (SqlCommand cmd = new SqlCommand(query, conn))
 {
     cmd.Parameters.AddWithValue("@Nombre", promocion.Nombre);
 cmd.Parameters.AddWithValue("@Descuento", promocion.Descuento);
   cmd.Parameters.AddWithValue("@FechaInicio", promocion.FechaInicio);
           cmd.Parameters.AddWithValue("@FechaFin", promocion.FechaFin);
 cmd.Parameters.AddWithValue("@Estado", promocion.Estado ?? "Activa");
  cmd.Parameters.AddWithValue("@IdPromocion", promocion.IdPromocion);

         return cmd.ExecuteNonQuery() > 0;
 }
}
        }

  // Eliminar promoción (cambiar estado a Inactiva)
  public bool EliminarPromocion(int idPromocion)
   {
 using (SqlConnection conn = new SqlConnection(_connectionString))
   {
        conn.Open();
        
        // Verificar si existe
        string checkQuery = "SELECT COUNT(*) FROM [menu].[Promocion] WHERE IdPromocion = @IdPromocion";
        using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
        {
            checkCmd.Parameters.AddWithValue("@IdPromocion", idPromocion);
    int count = (int)checkCmd.ExecuteScalar();
         
    if (count == 0)
      {
      throw new Exception($"La promoción con ID {idPromocion} no existe");
            }
   }
        
   string query = "UPDATE [menu].[Promocion] SET Estado = 'Inactiva' WHERE IdPromocion = @IdPromocion";

using (SqlCommand cmd = new SqlCommand(query, conn))
    {
  cmd.Parameters.AddWithValue("@IdPromocion", idPromocion);
       return cmd.ExecuteNonQuery() > 0;
        }
       }
        }

      private Promocion MapearPromocion(SqlDataReader reader)
        {
 return new Promocion
       {
    IdPromocion = (int)reader["IdPromocion"],
    IdRestaurante = (int)reader["IdRestaurante"],
     Nombre = (string)reader["Nombre"],
 Descuento = (decimal)reader["Descuento"],
   FechaInicio = (DateTime)reader["FechaInicio"],
   FechaFin = (DateTime)reader["FechaFin"],
        Estado = reader["Estado"] != DBNull.Value ? (string)reader["Estado"] : "Activa"
       };
        }
    }
}
