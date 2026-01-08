using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MenuService.Models;

namespace MenuService.Data
{
public class PlatoDAO
    {
        private readonly string _connectionString;

        public PlatoDAO(string connectionString)
{
            _connectionString = connectionString;
        }

  // Crear plato
  public Plato CrearPlato(Plato plato)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
      {
 conn.Open();
  string query = @"
       INSERT INTO [menu].[Plato] 
              (IdRestaurante, Nombre, Descripcion, Precio, Categoria, TipoComida, ImagenURL, Stock, Estado)
             VALUES (@IdRestaurante, @Nombre, @Descripcion, @Precio, @Categoria, @TipoComida, @ImagenURL, @Stock, @Estado);
     SELECT SCOPE_IDENTITY();";

     using (SqlCommand cmd = new SqlCommand(query, conn))
    {
        cmd.Parameters.AddWithValue("@IdRestaurante", plato.IdRestaurante);
         cmd.Parameters.AddWithValue("@Nombre", plato.Nombre);
 cmd.Parameters.AddWithValue("@Descripcion", plato.Descripcion ?? (object)DBNull.Value);
           cmd.Parameters.AddWithValue("@Precio", plato.Precio);
  cmd.Parameters.AddWithValue("@Categoria", plato.Categoria ?? (object)DBNull.Value);
     cmd.Parameters.AddWithValue("@TipoComida", plato.TipoComida ?? (object)DBNull.Value);
      cmd.Parameters.AddWithValue("@ImagenURL", plato.ImagenURL ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@Stock", plato.Stock);
         cmd.Parameters.AddWithValue("@Estado", plato.Estado ?? "ACTIVO");

plato.IdPlato = (int)(decimal)cmd.ExecuteScalar();
            }
      }
  return plato;
        }

    // Obtener plato por ID
  public Plato ObtenerPlatoById(int idPlato)
        {
        using (SqlConnection conn = new SqlConnection(_connectionString))
            {
   conn.Open();
      string query = "SELECT * FROM [menu].[Plato] WHERE IdPlato = @IdPlato";

      using (SqlCommand cmd = new SqlCommand(query, conn))
       {
cmd.Parameters.AddWithValue("@IdPlato", idPlato);
using (SqlDataReader reader = cmd.ExecuteReader())
            {
     if (reader.Read())
           {
   return MapearPlato(reader);
       }
         }
     }
  }
          return null;
      }

  // Listar todos los platos activos
        public List<Plato> ListarTodosPlatos()
        {
            List<Plato> platos = new List<Plato>();
         using (SqlConnection conn = new SqlConnection(_connectionString))
   {
                conn.Open();
       string query = "SELECT * FROM [menu].[Plato] WHERE Estado = 'ACTIVO' ORDER BY Categoria, Nombre";

     using (SqlCommand cmd = new SqlCommand(query, conn))
       {
   using (SqlDataReader reader = cmd.ExecuteReader())
         {
          while (reader.Read())
   {
     platos.Add(MapearPlato(reader));
      }
             }
                }
            }
     return platos;
        }

        // Listar platos por categoría
        public List<Plato> ListarPlatosPorCategoria(string categoria)
        {
    List<Plato> platos = new List<Plato>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
  {
            conn.Open();
         string query = "SELECT * FROM [menu].[Plato] WHERE Categoria = @Categoria AND Estado = 'ACTIVO' ORDER BY Nombre";

     using (SqlCommand cmd = new SqlCommand(query, conn))
     {
           cmd.Parameters.AddWithValue("@Categoria", categoria);
          using (SqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
       {
          platos.Add(MapearPlato(reader));
    }
  }
             }
  }
            return platos;
        }

// Actualizar plato
        public bool ActualizarPlato(Plato plato)
        {
        using (SqlConnection conn = new SqlConnection(_connectionString))
       {
            conn.Open();
     string query = @"
             UPDATE [menu].[Plato] 
       SET Nombre = @Nombre, Descripcion = @Descripcion, Precio = @Precio, 
      Categoria = @Categoria, TipoComida = @TipoComida, ImagenURL = @ImagenURL,
  Stock = @Stock, Estado = @Estado
    WHERE IdPlato = @IdPlato";

     using (SqlCommand cmd = new SqlCommand(query, conn))
                {
 cmd.Parameters.AddWithValue("@Nombre", plato.Nombre);
             cmd.Parameters.AddWithValue("@Descripcion", plato.Descripcion ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@Precio", plato.Precio);
  cmd.Parameters.AddWithValue("@Categoria", plato.Categoria ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@TipoComida", plato.TipoComida ?? (object)DBNull.Value);
                 cmd.Parameters.AddWithValue("@ImagenURL", plato.ImagenURL ?? (object)DBNull.Value);
           cmd.Parameters.AddWithValue("@Stock", plato.Stock);
   cmd.Parameters.AddWithValue("@Estado", plato.Estado ?? "ACTIVO");
              cmd.Parameters.AddWithValue("@IdPlato", plato.IdPlato);

 return cmd.ExecuteNonQuery() > 0;
            }
    }
        }

        // Eliminar plato (cambiar estado a INACTIVO)
        public bool EliminarPlato(int idPlato)
        {
    using (SqlConnection conn = new SqlConnection(_connectionString))
   {
     conn.Open();
     
        // Primero verificar si el plato existe
 string checkQuery = "SELECT COUNT(*) FROM [menu].[Plato] WHERE IdPlato = @IdPlato";
              using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
         {
  checkCmd.Parameters.AddWithValue("@IdPlato", idPlato);
          int count = (int)checkCmd.ExecuteScalar();
               
       if (count == 0)
            {
       throw new Exception($"El plato con ID {idPlato} no existe");
       }
     }
        
     // Cambiar el estado a INACTIVO
             string query = "UPDATE [menu].[Plato] SET Estado = 'INACTIVO' WHERE IdPlato = @IdPlato";
     using (SqlCommand cmd = new SqlCommand(query, conn))
  {
         cmd.Parameters.AddWithValue("@IdPlato", idPlato);
     int rowsAffected = cmd.ExecuteNonQuery();
             return rowsAffected > 0;
}
 }
  }

        private Plato MapearPlato(SqlDataReader reader)
        {
            return new Plato
      {
          IdPlato = (int)reader["IdPlato"],
       IdRestaurante = (int)reader["IdRestaurante"],
     Nombre = (string)reader["Nombre"],
       Descripcion = reader["Descripcion"] != DBNull.Value ? (string)reader["Descripcion"] : null,
  Precio = (decimal)reader["Precio"],
      Categoria = reader["Categoria"] != DBNull.Value ? (string)reader["Categoria"] : null,
        TipoComida = reader["TipoComida"] != DBNull.Value ? (string)reader["TipoComida"] : null,
  ImagenURL = reader["ImagenURL"] != DBNull.Value ? (string)reader["ImagenURL"] : null,
         Stock = (int)reader["Stock"],
          Estado = reader["Estado"] != DBNull.Value ? (string)reader["Estado"] : "ACTIVO"
    };
        }
    }
}
