class Viaje {
    constructor(id = null, usuario = null, ticket = null, turno = null, ubicacion = null, fecha = null, conductor = null) {
        this._id = id;
        this._usuario = usuario;
        this._ticket = ticket;
        this._turno = turno;
        this._ubicacion = ubicacion;
        this._fecha = fecha;
        this._conductor = conductor;
    }
    get id() {
        return this._id;
    }

    set id(id) {
        this._id = id;
    }

    get usuario() {
        return this._usuario;
    }

    set usuario(usuario) {
        this._usuario = usuario;
    }

    get ticket() {
        return this._ticket;
    }

    set ticket(ticket) {
        this._ticket = ticket;
    }

    get turno() {
        return this._turno;
    }

    set turno(turno) {
        this._turno = turno;
    }

    get ubicacion() {
        return this._ubicacion;
    }

    set ubicacion(ubicacion) {
        this._ubicacion = ubicacion;
    }

    get fecha() {
        return this._fecha;
    }

    set fecha(fecha) {
        this._fecha = fecha;
    }

    get conductor() {
        return this._conductor;
    }

    set conductor(conductor) {
        this._conductor = conductor;
    }

}