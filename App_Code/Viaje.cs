using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace es.itsduero.iecscyl
{
    [DataContract]
    public class Viaje
    {
        [XmlElement("ViajeUsuario")]
        public Usuario usuario { get; set; }
        [XmlElement("TViajeTicket")]
        public Ticket ticket { get; set; }
        [XmlElement("ViajeID")]
        public int ID { get; set; }
        [XmlElement("ViajeTurno")]
        public short turno { get; set; } //0 descanso, 1 mañanas, 2 tardes, 3 noches
        [XmlElement("ViajeUbicacion")]
        public Ubicacion ubicacion { get; set; }
        [XmlElement("ViajeFecha")]
        public DateTime fecha { get; set; }
        [XmlElement("ViajeConductor")]
        public Usuario conductor { get; set; }
    }
}