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

    public class BDTickets
    {

        /// <summary>
        /// Inserta un nuevo ticket en la tabla Ticket
        /// </summary>        
        /// <param name="ticket">Objeto contenedor de los valores a insertar</param>
        /// <autor>dalvaro@itsduero.es</autor>
        public void Insert(ref Ticket ticket)
        {
            // Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                Boolean fechaPago = ticket.fechaPago != new DateTime();
                // Declaramos nuestra consulta de oficina Sql parametrizada
                string sqlQuery = "INSERT INTO Tickets (ID, Usuario, FechaInicio, FechaFin, Importe, Pagado" + (fechaPago ? ", FechaPago" : "") + ") VALUES (@ID, @Usuario, @FechaInicio, @FechaFin, @Importe, @Pagado" + (fechaPago ? ", @FechaPago" : "") + ")";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Damos valor al ID del ticket
                    ticket.ID = GetNextID();

                    // Añadimos los valores del objeto pasado como parámetro para crearlo en la base de datos
                    cmd.Parameters.AddWithValue("@ID", ticket.ID);
                    cmd.Parameters.AddWithValue("@Usuario", ticket.usuario.ID);
                    cmd.Parameters.AddWithValue("@FechaInicio", ticket.fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", ticket.fechaFin);
                    cmd.Parameters.AddWithValue("@Pagado", ticket.pagado);

                    if (fechaPago)
                    {
                        cmd.Parameters.AddWithValue("@FechaPago", ticket.fechaPago);
                    }

                    if (ticket.usuario.tipoTicket == 'M')
                    {
                        if(ticket.importe == null){
                            cmd.Parameters.AddWithValue("@Importe", 110.00);
                        }
                    }
                    else if (ticket.usuario.tipoTicket == 'D')
                    {
                        cmd.Parameters.AddWithValue("@Importe", 7.00);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Modifica un ticket en la tabla Ticket
        /// </summary>
        /// <param name="vale">Objeto contenedor de los valores a modificar</param>
        /// <autor>dalvaro@itsduero.es</autor>
        public void Update(Ticket ticket)
        {
            // Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                Boolean fechaPago = ticket.fechaPago != new DateTime();
                // Declaramos nuestra consulta de acción Sql parametrizada
                string sqlQuery = "UPDATE Tickets SET FechaFin = @FechaFin, Usuario = @Usuario, FechaInicio = @FechaInicio, Importe = @Importe, Pagado = @Pagado, " + (fechaPago ? " FechaPago = @FechaPago," : "") + "ID = @ID WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Añadimos los valores del objeto pasado como parámetro para crearlo en la base de datos
                    cmd.Parameters.AddWithValue("@ID", ticket.ID);
                    cmd.Parameters.AddWithValue("@Usuario", ticket.usuario.ID);
                    cmd.Parameters.AddWithValue("@FechaInicio", ticket.fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", ticket.fechaFin);
                    cmd.Parameters.AddWithValue("@Pagado", ticket.pagado);
                    cmd.Parameters.AddWithValue("@FechaPagado", ticket.fechaPago);
                    cmd.Parameters.AddWithValue("@Importe", ticket.importe);
                    if (fechaPago)
                    {
                        cmd.Parameters.AddWithValue("@FechaPago", ticket.fechaPago);
                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Elimina un ticket en la tabla Ticket
        /// </summary>        
        /// <param name="ticket">Objeto contenedor de los valores a eliminar</param>
        /// <autor>dalvaro@itsduero.es</autor>
        public void Delete(Ticket ticket)
        {
            // Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                // Declaramos nuestra consulta de referencia Sql parametrizada
                string sqlQuery = "DELETE FROM Tickets WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Añadimos los valores del objeto pasado como parámetro para eliminarlo de la base de datos
                    cmd.Parameters.AddWithValue("@ID", ticket.ID);

                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Obtiene un Ticket a partir del resultado de la consulta SQL
        /// </summary>
        /// <param name="dataReader">DataReader con el resultado de la consulta SQL</param>
        /// <returns>Un ArrayList con los valores del Ticket</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        private Ticket GetByDataReader(SqlDataReader dataReader)
        {
            //Console.WriteLine(Convert.ToDouble(dataReader["Importe"]));
            Ticket tic = new Ticket
            {
                ID = Convert.ToInt32(dataReader["ID"]),
                usuario = new BDUsuario().GetByID(Convert.ToInt32(dataReader["Usuario"])),
                fechaInicio = Convert.ToDateTime(dataReader["FechaInicio"]),
                fechaFin = Convert.ToDateTime(dataReader["FechaFin"]),
                pagado = Convert.ToBoolean(dataReader["Pagado"]),
                fechaPago = !DBNull.Value.Equals(dataReader["FechaPago"]) ? Convert.ToDateTime(dataReader["FechaPago"]) : new DateTime(),
                importe = !DBNull.Value.Equals(dataReader["Importe"]) ? Convert.ToDouble(dataReader["Importe"]) : 0.0
            };

            return tic;
        }

        /// <summary>
        /// Devuelve un objeto Ticket
        /// </summary>
        /// <param name="ID">ID del ticket a buscar</param>
        /// <returns>Un registro con los valores del Ticket</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public Ticket GetByID(int ID)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT * FROM Tickets WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Utilizamos el valor del parámetro para enviarlo al parámetro declarado en la consulta de selección SQL
                    cmd.Parameters.AddWithValue("@ID", ID);
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.Read())
                    {
                        Ticket tic = GetByDataReader(dataReader);
                        return tic;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Devuelve el siguiente ID de Ticket a insertar en la base de datos
        /// </summary>        
        /// <returns>El valor máximo del campo ID de la tabla Ticket + 1</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public int GetNextID()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT MAX(ID) AS MaxID FROM Tickets";
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
        /// Devuelve todos los objetos Tickets (solo acceso autorizado)
        /// </summary>
        /// <returns>Un ArrayList con los inscritos</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public ArrayList GetAll()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT * FROM Tickets";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    ArrayList tickets = new ArrayList();

                    while (dataReader.Read())
                    {
                        Ticket ticket = GetByDataReader(dataReader);
                        tickets.Add(ticket);
                    }

                    return tickets;
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

                string sqlQuery = "SELECT COUNT(*) AS Num FROM vTickets";

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

                string sqlQuery = "SELECT * FROM vTickets";

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

                    ArrayList tickets = new ArrayList();

                    while (dataReader.Read())
                    {
                        Ticket ticket = GetByDataReader(dataReader);
                        tickets.Add(ticket);
                    }

                    return tickets;
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

                string sqlQuery = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY " + orden + ") AS RowNum, * FROM vTickets";

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

                    ArrayList tickets = new ArrayList();

                    while (dataReader.Read())
                    {
                        Ticket ticket = GetByDataReader(dataReader);
                        tickets.Add(ticket);
                    }

                    return tickets;
                }
            }
        }


        public ArrayList TicketsByUserId(int id)
        {
            ArrayList cadenasBusqueda = new ArrayList();
            ArrayList parametrosBusqueda = new ArrayList();
            ArrayList valoresBusqueda = new ArrayList();
            string orden = "ID";

            cadenasBusqueda.Add("Usuario = @Usuario");
            parametrosBusqueda.Add("@Usuario");
            valoresBusqueda.Add(id);
            //cadenasBusqueda.Add("pagado = @Pagado");
            //parametrosBusqueda.Add("@Pagado");
            //valoresBusqueda.Add(true);
            return Search(cadenasBusqueda, parametrosBusqueda, valoresBusqueda, orden);
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
                string sqlQuery = "SELECT * FROM Tickets WHERE ID = @ID AND (ID IN (SELECT Ticket FROM Viajes))OR Pagado = 'true' AND ID = @ID";
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