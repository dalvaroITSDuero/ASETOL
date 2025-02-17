using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

/// <summary>
/// Descripción breve de Permisos
/// </summary>

namespace es.itsduero.iecscyl
{
    [DataContract]
    public class Permisos
    {
        [XmlElement ("PermisosID")]
        public int ID { get; set; }
        [XmlIgnore]
        public string descripcion { get; set; }
    }
}