using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

/// <summary>
/// Descripción breve de Empresa
/// </summary>
namespace es.itsduero.iecscyl
{
    [DataContract]
    public class Empresa
    {
        [XmlElement ("EmpresaID")]
        public int ID { get; set; }
        [XmlElement("EmpresaRazonSocial")]
        public string razonSocial { get; set; }
        [XmlElement("EmpresaNombreComercial")]
        public string nombreComercial { get; set; }
        [XmlElement("EmpresaActivo")]
        public bool activo { get; set; }
    }
}