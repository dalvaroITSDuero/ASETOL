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
/// Descripción breve de WebServiceViajes
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
// [System.Web.Script.Services.ScriptService]
public class WebServiceViajes : System.Web.Services.WebService {


    private Viaje _viaje = new Viaje();
    private Ticket _ticket = new Ticket();
    private readonly BDUsuario _bdUsuario = new BDUsuario();
    private readonly BDViajes _bdViajes = new BDViajes();
    private readonly BDTickets _bdTickets = new BDTickets();
    private Usuario _usuario = new Usuario();
    private readonly Acceso _acceso = new Acceso();


    public WebServiceViajes () {

        //Elimine la marca de comentario de la línea siguiente si utiliza los componentes diseñados 
        //InitializeComponent(); 
    }

    public class RespuestaViajes
    {
        public int Estado { get; set; }
        public Viaje[] viajes { get; set; }
        public Usuario usuario { get; set; }
        public string Mensaje { get; set; }
        public string Token { get; set; }
    }



    [WebMethod(Description = "Generar viaje a usuario")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    [OperationContract]
    public RespuestaViajes CreateViaje(int id, string token, int idTicket, short turno)
    {
        RespuestaViajes respuesta = new RespuestaViajes();
        try
        {
            _usuario = _acceso.identificarUsuario(short.Parse(id.ToString()), token);
            if (_usuario != null)
            {
                _ticket = _bdTickets.GetByID(idTicket);
                Viaje nuevoViaje = new Viaje();

                ArrayList cadenasBusqueda = new ArrayList();
                ArrayList parametrosBusqueda = new ArrayList();
                ArrayList valoresBusqueda = new ArrayList();
                string orden = "ID";
                cadenasBusqueda.Add("Fecha = @Fecha");
                parametrosBusqueda.Add("@Fecha");
                valoresBusqueda.Add(DateTime.Now.Date);
                cadenasBusqueda.Add("Usuario = @Usuario");
                parametrosBusqueda.Add("@Usuario");
                valoresBusqueda.Add(_usuario.ID);
                ArrayList viajes = _bdViajes.Search(cadenasBusqueda, parametrosBusqueda, valoresBusqueda, orden);
                if (viajes.Count == 0)
                {
                    nuevoViaje.usuario = _usuario;
                    nuevoViaje.ticket = _ticket;
                    nuevoViaje.usuario = _usuario;
                    nuevoViaje.turno = turno;
                    nuevoViaje.ubicacion = _usuario.ubicacion;
                    nuevoViaje.fecha = DateTime.Now.Date;
                    _bdViajes.Insert(ref nuevoViaje);
                }

                respuesta.Estado = 1;
                respuesta.Mensaje = "Viaje creado correctamente";
                Global.GenerarLog(_usuario, Global.GetUserIP(), "Usuario: "+_usuario.dni + " - Creacion viaje");
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
            respuesta.Mensaje = "Error al generar viaje";
        }
        return respuesta;
    }

    [WebMethod(Description = "Existe viaje en el dia de hoy?")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    [OperationContract]
    public RespuestaViajes GetViajeHoy(int id, string token)
    {
        RespuestaViajes respuesta = new RespuestaViajes();
        try
        {
            _usuario = _acceso.identificarUsuario(short.Parse(id.ToString()), token);
            if (_usuario != null)
            {

                ArrayList cadenasBusqueda = new ArrayList();
                ArrayList parametrosBusqueda = new ArrayList();
                ArrayList valoresBusqueda = new ArrayList();
                string orden = "ID";
                cadenasBusqueda.Add("Fecha = @Fecha");
                parametrosBusqueda.Add("@Fecha");
                valoresBusqueda.Add(DateTime.Now.Date);
                cadenasBusqueda.Add("Usuario = @Usuario");
                parametrosBusqueda.Add("@Usuario");
                valoresBusqueda.Add(_usuario.ID);
                ArrayList viajes = _bdViajes.Search(cadenasBusqueda, parametrosBusqueda, valoresBusqueda, orden);
                if (viajes.Count == 0)
                {
                    respuesta.Estado = 0;
                    respuesta.Mensaje = "Viaje no encontrado";
                }
                else
                {
                    respuesta.viajes = viajes.Cast<Viaje>().ToArray();
                    respuesta.Estado = 1;
                    respuesta.Mensaje = "Viaje obtenido correctamente";
                    Global.GenerarLog(_usuario, Global.GetUserIP(), "Usuario: " + _usuario.dni + " - Obtencion viaje del dia");
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
            respuesta.Mensaje = "Error al buscar viaje";
        }
        return respuesta;
    }


    [WebMethod(Description = "Actualiza el viaje a canjeado añadiendo el conductor, si se puede")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    [OperationContract]
    public RespuestaViajes UpdateDriverTravel(int id, string token, int viaje)
    {
        RespuestaViajes respuesta = new RespuestaViajes();
        try
        {
            _usuario = _acceso.identificarUsuario(short.Parse(id.ToString()), token);
            if (_usuario != null)
            {
                _viaje = _bdViajes.GetByID(viaje);
                respuesta.usuario = _viaje.usuario;
                if (_viaje.conductor == null)
                {
                    _viaje.conductor = _usuario;
                    _bdViajes.Update(_viaje);
                    respuesta.Estado = 1;
                    respuesta.Mensaje = "Viaje actualizado correctamente";
                    Global.GenerarLog(_usuario, Global.GetUserIP(), "Usuario: " + _usuario.dni + " - Actualiza el conductor");
                }
                else
                {
                    respuesta.Estado = 0;
                    respuesta.Mensaje = "Viaje ya ha sido canjeado";
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
            respuesta.Mensaje = "Error al buscar viaje: " +_viaje;
        }
        return respuesta;
    }

    [WebMethod(Description = "Obtiene todos los viajes por el id del usuario, en orden del mas reciente al mas antiguo")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    [OperationContract]
    public RespuestaViajes GetViajesById(int id, string token, bool conductor)
    {
        RespuestaViajes respuesta = new RespuestaViajes();
        try
        {
            _usuario = _acceso.identificarUsuario(short.Parse(id.ToString()), token);
            if (_usuario != null)
            {

                ArrayList cadenasBusqueda = new ArrayList();
                ArrayList parametrosBusqueda = new ArrayList();
                ArrayList valoresBusqueda = new ArrayList();
                string orden = "ID DESC, Turno";
                if (conductor)
                {
                    cadenasBusqueda.Add("Conductor = @Conductor");
                    parametrosBusqueda.Add("@Conductor");
                    valoresBusqueda.Add(_usuario.ID);
                }
                else
                {
                    cadenasBusqueda.Add("Usuario = @Usuario");
                    parametrosBusqueda.Add("@Usuario");
                    valoresBusqueda.Add(_usuario.ID);
                }
                respuesta.Estado = 0;
                respuesta.Mensaje = "No tiene ningún viaje";
                ArrayList viajes = _bdViajes.Search(cadenasBusqueda, parametrosBusqueda, valoresBusqueda, orden);
                respuesta.viajes = viajes.Cast<Viaje>().ToArray();
                respuesta.Estado = 1;
                respuesta.Mensaje = "Viajes recuperados correctamente";
                Global.GenerarLog(_usuario, Global.GetUserIP(), "Usuario: " + _usuario.dni + " - Obtencion de sus viajes");
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
            respuesta.Mensaje = "Error al buscar viaje: " + _viaje;
        }
        return respuesta;
    }

    [WebMethod(Description = "Devuelve los viajes que correspondan con ciertos filtros?")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    [OperationContract]
    public RespuestaViajes GetAllTravelsFilter(int id, string token, int usuario, int turno, string fecha)
    {
        RespuestaViajes respuesta = new RespuestaViajes();
        try
        {
            _usuario = _acceso.identificarUsuario(short.Parse(id.ToString()), token);
            if (_usuario != null)
            {
                ArrayList cadenasBusqueda = new ArrayList();
                ArrayList parametrosBusqueda = new ArrayList();
                ArrayList valoresBusqueda = new ArrayList();
                string orden = "ID DESC";
                cadenasBusqueda.Add("Conductor = @Conductor");
                parametrosBusqueda.Add("@Conductor");
                valoresBusqueda.Add(_usuario.ID);
                if (turno != 0)
                {
                    cadenasBusqueda.Add("Turno = @Turno");
                    parametrosBusqueda.Add("@Turno");
                    valoresBusqueda.Add(turno);
                }
                if (fecha != "0")
                {
                    cadenasBusqueda.Add("Fecha = @Fecha");
                    parametrosBusqueda.Add("@Fecha");
                    valoresBusqueda.Add(DateTime.Parse(fecha));
                }
                ArrayList viajes = _bdViajes.Search(cadenasBusqueda, parametrosBusqueda, valoresBusqueda, orden);
                if (viajes.Count == 0)
                {
                    respuesta.Estado = 0;
                    respuesta.Mensaje = "Viaje no encontrado";
                }
                else
                {
                    respuesta.viajes = viajes.Cast<Viaje>().ToArray();
                    respuesta.Estado = 1;
                    respuesta.Mensaje = "Viaje encontrados";
                    Global.GenerarLog(_usuario, Global.GetUserIP(), "Usuario: " + _usuario.dni + " - Obtencion de viajes existentes filtrados");
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
            respuesta.Mensaje = "Error al buscar viaje";
        }
        return respuesta;
    }
}
