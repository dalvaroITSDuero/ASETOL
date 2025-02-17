using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de BDViajes
/// </summary>
namespace es.itsduero.iecscyl.data
{

    public class BDViajes
    {

        /// <summary>
        /// Inserta un nuevo viaje en la tabla Viaje
        /// </summary>        
        /// <param name="viaje">Objeto contenedor de los valores a insertar</param>
        /// <autor>dalvaro@itsduero.es</autor>
        public void Insert(ref Viaje viaje)
        {
            // Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                Boolean conductor = viaje.conductor != null;

                conn.Open();
                // Declaramos nuestra consulta de oficina Sql parametrizada
                string sqlQuery = "INSERT INTO Viajes (ID, Usuario, Ticket, Turno, Ubicacion, Fecha" + (conductor ? ", Conductor" : "") + ") VALUES (@ID, @Usuario, @Ticket, @Turno, @Ubicacion, @Fecha" + (conductor ? ", @Conductor" : "") + ")";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Damos valor al ID del viaje
                    viaje.ID = GetNextID();

                    // Añadimos los valores del objeto pasado como parámetro para crearlo en la base de datos
                    cmd.Parameters.AddWithValue("@Usuario", viaje.usuario.ID);
                    cmd.Parameters.AddWithValue("@Ticket", viaje.ticket.ID);
                    cmd.Parameters.AddWithValue("@ID", viaje.ID);
                    cmd.Parameters.AddWithValue("@Turno", viaje.turno);
                    cmd.Parameters.AddWithValue("@Ubicacion", viaje.ubicacion.ID);
                    cmd.Parameters.AddWithValue("@Fecha", viaje.fecha);
                    if (conductor)
                    {
                        cmd.Parameters.AddWithValue("@Conductor", viaje.conductor);
                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Modifica un viaje en la tabla Viaje
        /// </summary>
        /// <param name="vale">Objeto contenedor de los valores a modificar</param>
        /// <autor>dalvaro@itsduero.es</autor>
        public void Update(Viaje viaje)
        {
            // Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                // Declaramos nuestra consulta de acción Sql parametrizada
                string sqlQuery = "UPDATE Viajes SET Ubicacion = @Ubicacion, Usuario = @Usuario, Turno = @Turno, Fecha = @Fecha, ID = @ID, Conductor = @Conductor WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Añadimos los valores del objeto pasado como parámetro para crearlo en la base de datos
                    cmd.Parameters.AddWithValue("@Usuario", viaje.usuario.ID);
                    cmd.Parameters.AddWithValue("@ID", viaje.ID);
                    cmd.Parameters.AddWithValue("@Turno", viaje.turno);
                    cmd.Parameters.AddWithValue("@Ubicacion", viaje.ubicacion.ID);
                    cmd.Parameters.AddWithValue("@Fecha", viaje.fecha);
                    cmd.Parameters.AddWithValue("@Conductor", viaje.conductor.ID);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Elimina un viaje en la tabla Viaje
        /// </summary>        
        /// <param name="viaje">Objeto contenedor de los valores a eliminar</param>
        /// <autor>dalvaro@itsduero.es</autor>
        public void Delete(Viaje viaje)
        {
            // Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                // Declaramos nuestra consulta de referencia Sql parametrizada
                string sqlQuery = "DELETE FROM Viajes WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Añadimos los valores del objeto pasado como parámetro para eliminarlo de la base de datos
                    cmd.Parameters.AddWithValue("@ID", viaje.ID);

                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Obtiene un Viaje a partir del resultado de la consulta SQL
        /// </summary>
        /// <param name="dataReader">DataReader con el resultado de la consulta SQL</param>
        /// <returns>Un ArrayList con los valores del Viaje</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        private Viaje GetByDataReader(SqlDataReader dataReader)
        {
            Viaje viaje = new Viaje
            {
                usuario = new BDUsuario().GetByID(Convert.ToInt32(dataReader["Usuario"])),
                ticket = new BDTickets().GetByID(Convert.ToInt32(dataReader["Ticket"])),
                ID = Convert.ToInt32(dataReader["ID"]),
                turno = Convert.ToInt16(dataReader["Turno"]),
                ubicacion = new BDUbicacion().GetByID(Convert.ToInt32(dataReader["Ubicacion"])),
                fecha = Convert.ToDateTime(dataReader["Fecha"]),
                conductor = !DBNull.Value.Equals(dataReader["Conductor"]) ? new BDUsuario().GetByID(Convert.ToInt16(dataReader["Conductor"])) : null
            };
            //conductor = new BDUsuario().GetByID(Convert.ToInt32(dataReader["Conductor"]))
            return viaje;
        }

        /// <summary>
        /// Devuelve un objeto Viaje
        /// </summary>
        /// <param name="ID">ID del viaje a buscar</param>
        /// <returns>Un registro con los valores del Viaje</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public Viaje GetByID(int ID)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT * FROM Viajes WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Utilizamos el valor del parámetro para enviarlo al parámetro declarado en la consulta de selección SQL
                    cmd.Parameters.AddWithValue("@ID", ID);
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.Read())
                    {
                        Viaje viaje = GetByDataReader(dataReader);
                        return viaje;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Devuelve el siguiente ID de Viaje a insertar en la base de datos
        /// </summary>        
        /// <returns>El valor máximo del campo ID de la tabla Viaje + 1</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public int GetNextID()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT MAX(ID) AS MaxID FROM Viajes";
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
        /// Devuelve todos los objetos Viajes (solo acceso autorizado)
        /// </summary>
        /// <returns>Un ArrayList con los inscritos</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public ArrayList GetAll()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT * FROM Viajes";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    ArrayList viajes = new ArrayList();

                    while (dataReader.Read())
                    {
                        Viaje viaje = GetByDataReader(dataReader);
                        viajes.Add(viaje);
                    }

                    return viajes;
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

                string sqlQuery = "SELECT COUNT(*) AS Num FROM vViajes";

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
        /// Devuelve todos los objetos Ticket que coincidan con los campos de búsqueda
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

                string sqlQuery = "SELECT * FROM vViajes";

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

                    ArrayList viajes = new ArrayList();

                    while (dataReader.Read())
                    {
                        Viaje viaje = GetByDataReader(dataReader);
                        viajes.Add(viaje);
                    }

                    return viajes;
                }
            }
        }


        /// <summary>
        /// Devuelve todos los objetos Ticket entre un índice inferior y otro superior que coincidan con los campos de búsqueda
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

                string sqlQuery = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY " + orden + ") AS RowNum, * FROM vViajes";

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

                    ArrayList viajes = new ArrayList();

                    while (dataReader.Read())
                    {
                        Viaje viaje = GetByDataReader(dataReader);
                        viajes.Add(viaje);
                    }

                    return viajes;
                }
            }
        }


        /// <summary>
        /// Comprueba si la ticket puede eliminarse
        /// </summary>        
        /// <param name="id">Ticket a comprobar</param>
        /// <param name="motivo">El motivo por el que no se puede eliminar</param>
        /// <returns>Si el usuario se puede eliminar o no</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public bool CheckDelete(Ticket ticket, ref string motivo)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                string sqlQuery = "SELECT * FROM Tickets WHERE ID = @ID AND (ID IN (SELECT Ticket FROM Viajes))OR ID = @ID AND Pagado ='true'";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Utilizamos el valor del parámetro para enviarlo al parámetro declarado en la consulta de selección SQL
                    cmd.Parameters.AddWithValue("@ID", ticket.ID);

                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.Read())
                    {
                        motivo = "No se puede eliminar un Ticket que ya tiene clientes viajes asociados o que están pagados.";
                        return false;
                    }
                }

            }
            return true;
        }
    }
}