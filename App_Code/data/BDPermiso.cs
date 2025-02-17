using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de BDPermisos
/// </summary>
namespace es.itsduero.iecscyl.data
{

    public class BDPermiso
    {
        /// <summary>
        /// Inserta un nuevo permiso en la tabla Permisos
        /// </summary>        
        /// <param name="permiso">Objeto contenedor de los valores a insertar</param>
        /// <autor>dalvaro@itsduero.es</autor>
        public void Insert(ref Permisos permiso)
        {
            // Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                // Declaramos nuestra consulta de oficina Sql parametrizada
                string sqlQuery = "INSERT INTO Permisos (ID, Descripcion) VALUES (@ID, @Descripcion)";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Damos valor al ID del permiso
                    permiso.ID = GetNextID();

                    // Añadimos los valores del objeto pasado como parámetro para crearlo en la base de datos
                    cmd.Parameters.AddWithValue("@ID", permiso.ID);
                    cmd.Parameters.AddWithValue("@Descripcion", permiso.descripcion);

                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Modifica un permiso en la tabla Permisos
        /// </summary>
        /// <param name="vale">Objeto contenedor de los valores a modificar</param>
        /// <autor>dalvaro@itsduero.es</autor>
        public void Update(Permisos permiso)
        {
            // Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                // Declaramos nuestra consulta de acción Sql parametrizada
                string sqlQuery = "UPDATE Permisos SET Descripcion = @Descripcion, ID = @ID WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Añadimos los valores del objeto pasado como parámetro para crearlo en la base de datos
                    cmd.Parameters.AddWithValue("@ID", permiso.ID);
                    cmd.Parameters.AddWithValue("@Descripcion", permiso.descripcion);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Elimina un permiso en la tabla Permisos
        /// </summary>        
        /// <param name="permiso">Objeto contenedor de los valores a eliminar</param>
        /// <autor>dalvaro@itsduero.es</autor>
        public void Delete(Permisos permiso)
        {
            // Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                // Declaramos nuestra consulta de referencia Sql parametrizada
                string sqlQuery = "DELETE FROM Permisos WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Añadimos los valores del objeto pasado como parámetro para eliminarlo de la base de datos
                    cmd.Parameters.AddWithValue("@ID", permiso.ID);

                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Obtiene un Permisos a partir del resultado de la consulta SQL
        /// </summary>
        /// <param name="dataReader">DataReader con el resultado de la consulta SQL</param>
        /// <returns>Un ArrayList con los valores del Permisos</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        private Permisos GetByDataReader(SqlDataReader dataReader)
        {
            Permisos permi = new Permisos
            {
                ID = Convert.ToInt32(dataReader["ID"]),
                descripcion = Convert.ToString(dataReader["Descripcion"])

            };

            return permi;
        }

        /// <summary>
        /// Devuelve un objeto Permisos
        /// </summary>
        /// <param name="ID">ID del permiso a buscar</param>
        /// <returns>Un registro con los valores del Permisos</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public Permisos GetByID(int ID)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT * FROM Permisos WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Utilizamos el valor del parámetro para enviarlo al parámetro declarado en la consulta de selección SQL
                    cmd.Parameters.AddWithValue("@ID", ID);
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.Read())
                    {
                        Permisos permi = GetByDataReader(dataReader);
                        return permi;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Devuelve el siguiente ID de Permisos a insertar en la base de datos
        /// </summary>        
        /// <returns>El valor máximo del campo ID de la tabla Permisos + 1</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public int GetNextID()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT MAX(ID) AS MaxID FROM Permisos";
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
        /// Devuelve todos los objetos Permisos (solo acceso autorizado)
        /// </summary>
        /// <returns>Un ArrayList con los inscritos</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public ArrayList GetAll()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT * FROM Permisos";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    ArrayList permisos = new ArrayList();

                    while (dataReader.Read())
                    {
                        Permisos permiso = GetByDataReader(dataReader);
                        permisos.Add(permiso);
                    }

                    return permisos;
                }
            }
        }
    }
}