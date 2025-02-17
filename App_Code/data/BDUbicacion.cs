using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de BDUbicacion
/// </summary>
namespace es.itsduero.iecscyl.data
{

    public class BDUbicacion
    {

        /// <summary>
        /// Inserta un nuevo ubicacion en la tabla Ubicacion
        /// </summary>        
        /// <param name="ubicacion">Objeto contenedor de los valores a insertar</param>
        /// <autor>dalvaro@itsduero.es</autor>
        public void Insert(ref Ubicacion ubicacion)
        {
            // Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                // Declaramos nuestra consulta de oficina Sql parametrizada
                string sqlQuery = "INSERT INTO Ubicaciones (ID, Descripcion) VALUES (@ID, @Descripcion)";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Damos valor al ID del ubicacion
                    ubicacion.ID = GetNextID();

                    // Añadimos los valores del objeto pasado como parámetro para crearlo en la base de datos
                    cmd.Parameters.AddWithValue("@ID", ubicacion.ID);
                    cmd.Parameters.AddWithValue("@Descripcion", ubicacion.descripcion);

                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Modifica un ubicacion en la tabla Ubicacion
        /// </summary>
        /// <param name="vale">Objeto contenedor de los valores a modificar</param>
        /// <autor>dalvaro@itsduero.es</autor>
        public void Update(Ubicacion ubicacion)
        {
            // Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                // Declaramos nuestra consulta de acción Sql parametrizada
                string sqlQuery = "UPDATE Ubicaciones SET Descripcion = @Descripcion, ID = @ID WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Añadimos los valores del objeto pasado como parámetro para crearlo en la base de datos
                    cmd.Parameters.AddWithValue("@ID", ubicacion.ID);
                    cmd.Parameters.AddWithValue("@Descripcion", ubicacion.descripcion);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Elimina un ubicacion en la tabla Ubicacion
        /// </summary>        
        /// <param name="ubicacion">Objeto contenedor de los valores a eliminar</param>
        /// <autor>dalvaro@itsduero.es</autor>
        public void Delete(Ubicacion ubicacion)
        {
            // Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                // Declaramos nuestra consulta de referencia Sql parametrizada
                string sqlQuery = "DELETE FROM Ubicaciones WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Añadimos los valores del objeto pasado como parámetro para eliminarlo de la base de datos
                    cmd.Parameters.AddWithValue("@ID", ubicacion.ID);

                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Obtiene un Ubicacion a partir del resultado de la consulta SQL
        /// </summary>
        /// <param name="dataReader">DataReader con el resultado de la consulta SQL</param>
        /// <returns>Un ArrayList con los valores del Ubicacion</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        private Ubicacion GetByDataReader(SqlDataReader dataReader)
        {
            Ubicacion ubi = new Ubicacion
            {
                ID = Convert.ToInt32(dataReader["ID"]),
                descripcion = Convert.ToString(dataReader["Descripcion"])

            };

            return ubi;
        }

        /// <summary>
        /// Devuelve un objeto Ubicacion
        /// </summary>
        /// <param name="ID">ID del ubicacion a buscar</param>
        /// <returns>Un registro con los valores del Ubicacion</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public Ubicacion GetByID(int ID)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT * FROM Ubicaciones WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Utilizamos el valor del parámetro para enviarlo al parámetro declarado en la consulta de selección SQL
                    cmd.Parameters.AddWithValue("@ID", ID);
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.Read())
                    {
                        Ubicacion ubi = GetByDataReader(dataReader);
                        return ubi;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Devuelve el siguiente ID de Ubicacion a insertar en la base de datos
        /// </summary>        
        /// <returns>El valor máximo del campo ID de la tabla Ubicacion + 1</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public int GetNextID()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT MAX(ID) AS MaxID FROM Ubicaciones";
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
        /// Devuelve todos los objetos Ubicaciones (solo acceso autorizado)
        /// </summary>
        /// <returns>Un ArrayList con los inscritos</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public ArrayList GetAll()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT * FROM Ubicaciones";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    ArrayList ubicaciones = new ArrayList();

                    while (dataReader.Read())
                    {
                        Ubicacion ubicacion = GetByDataReader(dataReader);
                        ubicaciones.Add(ubicacion);
                    }

                    return ubicaciones;
                }
            }
        }
    }
}