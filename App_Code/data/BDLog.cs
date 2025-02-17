using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections;
using es.itsduero.iecscyl;
using es.itsduero.iecscyl.data;

namespace es.itsduero.iecscyl.data 
{

    public class BDLog
    {

        /// <summary>
        /// Inserta un nuevo Log en la tabla Logs
        /// </summary>
        /// <param name="_log">Objeto contenedor de los valores a insertar</param>
/// <autor>dalvaro@itsduero.es</autor>
        public void Insert(Log _log)
        {
            // Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                // Declaramos nuestra consulta de acción Sql parametrizada
                string sqlQuery = "INSERT INTO Logs (Fecha, Usuario, Accion, IP) values (@Fecha, @Usuario, @Accion, @IP)";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Añadimos los valores del objeto pasado como parámetro para crearlo en la base de datos
                    cmd.Parameters.AddWithValue("@Fecha", _log.fecha);
                    cmd.Parameters.AddWithValue("@Usuario", _log.usuario.ID);
                    cmd.Parameters.AddWithValue("@Accion", _log.accion);
                    cmd.Parameters.AddWithValue("@IP", _log.IP);

                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Obtiene un Log a partir del resultado de la consulta SQL
        /// </summary>
        /// <param name="dataReader">DataReader con el resultado de la consulta SQL</param>
        /// <returns>Un ArrayList con los valores del Log</returns>
/// <autor>dalvaro@itsduero.es</autor>
        private Log GetByDataReader(SqlDataReader dataReader)
        {
            Log _log = new Log
            {
                fecha = Convert.ToDateTime(dataReader["Fecha"]),
                usuario = new BDUsuario().GetByID(Convert.ToInt16(dataReader["Usuario"])),
                accion = Convert.ToString(dataReader["Accion"]),
                IP = Convert.ToString(dataReader["IP"])
            };

            return _log;
        }


        /// <summary>
        /// Devuelve el log de acceso
        /// </summary>
        /// <param name="_empleado">Empleado del que se quieren obtener su log</param>
        /// <param name="_fecha">Fecha en la que se quiere comprobar su log</param>
        /// <returns>Log</returns>
/// <autor>dalvaro@itsduero.es</autor>
        public Log GetByEmpleadoFecha(Usuario _usuario, DateTime _fecha)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT * FROM Logs WHERE Usuario = @Usuario AND Fecha = @Fecha";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Utilizamos el valor del parámetro para enviarlo al parámetro declarado en la consulta de selección SQL
                    cmd.Parameters.AddWithValue("@Usuario", _usuario.ID);
                    cmd.Parameters.AddWithValue("@Fecha", _fecha);

                    SqlDataReader dataReader = cmd.ExecuteReader();

                    if (dataReader.Read())
                    {
                        return GetByDataReader(dataReader);
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// Devuelve todos los objetos Log entre un índice inferior y otro superior que coincidan con los campos de búsqueda
        /// </summary>       
        /// <param name="indiceInferior">Índice inferior de la consulta</param>
        /// <param name="indiceSuperior">Índice superior de la consulta</param>
        /// <param name="orden">Campo por el que queremos ordenar la consulta</param>
        /// <returns>Un ArrayList con los valores de todos los objetos Log entre un índice inferior y otro superior que coincidan con los campos de búsqueda</returns>
/// <autor>dalvaro@itsduero.es</autor>
        public ArrayList Search(ArrayList cadenasBusqueda, ArrayList parametrosBusqueda, ArrayList valoresBusqueda, short indiceInferior, short indiceSuperior, string orden)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT * FROM Logs";

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

                sqlQuery += " ORDER BY " + orden + " OFFSET " + ((indiceInferior - 1) >= 0 ? (indiceInferior - 1).ToString() : "0") + " ROWS FETCH NEXT " + ((indiceSuperior - indiceInferior) + 1).ToString() + " ROWS ONLY";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Utilizamos el valor del parámetro para enviarlo al parámetro declarado en la consulta de selección SQL
                    for (int i = 0; i < parametrosBusqueda.Count; i++)
                    {
                        // Parámetros y valores de búsqueda
                        cmd.Parameters.AddWithValue(parametrosBusqueda[i].ToString(), valoresBusqueda[i].ToString());
                    }
                    SqlDataReader dataReader = cmd.ExecuteReader();

                    ArrayList Log = new ArrayList();

                    while (dataReader.Read())
                    {
                        Log log = GetByDataReader(dataReader);
                        Log.Add(log);
                    }

                    return Log;
                }
            }
        }


        /// <summary>
        /// Devuelve el número de objetos Log que coincidan con los campos de búsqueda
        /// </summary>
        /// <param name="cadenaConexionUNIS">Cadena de conexión a la base de datos de UNIS</param>
        /// <param name="cadenaConexionUNIS_ITS">Cadena de conexión a la base de datos de ITS para el control de fichajes</param>
        /// <param name="cadenasBusqueda">Cadenas de la búsqueda</param>
        /// <param name="parametrosBusqueda">Parámetros de la búsqueda</param>
        /// <param name="valoresBusqueda">Valores por los que buscar</param>
        /// <returns>El número de objetos Log que coincidan con los campos de búsqueda</returns>
/// <autor>dalvaro@itsduero.es</autor>
        public short Count(ArrayList cadenasBusqueda, ArrayList parametrosBusqueda, ArrayList valoresBusqueda)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT COUNT(*) AS Num FROM Logs";

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
                        short num = (!DBNull.Value.Equals(dataReader["Num"]) ? Convert.ToInt16(dataReader["Num"]) : (short)0);

                        return num;
                    }
                }
            }

            return 0;
        }
    }
}