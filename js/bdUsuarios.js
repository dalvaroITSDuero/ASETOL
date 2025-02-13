/**
 * Verificar si el DNI/NIF ya esta registrado en la base de datos
 * 
 * @param {string} dni DNI/NIF a verificar
 * @returns {undefined}
 */
async function usuarioRepeat(dni) {
    // console.log(dni);
    var soapMessage = '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">\
  <soap:Body>\
    <UserNameRepeat xmlns="http://tempuri.org/">\
      <userName>'+ dni + '</userName>\
    </UserNameRepeat>\
  </soap:Body>\
</soap:Envelope>';
    $.ajax({
        type: 'POST',
        url: url + 'WebServiceUsuarios.asmx?op=UserNameRepeat',
        data: soapMessage,
        contentType: 'text/xml; charset=utf-8',
        success: function (data) {
            var contenido = formateoEstado(data, 'UserNameRepeatResult');
            repeatDNI(contenido);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            Spinner.hide();
        }

    });
}


/**
 * Extrae el estado de un registro de usuario de la respuesta del servidor
 * @param {Document} xml Documento XML con la respuesta del servidor
 * @returns {string|null} Estado del registro de usuario, o null si falla
 */
function formateoEstado(xml, busqueda) {
    try {
        // Extraer el nodo raíz (soap:Envelope)
        const envelope = xml.documentElement;

        // Extraer el primer elemento hijo (soap:Body)
        const body = envelope.firstElementChild;

        // Extraer información específica dentro de Body
        const UserNameRepeatResult = body.querySelector(busqueda);
        if (UserNameRepeatResult) {
            // console.log(UserNameRepeatResult);
            return UserNameRepeatResult.querySelector('Estado')?.textContent || "Sin estado";
        } else {
            console.warn("No se encontró '" + busqueda + "'");
            return null;
        }
    } catch (error) {
        Spinner.hide();
        console.error("Error procesando el XML:", error);
        return null;
    }
}


/**
 * Crea un nuevo usuario en la base de datos
 * @param {string} dni DNI del usuario a crear
 * @param {string} nombre Nombre del usuario a crear
 * @param {string} apellidos Apellidos del usuario a crear
 * @param {string} telefono Teléfono del usuario a crear
 * @param {string} email Email del usuario a crear
 * @param {string} password Contraseña del usuario a crear
 * @returns {Promise<void>} Promesa que se resuelve si el registro es exitoso
 */

/**
 * Crea un nuevo usuario en la base de datos
 * @param {string} dni DNI del usuario a crear
 * @param {string} nombre Nombre del usuario a crear
 * @param {string} apellidos Apellidos del usuario a crear
 * @param {string} telefono Teléfono del usuario a crear
 * @param {string} email Email del usuario a crear
 * @param {string} password Contraseña del usuario a crear
 * @returns {Promise<void>} Promesa que se resuelve si el registro es exitoso
 */
async function usuarioCreate(dni, nombre, apellidos, telefono, email, password) {
    // console.log(dni);
    var soapMessage = '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">\
  <soap:Body>\
    <UserCreate xmlns="http://tempuri.org/">\
      <dni>'+ dni + '</dni>\
      <nombre>'+ nombre + '</nombre>\
      <apellidos>'+ apellidos + '</apellidos>\
      <telefono>'+ telefono + '</telefono>\
      <email>'+ email + '</email>\
      <password>'+ password + '</password>\
    </UserCreate>\
  </soap:Body>\
</soap:Envelope>';
    $.ajax({
        type: 'POST',
        url: url + 'WebServiceUsuarios.asmx?op=UserCreate',
        data: soapMessage,
        contentType: 'text/xml; charset=utf-8',
        success: function (data) {
            successRegister(formateoEstado(data, 'UserCreateResult'));
        },
        error: function (jqXHR, textStatus, errorThrown) {
            Spinner.hide();
            SweetAlert.alert("Ocurrio un error en la creación del usuario");
        }

    });
}

/**
 * Obtiene un objeto usuario por su ID
 * @param {number} id ID del usuario a obtener
 * @returns {Promise<void>} Promesa que se resuelve con la información del usuario
 */
async function getUsuarioById(id, fnc) {
    // console.log(id);
    var soapMessage = '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">\
  <soap:Body>\
    <GetUserById xmlns="http://tempuri.org/">\
      <id>'+ id + '</id>\
      <token>'+ getCookie("token") + '</token>\
    </GetUserById>\
  </soap:Body>\
</soap:Envelope>';
    $.ajax({
        type: 'POST',
        url: url + 'WebServiceUsuarios.asmx?op=UserCreate',
        data: soapMessage,
        contentType: 'text/xml; charset=utf-8',
        success: function (data) {
            fnc(formateoUsuario(data));
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.warn("Ocurrio un error en la recuperacion de un usuario");
            Spinner.hide();
        }

    });
}

/**
 * Extrae la información de un usuario de la respuesta del servidor
 * @param {Document} xml Documento XML con la respuesta del servidor
 * @returns {Usuario} Información del usuario, o null si falla
 */
function formateoUsuario(xml) {
    try {
        var usuario = new Usuario();
        var empresa = new Empresa();
        // Extraer el nodo raíz (soap:Envelope)
        const envelope = xml.documentElement;

        // Extraer el primer elemento hijo (soap:Body)
        const body = envelope.firstElementChild;

        // Extraer información específica dentro de Body
        const UserNameRepeatResult = body.querySelector("GetUserByIdResponse");
        if (UserNameRepeatResult) {
            usuario.estado = UserNameRepeatResult.querySelector('Estado')?.textContent || "Sin estado";
            if (usuario.estado === "-1" || usuario.estado === "0") {
                return usuario;
            }
            const usuarioidentificado = UserNameRepeatResult.querySelector('usuario');
            return formateoUsuarioIndividual(usuarioidentificado);

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
 * Actualiza la información de un usuario en la base de datos
 * @param {string} dni DNI del usuario a actualizar
 * @param {string} nombre Nuevo nombre del usuario
 * @param {string} apellidos Nuevos apellidos del usuario
 * @param {string} telefono Nuevo teléfono del usuario
 * @param {number} ubicacion ID de la ubicación del usuario
 * @param {number} empresa ID de la empresa del usuario
 * @param {string} email Nuevo email del usuario
 * @param {string} [password=''] Nueva contraseña del usuario, si no se proporciona, no se modifica
 * @returns {Promise<void>} Promesa que se resuelve si el registro es exitoso
 */
async function usuarioUpdate(usuario) {
    // console.log(usuario);
    var soapMessage = '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">\
  <soap:Body>\
    <UserUpdate xmlns="http://tempuri.org/">\
      <ID>'+ usuario.id + '</ID>\
      <dni>'+ usuario.dni + '</dni>\
      <nombre>'+ usuario.nombre + '</nombre>\
      <apellidos>'+ usuario.apellidos + '</apellidos>\
      <telefono>'+ usuario.telefono + '</telefono>\
      <ubicacion>'+ usuario.ubicacion + '</ubicacion>\
      <empresa>'+ usuario.empresa + '</empresa>\
      <email>'+ usuario.email + '</email>\
      <password>'+ (usuario.password ? usuario.password : "") + '</password>\
      <token>'+ usuario.token + '</token>\
      <tipoTicket>'+ ((usuario.tipoTicket == "77" || usuario.tipoTicket == "68") ? (usuario.tipoTicket == "77" ? "M" : "D") : "Z") + '</tipoTicket>\
      <imgUser>'+ (usuario.img ? usuario.img : "") + '</imgUser>\
      </UserUpdate>\
      </soap:Body>\
      </soap:Envelope>';
    // console.log(soapMessage);
    $.ajax({
        type: 'POST',
        url: url + 'WebServiceUsuarios.asmx?op=UserUpdate',
        data: soapMessage,
        contentType: 'text/xml; charset=utf-8',
        success: function (data) {
            successUpdate(formateoEstado(data, 'UserUpdateResult'));
        },
        error: function (jqXHR, textStatus, errorThrown) {
            Spinner.hide();
            console.warn("Ocurrio un error en la Actualizacion del usuario");
        }

    });
}



/**
 * Obtiene la imagen de un usuario por su ID
 * @param {number} id ID del usuario
 * @param {function} fnc Función que se ejecuta cuando se recibe la respuesta del servidor
 * La función debe recibir dos parámetros: la imagen codificada en base64 y el imgUser
 * @param {string} [imgUser=null] Imagen actual del usuario, se pasa para que la función
 * pueda elegir en un futuro si la imagen que se va a cargar es la del perfil o la del navBar
 * @returns {Promise<void>} Promesa que se resuelve cuando se recibe la respuesta del servidor
 */
async function getImageUser(id, fnc, imgUser = null) {
    var soapMessage = '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">\
  <soap:Body>\
    <GetImageId xmlns="http://tempuri.org/">\
      <ID>'+ id + '</ID>\
      <token>'+ getCookie("token") + '</token>\
    </GetImageId>\
  </soap:Body>\
</soap:Envelope>';
    $.ajax({
        type: 'POST',
        url: url + 'WebServiceUsuarios.asmx?op=GetImageId',
        data: soapMessage,
        contentType: 'text/xml; charset=utf-8',
        success: function (data) {
            var img = formatImage(data);
            fnc(img, imgUser);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            Spinner.hide();
        }

    });
}

/**
 * Extrae la imagen de un usuario de la respuesta del servidor
 * @param {Document} xml Documento XML con la respuesta del servidor
 * @returns {string|null} Imagen codificada en base64, o null si falla
 */
function formatImage(xml) {
    try {
        // Extraer el nodo raíz (soap:Envelope)
        const envelope = xml.documentElement;

        // Extraer el primer elemento hijo (soap:Body)
        const body = envelope.firstElementChild;

        // Extraer información específica dentro de Body
        const GetImageIdResult = body.querySelector("GetImageIdResult");
        if (GetImageIdResult) {
            // console.log(GetImageIdResult);
            return GetImageIdResult.querySelector('UsuarioImgUser')?.textContent || "Sin estado";
        } else {
            console.warn("No se encontró '" + "GetImageIdResult" + "'");
            return null;
        }
    } catch (error) {
        Spinner.hide();
        console.error("Error procesando el XML:", error);
        return null;
    }
}


/**
 * Obtiene todos los pasajeros de la base de datos.
 * @param {function} func Función que se llamará cuando se reciba la respuesta del servidor.
 * La función debe recibir un array de objetos con la información de los pasajeros.
 */
async function getAllPassengers(func) {
    // console.log(dni);
    var soapMessage = '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">\
  <soap:Body>\
    <GetAllPassengers xmlns="http://tempuri.org/">\
        <id>'+ getCookie("usuario") + '</id>\
        <token>'+ getCookie("token") + '</token>\
    </GetAllPassengers>\
  </soap:Body>\
</soap:Envelope>';
    // console.log(soapMessage);
    $.ajax({
        type: 'POST',
        url: url + 'WebServiceUsuarios.asmx?op=GetAllPassengers',
        data: soapMessage,
        contentType: 'text/xml; charset=utf-8',
        success: function (data) {
            var contenido = formateoUsuarios(data);
            func(contenido);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            Spinner.hide();
        }

    });
}

/**
 * Formatea el XML recibido de la petición GetAllPassengers en un arreglo de objetos con la información de cada pasajero.
 * 
 * @param {XMLDocument} xml Documento XML con la respuesta de la petición
 * @returns {Array<Object>|Object|null} Un arreglo de objetos con la información de cada pasajero, o un objeto con estado "-1" o "0" si falla, o null si ocurre un error.
 */
function formateoUsuarios(xml) {
    try {
        var datos = new Object();
        var usuarios = [];
        // Extraer el nodo raíz (soap:Envelope)
        const envelope = xml.documentElement;

        // Extraer el primer elemento hijo (soap:Body)
        const body = envelope.firstElementChild;

        // Extraer información específica dentro de Body
        const UserNameRepeatResult = body.querySelector("GetAllPassengersResponse");
        if (UserNameRepeatResult) {
            datos.estado = UserNameRepeatResult.querySelector('Estado')?.textContent || "Sin estado";
            if (datos.estado === "-1" || datos.estado === "0") {
                return datos;
            }
            const usuariosResult = UserNameRepeatResult.querySelectorAll("usuarios > Usuario");
            usuariosResult.forEach((usu) => {
                usuarios.push(formateoUsuarioIndividual(usu));
            });
            return usuarios;
        } else {
            console.warn("No se encontró 'GetAllPassengersResponse'");
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
 * Extracts user information from an XML node and returns a populated Usuario object.
 *
 * @param {Element} datos - XML element containing user data.
 * @returns {Usuario} A Usuario object with details such as ID, DNI, name, email, permissions, and company info.
 */
function formateoUsuarioIndividual(datos) {
    var usuario = new Usuario();
    var empresa = new Empresa();
    usuario.id = datos.querySelector("ID").textContent;
    usuario.dni = datos.querySelector("UsuarioDni").textContent;
    usuario.nombre = datos.querySelector("UsuarioNombre").textContent;
    usuario.apellidos = datos.querySelector("UsuarioApellidos").textContent;
    usuario.password = datos.querySelector("UsuarioPass").textContent;
    usuario.telefono = datos.querySelector("UsuarioTelefono").textContent;
    usuario.email = datos.querySelector("UsuarioEmail").textContent;
    usuario.token = datos.querySelector("UsuarioToken").textContent;
    usuario.activo = datos.querySelector("UsuarioActivo").textContent;
    usuario.tipoTicket = datos.querySelector("UsuarioTipoTicket").textContent;

    const permisosUsuario = datos.querySelector('UsuarioPermisos');
    usuario.permisos = permisosUsuario.querySelector('PermisosID').textContent;
    const empresaUsuario = datos.querySelector('UsuarioEmpresa');
    if (usuario.permisos == 3 || empresaUsuario == null) {
        empresa.id = null;
        empresa.razonSocial = null;
        empresa.nombreComercial = null;
        empresa.activo = null;
        usuario.empresa = empresa;
        usuario.ubicacion = null;
        return usuario;
    }
    empresa.id = empresaUsuario.querySelector('EmpresaID').textContent;
    empresa.razonSocial = empresaUsuario.querySelector('EmpresaRazonSocial').textContent;
    empresa.nombreComercial = empresaUsuario.querySelector('EmpresaNombreComercial').textContent;
    empresa.activo = empresaUsuario.querySelector('EmpresaActivo').textContent;
    usuario.empresa = empresa;
    const ubicacionUsuario = datos.querySelector('UsuarioUbicacion');
    usuario.ubicacion = ubicacionUsuario.querySelector('UbicacionID').textContent;
    // console.log(usuario);
    return usuario;
}