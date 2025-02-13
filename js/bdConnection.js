const url = 'https://autobusesrubio.gestionando.online/';


/**
 * Realiza la petición de inicio de sesión al servidor.
 * E inicializa la sesión del usuario si la petición es exitosa mediante cookies.
 * @param {string} dni - DNI del usuario que se quiere loguear.
 * @param {string} password - Contraseña del usuario que se quiere loguear.
 * @param {boolean} recordar - Indica si se quiere recordar el usuario.
 * @returns {Promise<void>}
 * @throws {Error} Si la petición al servidor falla.
 */
async function entradaLogin(dni, password, recordar) {
    Spinner.show();
    var soapMessage = '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body>\
    <Identificacion xmlns="http://tempuri.org/">\
      <usuario>'+ dni + '</usuario>\
      <password>'+ password + '</password>\
    </Identificacion>\
  </soap:Body>\
</soap:Envelope>';
    $.ajax({
        type: 'POST',
        url: url + 'WebServiceLogin.asmx?op=Identificacion',
        data: soapMessage,
        contentType: 'text/xml; charset=utf-8',
        success: function (data) {
            var usuario = formateoLogin(data);
            // console.log(usuario)
            if (usuario.estado === "1") {
                if (recordar) {
                    setCookie("usuario", usuario.id, 30);
                    setCookie("token", usuario.token, 30);
                    setCookie("tipo", usuario.permisos, 30);
                    setCookie("ticket", usuario.tipoTicket, 30);
                } else {
                    setCookie("usuario", usuario.id, 1);
                    setCookie("token", usuario.token, 1);
                    setCookie("tipo", usuario.permisos, 1);
                    setCookie("ticket", usuario.tipoTicket, 1);
                }
                usuario.permisos === "2" ? window.location.href = "pasajero/indexPasajero.html" : window.location.href = "conductor/indexConductor.html";
            } else {
                SweetAlert.alert("Credenciales incorrectas. Intenta nuevamente.");
            }
            Spinner.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            Spinner.hide();
        }

    });
}

/**
 * Verificar si el usuario existe y tiene el token correcto
 * @param {string} id Identificador del usuario
 * @param {string} token Token del usuario
 * @returns {string|null} Estado del usuario, o null si falla
 */
async function getUsuarioByIdToken(id, token, params) {
    // console.log(id, token);
    var soapMessage = '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">\
  <soap:Body>\
    <Verificacion xmlns="http://tempuri.org/">\
      <id>'+ id + '</id>\
      <token>'+ token + '</token>\
    </Verificacion>\
  </soap:Body>\
</soap:Envelope>';
    $.ajax({
        type: 'POST',
        url: url + 'WebServiceLogin.asmx?op=Verificacion',
        data: soapMessage,
        contentType: 'text/xml; charset=utf-8',
        success: function (data) {
            redireccionarCoockies(formateoEstado(data, "VerificacionResult"), params);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            Spinner.hide();
        }

    });
}

/**
 * Extrae el usuario de la respuesta del servidor
 * @param {Document} xml Documento XML con la respuesta del servidor
 * @returns {Usuario|null} Usuario con sus datos, o null si falla
 */

function formateoLogin(xml) {
    try {
        // Extraer el nodo raíz (soap:Envelope)
        const envelope = xml.documentElement;

        // Extraer el primer elemento hijo (soap:Body)
        const body = envelope.firstElementChild;

        // Extraer información específica dentro de Body
        const identificacionResult = body.querySelector('IdentificacionResult');
        if (identificacionResult) {
            var usuario = new Usuario();
            // console.log(identificacionResult);
            usuario.estado = identificacionResult.querySelector('Estado')?.textContent || "Sin estado";
            if (usuario.estado === "0") {
                return usuario;
            }
            usuario.token = identificacionResult.querySelector('Token')?.textContent || "Sin token";

            const usuarioidentificado = identificacionResult.querySelector('usuario');
            usuario.id = usuarioidentificado.querySelector('ID')?.textContent || '';
            const permisosUsuario = usuarioidentificado.querySelector('UsuarioPermisos');
            usuario.permisos = permisosUsuario.querySelector('PermisosID')?.textContent || "Sin permisos";
            usuario.tipoTicket = identificacionResult.querySelector('UsuarioTipoTicket')?.textContent || "Sin tipoTicket";
            usuario.tipoTicket = Number.isFinite(Number(usuario.tipoTicket)) ?String.fromCharCode(usuario.tipoTicket): usuario.tipoTicket;
            return usuario;
        } else {
            console.warn("No se encontró 'IdentificacionResult'");
            return null;
        }
    } catch (error) {
        console.error("Error procesando el XML:", error);
        Spinner.hide();
        return null;
    }
}