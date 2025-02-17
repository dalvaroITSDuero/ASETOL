using es.itsduero.iecscyl;
using es.itsduero.iecscyl.data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using RedsysAPIPrj;
using Newtonsoft.Json;
using System.Globalization;

/// <summary>
/// Descripción breve de WebServicePago
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
// [System.Web.Script.Services.ScriptService]
public class WebServicePago : System.Web.Services.WebService
{



    ArrayList inscritos = new ArrayList();
    public float precioCarrera = 0;
    //public float precio = 110;
    public string grupoPago = "";


    private readonly BDTickets _bdTickets = new BDTickets();
    private readonly BDUsuario _bdUsuario = new BDUsuario();
    private readonly Acceso _acceso = new Acceso();
    private Usuario _usuario = new Usuario();
    private Ticket _ticket = new Ticket();

    public WebServicePago()
    {

        //Elimine la marca de comentario de la línea siguiente si utiliza los componentes diseñados 
        //InitializeComponent(); 
    }

    public class PaymentData
    {
        public string Ds_Date { get; set; }
        public string Ds_Hour { get; set; }
        public string Ds_Order { get; set; }
        // Incluye otras propiedades si es necesario
    }

    public class Respuesta
    {
        public String Ds_Merchant_MerchantParameters;
        public String Ds_Merchant_MerchantSignature;

        public int Estado { get; set; }
        public Calendario[] Calendarios { get; set; }
        public string Mensaje { get; set; }

    }

    [WebMethod(Description = "Creación o actualización del calendario de usuario")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    [OperationContract]
    public Respuesta DatosPay(int id, string token, int idTicket)
    {
        String Ds_Merchant_Amount;
        String Ds_Merchant_Order;
        String Ds_Merchant_ProductDescription;
        String Ds_Merchant_MerchantCode;
        String Ds_Merchant_Titular;

        String Ds_Merchant_MerchantURL;
        String Ds_Merchant_UrlOK;
        String Ds_Merchant_UrlKO;


        String CLAVESECRETA;
        Respuesta respuesta = new Respuesta();
        try
        {
            _usuario = _acceso.identificarUsuario(short.Parse(id.ToString()), token);
            if (_usuario != null)
            {
                _ticket = _bdTickets.GetByID(idTicket);

                _ticket.importe *= 100;

                Ds_Merchant_MerchantCode = ConfigurationManager.AppSettings["fuc"].ToString();
                //CLAVESECRETA = "085249159"; 
                CLAVESECRETA = ConfigurationManager.AppSettings["claveSecreta"].ToString();
                //"sq7HjrUOBfKmC576ILgskD5srU870gJ7";
                Ds_Merchant_Amount = _ticket.importe.ToString();
                Ds_Merchant_Order = _ticket.ID + "PED" + DateTime.Now.Minute + DateTime.Now.Second;
                Ds_Merchant_ProductDescription = "INSCRIPCIÓN " + _usuario.dni + " (ref. " + _ticket.ID + ")";
                Ds_Merchant_Titular = "ASETOL";

                // Obtenemos la URL y reemplazamos http por https para realizar el pago en redsys
                String strPathAndQuery = HttpContext.Current.Request.Url.PathAndQuery;
                String dominio = ConfigurationManager.AppSettings["url"].ToString();

                Ds_Merchant_MerchantURL = "https://autobusesrubio.gestionando.online/notificacionOnline.aspx"; // ConfigurationManager.AppSettings["url"].ToString() + "
                if (_ticket.usuario.tipoTicket == 'D')
                {
                    Ds_Merchant_UrlOK = "https://asetol.es/pasajero/indexPasajero.html?OK=1";
                    Ds_Merchant_UrlKO = "https://asetol.es/pasajero/indexPasajero.html?OK=0";
                }
                else
                {
                    Ds_Merchant_UrlOK = "https://asetol.es/pasajero/tickets.html?OK=1";
                    Ds_Merchant_UrlKO = "https://asetol.es/pasajero/tickets.html?OK=0";
                }

                string cadenaFirma = Ds_Merchant_Amount + Ds_Merchant_Order + Ds_Merchant_MerchantCode + "978" + "0" + Ds_Merchant_MerchantURL + CLAVESECRETA;


                RedsysAPI r = new RedsysAPI();

                r.SetParameter("DS_MERCHANT_AMOUNT", Ds_Merchant_Amount);
                r.SetParameter("DS_MERCHANT_ORDER", Ds_Merchant_Order);
                r.SetParameter("DS_MERCHANT_MERCHANTCODE", Ds_Merchant_MerchantCode);
                r.SetParameter("DS_MERCHANT_CURRENCY", "978");
                r.SetParameter("DS_MERCHANT_TRANSACTIONTYPE", "0");
                r.SetParameter("DS_MERCHANT_TERMINAL", "101");
                r.SetParameter("DS_MERCHANT_MERCHANTURL", Ds_Merchant_MerchantURL);
                r.SetParameter("DS_MERCHANT_URLOK", Ds_Merchant_UrlOK);
                r.SetParameter("DS_MERCHANT_URLKO", Ds_Merchant_UrlKO);

                // Calculate Ds_MerchantParameters
                respuesta.Ds_Merchant_MerchantParameters = r.createMerchantParameters();

                // Calculate Ds_Signature
                respuesta.Ds_Merchant_MerchantSignature = r.createMerchantSignature(CLAVESECRETA);

                respuesta.Estado = 1;
                respuesta.Mensaje = "Pago redireccionado";
                Global.GenerarLog(_usuario, Global.GetUserIP(), "Usuario: " + _usuario.dni + " - Redirecion de pago correcta");
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
            respuesta.Mensaje = "Error en el pago";
        }
        return respuesta;
    }

    //[WebMethod(Description = "Confirmación de pago")]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    //[OperationContract]
    //public Respuesta ConfirmarPago(int id, string token, string Ds_Signature, string Ds_MerchantParameters)
    //{
    //    Respuesta respuesta = new Respuesta();


    //    //String CLAVESECRETA = "085249159abc";
    //    String CLAVESECRETA = ConfigurationManager.AppSettings["claveSecreta"].ToString();
    //    //"sq7HjrUOBfKmC576ILgskD5srU870gJ7";

    //    string fileName = @"\imgUser\" + Guid.NewGuid() + ".log";
    //    string path = Server.MapPath("~") + fileName;

    //    File.Create(path).Dispose();
    //    TextWriter tw = new StreamWriter(path, true, System.Text.Encoding.GetEncoding(1252));
    //    try
    //    {
    //        RedsysAPI r = new RedsysAPI();
    //        // Decode Base 64 data
    //        string deco = r.decodeMerchantParameters(Ds_MerchantParameters);

    //        // Get Signature notificacion
    //        string notif = r.createMerchantSignatureNotif(CLAVESECRETA, Ds_MerchantParameters);
    //        tw.WriteLine("generacion de deco y notif");


    //        if (notif == Ds_Signature && notif != "")
    //        {
    //            tw.WriteLine("dentro del if");

    //            PaymentData paymentData = JsonConvert.DeserializeObject<PaymentData>(deco);
    //            tw.WriteLine("despues de conversion de paymentData");

    //            string idTicket = string.Join("", paymentData.Ds_Order.TakeWhile(c => !char.IsLetter(c))); // "22"
    //            tw.WriteLine("obtencion de ticket id");
                    
    //            BDTickets _bdTickets = new BDTickets();
    //            Ticket _ticket = _bdTickets.GetByID(int.Parse(idTicket));
    //            tw.WriteLine("obtencion del ticket");

    //            _ticket.pagado = true;
                
    //            // Combinar las cadenas de fecha y hora
    //            string dateTimeString = paymentData.Ds_Date +" "+ paymentData.Ds_Hour;

    //            // Formato esperado
    //            string dateFormat = "dd/MM/yyyy HH:mm";
    //            tw.WriteLine("formateo antes de la fecha");

    //            _ticket.fechaPago = DateTime.ParseExact(dateTimeString, dateFormat, CultureInfo.InvariantCulture);
    //            tw.WriteLine("formateo del ticket");

    //            _bdTickets.Update(_ticket);
    //            tw.WriteLine("actualizacion del ticket");

    //            respuesta.Estado = 1;
    //        }
    //        else
    //        {
    //            respuesta.Estado = 0;
    //        }
    //    }
    //    catch
    //    {
    //        tw.Close();
    //        respuesta.Estado = -1;
    //    }
    //    tw.Close();


    //    respuesta.Mensaje = "confirmacion de pago";

    //    return respuesta;
    //}

}
