
/**
 * Obtiene todas las empresas de la base de datos.
 * @param {function} func Función que se ejecuta cuando se recibe la respuesta del servidor.
 * La función debe recibir un array de objetos Empresa.
 */
async function getEmpresas(func) {
    // console.log('get empresas');
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
        url: url + 'WebServiceEmpresas.asmx?op=GetAll',
        data: soapMessage,
        contentType: 'text/xml; charset=utf-8',
        success: function (data) {
            var contenido = formateoEmpresa(data, 'UserNameRepeatResult');
            func(contenido);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            Spinner.hide();
        }

    });
}


/**
 * Procesa un documento XML de respuesta del servidor para extraer la información de empresas.
 * @param {Document} xml - Documento XML con la respuesta del servidor.
 * @returns {Array<Object>|null} Un array de objetos que representan empresas, o null si ocurre un error.
 * Cada objeto de empresa contiene los campos: id, razonSocial, nombreComercial, y activo.
 */
async function formateoEmpresa(xml) {
    try {
        var empresas = [];
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
                console.warn("Estado indica error:", estado);
                return empresa;
            }
            const empresasUsuario = UserNameRepeatResult.querySelectorAll("empresas > Empresa");
            empresasUsuario.forEach((empr) => {
                const empresa = {
                    id: empr.querySelector("EmpresaID")?.textContent || "Sin ID",
                    razonSocial: empr.querySelector("EmpresaRazonSocial")?.textContent || "Sin razón social",
                    nombreComercial: empr.querySelector("EmpresaNombreComercial")?.textContent || "Sin nombre comercial",
                    activo: empr.querySelector("EmpresaActivo")?.textContent === "true",
                };
                empresas.push(empresa)
            });
            return empresas;

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
