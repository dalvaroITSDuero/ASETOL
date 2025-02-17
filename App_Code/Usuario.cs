using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace es.itsduero.iecscyl
{
    [DataContract]
    public class Usuario
    {
        [XmlElement ("ID")]
        public int ID { get; set; }
        [XmlElement("UsuarioNombre")]
        public string nombre { get; set; }
        [XmlElement("UsuarioApellidos")]
        public string apellidos { get; set; }
        [XmlElement("UsuarioDni")]
        public string dni { get; set; }
        [XmlElement("UsuarioPass")]
        public string password { get; set; }
        [XmlElement("UsuarioTelefono")]
        public int telefono { get; set; }
        [XmlElement("UsuarioToken")]
        public string token { get; set; }
        [XmlElement("UsuarioPermisos")]
        public Permisos permisos { get; set; } // 1 admin, 2 usuario normal, 3 conductor
        [XmlElement("UsuarioEmail")]
        public string email { get; set; }
        [XmlElement("UsuarioEmpresa")]
        public Empresa empresa { get; set; }
        [XmlElement("UsuarioActivo")]
        public bool activo { get; set; }
        [XmlElement("UsuarioUbicacion")]
        public Ubicacion ubicacion { get; set; }
        [XmlElement("UsuarioTipoTicket")]
        public char tipoTicket { get; set; } // D = diario, M = mensual, Z = null
        [XmlElement("UsuarioImgUser")]
        public string imgUser { get; set; }

        public override string ToString()
        {
            return base.ToString()+": "+
                " id: " + ID + " nombre: " + nombre + " apellidos: " + apellidos + " dni: " + dni + 
                " password: " +password + " telefono: "+telefono +" token: "+ token + " permisos: "+ permisos + 
                " email: "+email + " empresa: " +empresa + " activo: "+ activo + " ubicacion: "+ubicacion +" tipoTicket: "+ tipoTicket;
        }
    }

}