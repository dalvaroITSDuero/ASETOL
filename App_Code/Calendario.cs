using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace es.itsduero.iecscyl
{
    [DataContract]
    public class Calendario
    {
        [XmlElement ("CalendarioID")]
        public int ID { get; set; }
        [XmlElement("CalendarioUsuario")]
        public Usuario usuario { get; set; }
        [XmlElement("CalendarioFecha")]
        public DateTime fecha { get; set; }
        [XmlElement("CalendarioTurno")]
        public short turno { get; set; } //0 descanso, 1 mañanas, 2 tardes, 3 noches
        public bool haveTicket { get; set; }
    }
}