class Usuario {
    constructor(id = null, dni = null, nombre = null, apellidos = null,
        password = null, telefono = null, email = null, empresa = null,
        activo = null, permisos = null, tipoTicket = null, ubicacion = null,
        token = null, estado = null, img = null) {
        this._id = id;
        this._nombre = nombre;
        this._apellidos = apellidos;
        this._dni = dni;
        this._password = password;
        this._telefono = telefono;
        this._token = token;
        this._permisos = permisos;
        this._email = email;
        this._empresa = empresa;
        this._activo = activo;
        this._ubicacion = ubicacion;
        this._tipoTicket = tipoTicket;
        this._estado = estado;
        this._img = img;
    }
    // Getters
    get id() {
        return this._id;
    }

    get dni() {
        return this._dni;
    }

    get nombre() {
        return this._nombre;
    }

    get apellidos() {
        return this._apellidos;
    }

    get password() {
        return this._password;
    }

    get telefono() {
        return this._telefono;
    }

    get email() {
        return this._email;
    }

    get empresa() {
        return this._empresa;
    }

    get activo() {
        return this._activo;
    }

    get permisos() {
        return this._permisos;
    }

    get tipoTicket() {
        return this._tipoTicket;
    }

    get ubicacion() {
        return this._ubicacion;
    }

    get token() {
        return this._token;
    }

    get estado() {
        return this._estado;
    }

    get img() {
        return this._img;
    }

    // Setters
    set id(value) {
        this._id = value;
    }

    set dni(value) {
        this._dni = value;
    }

    set nombre(value) {
        this._nombre = value;
    }

    set apellidos(value) {
        this._apellidos = value;
    }

    set password(value) {
        this._password = value;
    }

    set telefono(value) {
        this._telefono = value;
    }

    set email(value) {
        this._email = value;
    }

    set empresa(value) {
        this._empresa = value;
    }

    set activo(value) {
        this._activo = value;
    }

    set permisos(value) {
        this._permisos = value;
    }

    set tipoTicket(value) {
        this._tipoTicket = value;
    }

    set ubicacion(value) {
        this._ubicacion = value;
    }

    set token(value) {
        this._token = value;
    }

    set estado(value) {
        this._estado = value;
    }

    set img(value) {
        this._img = value;
    }

    toString() {
        return `Usuario: ${this._nombre} ${this._apellidos}, ${this._token}, ${this._telefono}, ${this._email}, ${this._img}, ${this._estado}, ${this._ubicacion}, ${this._permisos}, ${this._tipoTicket}, ${this._empresa}`;
    }

}