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
/// Descripción breve de WebServiceTickets
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
// [System.Web.Script.Services.ScriptService]
public class WebServiceTickets : System.Web.Services.WebService {

    private readonly BDTickets _bdTickets= new BDTickets();
    private Usuario _usuario = new Usuario();
    private readonly Acceso _acceso = new Acceso();

    public class RespuestaCalendarios
    {
        public int Estado { get; set; }
        public Ticket[] Tickets { get; set; }
        public string Mensaje { get; set; }

    }

    public WebServiceTickets () {

        //Elimine la marca de comentario de la línea siguiente si utiliza los componentes diseñados 
        //InitializeComponent(); 
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

                ArrayList tickets = _bdTickets.TicketsByUserId(id);

                if (tickets.Count != 0)
                {
                    foreach (Ticket tick in tickets)
                    {
                        tick.usuario = null;
                    }
                    respuesta.Tickets = tickets.Cast<Ticket>().ToArray();
                    respuesta.Estado = 1;
                    respuesta.Mensaje = "tickets Encontrado";
                    Global.GenerarLog(_usuario, Global.GetUserIP(), "Usuario: " + _usuario.dni + " - Obtencion de todos sus tickets");

                }
                else
                {
                    respuesta.Estado = 0;
                    respuesta.Mensaje = "ticket vacio";
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
            respuesta.Mensaje = "Error al buscar tickets";
        }
        return respuesta;
    }
}
