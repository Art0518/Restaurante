using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ReservasService.Models;

namespace ReservasService.Data
{
    public class MesaDAO
    {
        private readonly string _connectionString;

        public MesaDAO(string connectionString)
        {
            _connectionString = connectionString;
        }

        // =========================================================
        // LISTAR TODAS LAS MESAS
        // =========================================================
        public DataTable ListarMesas()
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("usp_Mesa_ListarTodas", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }

        // =========================================================
        // LISTAR MESAS POR RESTAURANTE
        // Ahora acepta un parámetro opcional `tipo` para filtrar por TipoMesa (Interior/Exterior)
        // Usamos LIKE con COLLATE para tolerar mayúsculas/minúsculas y acentos
        // =========================================================
        public List<Mesa> ListarMesasPorRestaurante(int idRestaurante, string? tipo = null)
        {
            List<Mesa> mesas = new List<Mesa>();

            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                string query = @"
   SELECT IdMesa, IdRestaurante, NumeroMesa, TipoMesa, Capacidad, Precio, ImagenURL, Estado
     FROM reservas.Mesa
 WHERE IdRestaurante = @IdRestaurante AND Estado = 'DISPONIBLE'";

                if (!string.IsNullOrWhiteSpace(tipo))
                {
                    // Usar COLLATE para ignorar mayúsculas/minúsculas y acentos, y LIKE para tolerar variaciones
                    query += " AND UPPER(LTRIM(RTRIM(TipoMesa))) COLLATE Latin1_General_CI_AI LIKE '%' + UPPER(LTRIM(RTRIM(@TipoMesa))) + '%'";
                }

                query += "\n ORDER BY NumeroMesa";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@IdRestaurante", idRestaurante);

                if (!string.IsNullOrWhiteSpace(tipo))
                {
                    cmd.Parameters.AddWithValue("@TipoMesa", tipo.Trim());
                }

                cn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        mesas.Add(new Mesa
                        {
                            IdMesa = (int)reader["IdMesa"],
                            IdRestaurante = (int)reader["IdRestaurante"],
                            NumeroMesa = (int)reader["NumeroMesa"],
                            TipoMesa = reader["TipoMesa"].ToString(),
                            Capacidad = (int)reader["Capacidad"],
                            Precio = reader["Precio"] != DBNull.Value ? (decimal)reader["Precio"] : 0m,
                            ImagenURL = reader["ImagenURL"] != DBNull.Value ? reader["ImagenURL"].ToString() : "",
                            Estado = reader["Estado"].ToString()
                        });
                    }
                }
            }

            return mesas;
        }

        // =========================================================
        // MESAS DISPONIBLES PARA RESERVAS
        // =========================================================
        public DataTable MesasDisponibles(string zona, int personas)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(@"
   SELECT TOP 1 *
   FROM reservas.Mesa
   WHERE Estado = 'DISPONIBLE'
        AND TipoMesa = @zona
  AND Capacidad >= @personas
  ORDER BY Capacidad ASC", cn);

                cmd.Parameters.AddWithValue("@zona", zona);
                cmd.Parameters.AddWithValue("@personas", personas);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }

        // =========================================================
        // OBTENER DISPONIBILIDAD DE MESA POR FECHA
        // =========================================================
        public DataTable ObtenerDisponibilidad(int idMesa, DateTime fecha)
        {
            string estados = "'PENDIENTE', 'CONFIRMADA', 'COMPLETADA', 'HOLD'";

            string sql = $@"
         SELECT CONVERT(VARCHAR(8), Hora, 108) AS Hora
       FROM reservas.Reserva
      WHERE IdMesa = {idMesa}
     AND Fecha = '{fecha:yyyy-MM-dd}'
       AND Estado IN ({estados})
  ";

            return EjecutarConsulta(sql);
        }

        // =========================================================
        // EJECUTAR CONSULTA SQL DIRECTA
// =========================================================
        public DataTable EjecutarConsulta(string sql)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(sql, cn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // =========================================================
        // ACTUALIZAR ESTADO
        // =========================================================
        public void ActualizarEstado(int idMesa, string estado)
        {
      using (SqlConnection cn = new SqlConnection(_connectionString))
      {
     SqlCommand cmd = new SqlCommand("usp_Mesa_ActualizarEstado", cn);
       cmd.CommandType = CommandType.StoredProcedure;

       cmd.Parameters.AddWithValue("@IdMesa", idMesa);
     cmd.Parameters.AddWithValue("@NuevoEstado", estado);

 cn.Open();
       cmd.ExecuteNonQuery();
  }
        }

        // =========================================================
// GESTIONAR MESA (CREAR O ACTUALIZAR)
        // =========================================================
        public void GestionarMesa(string operacion, int? idMesa, int idRestaurante, int numeroMesa,
         string tipoMesa, int capacidad, decimal? precio, string imagenURL, string estado)
    {
        using (SqlConnection cn = new SqlConnection(_connectionString))
            {
           SqlCommand cmd = new SqlCommand("usp_Mesa_Gestionar", cn);
   cmd.CommandType = CommandType.StoredProcedure;

      cmd.Parameters.AddWithValue("@Operacion", operacion);
 
        // Corregir: Cuando es INSERT, pasar NULL en lugar de 0
        if (operacion.Equals("INSERT", StringComparison.OrdinalIgnoreCase) || !idMesa.HasValue || idMesa.Value == 0)
  {
      cmd.Parameters.AddWithValue("@IdMesa", DBNull.Value);
  }
        else
        {
      cmd.Parameters.AddWithValue("@IdMesa", idMesa.Value);
        }
        
        cmd.Parameters.AddWithValue("@IdRestaurante", idRestaurante);
        cmd.Parameters.AddWithValue("@NumeroMesa", numeroMesa);
        cmd.Parameters.AddWithValue("@TipoMesa", tipoMesa);
  cmd.Parameters.AddWithValue("@Capacidad", capacidad);
        cmd.Parameters.AddWithValue("@Precio", (object)precio ?? DBNull.Value);
     cmd.Parameters.AddWithValue("@ImagenURL", (object)imagenURL ?? DBNull.Value);
      cmd.Parameters.AddWithValue("@Estado", (object)estado ?? DBNull.Value);

        cn.Open();
      cmd.ExecuteNonQuery();
    }
}
    }
}
