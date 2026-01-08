using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ReservasService.Models;

namespace ReservasService.Data
{
    public class RestauranteDAO
    {
        private readonly string _connectionString;

 public RestauranteDAO(string connectionString)
   {
         _connectionString = connectionString;
        }

        // Obtener restaurante por ID
  public Restaurante ObtenerRestauranteById(int idRestaurante)
        {
     using (SqlConnection conn = new SqlConnection(_connectionString))
      {
conn.Open();
      string query = "SELECT * FROM reservas.Restaurante WHERE IdRestaurante = @IdRestaurante";

   using (SqlCommand cmd = new SqlCommand(query, conn))
 {
      cmd.Parameters.AddWithValue("@IdRestaurante", idRestaurante);
      using (SqlDataReader reader = cmd.ExecuteReader())
       {
      if (reader.Read())
          {
      return MapearRestaurante(reader);
     }
           }
        }
      }
     return null;
        }

        // Listar todos los restaurantes activos
   public List<Restaurante> ListarRestaurantesActivos()
        {
    List<Restaurante> restaurantes = new List<Restaurante>();
    using (SqlConnection conn = new SqlConnection(_connectionString))
    {
        conn.Open();
 string query = "SELECT * FROM reservas.Restaurante WHERE Activo = 1 ORDER BY Nombre";

  using (SqlCommand cmd = new SqlCommand(query, conn))
      {
  using (SqlDataReader reader = cmd.ExecuteReader())
    {
        while (reader.Read())
   {
  restaurantes.Add(MapearRestaurante(reader));
            }
    }
            }
       }
    return restaurantes;
    }

        // Listar todos los restaurantes
    public List<Restaurante> ListarTodosRestaurantes()
    {
List<Restaurante> restaurantes = new List<Restaurante>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
     {
    conn.Open();
    string query = "SELECT * FROM reservas.Restaurante ORDER BY Nombre";

 using (SqlCommand cmd = new SqlCommand(query, conn))
         {
  using (SqlDataReader reader = cmd.ExecuteReader())
    {
       while (reader.Read())
  {
  restaurantes.Add(MapearRestaurante(reader));
   }
       }
       }
         }
    return restaurantes;
}

        // Crear restaurante
    public Restaurante CrearRestaurante(Restaurante restaurante)
        {
    using (SqlConnection conn = new SqlConnection(_connectionString))
  {
   conn.Open();
        string query = @"
    INSERT INTO reservas.Restaurante 
  (Nombre, Direccion, Telefono, HorarioApertura, HorarioCierre, Activo, FechaCreacion)
    VALUES (@Nombre, @Direccion, @Telefono, @HorarioApertura, @HorarioCierre, @Activo, @FechaCreacion);
       SELECT SCOPE_IDENTITY();";

          using (SqlCommand cmd = new SqlCommand(query, conn))
 {
     cmd.Parameters.AddWithValue("@Nombre", restaurante.Nombre);
       cmd.Parameters.AddWithValue("@Direccion", restaurante.Direccion ?? (object)DBNull.Value);
       cmd.Parameters.AddWithValue("@Telefono", restaurante.Telefono ?? (object)DBNull.Value);
    cmd.Parameters.AddWithValue("@HorarioApertura", restaurante.HorarioApertura ?? (object)DBNull.Value);
   cmd.Parameters.AddWithValue("@HorarioCierre", restaurante.HorarioCierre ?? (object)DBNull.Value);
  cmd.Parameters.AddWithValue("@Activo", restaurante.Activo);
   cmd.Parameters.AddWithValue("@FechaCreacion", restaurante.FechaCreacion);

     restaurante.IdRestaurante = (int)(decimal)cmd.ExecuteScalar();
 }
    }
  return restaurante;
        }

        // Actualizar restaurante
   public bool ActualizarRestaurante(Restaurante restaurante)
   {
  using (SqlConnection conn = new SqlConnection(_connectionString))
    {
     conn.Open();
     string query = @"
   UPDATE reservas.Restaurante 
      SET Nombre = @Nombre, Direccion = @Direccion, Telefono = @Telefono,
   HorarioApertura = @HorarioApertura, HorarioCierre = @HorarioCierre, Activo = @Activo
WHERE IdRestaurante = @IdRestaurante";

      using (SqlCommand cmd = new SqlCommand(query, conn))
          {
  cmd.Parameters.AddWithValue("@Nombre", restaurante.Nombre);
           cmd.Parameters.AddWithValue("@Direccion", restaurante.Direccion ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@Telefono", restaurante.Telefono ?? (object)DBNull.Value);
  cmd.Parameters.AddWithValue("@HorarioApertura", restaurante.HorarioApertura ?? (object)DBNull.Value);
           cmd.Parameters.AddWithValue("@HorarioCierre", restaurante.HorarioCierre ?? (object)DBNull.Value);
     cmd.Parameters.AddWithValue("@Activo", restaurante.Activo);
cmd.Parameters.AddWithValue("@IdRestaurante", restaurante.IdRestaurante);

         return cmd.ExecuteNonQuery() > 0;
           }
        }
        }

        private Restaurante MapearRestaurante(SqlDataReader reader)
 {
return new Restaurante
 {
      IdRestaurante = (int)reader["IdRestaurante"],
  Nombre = (string)reader["Nombre"],
Direccion = reader["Direccion"] != DBNull.Value ? (string)reader["Direccion"] : null,
    Telefono = reader["Telefono"] != DBNull.Value ? (string)reader["Telefono"] : null,
          HorarioApertura = reader["HorarioApertura"] != DBNull.Value ? (string)reader["HorarioApertura"] : null,
        HorarioCierre = reader["HorarioCierre"] != DBNull.Value ? (string)reader["HorarioCierre"] : null,
     Activo = (bool)reader["Activo"],
         FechaCreacion = (DateTime)reader["FechaCreacion"]
          };
        }
    }
}
