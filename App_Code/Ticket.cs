using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace es.itsduero.iecscyl
{
    [DataContract]
    public class Ticket
    {
        [XmlElement ("TicketID")]
        public int ID { get; set; }
        [XmlElement("TicketUsuario")]
        public Usuario usuario { get; set; }
        [XmlElement("TicketFechaInicio")]
        public DateTime fechaInicio { get; set; }
        [XmlElement("TicketFechaFin")]
        public DateTime fechaFin { get; set; }
        [XmlElement("TicketPagado")]
        public bool pagado { get; set; }
        [XmlElement("TicketFechaPago")]
        public DateTime fechaPago { get; set; }
        [XmlElement("TicketImporte")]
        public Double importe { get; set; }
    }
}