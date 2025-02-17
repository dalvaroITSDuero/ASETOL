using es.itsduero.iecscyl;
using es.itsduero.iecscyl.data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
/// <summary>
/// Descripción breve de WebServiceUbicacion
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
// [System.Web.Script.Services.ScriptService]
public class WebServiceUbicaciones : System.Web.Services.WebService {

    private Ubicacion _ubicacion = new Ubicacion();
    private readonly BDUbicacion _bdUbicacion = new BDUbicacion();
    private Usuario _usuario = new Usuario();
    private readonly Acceso _acceso = new Acceso();

    public WebServiceUbicaciones()
    {
        //Elimine la marca de comentario de la línea siguiente si utiliza los componentes diseñados 
        //InitializeComponent(); 
    }

    public class RespuestaUbicaciones
    {
        public int Estado { get; set; }
        public Ubicacion[] ubicaciones { get; set; }
        public string Mensaje { get; set; }

    }

    [WebMethod(Description = "Comprobación de si existe un usuario con el mismo username (DNI)")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    [OperationContract]
    public RespuestaUbicaciones GetAll(int id, string token)
    {
        RespuestaUbicaciones respuesta = new RespuestaUbicaciones();
        try
        {
            _usuario = _acceso.identificarUsuario(short.Parse(id.ToString()), token);
            if (_usuario != null)
            {
                ArrayList ubicaciones = _bdUbicacion.GetAll();
                if (ubicaciones.Count != 0)
                {
                    respuesta.ubicaciones = ubicaciones.Cast<Ubicacion>().ToArray();
                    respuesta.Estado = 1;
                    respuesta.Mensaje = "Empresas encontradas";
                    Global.GenerarLog(_usuario, Global.GetUserIP(), "Usuario: " + _usuario.dni + " - Obtencion de todas las ubicaciones");
                }
                else
                {
                    respuesta.Estado = 0;
                    respuesta.Mensaje = "Empresas no encontradas";
                }
            }
            else
            {
                respuesta.Estado = 0;
                respuesta.Mensaje = "Usuario no verificado";
            }
        }
        catch
        {
            respuesta.Estado = -1;
            respuesta.Mensaje = "Error de busqueda";
        }
        return respuesta;
    }
}
