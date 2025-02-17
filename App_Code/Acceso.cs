using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;
using es.itsduero.iecscyl.data;

namespace es.itsduero.iecscyl
{
    public class Acceso
    {

        /// <summary>
        /// Devuelve el Usuario identificado
        /// </summary>
        /// <param name="cookies">Colección de cookies del navegador del usuario</param>
        /// <returns>Devuelve el usuario identificado</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public Usuario identificarUsuario(HttpCookieCollection cookies)
        {
            Usuario _usuario;

            try
            {
                // Obtenemos el valor de las cookies de usuario y token
                HttpCookie cookieUsuario = cookies[ConfigurationManager.AppSettings["nameCookie"] + "_usuario"];
                HttpCookie cookieToken = cookies[ConfigurationManager.AppSettings["nameCookie"] + "_token"];

                _usuario = new BDUsuario().GetByID(short.Parse(cookieUsuario.Value));
                if (!_usuario.token.Equals(cookieToken.Value))
                {
                    _usuario = null;
                }
            }
            catch
            {
                _usuario = null;
            }

            return _usuario;
        }


        /// <summary>
        /// Devuelve el Usuario identificado
        /// </summary>
        /// <param name="id">ID externo del Usuario</param>
        /// <param name="token">Token de la sesión del Usuario</param>
        /// <returns>Devuelve el Usuario identificado</returns>
        /// <autor>dalvaro@itsduero.es</autor>
        public Usuario identificarUsuario(short id, string token)
        {
            Usuario _usuario;

            try
            {
                _usuario = new BDUsuario().GetByID(id);
                if (!_usuario.token.Equals(token))
                {
                    _usuario = null;
                }
            }
            catch
            {
                _usuario = null;
            }

            return _usuario;
        }

    }
}