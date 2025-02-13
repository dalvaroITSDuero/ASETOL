class Ticket {
    constructor(id = null, usuario = null, fechaInicio = null, fechaFin = null, pagado = null, fechaPago = null, importe = null) {
        this._id = id;
        this._usuario = usuario;
        this._fechaInicio = fechaInicio;
        this._fechaFin = fechaFin;
        this._pagado = pagado;
        this._fechaPago = fechaPago;
        this._importe = this._importe;
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

    get fechaInicio() {
        return this._fechaInicio;
    }

    set fechaInicio(fechaInicio) {
        this._fechaInicio = fechaInicio;
    }

    get fechaFin() {
        return this._fechaFin;
    }

    set fechaFin(fechaFin) {
        this._fechaFin = fechaFin;
    }

    get pagado() {
        return this._pagado;
    }

    set pagado(pagado) {
        this._pagado = pagado;
    }

    get fechaPago() {
        return this._fechaPago;
    }

    set fechaPago(fechaPago) {
        this._fechaPago = fechaPago;
    }

    get importe() {
        return this._importe;
    }

    set importe(importe) {
        this._importe = importe;
    }
}