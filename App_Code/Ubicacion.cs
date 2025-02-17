using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

/// <summary>
/// Descripción breve de Ubicacion
/// </summary>
namespace es.itsduero.iecscyl
{
    [DataContract]
    public class Ubicacion
    {
        [XmlElement ("UbicacionID")]
        public int ID { get; set; }
        [XmlElement ("UbicacionDescripcion")]
        public string descripcion { get; set; }
    }
}