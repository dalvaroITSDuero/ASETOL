
/**
 * Obtiene todas las ubicaciones de la base de datos.
 * @param {function} func Función que se ejecuta cuando se recibe la respuesta del servidor.
 * La función debe recibir un array de objetos Ubicacion.
 */
async function getUbicaciones(func) {
    // console.log('get ubicaciones');
    var soapMessage = '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">\
  <soap:Body>\
    <GetAll xmlns="http://tempuri.org/">\
      <id>'+ getCookie("usuario") + '</id>\
      <token>'+ getCookie("token") + '</token>\
      </GetAll>\
  </soap:Body>\
</soap:Envelope>';
    $.ajax({
        type: 'POST',
        url: url + 'WebServiceUbicaciones.asmx?op=GetAll',
        data: soapMessage,
        contentType: 'text/xml; charset=utf-8',
        success: function (data) {
            var contenido = formateoUbicaciones(data, 'UserNameRepeatResult');
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
async function formateoUbicaciones(xml) {
    try {
        var ubicaciones = [];
        var estado;
        // Extraer el nodo raíz (soap:Envelope)
        const envelope = xml.documentElement;

        // Extraer el primer elemento hijo (soap:Body)
        const body = envelope.firstElementChild;

        // Extraer información específica dentro de Body
        const UserNameRepeatResult = body.querySelector("GetAllResponse");
        if (UserNameRepeatResult) {
            estado = UserNameRepeatResult.querySelector('Estado').textContent;
            if (estado === "-1" || estado === "0") {
                // console.warn("Estado indica error:", estado);
                return ubicaciones;
            }
            const ubicacionesSer = UserNameRepeatResult.querySelectorAll("ubicaciones > Ubicacion");
            ubicacionesSer.forEach((ubi) => {
                const ubicacion = {
                    id: ubi.querySelector("UbicacionID")?.textContent || "Sin ID",
                    descripcion: ubi.querySelector("UbicacionDescripcion")?.textContent || "Sin descripción",
                };
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
