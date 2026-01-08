using SeguridadService.Data;
using SeguridadService.Models;
using Microsoft.Extensions.Configuration;

namespace SeguridadService.GraphQL
{
    public class SeguridadQuery
    {
        private readonly string _connectionString;

        public SeguridadQuery(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Usuario> GetUsuarios()
        {
            var usuarioDAO = new UsuarioDAO(_connectionString);
            var dt = usuarioDAO.Listar(null, null);
            var usuarios = new List<Usuario>();

            foreach (System.Data.DataRow row in dt.Rows)
            {
                usuarios.Add(new Usuario
                {
                    IdUsuario = Convert.ToInt32(row["IdUsuario"]),
                    Nombre = row["Nombre"].ToString(),
                    Email = row["Email"].ToString(),
                    Telefono = row["Telefono"].ToString(),
                    Rol = row["Rol"].ToString(),
                    Estado = row["Estado"].ToString()
                });
            }

            return usuarios;
        }

        public Usuario GetUsuario(int id)
        {
            var usuarioDAO = new UsuarioDAO(_connectionString);
            var dt = usuarioDAO.ObtenerPorId(id);

            if (dt.Rows.Count == 0)
                return null;

            var row = dt.Rows[0];
            return new Usuario
            {
                IdUsuario = Convert.ToInt32(row["IdUsuario"]),
                Nombre = row["Nombre"].ToString(),
                Email = row["Email"].ToString(),
                Telefono = row["Telefono"] != DBNull.Value ? row["Telefono"].ToString() : "",
                Rol = row["Rol"].ToString(),
                Estado = row["Estado"].ToString()
            };
        }

        public List<Usuario> GetUsuariosPorRol(string rol)
        {
            var usuarioDAO = new UsuarioDAO(_connectionString);
            var dt = usuarioDAO.Listar(rol, null);
            var usuarios = new List<Usuario>();

            foreach (System.Data.DataRow row in dt.Rows)
            {
                usuarios.Add(new Usuario
                {
                    IdUsuario = Convert.ToInt32(row["IdUsuario"]),
                    Nombre = row["Nombre"].ToString(),
                    Email = row["Email"].ToString(),
                    Telefono = row["Telefono"].ToString(),
                    Rol = row["Rol"].ToString(),
                    Estado = row["Estado"].ToString()
                });
            }

            return usuarios;
        }
    }
}
