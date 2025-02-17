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
/// Descripción breve de WebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
// [System.Web.Script.Services.ScriptService]
public class WebServiceLogin : System.Web.Services.WebService {

    private Usuario _usuario = new Usuario();
    private readonly BDUsuario _bdUsuario = new BDUsuario();
    private readonly BDTickets _bdTickets = new BDTickets();

    public WebServiceLogin () {

        //Elimine la marca de comentario de la línea siguiente si utiliza los componentes diseñados 
        //InitializeComponent(); 
    }

    public class RespuestaLogin
    {
        public int Estado { get; set; }
        public Usuario usuario { get; set; }
        public string Mensaje { get; set; }
        public string Token { get; set; }
    }

    [WebMethod(Description = "Identificación en la plataforma con usuario y contraseña")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    [OperationContract]
    public RespuestaLogin Identificacion(string usuario, string password)
    {
        RespuestaLogin respuesta = new RespuestaLogin();
        try
        {
            _usuario = _bdUsuario.GetByUsuarioPassword(usuario, password);

            if (_usuario != null && _usuario.permisos.ID != 1)
            {
                respuesta.Estado = 1;
                respuesta.usuario = _usuario;
                respuesta.Mensaje = "Login correcto";
                respuesta.Token = _usuario.token;
                Global.GenerarLog(_usuario, Global.GetUserIP(), "Usuario: " + _usuario.dni + " - Login");
            }
            else
            {
                respuesta.Estado = 0;
                respuesta.Mensaje = "Error de identificación de usuario";
            }
        }
        catch
        {
            respuesta.Estado = -1;
            respuesta.Mensaje = "Error indeterminado";
        }

        return respuesta;
    }

    [WebMethod(Description = "Identificación en la plataforma con usuario y contraseña")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    [OperationContract]
    public RespuestaLogin Verificacion(int id, string token)
    {
        RespuestaLogin respuesta = new RespuestaLogin();
        try
        {
            _usuario = _bdUsuario.GetByID(id);

            if (_usuario != null && _usuario.token.Equals(token))
            {
                respuesta.Estado = 1;
                respuesta.Mensaje = "Login correcto";
                //en un futuro habrá que hacerlo para varios tipos de tickets (diarios)

                ArrayList cadenasBusqueda = new ArrayList();
                ArrayList parametrosBusqueda = new ArrayList();
                ArrayList valoresBusqueda = new ArrayList();
                string orden = "FechaFin";

                //busca los tickets del usuario y respecto al ticket mas antiguo que tiene te genera el siguiente, aunque sin pagar :)
                cadenasBusqueda.Add("Usuario = @Usuario");
                parametrosBusqueda.Add("@Usuario");
                valoresBusqueda.Add(_usuario.ID);
                Global.GenerarLog(_usuario, Global.GetUserIP(), "Usuario: " + _usuario.dni + " - Verificacion login");
                ArrayList tickets = _bdTickets.Search(cadenasBusqueda, parametrosBusqueda, valoresBusqueda, orden);
                if (tickets.Count != 0 && _usuario.tipoTicket == 'M')
                {
                    Ticket ultimoTicket = (Ticket)tickets[tickets.Count - 1];
                    DateTime today = DateTime.Now;
                    today = today.AddDays(7);
                    if (ultimoTicket.fechaFin <= today)
                    {
                        Ticket nuevo = new Ticket();
                        nuevo.usuario = _usuario;
                        DateTime fechaInicial = new DateTime(ultimoTicket.fechaInicio.Year, ultimoTicket.fechaInicio.Month, 1);
                        nuevo.fechaInicio = fechaInicial.AddMonths(1);
                        nuevo.fechaFin = fechaInicial.AddMonths(2).AddDays(-1);
                        ProrrateoDias(ref nuevo);
                        EsCyndeaETT(ref nuevo);
                        _bdTickets.Insert(ref nuevo);
                    }
                }
                else if (_usuario.tipoTicket == 'M')
                {
                    DateTime fechaInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    //falta hacer prorrateo de días
                    //supuesto prorrateo de tickets
                    Ticket nuevo = new Ticket();
                    nuevo.usuario = _usuario;
                    nuevo.fechaInicio = DateTime.Now.Date;
                    nuevo.fechaFin = fechaInicial.AddMonths(1).AddDays(-1);
                    EsCyndeaETT(ref nuevo);
                    ProrrateoDias(ref nuevo);
                    _bdTickets.Insert(ref nuevo);
                }
            }
            else
            {
                respuesta.Estado = 0;
                respuesta.Mensaje = "Error de identificación de usuario";
            }
        }
        catch
        {
            respuesta.Estado = -1;
            respuesta.Mensaje = "Error indeterminado";
        }

        return respuesta;
    }

    private void ProrrateoDias(ref Ticket ticket)
    {
        TimeSpan diferencia = ticket.fechaFin - ticket.fechaInicio;
        int numDias = diferencia.Days +1;
        if (numDias > 15)
        {
            ticket.importe = 110.00;
        }
        else
        {
            ticket.importe = numDias * 7.00;
        }
    }

    private void EsCyndeaETT(ref Ticket ticket)
    {
        if (ticket.usuario.empresa.ID == 8)
        {
            ticket.pagado = true;
            ticket.fechaPago = DateTime.Now;
        }
        else
        {
            ticket.pagado = false;
            ticket.fechaPago = new DateTime(1753, 1, 1);
        }
    }
}
