using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de Class1
/// </summary>
namespace es.itsduero.iecscyl.data
{

    public class BDEmpresa
    {

        /// <summary>
        /// Inserta un nuevo empresa en la tabla Empresa
        /// </summary>        
        /// <param name="empresa">Objeto contenedor de los valores a insertar</param>
        /// <autor>dalvaro@itsduero.es</autor>
        public void Insert(ref Empresa empresa)
        {
            // Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                // Declaramos nuestra consulta de oficina Sql parametrizada
                string sqlQuery = "INSERT INTO Empresas (ID, RazonSocial, NombreComercial, Activo) VALUES (@ID, @RazonSocial, @NombreComercial, @Activo)";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Damos valor al ID del empresa
                    empresa.ID = GetNextID();

                    // Añadimos los valores del objeto pasado como parámetro para crearlo en la base de datos
                    cmd.Parameters.AddWithValue("@ID", empresa.ID);
                    cmd.Parameters.AddWithValue("@RazonSocial", empresa.razonSocial);
                    cmd.Parameters.AddWithValue("@NombreComercial", empresa.nombreComercial);
                    cmd.Parameters.AddWithValue("@Activo", empresa.activo);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Modifica un empresa en la tabla Empresa
        /// </summary>
        /// <param name="vale">Objeto contenedor de los valores a modificar</param>
        /// <autor>dalvaro@itsduero.es</autor>
        public void Update(Empresa empresa)
        {
            // Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                // Declaramos nuestra consulta de acción Sql parametrizada
                string sqlQuery = "UPDATE Empresas SET NombreComercial = @NombreComercial, RazonSocial = @RazonSocial, ID = @ID, Activo = @Activo WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Añadimos los valores del objeto pasado como parámetro para crearlo en la base de datos
                    cmd.Parameters.AddWithValue("@ID", empresa.ID);
                    cmd.Parameters.AddWithValue("@RazonSocial", empresa.razonSocial);
                    cmd.Parameters.AddWithValue("@NombreComercial", empresa.nombreComercial);
                    cmd.Parameters.AddWithValue("@Activo", empresa.activo);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Elimina un empresa en la tabla Empresa
        /// </summary>        
        /// <param name="empresa">Objeto contenedor de los valores a eliminar</param>
        /// <autor>dalvaro@itsduero.es</autor>
        public void Delete(Empresa empresa)
        {
            // Creamos nuestro objeto de conexion usando nuestro archivo de configuraciones
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                // Declaramos nuestra consulta de referencia Sql parametrizada
                string sqlQuery = "DELETE FROM Empresas WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Añadimos los valores del objeto pasado como parámetro para eliminarlo de la base de datos
                    cmd.Parameters.AddWithValue("@ID", empresa.ID);

                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Obtiene un Empresa a partir del resultado de la consulta SQL
        /// </summary>
        /// <param name="dataReader">DataReader con el resultado de la consulta SQL</param>
        /// <returns>Un ArrayList con los valores del Empresa</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        private Empresa GetByDataReader(SqlDataReader dataReader)
        {
            Empresa empre = new Empresa
            {
                ID = Convert.ToInt32(dataReader["ID"]),
                razonSocial = Convert.ToString(dataReader["RazonSocial"]),
                nombreComercial = Convert.ToString(dataReader["NombreComercial"]),
                activo = Convert.ToBoolean(dataReader["Activo"])

            };

            return empre;
        }

        /// <summary>
        /// Devuelve un objeto Empresa
        /// </summary>
        /// <param name="ID">ID del empresa a buscar</param>
        /// <returns>Un registro con los valores del Empresa</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public Empresa GetByID(int ID)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT * FROM Empresas WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Utilizamos el valor del parámetro para enviarlo al parámetro declarado en la consulta de selección SQL
                    cmd.Parameters.AddWithValue("@ID", ID);
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.Read())
                    {
                        Empresa empre = GetByDataReader(dataReader);
                        return empre;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Devuelve el siguiente ID de Empresa a insertar en la base de datos
        /// </summary>        
        /// <returns>El valor máximo del campo ID de la tabla Empresa + 1</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public int GetNextID()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT MAX(ID) AS MaxID FROM Empresas";
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
        /// Devuelve todos los objetos Empresas (solo acceso autorizado)
        /// </summary>
        /// <returns>Un ArrayList con los inscritos</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public ArrayList GetAll()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();

                string sqlQuery = "SELECT * FROM Empresas";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    ArrayList empresas = new ArrayList();

                    while (dataReader.Read())
                    {
                        Empresa empresa = GetByDataReader(dataReader);
                        empresas.Add(empresa);
                    }

                    return empresas;
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

                string sqlQuery = "SELECT COUNT(*) AS Num FROM Empresas";

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
        /// Devuelve todos los objetos Empresa que coincidan con los campos de búsqueda
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

                string sqlQuery = "SELECT * FROM Empresas";

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

                    ArrayList empresas = new ArrayList();

                    while (dataReader.Read())
                    {
                        Empresa empresa = GetByDataReader(dataReader);
                        empresas.Add(empresa);
                    }

                    return empresas;
                }
            }
        }


        /// <summary>
        /// Devuelve todos los objetos Empresa entre un índice inferior y otro superior que coincidan con los campos de búsqueda
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

                string sqlQuery = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY " + orden + ") AS RowNum, * FROM Empresas";

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

                    ArrayList empresas = new ArrayList();

                    while (dataReader.Read())
                    {
                        Empresa empresa = GetByDataReader(dataReader);
                        empresas.Add(empresa);
                    }

                    return empresas;
                }
            }
        }


        /// <summary>
        /// Comprueba si la empresa puede eliminarse
        /// </summary>        
        /// <param name="usuario">Usuario a comprobar</param>
        /// <param name="motivo">El motivo por el que no se puede eliminar</param>
        /// <returns>Si el usuario se puede eliminar o no</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public bool CheckDelete(Empresa empresa, ref string motivo)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                conn.Open();
                string sqlQuery = "SELECT * FROM Empresas WHERE ID = @ID AND (ID IN (SELECT Empresa FROM Usuarios))";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    // Utilizamos el valor del parámetro para enviarlo al parámetro declarado en la consulta de selección SQL
                    cmd.Parameters.AddWithValue("@ID", empresa.ID);

                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.Read() || empresa.ID == 9)
                    {
                        motivo = "No se puede eliminar un Empresa que ya tiene trabajadores asociados.";
                        return false;
                    }
                }

            }
            return true;
        }
    }
}