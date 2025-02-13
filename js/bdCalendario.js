/**
 * Verificar si el DNI/NIF ya esta registrado en la base de datos
 * 
 * @param {string} dni DNI/NIF a verificar
 * @returns {undefined}
 */
async function updateOrCreateCalendar(fecha, turno) {
    // console.log(dni);
    var soapMessage = '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">\
  <soap:Body>\
    <UpdateOrCreateCalendar xmlns="http://tempuri.org/">\
      <id>'+ getCookie("usuario") + '</id>\
      <token>'+ getCookie("token") + '</token>\
      <fecha>'+ fecha + '</fecha>\
      <turno>'+ turno + '</turno>\
    </UpdateOrCreateCalendar>\
  </soap:Body>\
</soap:Envelope>';
    $.ajax({
        type: 'POST',
        url: url + 'WebServiceCalendarios.asmx?op=UpdateOrCreateCalendar',
        data: soapMessage,
        contentType: 'text/xml; charset=utf-8',
        success: function (data) {
            var contenido = formateoEstado(data, "UpdateOrCreateCalendarResult");
            Spinner.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            Spinner.hide();
        }

    });
}

/**
 * Verificar si el DNI/NIF ya esta registrado en la base de datos
 * 
 * @param {string} dni DNI/NIF a verificar
 * @returns {undefined}
 */
async function deleteCalendar(fecha) {
    // console.log(dni);
    var soapMessage = '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">\
  <soap:Body>\
    <DeleteCalendar xmlns="http://tempuri.org/">\
      <id>'+ getCookie("usuario") + '</id>\
      <token>'+ getCookie("token") + '</token>\
      <fecha>'+ fecha + '</fecha>\
    </DeleteCalendar>\
  </soap:Body>\
</soap:Envelope>';
    $.ajax({
        type: 'POST',
        url: url + 'WebServiceCalendarios.asmx?op=UpdateOrCreateCalendar',
        data: soapMessage,
        contentType: 'text/xml; charset=utf-8',
        success: function (data) {
            var contenido = formateoEstado(data, "DeleteCalendarResult");
            Spinner.hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            Spinner.hide();
        }

    });
}


/**
 * Verificar si el DNI/NIF ya esta registrado en la base de datos
 * 
 * @param {string} dni DNI/NIF a verificar
 * @returns {undefined}
 */
async function getCalendar(func) {
    // console.log(dni);
    var soapMessage = '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">\
  <soap:Body>\
    <GetAllById xmlns="http://tempuri.org/">\
      <id>'+ getCookie("usuario") + '</id>\
      <token>'+ getCookie("token") + '</token>\
    </GetAllById>\
  </soap:Body>\
</soap:Envelope>';
    $.ajax({
        type: 'POST',
        url: url + 'WebServiceCalendarios.asmx?op=UpdateOrCreateCalendar',
        data: soapMessage,
        contentType: 'text/xml; charset=utf-8',
        success: function (data) {
            var contenido = formatCal(data);
            func(contenido);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            Spinner.hide();
        }

    });
}

/**
 * Formatea el XML recibido de la petición GetCalendar en un arreglo de objetos con la información de cada calendario.
 * 
 * @param {XMLDocument} xml Documento XML con la respuesta de la petición
 * @returns {Promise<Array<{id: string, fecha: string, turno: string}>>} Un arreglo de objetos con la información de cada calendario
 */
async function formatCal(xml) {
    try {
        var calendarios = [];
        var estado;
        // Extraer el nodo raíz (soap:Envelope)
        const envelope = xml.documentElement;

        // Extraer el primer elemento hijo (soap:Body)
        const body = envelope.firstElementChild;

        // Extraer información específica dentro de Body
        const UserNameRepeatResult = body.querySelector("GetAllByIdResponse");
        if (UserNameRepeatResult) {
            estado = UserNameRepeatResult.querySelector('Estado').textContent;
            if (estado === "-1" || estado === "0") {
                // console.warn("Estado indica error:", estado);
                return calendarios;
            }
            const calendariosUsuario = UserNameRepeatResult.querySelectorAll("Calendarios > Calendario");
            calendariosUsuario.forEach((calendario) => {
                calendario = {
                    id: calendario.querySelector("CalendarioID")?.textContent || "Sin ID",
                    fecha: calendario.querySelector("CalendarioFecha")?.textContent || "Sin fecha",
                    turno: calendario.querySelector("CalendarioTurno")?.textContent || "Sin turno",
                };
                calendario.fecha = convertToDateOnly(calendario.fecha);

                calendarios.push(calendario)
            });
            return calendarios;

        } else {
            console.warn("No se encontró 'GetUserByIdResult'");
            Spinner.hide();
            return null;
        }
    } catch (error) {
        console.error("Error procesando el XML:", error);
        Spinner.hide();
        return null;
    }
}