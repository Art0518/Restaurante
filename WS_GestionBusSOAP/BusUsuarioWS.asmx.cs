using System;
using System.Data;
using System.Web.Services;
using Logica.Servicios;
using GDatos.Entidades;

namespace WS_GestionBusSOAP
{
    [WebService(Namespace = "http://cafesanjuansoap.runasp.net/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class WS_Usuario : WebService
    {
        private readonly UsuarioLogica usuarioLogica = new UsuarioLogica();

        [WebMethod(Description = "Registrar un nuevo usuario (equivalente a POST /usuarios)")]
        public DataSet CrearUsuario(string nombre, string email, string contrasena, string rol, string telefono, string direccion)
        {
            DataSet ds = new DataSet("ResultadoRegistroUsuario");

            try
            {
                if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(contrasena))
                    throw new Exception("Nombre, email y contraseña son campos obligatorios.");

                if (string.IsNullOrEmpty(rol)) rol = "CLIENTE";
                if (telefono == null) telefono = "";
                if (direccion == null) direccion = "";

                var usuario = new Usuario
                {
                    Nombre = nombre,
                    Email = email,
                    Contrasena = contrasena,
                    Rol = rol,
                    Estado = "ACTIVO",
                    Telefono = telefono,
                    Direccion = direccion
                };

                usuarioLogica.Registrar(usuario);

                DataTable dt = new DataTable("UsuarioRegistrado");
                dt.Columns.Add("Mensaje");
                dt.Columns.Add("Nombre");
                dt.Columns.Add("Email");
                dt.Columns.Add("Rol");
                dt.Columns.Add("Estado");

                dt.Rows.Add("Usuario registrado correctamente.", nombre, email, rol, "ACTIVO");
                ds.Tables.Add(dt);

                return ds;
            }
            catch (Exception ex)
            {
                DataTable error = new DataTable("Error");
                error.Columns.Add("Mensaje");
                error.Rows.Add("Error al registrar usuario: " + ex.Message);
                ds.Tables.Add(error);
                return ds;
            }
        }

        [WebMethod(Description = "Listar todos los usuarios registrados (equivalente a GET /usuarios)")]
        public DataSet ListarUsuarios()
        {
            DataSet ds = new DataSet("ResultadoUsuarios");

            try
            {
                DataTable usuarios = usuarioLogica.Listar();
                usuarios.TableName = "Usuarios";
                ds.Tables.Add(usuarios);
                return ds;
            }
            catch (Exception ex)
            {
                DataTable error = new DataTable("Error");
                error.Columns.Add("Mensaje");
                error.Rows.Add("Error al listar usuarios: " + ex.Message);
                ds.Tables.Add(error);
                return ds;
            }
        }
    }
}
