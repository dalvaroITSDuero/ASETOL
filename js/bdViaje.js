/**
 * Crea un viaje para el usuario actual con el ticket y el turno dados.
 * @param {number} ticket - Id del ticket.
 * @param {string} turno - Turno del viaje.
 * @param {function} func - Función que se llamará cuando se reciba la respuesta del servidor.
 * La función debe recibir un objeto con la información del viaje.
 */
async function CreateViaje(ticket, turno, func) {
    // console.log(dni);
    var soapMessage = '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">\
  <soap:Body>\
    <CreateViaje xmlns="http://tempuri.org/">\
      <id>'+ getCookie("usuario") + '</id>\
      <token>'+ getCookie("token") + '</token>\
      <idTicket>'+ ticket + '</idTicket>\
      <turno>' + turno + '</turno>\
    </CreateViaje>\
  </soap:Body>\
</soap:Envelope>';
    // console.log(soapMessage);
    $.ajax({
        type: 'POST',
        url: url + 'WebServiceViajes.asmx?op=CreateViaje',
        data: soapMessage,
        contentType: 'text/xml; charset=utf-8',
        success: function (data) {
            var contenido = formateoEstado(data, "CreateViajeResult");
            func(contenido);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            Spinner.hide();
        }

    });
}

/**
 * Obtiene el viaje de hoy para el usuario actual.
 * @param {function} func Función que se llamará cuando se reciba la respuesta del servidor.
 * La función debe recibir un objeto con la información del viaje.
 */
async function getViajePorFecha(func) {
    // console.log(dni);
    var soapMessage = '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">\
  <soap:Body>\
    <GetViajeHoy xmlns="http://tempuri.org/">\
      <id>'+ getCookie("usuario") + '</id>\
      <token>'+ getCookie("token") + '</token>\
    </GetViajeHoy>\
  </soap:Body>\
</soap:Envelope>';
    // console.log(soapMessage);
    $.ajax({
        type: 'POST',
        url: url + 'WebServiceViajes.asmx?op=GetViajeHoy',
        data: soapMessage,
        contentType: 'text/xml; charset=utf-8',
        success: function (data) {
            var contenido = formateoViaje(data);
            func(contenido);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            Spinner.hide();
        }

    });
}

/**
 * Formatea el XML recibido de la petición GetViajeHoy en un objeto Viaje.
 * 
 * @param {XMLDocument} xml Documento XML con la respuesta de la petición
 * @returns {Viaje|null} El viaje, o null si falla
 */
function formateoViaje(xml) {
    try {
        var usuario = new Usuario();
        var viaje = new Viaje();
        // Extraer el nodo raíz (soap:Envelope)
        const envelope = xml.documentElement;

        // Extraer el primer elemento hijo (soap:Body)
        const body = envelope.firstElementChild;

        // Extraer información específica dentro de Body
        const UserNameRepeatResult = body.querySelector("GetViajeHoyResponse");
        if (UserNameRepeatResult) {
            viaje.estado = UserNameRepeatResult.querySelector('Estado')?.textContent || "Sin estado";
            if (viaje.estado === "-1" || viaje.estado === "0") {
                return viaje;
            }
            const viajeidentificado = UserNameRepeatResult.querySelector('viajes');
            const viajeUsuario = UserNameRepeatResult.querySelector('ViajeUsuario');
            const viajeTicket = UserNameRepeatResult.querySelector('TViajeTicket');
            const viajeUbicacion = UserNameRepeatResult.querySelector('ViajeUbicacion');
            const viajeConductor = UserNameRepeatResult.querySelector('ViajeConductor');
            viaje.id = viajeidentificado.querySelector("ViajeID").textContent;
            viaje.usuario = viajeUsuario.querySelector("ID").textContent;
            viaje.ticket = viajeTicket.querySelector("TicketID").textContent;
            viaje.turno = viajeidentificado.querySelector("ViajeTurno").textContent;
            viaje.ubicacion = viajeUbicacion.querySelector("UbicacionID").textContent;
            viaje.fecha = viajeidentificado.querySelector("ViajeFecha").textContent;
            if (viajeConductor == null) {
                viaje.conductor = null;
                return viaje;
            }
            viaje.conductor = viajeConductor.querySelector("ID").textContent;
            return viaje;

        } else {
            console.warn("No se encontró 'GetViajeHoyResponse'");
            Spinner.hide();
            return null;
        }
    } catch (error) {
        console.error("Error procesando el XML:", error);
        Spinner.hide();
        return null;
    }
}

/**
 * Actualiza el viaje de un conductor en la base de datos.
 * @param {string} viaje - Cadena que contiene el id del viaje y el id del conductor, separados por un guión.
 * @param {function} func - Función que se llamará cuando se reciba la respuesta del servidor.
 * La función debe recibir un objeto con la información del viaje.
 */
async function updateViaje(viaje, func) {
    // console.log(viaje);
    viaje = viaje.split('-');
    var soapMessage = '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">\
  <soap:Body>\
    <UpdateDriverTravel xmlns="http://tempuri.org/">\
      <id>'+ getCookie("usuario") + '</id>\
      <token>'+ getCookie("token") + '</token>\
      <viaje>'+ viaje[0] + '</viaje>\
    </UpdateDriverTravel>\
  </soap:Body>\
</soap:Envelope>';
    // console.log(soapMessage);
    $.ajax({
        type: 'POST',
        url: url + 'WebServiceViajes.asmx?op=UpdateDriverTravel',
        data: soapMessage,
        contentType: 'text/xml; charset=utf-8',
        success: function (data) {
            var contenido = formateoUpdateViaje(data);
            func(contenido);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            Spinner.hide();
        }

    });
}

/**
 * Procesa el XML de respuesta de actualización de viaje y extrae información relevante.
 * 
 * @param {XMLDocument} xml - Documento XML con la respuesta de la petición.
 * @returns {Object|null} Un objeto que incluye el estado, mensaje y datos del usuario,
 *                        o null si ocurre un error durante el procesamiento.
 */
function formateoUpdateViaje(xml) {
    try {
        var datos = new Object();
        var usuar = new Usuario();
        // Extraer el nodo raíz (soap:Envelope)
        const envelope = xml.documentElement;

        // Extraer el primer elemento hijo (soap:Body)
        const body = envelope.firstElementChild;
        // Extraer información específica dentro de Body
        const UserNameRepeatResult = body.querySelector("UpdateDriverTravelResponse");
        if (UserNameRepeatResult) {
            datos.estado = UserNameRepeatResult.querySelector('Estado')?.textContent || "Sin estado";
            if (datos.estado === "-1" || datos.estado === "0") {
                datos.mensaje = UserNameRepeatResult.querySelector('Mensaje').textContent;
                // return datos;
            }
            const usuario = UserNameRepeatResult.querySelector('usuario');
            usuar.id = usuario.querySelector("ID").textContent;
            usuar.nombre = usuario.querySelector("UsuarioNombre").textContent;
            usuar.apellidos = usuario.querySelector("UsuarioApellidos").textContent;
            datos.usuario = usuar;
            console.log(usuar.id);
            return datos;

        } else {
            console.warn("No se encontró 'UpdateDriverTravel'");
            Spinner.hide();
            return null;
        }
    } catch (error) {
        console.error("Error procesando el XML:", error);
        Spinner.hide();
        return null;
    }
}

/**
 * Obtiene todos los viajes para el usuario actual desde la base de datos.
 * 
 * @param {function} func - Función que se llamará cuando se reciba la respuesta del servidor.
 * La función debe recibir un array de objetos con la información de los viajes.
 * @param {boolean} [conductor=false] - Indica si se deben obtener solo los viajes del conductor.
 * 
 * Envía una solicitud al servicio web para obtener los viajes y llama a la función proporcionada
 * con el resultado formateado.
 */
async function getViajesById(func, conductor = false) {
    // console.log(func, conductor);
    var soapMessage = '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">\
  <soap:Body>\
    <GetViajesById xmlns="http://tempuri.org/">\
      <id>'+ getCookie("usuario") + '</id>\
      <token>'+ getCookie("token") + '</token>\
      <conductor>'+ conductor + '</conductor>\
    </GetViajesById>\
  </soap:Body>\
</soap:Envelope>';
    $.ajax({
        type: 'POST',
        url: url + 'WebServiceViajes.asmx?op=GetViajesById',
        data: soapMessage,
        contentType: 'text/xml; charset=utf-8',
        success: function (data) {
            var contenido = formateoViajes(data, 'GetViajesByIdResponse');
            func(contenido, conductor);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            Spinner.hide();
        }

    });
}


/**
 * Formatea el XML recibido de la petición GetViajesById en un arreglo de objetos con la información de cada viaje.
 * 
 * @param {XMLDocument} xml - Documento XML con la respuesta de la petición.
 * @returns {Array<Object>|null} Un arreglo de objetos con la información de cada viaje, o null si ocurre un error.
 */
async function formateoViajes(xml, ruta) {
    try {
        var viajes = [];
        var estado;
        // Extraer el nodo raíz (soap:Envelope)
        const envelope = xml.documentElement;

        // Extraer el primer elemento hijo (soap:Body)
        const body = envelope.firstElementChild;

        // Extraer información específica dentro de Body
        const UserNameRepeatResult = body.querySelector(ruta);
        if (UserNameRepeatResult) {
            estado = UserNameRepeatResult.querySelector('Estado').textContent;
            if (estado === "-1" || estado === "0") {
                // console.warn("Estado indica error:", estado);
                return viajes;
            }
            const viajesSer = UserNameRepeatResult.querySelectorAll("viajes > Viaje");
            // console.log(viajesSer);
            viajesSer.forEach((viaj) => {

                let ubicacion = viaj.querySelector("ViajeUbicacion");
                let ticket = viaj.querySelector("TViajeTicket");
                const viaje = {
                    id: viaj.querySelector("ViajeID")?.textContent || "Sin ID",
                    usuario: formateoUsuarioIndividual(viaj.querySelector("ViajeUsuario")),
                    ticket: ticket.querySelector("TicketID")?.textContent || "Sin ticket",
                    turno: viaj.querySelector("ViajeTurno")?.textContent || "Sin turno",
                    ubicacion: ubicacion.querySelector("UbicacionDescripcion")?.textContent || "Sin ubicación",
                    fecha: viaj.querySelector("ViajeFecha")?.textContent || "Sin fecha",
                    conductor: (viaj.querySelector("ViajeConductor") != null ? formateoUsuarioIndividual(viaj.querySelector("ViajeConductor")) : null),
                };
                viajes.push(viaje)
            });
            return viajes;

        } else {
            console.warn("No se encontró " + ruta);
            Spinner.hide();
            return null;
        }
    } catch (error) {
        console.error("Error procesando el XML:", error);
        Spinner.hide();
        return null;
    }
}



/**
 * Obtiene todos los viajes filtrados por usuario, turno y fecha desde la base de datos.
 * 
 * @param {function} func - Función que se llamará cuando se reciba la respuesta del servidor.
 *                          La función debe recibir un array de objetos con la información de los viajes.
 * @param {string} usuario - Id del usuario para el cual se filtran los viajes.
 * @param {string} turno - Turno específico para filtrar los viajes.
 * @param {string} fecha - Fecha específica para filtrar los viajes.
 * 
 * Envía una solicitud al servicio web para obtener los viajes filtrados y llama a la función proporcionada
 * con el resultado formateado.
 */
async function getAllTravelsFilter(func, usuario, turno, fecha) {
    // console.log(func, usuario, turno, fecha);
    var soapMessage = '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">\
  <soap:Body>\
    <GetAllTravelsFilter xmlns="http://tempuri.org/">\
      <id>'+ getCookie("usuario") + '</id>\
      <token>'+ getCookie("token") + '</token>\
      <usuario>'+ usuario + '</usuario>\
      <turno>'+ turno + '</turno>\
      <fecha>'+ fecha + '</fecha>\
    </GetAllTravelsFilter>\
  </soap:Body>\
</soap:Envelope>';
    $.ajax({
        type: 'POST',
        url: url + 'WebServiceViajes.asmx?op=GetAllTravelsFilter',
        data: soapMessage,
        contentType: 'text/xml; charset=utf-8',
        success: function (data) {
            var contenido = formateoViajes(data, "GetAllTravelsFilterResponse");
            func(contenido);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            Spinner.hide();
        }

    });
}