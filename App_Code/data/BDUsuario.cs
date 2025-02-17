using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections;

namespace es.itsduero.iecscyl.data
{

    public class BDUsuario
    {

        /// <summary>
        /// Inserta un nuevo usuario en la tabla Usuarios
        /// </summary>        
        /// <param name="usuario">Objeto contenedor de los valores a insertar</param>
        /// <autor>dalvaro@itsduero.es</autor>
        public void Insert(ref Usuario usuario)
        {
            // Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                Boolean empresa = usuario.empresa != null;
                Boolean ubicacion = usuario.ubicacion != null;
                Boolean tipoTicket = usuario.tipoTicket != null;
                // Declaramos nuestra consulta de oficina Sql parametrizada
                string sqlQuery = "INSERT INTO Usuarios (ID, Nombre, Apellidos, DNI, Password, Telefono, Permisos, Email," + (empresa ? " Empresa," : "") + " Activo," + (ubicacion ? " Ubicacion," : "") + " TipoTicket) VALUES (@ID, @Nombre, @Apellidos, @DNI, @Password, @Telefono, @Permisos, @Email," + (empresa ? " @Empresa," : "") + " @Activo" + (ubicacion ? ", @Ubicacion" : "") + "" + (tipoTicket ? ", @TipoTicket" : "") + ")";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Damos valor al ID del usuario
                    usuario.ID = GetNextID();

                    // Añadimos los valores del objeto pasado como parámetro para crearlo en la base de datos
                    cmd.Parameters.AddWithValue("@ID", usuario.ID);
                    cmd.Parameters.AddWithValue("@Nombre", usuario.nombre);
                    cmd.Parameters.AddWithValue("@Apellidos", usuario.apellidos);
                    cmd.Parameters.AddWithValue("@DNI", usuario.dni.ToUpper());
                    cmd.Parameters.AddWithValue("@Password", usuario.password);
                    cmd.Parameters.AddWithValue("@Telefono", usuario.telefono);
                    //cmd.Parameters.AddWithValue("@Token", usuario.token);
                    cmd.Parameters.AddWithValue("@Permisos", usuario.permisos.ID);
                    cmd.Parameters.AddWithValue("@Email", usuario.email);

                    // para los nulos dbnull.value para insertar valores nulos en base de datos

                    //falta corregir error de cuando se crea un conductor
                    if (empresa)
                    {
                        cmd.Parameters.AddWithValue("@Empresa", usuario.empresa.ID);
                    }
                    cmd.Parameters.AddWithValue("@Activo", usuario.activo);
                    if (ubicacion)
                    {
                        cmd.Parameters.AddWithValue("@Ubicacion", usuario.ubicacion.ID);
                    }
                    if (tipoTicket)
                    {
                        cmd.Parameters.AddWithValue("@TipoTicket", usuario.tipoTicket);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Modifica un usuario en la tabla Usuarios
        /// </summary>
        /// <param name="vale">Objeto contenedor de los valores a modificar</param>
        /// <autor>dalvaro@itsduero.es</autor>
        public void Update(Usuario usuario)
        {
            // Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                Boolean permisos = usuario.permisos != null;
                Boolean empresa = usuario.empresa != null;
                Boolean ubicacion = usuario.ubicacion != null;
                Boolean password = usuario.password != null;
                // Declaramos nuestra consulta de acción Sql parametrizada
                string sqlQuery = "UPDATE Usuarios SET Nombre = @Nombre, Apellidos = @Apellidos, DNI = @Dni, Password = @Password, Telefono = @Telefono, Token = @Token, Email = @Email," + (permisos ? " Permisos = @Permisos," : "")
                    + "" + (empresa ? " Empresa = @Empresa," : "") + "" + (ubicacion ? " Ubicacion = @Ubicacion," : "") + " Activo = @Activo, TipoTicket = @TipoTicket WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Añadimos los valores del objeto pasado como parámetro para crearlo en la base de datos
                    cmd.Parameters.AddWithValue("@ID", usuario.ID);
                    cmd.Parameters.AddWithValue("@Nombre", usuario.nombre);
                    cmd.Parameters.AddWithValue("@Apellidos", usuario.apellidos);
                    cmd.Parameters.AddWithValue("@Dni", usuario.dni.ToUpper());
                    cmd.Parameters.AddWithValue("@Password", usuario.password);
                    cmd.Parameters.AddWithValue("@Telefono", usuario.telefono);
                    cmd.Parameters.AddWithValue("@Token", usuario.token);
                    if (permisos)
                    {
                        cmd.Parameters.AddWithValue("@Permisos", usuario.permisos.ID);
                    }
                    cmd.Parameters.AddWithValue("@Email", usuario.email);
                    if (empresa)
                    {
                        cmd.Parameters.AddWithValue("@Empresa", usuario.empresa.ID);
                    }
                    cmd.Parameters.AddWithValue("@Activo", usuario.activo);
                    if (ubicacion)
                    {
                        cmd.Parameters.AddWithValue("@Ubicacion", usuario.ubicacion.ID);
                    }
                    cmd.Parameters.AddWithValue("@TipoTicket", usuario.tipoTicket);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Elimina un usuario en la tabla Usuarios
        /// </summary>        
        /// <param name="usuario">Objeto contenedor de los valores a eliminar</param>
        /// <autor>dalvaro@itsduero.es</autor>
        public void Delete(Usuario usuario)
        {
            // Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                // Declaramos nuestra consulta de referencia Sql parametrizada
                string sqlQuery = "DELETE FROM Usuarios WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Añadimos los valores del objeto pasado como parámetro para eliminarlo de la base de datos
                    cmd.Parameters.AddWithValue("@ID", usuario.ID);

                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Obtiene un Usuario a partir del resultado de la consulta SQL
        /// </summary>
        /// <param name="dataReader">DataReader con el resultado de la consulta SQL</param>
        /// <returns>Un ArrayList con los valores del Usuario</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        private Usuario GetByDataReader(SqlDataReader dataReader)
        {
            Usuario user = new Usuario
            {
                ID = Convert.ToInt32(dataReader["ID"]),
                nombre = Convert.ToString(dataReader["Nombre"]),
                apellidos = Convert.ToString(dataReader["Apellidos"]),
                dni = Convert.ToString(dataReader["DNI"]).ToUpper(),
                password = Convert.ToString(dataReader["Password"]),
                telefono = Convert.ToInt32(dataReader["Telefono"]),
                token = Convert.ToString(dataReader["Token"]),
                permisos = !DBNull.Value.Equals(dataReader["Permisos"]) ? new BDPermiso().GetByID(Convert.ToInt16(dataReader["Permisos"])) : null,
                email = Convert.ToString(dataReader["Email"]),
                empresa = !DBNull.Value.Equals(dataReader["Empresa"]) ? new BDEmpresa().GetByID(Convert.ToInt16(dataReader["Empresa"])) : null,
                activo = Convert.ToBoolean(dataReader["Activo"]),
                ubicacion = !DBNull.Value.Equals(dataReader["Ubicacion"]) ? new BDUbicacion().GetByID(Convert.ToInt16(dataReader["Ubicacion"])) : null,
                tipoTicket = !DBNull.Value.Equals(dataReader["TipoTicket"]) ? Convert.ToChar(dataReader["TipoTicket"]) : 'Z'
            };

            return user;
        }


        /// <summary>
        /// Devuelve un objeto Usuario
        /// </summary>
        /// <param name="usuario">Nombre de usuario del usuario a buscar</param>
        /// <param name="password">Contraseña del usuario a buscar</param>
        /// <returns>Un registro con los valores del Usuario</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public Usuario GetByUsuarioPassword(string usuario, string password, bool admin = false)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT * FROM Usuarios WHERE Activo = 'true' and DNI COLLATE Latin1_General_CS_AS = @Dni and Password COLLATE Latin1_General_CS_AS = @Password" + (admin ? " and Permisos = 1" : "");
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Utilizamos el valor del parámetro para enviarlo al parámetro declarado en la consulta de selección SQL
                    cmd.Parameters.AddWithValue("@Dni", usuario.ToUpper());
                    cmd.Parameters.AddWithValue("@Password", password);
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.Read())
                    {
                        Usuario user = GetByDataReader(dataReader);

                        // Generamos un token
                        string token = Global.generarTokenAleatorio();
                        user.token = token;
                        Update(user);
                        return user;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// Devuelve true si el nombre de usuario ya existe (DNI)
        /// </summary>
        /// <param name="usuario">Nombre de usuario del usuario a buscar</param>
        /// <param name="password">Contraseña del usuario a buscar</param>
        /// <returns>Un registro con los valores del Usuario</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public bool UserRepeat(string username)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT * FROM Usuarios WHERE DNI COLLATE Latin1_General_CS_AS = @Dni";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Utilizamos el valor del parámetro para enviarlo al parámetro declarado en la consulta de selección SQL
                    cmd.Parameters.AddWithValue("@Dni", username.ToUpper());
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    return dataReader.Read() ? true : false;
                }
            }
        }


        /// <summary>
        /// Devuelve un objeto Usuario
        /// </summary>
        /// <param name="usuario">Nombre de usuario del usuario a buscar</param>
        /// <param name="token">Token del usuario a buscar</param>
        /// <returns>Un registro con los valores del Usuario</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public Usuario GetByUsuarioToken(string usuario, string token)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT * FROM Usuarios WHERE DNI COLLATE Latin1_General_CS_AS = @Dni and Token COLLATE Latin1_General_CS_AS = @Token";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Utilizamos el valor del parámetro para enviarlo al parámetro declarado en la consulta de selección SQL
                    cmd.Parameters.AddWithValue("@Dni", usuario.ToUpper());
                    cmd.Parameters.AddWithValue("@Token", token);
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.Read())
                    {
                        Usuario user = GetByDataReader(dataReader);
                        return user;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// Devuelve un objeto Usuario
        /// </summary>
        /// <param name="ID">ID del usuario a buscar</param>
        /// <returns>Un registro con los valores del Usuario</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public Usuario GetByID(int ID)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT * FROM vUsuarios WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Utilizamos el valor del parámetro para enviarlo al parámetro declarado en la consulta de selección SQL
                    cmd.Parameters.AddWithValue("@ID", ID);
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.Read())
                    {
                        Usuario user = GetByDataReader(dataReader);
                        return user;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// Devuelve un objeto Usuario
        /// </summary>
        /// <param name="ID">ID del usuario a buscar</param>
        /// <returns>Un registro con los valores del Usuario</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public Usuario GetByUsuario(string usuario)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT * FROM vUsuarios WHERE DNI = @DNI";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Utilizamos el valor del parámetro para enviarlo al parámetro declarado en la consulta de selección SQL
                    cmd.Parameters.AddWithValue("@DNI", usuario.ToUpper());
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.Read())
                    {
                        Usuario user = GetByDataReader(dataReader);
                        return user;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// Devuelve todos los objetos Usuario que coincidan con los campos de búsqueda
        /// </summary>
        /// <param name="cadenasBusqueda">Cadenas de la búsqueda</param>
        /// <param name="parametrosBusqueda">Parámetros de la búsqueda</param>
        /// <param name="valoresBusqueda">Valores por los que buscar</param>
        /// <param name="orden">Campo por el que queremos ordenar la consulta</param>
        /// <returns>Un ArrayList con los valores de todos los objetos Usuario que coincidan con los campos de búsqueda</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public ArrayList Search(ArrayList cadenasBusqueda, ArrayList parametrosBusqueda, ArrayList valoresBusqueda, string orden)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT * FROM vUsuarios";

                // Cadenas de búsqueda
                for (int i = 0; i < cadenasBusqueda.Count; i++)
                {
                    if (i == 0)
                    {
                        sqlQuery += " WHERE ";
                    }
                    else
                    {
                        sqlQuery += " AND ";
                    }

                    sqlQuery += cadenasBusqueda[i].ToString();
                }

                sqlQuery += " ORDER BY " + orden;

                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Utilizamos el valor del parámetro para enviarlo al parámetro declarado en la consulta de selección SQL
                    for (int i = 0; i < parametrosBusqueda.Count; i++)
                    {
                        // Parámetros y valores de búsqueda
                        cmd.Parameters.AddWithValue(parametrosBusqueda[i].ToString(), valoresBusqueda[i].ToString());
                    }
                    SqlDataReader dataReader = cmd.ExecuteReader();

                    ArrayList usuarios = new ArrayList();

                    while (dataReader.Read())
                    {
                        Usuario usuario = GetByDataReader(dataReader);
                        usuarios.Add(usuario);
                    }

                    return usuarios;
                }
            }
        }


        /// <summary>
        /// Devuelve todos los objetos Usuario entre un índice inferior y otro superior que coincidan con los campos de búsqueda
        /// </summary>
        /// <param name="cadenasBusqueda">Cadenas de la búsqueda</param>
        /// <param name="parametrosBusqueda">Parámetros de la búsqueda</param>
        /// <param name="valoresBusqueda">Valores por los que buscar</param>        
        /// <param name="indiceInferior">Índice inferior de la consulta</param>
        /// <param name="indiceSuperior">Índice superior de la consulta</param>
        /// <param name="orden">Campo por el que queremos ordenar la consulta</param>
        /// <returns>Un ArrayList con los valores de todos los objetos Usuario entre un índice inferior y otro superior que coincidan con los campos de búsqueda</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public ArrayList Search(ArrayList cadenasBusqueda, ArrayList parametrosBusqueda, ArrayList valoresBusqueda, short indiceInferior, short indiceSuperior, string orden)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY " + orden + ") AS RowNum, * FROM vUsuarios";

                // Cadenas de búsqueda
                for (int i = 0; i < cadenasBusqueda.Count; i++)
                {
                    if (i == 0)
                    {
                        sqlQuery += " WHERE ";
                    }
                    else
                    {
                        sqlQuery += " AND ";
                    }

                    sqlQuery += cadenasBusqueda[i].ToString();
                }

                sqlQuery += ") AS RowConstrainedResult WHERE RowNum >= " + indiceInferior.ToString() + " AND RowNum <= " + indiceSuperior + " ORDER BY RowNum";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Utilizamos el valor del parámetro para enviarlo al parámetro declarado en la consulta de selección SQL
                    for (int i = 0; i < parametrosBusqueda.Count; i++)
                    {
                        // Parámetros y valores de búsqueda
                        cmd.Parameters.AddWithValue(parametrosBusqueda[i].ToString(), valoresBusqueda[i].ToString());
                    }
                    SqlDataReader dataReader = cmd.ExecuteReader();

                    ArrayList usuarios = new ArrayList();

                    while (dataReader.Read())
                    {
                        Usuario usuario = GetByDataReader(dataReader);
                        usuarios.Add(usuario);
                    }

                    return usuarios;
                }
            }
        }


        /// <summary>
        /// Devuelve el número de objetos Usuario que coincidan con los campos de búsqueda
        /// </summary>
        /// <param name="cadenasBusqueda">Cadenas de la búsqueda</param>
        /// <param name="parametrosBusqueda">Parámetros de la búsqueda</param>
        /// <param name="valoresBusqueda">Valores por los que buscar</param>
        /// <returns>El número de objetos Usuario que coincidan con los campos de búsqueda</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public int Count(ArrayList cadenasBusqueda, ArrayList parametrosBusqueda, ArrayList valoresBusqueda)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT COUNT(*) AS Num FROM vUsuarios";

                // Cadenas de búsqueda
                for (int i = 0; i < cadenasBusqueda.Count; i++)
                {
                    if (i == 0)
                    {
                        sqlQuery += " WHERE ";
                    }
                    else
                    {
                        sqlQuery += " AND ";
                    }

                    sqlQuery += cadenasBusqueda[i].ToString();
                }

                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Utilizamos el valor del parámetro para enviarlo al parámetro declarado en la consulta de selección SQL
                    for (int i = 0; i < parametrosBusqueda.Count; i++)
                    {
                        // Parámetros y valores de búsqueda
                        cmd.Parameters.AddWithValue(parametrosBusqueda[i].ToString(), valoresBusqueda[i].ToString());
                    }
                    SqlDataReader dataReader = cmd.ExecuteReader();

                    if (dataReader.Read())
                    {
                        int num = (!DBNull.Value.Equals(dataReader["Num"]) ? Convert.ToInt32(dataReader["Num"]) : (int)0);

                        return num;
                    }
                }
            }

            return 0;
        }


        /// <summary>
        /// Devuelve un objeto Usuario
        /// </summary>
        /// <param name="ID">ID del usuario a buscar</param>
        /// <returns>Un registro con los valores del Usuario</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public ArrayList GetAllPassengers(bool activo = false)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT * FROM Usuarios WHERE Permisos = '2' "+(activo ? "AND Activo = 1": "");
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    ArrayList usuarios = new ArrayList();

                    while (dataReader.Read())
                    {
                        Usuario usuario = GetByDataReader(dataReader);
                        usuarios.Add(usuario);
                    }

                    return usuarios;
                }
            }
        }

        /// <summary>
        /// Devuelve el siguiente ID de Usuario a insertar en la base de datos
        /// </summary>        
        /// <returns>El valor máximo del campo ID de la tabla Usuarios + 1</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public int GetNextID()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT MAX(ID) AS MaxID FROM Usuarios";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.Read())
                    {
                        int maxID = (!DBNull.Value.Equals(dataReader["MaxID"]) ? Convert.ToInt32(dataReader["MaxID"]) : (int)0);
                        maxID++;

                        return maxID;
                    }
                }
            }

            return 1;
        }


        /// <summary>
        /// Comprueba si el usuario puede eliminarse
        /// </summary>        
        /// <param name="usuario">Usuario a comprobar</param>
        /// <param name="motivo">El motivo por el que no se puede eliminar</param>
        /// <returns>Si el usuario se puede eliminar o no</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public bool CheckDelete(Usuario usuario, ref string motivo)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                //cuando el usuario tiene algún ticket pagado
                string sqlQuery = "SELECT * FROM Usuarios WHERE ID = @ID AND (ID IN (SELECT Usuario FROM Tickets WHERE Pagado ='true')) "
                + "OR ID = @ID AND (ID IN (SELECT Conductor FROM Viajes WHERE Conductor = @ID))";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Utilizamos el valor del parámetro para enviarlo al parámetro declarado en la consulta de selección SQL
                    cmd.Parameters.AddWithValue("@ID", usuario.ID);

                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.Read())
                    {
                        motivo = "No se puede eliminar un Usuario que ya tiene viajes asociados o tickets que están pagados, como mucho puede deshabilitar dicho usuario, pero no borrar su contenido ";
                        return false;
                    }
                }

            }
            return true;
        }

    }
}