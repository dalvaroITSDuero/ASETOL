using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;
using es.itsduero.iecscyl;

namespace es.itsduero.iecscyl
{
	public class Log
	{
        public DateTime fecha { get; set; }
        public Usuario usuario { get; set; }
        public string accion { get; set; }
        public string IP { get; set; }
	}
}