
/**
 * Obtiene todos los tickets de la base de datos.
 * @param {function} func Función que se ejecuta cuando se recibe la respuesta del servidor.
 * La función debe recibir un array de objetos Ubicacion.
 */
async function getTicketsByIdUsuario(func) {
    // console.log('get ubicaciones');
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
        url: url + 'WebServiceTickets.asmx?op=GetAllById',
        data: soapMessage,
        contentType: 'text/xml; charset=utf-8',
        success: function (data) {
            var contenido = formatTickets(data);
            func(contenido);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            Spinner.hide();
        }

    });
}


/**
 * Extrae las ubicaciones de la respuesta del servidor
 * @param {Document} xml Documento XML con la respuesta del servidor
 * @returns {Ubicacion[]} Array de objetos Ubicacion, o null si falla
 */
async function formatTickets(xml) {
    try {
        var ubicaciones = [];
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
                return ubicaciones;
            }
            const ubicacionesSer = UserNameRepeatResult.querySelectorAll("Tickets > Ticket");
            ubicacionesSer.forEach((ubi) => {
                const ubicacion = {
                    id: ubi.querySelector("TicketID")?.textContent || "Sin ID",
                    fechaInicio: ubi.querySelector("TicketFechaInicio")?.textContent || "Sin fecha",
                    fechaFin: ubi.querySelector("TicketFechaFin")?.textContent || "Sin fecha",
                    pagado: ubi.querySelector("TicketPagado")?.textContent || "Sin pago",
                    fechaPago: ubi.querySelector("TicketFechaPago")?.textContent || "Sin fecha",
                    importe: ubi.querySelector("TicketImporte")?.textContent || "Sin importe",
                };
                ubicacion.fechaInicio = convertToDateOnly(ubicacion.fechaInicio);
                ubicacion.fechaFin = convertToDateOnly(ubicacion.fechaFin);
                ubicacion.fechaPago = convertToDateOnly(ubicacion.fechaPago);
                ubicaciones.push(ubicacion)
            });
            return ubicaciones;

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

/**
 * Realiza el pago de un ticket.
 * @param {number} idTicket - Id del ticket a pagar.
 * @param {function} func - Función que se llamará cuando se reciba la respuesta del servidor.
 * La función debe recibir un objeto con la información del pago.
 */
async function pago(idTicket, func) {
    // console.log('get ubicaciones');
    Spinner.show();
    var soapMessage = '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">\
  <soap:Body>\
    <DatosPay xmlns="http://tempuri.org/">\
      <id>'+ getCookie("usuario") + '</id>\
      <token>'+ getCookie("token") + '</token>\
      <idTicket>'+ idTicket + '</idTicket>\
    </DatosPay>\
  </soap:Body>\
</soap:Envelope>';
// console.log(soapMessage);
    $.ajax({
        type: 'POST',
        url: url + 'WebServicePago.asmx?op=DatosPay',
        data: soapMessage,
        contentType: 'text/xml; charset=utf-8',
        success: function (data) {
            var contenido = formateoPago(data);
            func(contenido);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            Spinner.hide();
        }

    });
}

/**
 * Extrae la información de un ticket de la respuesta del servidor
 * @param {Document} xml Documento XML con la respuesta del servidor
 * @returns {{estado: string, Ds_Merchant_MerchantParameters: string, Ds_Merchant_MerchantSignature: string}|null} Información del ticket, o null si falla
 */
function formateoPago(xml) {
    try {
        var datos = new Object();
        // Extraer el nodo raíz (soap:Envelope)
        const envelope = xml.documentElement;

        // Extraer el primer elemento hijo (soap:Body)
        const body = envelope.firstElementChild;

        // Extraer información específica dentro de Body
        const UserNameRepeatResult = body.querySelector("DatosPayResult");
        if (UserNameRepeatResult) {
            datos.estado = UserNameRepeatResult.querySelector('Estado')?.textContent || "Sin estado";
            if (datos.estado === "-1" || datos.estado === "0") {
                return datos;
            }
            datos.Ds_Merchant_MerchantParameters = UserNameRepeatResult.querySelector("Ds_Merchant_MerchantParameters").textContent;
            datos.Ds_Merchant_MerchantSignature = UserNameRepeatResult.querySelector("Ds_Merchant_MerchantSignature").textContent;
            return datos;

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

async function pagoRequest(Ds_Signature, Ds_MerchantParameters) {
    // console.log('get ubicaciones');
    Spinner.show();
    var soapMessage = '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">\
  <soap:Body>\
    <ConfirmarPago xmlns="http://tempuri.org/">\
      <id>'+ getCookie("usuario") + '</id>\
      <token>'+ getCookie("token") + '</token>\
      <Ds_Signature>'+Ds_Signature+'</Ds_Signature>\
      <Ds_MerchantParameters>'+Ds_MerchantParameters+'</Ds_MerchantParameters>\
    </ConfirmarPago>\
  </soap:Body>\
</soap:Envelope>';
console.log(soapMessage);
    $.ajax({
        type: 'POST',
        url: url + 'WebServicePago.asmx?op=ConfirmarPago',
        data: soapMessage,
        contentType: 'text/xml; charset=utf-8',
        success: function (data) {
        },
        error: function (jqXHR, textStatus, errorThrown) {
            Spinner.hide();
        }

    });
}