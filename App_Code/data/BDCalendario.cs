using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de BDCalendario
/// </summary>
namespace es.itsduero.iecscyl.data
{

    public class BDCalendario
    {

        /// <summary>
        /// Inserta un nuevo calendario en la tabla Calendario
        /// </summary>        
        /// <param name="calendario">Objeto contenedor de los valores a insertar</param>
        /// <autor>dalvaro@itsduero.es</autor>
        public void Insert(ref Calendario calendario)
        {
            // Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                // Declaramos nuestra consulta de oficina Sql parametrizada
                string sqlQuery = "INSERT INTO Calendarios (ID, Usuario, Fecha, Turno) VALUES (@ID, @Usuario, @Fecha, @Turno)";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Damos valor al ID del calendario
                    calendario.ID = GetNextID();

                    // Añadimos los valores del objeto pasado como parámetro para crearlo en la base de datos
                    cmd.Parameters.AddWithValue("@ID", calendario.ID);
                    cmd.Parameters.AddWithValue("@Usuario", calendario.usuario.ID);
                    cmd.Parameters.AddWithValue("@Fecha", calendario.fecha);
                    cmd.Parameters.AddWithValue("@Turno", calendario.turno);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Si existe lo actualiza y sino lo crea
        /// </summary>        
        /// <param name="calendario">Objeto contenedor de los valores a insertar</param>
        /// <autor>dalvaro@itsduero.es</autor>
        public void UpdateOrCreate(ref Calendario calendario)
        {
            string orden = "ID";
            ArrayList cadenasBusqueda = new ArrayList();
            ArrayList parametrosBusqueda = new ArrayList();
            ArrayList valoresBusqueda = new ArrayList();
            cadenasBusqueda.Add("(Usuario = @usuario)");
            parametrosBusqueda.Add("@usuario");
            valoresBusqueda.Add(calendario.usuario.ID);
            cadenasBusqueda.Add("(Fecha = @fecha)");
            parametrosBusqueda.Add("@fecha");
            valoresBusqueda.Add(calendario.fecha);
            ArrayList calendarios = Search(cadenasBusqueda, parametrosBusqueda, valoresBusqueda, orden);

            if (calendarios.Count == 0)
            {
                Insert(ref calendario);
            }
            else
            {
                Calendario cal = (Calendario)calendarios[0];
                calendario.ID = cal.ID;
                Update(calendario);
            }
        }


        /// <summary>
        /// Modifica un calendario en la tabla Calendario
        /// </summary>
        /// <param name="vale">Objeto contenedor de los valores a modificar</param>
        /// <autor>dalvaro@itsduero.es</autor>
        public void Update(Calendario calendario)
        {
            // Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                // Declaramos nuestra consulta de acción Sql parametrizada
                string sqlQuery = "UPDATE Calendarios SET Turno = @Turno, Usuario = @Usuario, Fecha = @Fecha, ID = @ID WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Añadimos los valores del objeto pasado como parámetro para crearlo en la base de datos
                    cmd.Parameters.AddWithValue("@ID", calendario.ID);
                    cmd.Parameters.AddWithValue("@Usuario", calendario.usuario.ID);
                    cmd.Parameters.AddWithValue("@Fecha", calendario.fecha);
                    cmd.Parameters.AddWithValue("@Turno", calendario.turno);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Elimina un calendario en la tabla Calendario
        /// </summary>        
        /// <param name="calendario">Objeto contenedor de los valores a eliminar</param>
        /// <autor>dalvaro@itsduero.es</autor>
        public void Delete(Calendario calendario)
        {
            // Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                // Declaramos nuestra consulta de referencia Sql parametrizada
                string sqlQuery = "DELETE FROM Calendarios WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Añadimos los valores del objeto pasado como parámetro para eliminarlo de la base de datos
                    cmd.Parameters.AddWithValue("@ID", calendario.ID);

                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Obtiene un Calendario a partir del resultado de la consulta SQL
        /// </summary>
        /// <param name="dataReader">DataReader con el resultado de la consulta SQL</param>
        /// <returns>Un ArrayList con los valores del Calendario</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        private Calendario GetByDataReader(SqlDataReader dataReader)
        {
            Calendario cal = new Calendario
            {
                ID = Convert.ToInt32(dataReader["ID"]),
                usuario = new BDUsuario().GetByID(Convert.ToInt32(dataReader["Usuario"])),
                fecha = Convert.ToDateTime(dataReader["Fecha"]),
                turno = Convert.ToInt16(dataReader["Turno"])
            };
            if (Enumerable.Range(0, dataReader.FieldCount).Any(i => dataReader.GetName(i) == "HaveTicket"))
            {
                cal.haveTicket = dataReader["HaveTicket"] != DBNull.Value && dataReader["HaveTicket"] != null
                    ? Convert.ToBoolean(dataReader["HaveTicket"])
                    : false;
            }
            return cal;
        }

        /// <summary>
        /// Devuelve un objeto Calendario
        /// </summary>
        /// <param name="ID">ID del calendario a buscar</param>
        /// <returns>Un registro con los valores del Calendario</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public Calendario GetByID(int ID)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT * FROM Calendarios WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Utilizamos el valor del parámetro para enviarlo al parámetro declarado en la consulta de selección SQL
                    cmd.Parameters.AddWithValue("@ID", ID);
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.Read())
                    {
                        Calendario cal = GetByDataReader(dataReader);
                        return cal;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Devuelve el siguiente ID de Calendario a insertar en la base de datos
        /// </summary>        
        /// <returns>El valor máximo del campo ID de la tabla Calendario + 1</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public int GetNextID()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT MAX(ID) AS MaxID FROM Calendarios";
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
        /// Devuelve todos los objetos Calendarios (solo acceso autorizado)
        /// </summary>
        /// <returns>Un ArrayList con los inscritos</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public ArrayList GetAll()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT * FROM Calendarios";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    ArrayList calendarios = new ArrayList();

                    while (dataReader.Read())
                    {
                        Calendario calendario = GetByDataReader(dataReader);
                        calendarios.Add(calendario);
                    }

                    return calendarios;
                }
            }
        }

        /// <summary>
        /// Devuelve todos los objetos Calendario que coincidan con los campos de búsqueda
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

                string sqlQuery = "SELECT * FROM vCalendarioConTicket";

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

                    ArrayList calendarios = new ArrayList();

                    while (dataReader.Read())
                    {
                        Calendario calendario = GetByDataReader(dataReader);
                        calendarios.Add(calendario);
                    }

                    return calendarios;
                }
            }
        }


        /// <summary>
        /// Devuelve todos los objetos Calendario entre un índice inferior y otro superior que coincidan con los campos de búsqueda
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

                string sqlQuery = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY " + orden + ") AS RowNum, * FROM vCalendarioConTicket";

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

                    ArrayList calendarios = new ArrayList();

                    while (dataReader.Read())
                    {
                        Calendario calendario = GetByDataReader(dataReader);
                        calendarios.Add(calendario);
                    }

                    return calendarios;
                }
            }
        }

        /// <summary>
        /// Devuelve todos los objetos Calendario que coincidan con los campos de búsqueda
        /// </summary>
        /// <param name="userId">id del usuario que queremos sacar su calendario</param>
        /// <returns>Un ArrayList con los valores de todos los objetos Usuario que coincidan con los campos de búsqueda</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public ArrayList CalendarByUserId(int userID)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT * FROM Calendarios WHERE Usuario = @Usuario";



                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Usuario", userID);

                    SqlDataReader dataReader = cmd.ExecuteReader();

                    ArrayList calendarios = new ArrayList();

                    while (dataReader.Read())
                    {
                        Calendario calendario = GetByDataReader(dataReader);
                        calendarios.Add(calendario);
                    }

                    return calendarios;
                }
            }
        }


        /// <summary>
        /// Devuelve el numero de calendaios del usuario de una semana en concreto
        /// </summary>
        /// <param name="userId">id del usuario que queremos sacar su calendario</param>
        /// <returns>Un ArrayList con los valores de todos los objetos Usuario que coincidan con los campos de búsqueda</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public ArrayList CalendarPlanificationWeek(DateTime diaInicio, DateTime diaFin)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT Usuario, COUNT(*) AS TotalRegistros, Activo FROM vCalendarioConTicket WHERE Activo = 1 AND CAST(Fecha AS DATE) BETWEEN @FechaInicio AND @FechaFin GROUP BY Usuario, Activo ORDER BY Usuario";



                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@FechaInicio", diaInicio.ToString());
                    cmd.Parameters.AddWithValue("@FechaFin", diaFin.ToString());

                    SqlDataReader dataReader = cmd.ExecuteReader();

                    ArrayList calendarios = new ArrayList();

                    while (dataReader.Read())
                    {
                        Usuario usuario = new BDUsuario().GetByID(Convert.ToInt32(dataReader["Usuario"]));

                        Tuple<Usuario, short> tupla = Tuple.Create(usuario, Convert.ToInt16(dataReader["TotalRegistros"]));
                        calendarios.Add(tupla);
                    }

                    return calendarios;
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

                string sqlQuery = "SELECT COUNT(*) AS Num FROM vCalendarioConTicket";

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

        /// <summary>
        /// Comprueba si el calendario puede eliminarse
        /// </summary>        
        /// <param name="id">Calendario a comprobar</param>
        /// <param name="motivo">El motivo por el que no se puede eliminar</param>
        /// <returns>Si el usuario se puede eliminar o no</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public bool CheckDelete(Calendario calendario)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                //string sqlQuery = "SELECT * FROM Calendarios WHERE ID = @ID AND (ID IN (SELECT Calendario FROM Viajes))OR ID = @ID AND Pagado ='true'";
                string sqlQuery = "SELECT * FROM Calendarios WHERE CAST(Calendarios.Fecha AS DATE) < CAST(GETDATE() AS DATE)AND ID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Utilizamos el valor del parámetro para enviarlo al parámetro declarado en la consulta de selección SQL
                    cmd.Parameters.AddWithValue("@ID", calendario.ID);

                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.Read())
                    {
                        return true;
                    }
                    else { Delete(calendario); }
                }

            }
            return false;
        }
    }
}