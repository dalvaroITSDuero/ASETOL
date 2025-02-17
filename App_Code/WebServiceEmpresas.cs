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
/// Descripción breve de WebServiceEmpresas
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
// [System.Web.Script.Services.ScriptService]
public class WebServiceEmpresas : System.Web.Services.WebService {

    private Empresa _empresa = new Empresa();
    private readonly BDEmpresa _bdEmpresa = new BDEmpresa();
    private Usuario _usuario = new Usuario();
    private readonly Acceso _acceso = new Acceso();


    public WebServiceEmpresas () {

        //Elimine la marca de comentario de la línea siguiente si utiliza los componentes diseñados 
        //InitializeComponent(); 
    }

    public class RespuestaEmpresa
    {
        public int Estado { get; set; }
        public Empresa[] empresas { get; set; }
        public string Mensaje { get; set; }

    }

    [WebMethod(Description = "Comprobación de si existe un usuario con el mismo username (DNI)")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    [OperationContract]
    public RespuestaEmpresa GetAll(int id, string token)
    {
        RespuestaEmpresa respuesta = new RespuestaEmpresa();
        try
        {
            _usuario = _acceso.identificarUsuario(short.Parse(id.ToString()), token);
            if (_usuario != null)
            {
                ArrayList empresas = _bdEmpresa.GetAll();
                if (empresas.Count != 0)
                {
                    respuesta.empresas = empresas.Cast<Empresa>().ToArray();
                    respuesta.Estado = 1;
                    respuesta.Mensaje = "Empresas encontradas";
                    Global.GenerarLog(_usuario, Global.GetUserIP(), "Usuario: " + _usuario.dni + " - Obtencion todas las empresas");
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
