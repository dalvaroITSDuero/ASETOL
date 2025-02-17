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
/// Descripción breve de WebServiceCalendarios
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
// [System.Web.Script.Services.ScriptService]
public class WebServiceCalendarios : System.Web.Services.WebService {

    private readonly BDUsuario _bdUsuario = new BDUsuario();
    private readonly BDCalendario _bdCalendario = new BDCalendario();
    private readonly Acceso _acceso = new Acceso();
    private Usuario _usuario = new Usuario();
    private Calendario _calendario = new Calendario();


    public WebServiceCalendarios () {

        //Elimine la marca de comentario de la línea siguiente si utiliza los componentes diseñados 
        //InitializeComponent(); 
    }

    public class RespuestaCalendarios
    {
        public int Estado { get; set; }
        public Calendario[] Calendarios { get; set; }
        public string Mensaje { get; set; }
        public Ticket ticket { get; set; }
    }

    [WebMethod(Description = "Busqueda del calendario de un usuario")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    [OperationContract]
    public RespuestaCalendarios GetAllById(int id, string token)
    {
        RespuestaCalendarios respuesta = new RespuestaCalendarios();
        try
        {
            _usuario = _acceso.identificarUsuario(short.Parse(id.ToString()), token);
            if (_usuario != null)
            {

                ArrayList cal = _bdCalendario.CalendarByUserId(id);

                if (cal.Count != 0)
                {
                    foreach (Calendario calendar in cal)
                    {
                        calendar.usuario = null;
                    }
                    respuesta.Calendarios = cal.Cast<Calendario>().ToArray();
                    respuesta.Estado = 1;
                    respuesta.Mensaje = "Calendario Encontrado";
                    Global.GenerarLog(_usuario, Global.GetUserIP(), "Usuario: " + _usuario.dni + " - Obtencion de todos los registros del calendario");
                }
                else
                {
                    respuesta.Estado = 0;
                    respuesta.Mensaje = "Calendario vacio";
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
            respuesta.Mensaje = "Error al buscar calendario";
        }
        return respuesta;
    }


    [WebMethod(Description = "Creación o actualización del calendario de usuario")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    [OperationContract]
    public RespuestaCalendarios UpdateOrCreateCalendar(int id, string token, DateTime fecha, short turno)
    {
        RespuestaCalendarios respuesta = new RespuestaCalendarios();
        try
        {
            _usuario = _acceso.identificarUsuario(short.Parse(id.ToString()), token);
            if (_usuario != null)
            {
                Calendario cal = new Calendario();
                cal.usuario = new Usuario { ID = id };
                cal.fecha = fecha;
                cal.turno = turno;
                _bdCalendario.UpdateOrCreate(ref cal);

                respuesta.Estado = 1;
                respuesta.Mensaje = "Calendario Actualizado o creado";
                Global.GenerarLog(_usuario, Global.GetUserIP(), "Usuario: " + _usuario.dni + " - Actualizacion o creacion de un registro en calendario");
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
            respuesta.Mensaje = "Error al Actualizar o crear el calendario";
        }
        return respuesta;
    }

    [WebMethod(Description = "Eliminación del calendario de usuario")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    [OperationContract]
    public RespuestaCalendarios DeleteCalendar(int id, string token, DateTime fecha)
    {
        ArrayList cadenasBusqueda = new ArrayList();
        ArrayList parametrosBusqueda = new ArrayList();
        ArrayList valoresBusqueda = new ArrayList();
        string orden = "ID";

        RespuestaCalendarios respuesta = new RespuestaCalendarios();
        try
        {
            _usuario = _acceso.identificarUsuario(short.Parse(id.ToString()), token);
            if (_usuario != null)
            {

                cadenasBusqueda.Add("Usuario = @Usuario");
                parametrosBusqueda.Add("@Usuario");
                valoresBusqueda.Add(id);
                cadenasBusqueda.Add("Fecha = @Fecha");
                parametrosBusqueda.Add("@Fecha");
                valoresBusqueda.Add(fecha);
                ArrayList cal = _bdCalendario.Search(cadenasBusqueda, parametrosBusqueda, valoresBusqueda, orden);
                _bdCalendario.Delete((Calendario)cal[0]);
                respuesta.Estado = 1;
                respuesta.Mensaje = "Calendario Se ha eliminado correctamente";
                Global.GenerarLog(_usuario, Global.GetUserIP(), "Usuario: " + _usuario.dni + " - Eliminacion de un registro calendario");
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
            respuesta.Mensaje = "Error al Actualizar o crear el calendario";
        }
        return respuesta;
    }

    [WebMethod(Description = "Creación de calendario con ticket diario para dicho calendario")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    [OperationContract]
    public RespuestaCalendarios CreateCalendarAndDailyTicket(int id, string token, DateTime fecha, short turno)
    {
        RespuestaCalendarios respuesta = new RespuestaCalendarios();
        BDTickets _bdTickets = new BDTickets();
        Ticket ticket = new Ticket();

        try
        {
            _usuario = _acceso.identificarUsuario(short.Parse(id.ToString()), token);
            if (_usuario != null && _usuario.tipoTicket == 'D')
            {
                Calendario cal = new Calendario();
                cal.usuario = new Usuario { ID = id };
                cal.fecha = fecha;
                cal.turno = turno;
                _bdCalendario.UpdateOrCreate(ref cal);


                ArrayList cadenasBusqueda = new ArrayList();
                ArrayList parametrosBusqueda = new ArrayList();
                ArrayList valoresBusqueda = new ArrayList();

                cadenasBusqueda.Add("Usuario = @Usuario");
                parametrosBusqueda.Add("@Usuario");
                valoresBusqueda.Add(id);
                cadenasBusqueda.Add("FechaInicio = @FechaInicio");
                parametrosBusqueda.Add("@FechaInicio");
                valoresBusqueda.Add(fecha);
                ArrayList tickets = _bdTickets.Search(cadenasBusqueda, parametrosBusqueda, valoresBusqueda, "ID");
                if (tickets.Count == 1)
                {
                    respuesta.ticket = (Ticket)tickets[0];
                    respuesta.Estado = 2;
                    respuesta.Mensaje = "Calendario actualizado";
                    return respuesta;
                }
                ticket.usuario = _usuario;
                ticket.fechaInicio = fecha;
                ticket.fechaFin = fecha;
                ticket.pagado = false;
                ticket.fechaPago = new DateTime(1753, 1, 1);
                _bdTickets.Insert(ref ticket);

                respuesta.ticket = ticket;
                respuesta.Estado = 1;
                respuesta.Mensaje = "Calendario creado u actualizado y ticket para ese dia creado";
                
                Global.GenerarLog(_usuario, Global.GetUserIP(), "Usuario: " + _usuario.dni + " - Creacion de un registro en calendario y ticket (usaurio diario)");
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
            respuesta.Mensaje = "Error al crear el ticket o al crear el calendario";
        }
        return respuesta;
    }
}
