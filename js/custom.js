var curentTheme = localStorage.getItem("mode"),
    dark = document.querySelector("#darkTheme"),
    light = document.querySelector("#lightTheme"),
    switcher = document.querySelector("#modeSwitcher"),
    imagenPerfil = document.querySelector("#imagenPerfil"),
    darkSweetAlert = document.querySelector('#darkThemeSweetAlert'),
    defaultSweetAlert = document.querySelector('#defaultThemeSweetAlert')
    ;

//metodo para instanciar una coockie simple
function setCookie(name, value, days) {
    // console.log(`Configurando cookie: ${name}=${value} (expira en ${days} días)`);
    const date = new Date();
    date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
    const expires = `expires=${date.toUTCString()}`;
    document.cookie = `${name}=${encodeURIComponent(value)}; ${expires}; path=/; SameSite=None; Secure`;
}

//metodo para recuperar una cookie simple
function getCookie(name) {
    const cookies = document.cookie.split('; ');
    for (const cookie of cookies) {
        const [key, value] = cookie.split('=');
        if (key === name) {
            return value;
        }
    }
    return null; // Si la cookie no existe
}

//metodo para eliminar una cookie
function deleteCookie(name) {
    document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;`;
}

function saveImageToLocalStorage(key, contenido) {
    try {
        // Guardar la imagen bajo una clave específica
        localStorage.setItem(key, contenido);
        // console.log("Imagen guardada correctamente en localStorage.");
    } catch (error) {
        // console.error("Error al guardar la imagen en localStorage:", error);
    }
}

function loadImageFromLocalStorage(key) {
    try {
        // Recuperar la imagen desde localStorage
        const base64Image = localStorage.getItem(key);
        if (base64Image) {
            // console.log("Imagen recuperada correctamente.");
            return base64Image;
        } else {
            // console.warn("No se encontró ninguna imagen bajo la clave:", key);
            return null;
        }
    } catch (error) {
        // console.error("Error al recuperar la imagen desde localStorage:", error);
        return null;
    }
}

/**
 * Clase para manejar el spinner de carga
 */
class Spinner {
    /**
     * Muestra el spinner de carga
     * 
     * Quita la clase d-none del elemento con la clase loadingOverlay
     * para mostrar el spinner de carga.
     */
    static show() {
        document.querySelector('.loadingOverlay').classList.remove('d-none');
    }


    /**
     * Oculta el spinner de carga
     * 
     * Añade la clase d-none al elemento con la clase loadingOverlay
     * para ocultar el spinner de carga.
     */
    static hide() {
        document.querySelector('.loadingOverlay').classList.add('d-none');
    }
}

/**
 * Clase para manejar el botón desconectar
 */
class Desconectar {
    /**
     * Maneja el click en el botón desconectar
     * 
     * Elimina las cookies usuario y token, y redirige al usuario a login.html
     */
    static logout() {
        document.querySelector('.desconectar').addEventListener('click', function () {
            Spinner.show();
            getCookie("usuario") ? deleteCookie("usuario") : null;
            getCookie("token") ? deleteCookie("token") : null;
            getCookie("tipo") ? deleteCookie("tipo") : null;
            getCookie("ticket") ? deleteCookie("ticket") : null;
            loadImageFromLocalStorage("imagenPerfil") ? localStorage.removeItem("imagenPerfil") : null;
            Spinner.hide();
            window.location.href = "/login.html";
        });
    }
}


class Encryptar {

    /**
     * Cifra un string utilizando AES y el valor de fecha actual como clave.
     * 
     * @param {string} datos - El string a cifrar
     * @returns {string} El string cifrado
     */
    static cifrar(datos) {
        return CryptoJS.AES.encrypt(datos, convertToDateOnly(new Date)).toString();
    }

    /**
     * Descifra un string utilizando AES y el valor de fecha actual como clave.
     * 
     * @param {string} datos - El string a descifrar
     * @returns {string} El string descifrado
     */
    static descifrar(datos) {
        const desencriptados = CryptoJS.AES.decrypt(datos, convertToDateOnly(new Date));
        return desencriptados.toString(CryptoJS.enc.Utf8);
    }
}

class SweetAlert {

    static alert(contenido, titulo = "Oops...", action = null) {
        // console.log(curentTheme);
        if (action != null) {
            Swal.fire({
                icon: "error",
                title: titulo,
                text: contenido,
                didClose: () => {
                    action();
                }
            });
        } else {
            Swal.fire({
                icon: "error",
                title: titulo,
                text: contenido,
            });
        }
    }

    static successfull(contenido, titulo = "Correcto", action = null) {
        if (action != null) {
            Swal.fire({
                icon: "success",
                title: titulo,
                text: contenido,
                didClose: () => {
                    action();
                }
            });
        } else {
            Swal.fire({
                icon: "success",
                title: titulo,
                text: contenido,
            });
        }
    }
}

/**
 * Cambia el tema entre claro y oscuro
 *
 * Si existe un valor en localStorage para "mode", lo cambia por el
 * valor contrario. Si no existe, comprueba si el body tiene la clase
 * "dark" y lo cambia por el valor contrario.
 *
 * @private
 */
function modeSwitch() {
    // console.log("abc");
    var mode = localStorage.getItem("mode");

    if (mode) {
        if (mode === "dark") {
            dark.disabled = true;
            darkSweetAlert.disabled = true;
            light.disabled = false;
            defaultSweetAlert.disabled = false;
            localStorage.setItem("mode", "light");
        } else {
            dark.disabled = false;
            light.disabled = true;
            darkSweetAlert.disabled = false;
            defaultSweetAlert.disabled = true;
            localStorage.setItem("mode", "dark");
        }
    } else {
        if ($("body").hasClass("dark")) {
            dark.disabled = false;
            light.disabled = true;
            darkSweetAlert.disabled = false;
            defaultSweetAlert.disabled = true;
            localStorage.setItem("mode", "dark");
        } else {
            dark.disabled = true;
            light.disabled = false;
            darkSweetAlert.disabled = true;
            defaultSweetAlert.disabled = false;
            localStorage.setItem("mode", "light");
        }
    }
}


// console.log(curentTheme);
if (curentTheme) {
    if (curentTheme === "dark") {
        dark.disabled = false;
        light.disabled = true;
        darkSweetAlert.disabled = false;
        defaultSweetAlert.disabled = true;
    } else if (curentTheme === "light") {
        dark.disabled = true;
        light.disabled = false;
        darkSweetAlert.disabled = true;
        defaultSweetAlert.disabled = false;
    }
    switcher ? switcher.dataset.mode = curentTheme : "";
} else {
    localStorage.setItem("mode", "light");
    dark.disabled = true;
    light.disabled = false;
    darkSweetAlert.disabled = true;
    defaultSweetAlert.disabled = false;
}



/**
 * Verifica si ya hay un usuario logueado, si es así lo redirige a su
 * correspondiente interfaz de usuario.
 * @returns {Promise<void>}
 */
async function verificacionLogin() {
    Spinner.show();
    let usuYpas = document.cookie;
    // console.log("login: "+usuYpas);
    if (document.cookie) {
        await getUsuarioByIdToken(getCookie("usuario"), getCookie("token"));
    }else{
        Spinner.hide();
    }
}

async function redireccionarCoockies(estado = null, params = null) {
    if (estado == 0) {
        deleteCookie("usuario");
        deleteCookie("token");
        deleteCookie("tipo");
        window.location.href = "/login.html";
    } else if (estado == 1) {
        let redireccion;
        if (params == null) {
            redireccion = getCookie("tipo") == "2" ? "/pasajero/indexPasajero.html" : "/conductor/indexConductor.html";
            window.location.href = redireccion;
        } else {
            redireccion = params.includes("3") ? "/pasajero/indexPasajero.html" : "/conductor/indexConductor.html";
        }
        if (params != null && !params.includes(getCookie("tipo"))) {
            window.location.href = redireccion;
        }
        // params.includes(getCookie("tipo")) ? "" : window.location.href = redireccion;
    } else {
        SweetAlert.alert("login Fallido");
        Spinner.hide();
    }
    // Spinner.hide();
}

/**
 * Verifica si el usuario tiene los permisos necesarios para acceder a la interfaz
 * correspondiente.
 * 
 * Si el usuario no tiene permisos, lo redirige a login.html.
 * Si el usuario tiene permisos, lo redirige a la interfaz correspondiente.
 * @param {number} params - 2 para interfaz de pasajero, 3 para interfaz de conductor
 */
async function verificacionPermisos(params) {
    Spinner.show();
    let usuYpas = document.cookie;
    // console.log("verificacion: " + usuYpas + ", params: " + params);
    if (document.cookie) {
        await getUsuarioByIdToken(getCookie("usuario"), getCookie("token"), params);
    } else {
        Spinner.hide();
        window.location.href = "/login.html";
    }
}


var cargaFoto = 0;
/**
 * Carga una imagen de perfil de un usuario en un elemento img
 *
 * El primer parámetro es el src de la imagen, si no se proporciona se
 * intenta cargar la imagen desde localStorage. Si no se encuentra en
 * localStorage, se intenta obtener la imagen desde el servidor.
 *
 * El segundo parámetro es el elemento img donde se cargará la imagen.
 * Si no se proporciona, se carga en el elemento img con id "imagenPerfil"
 *
 * @param {string} data - src de la imagen a cargar
 * @param {HTMLImageElement} imgPerf - elemento img donde se cargará la imagen
 * @returns {Promise<void>}
 */
async function cargarFotoPerfil(data, imgPerf = null) {
    Spinner.show();
    var img = loadImageFromLocalStorage("imagenPerfil");
    var lugarDeCarga = imgPerf ? imgUser : imagenPerfil;
    cargaFoto++;
    if (data != null && data != "Sin estado" && img == null) {
        lugarDeCarga.src = data;
        saveImageToLocalStorage("imagenPerfil", data);
        cargaFoto = 0;
        Spinner.hide();
        return false;
    } else if (img != null) {
        lugarDeCarga.src = img;
        cargaFoto = 0;
        if (imgPerf) {
            Spinner.hide();
        }
        return false;
    } else if (cargaFoto == 2) {
        cargaFoto = 0;
        Spinner.hide();
        return false;
    }
    await getImageUser(getCookie("usuario"), cargarFotoPerfil);
}


async function firstLogin(usuario = null) {
    // console.log(window.location);
    var url = window.location.href;
    if (usuario != null && usuario.empresa.id == null) {
        if (!url.includes("/pasajero/inicial.html")) {
            window.location.href = "/pasajero/inicial.html";
        }
        return false;
    } else if (usuario != null) {
        if (url.includes("/pasajero/inicial.html") && usuario.empresa.id != null) {
            window.location.href = "/pasajero/indexPasajero.html";
        }
        return false;
    }
    await getUsuarioById(getCookie("usuario"), firstLogin);
}


/**
 * Genera las opciones del select de empresas en la pantalla de perfil.
 * @param {Object} Empresas - El objeto que contiene el array de empresas a mostrar.
 * @param {Array} Empresas.array - El array con 0los objetos empresas.
 */
async function generarEmpresas(empresas) {
    const empress = await empresas;
    empress.forEach((empresa) => {
        // console.log(empresa);
        const option = document.createElement("option");
        option.value = empresa.id;
        option.text = empresa.nombreComercial;
        empresaSelect.add(option);
    });
}


/**
 * Genera las opciones del select de ubicaciones en la pantalla de perfil.
 * @param {Object} ubicaciones - El objeto que contiene el array de ubicaciones a mostrar.
 * @param {Array} ubicaciones.array - El array con los objetos ubicacion.
 */
async function generarUbicaciones(ubicaciones) {
    const ubicacione = await ubicaciones;
    ubicacione.forEach(ubi => {
        // console.log(ubi);
        const option = document.createElement("option");
        option.value = ubi.id;
        option.text = ubi.descripcion;
        ubicacionSelect.add(option);
    });
}

/**
 * Convierte una cadena de texto que representa una fecha y hora en una cadena solo con la fecha.
 * El formato de la cadena de entrada debe ser ISO 8601, por ejemplo, "2022-01-01T12:00:00.000Z".
 * El formato de la cadena de salida sera "YYYY-MM-DD", por ejemplo, "2022-01-01".
 * @param {string} dateTimeString - La cadena de texto que representa la fecha y hora.
 * @returns {string} La cadena de texto con la fecha en formato YYYY-MM-DD.
 */
function convertToDateOnly(dateTimeString) {
    const date = new Date(dateTimeString); // Crear un objeto Date
    const year = date.getFullYear(); // Obtener el año
    const month = String(date.getMonth() + 1).padStart(2, '0'); // Mes (0 indexado)
    const day = String(date.getDate()).padStart(2, '0'); // Día

    return `${year}-${month}-${day}`; // Devolver en formato YYYY-MM-DD
}

/**
 * Comprueba si una fecha está dentro de un rango.
 * @param {string} startDate - La fecha de inicio del rango, en formato ISO 8601.
 * @param {string} endDate - La fecha de fin del rango, en formato ISO 8601.
 * @param {string} dateToCheck - La fecha a comprobar, en formato ISO 8601.
 * @returns {boolean} `true` si la fecha está en el rango, `false` en caso contrario.
 */
function isDateInRange(startDate, endDate, dateToCheck) {
    // Convertir las fechas a objetos Date
    const date = new Date(dateToCheck);
    const start = new Date(startDate);
    const end = new Date(endDate);

    // Comprobar que la fecha está en el rango (incluidas las fechas límite)
    return date >= start && date <= end;
}