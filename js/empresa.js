class Empresa {
    constructor(id = null, razonSocial = null, nombreComercial = null, activo = null, estado = null) {
        this._id = id;
        this._razonSocial = razonSocial;
        this._nombreComercial = nombreComercial;
        this._activo = activo;
        this._estado = estado;
    }
    get id() {
        return this._id;
    }

    set id(value) {
        this._id = value;
    }

    get razonSocial() {
        return this._razonSocial;
    }

    set razonSocial(value) {
        this._razonSocial = value;
    }

    get nombreComercial() {
        return this._nombreComercial;
    }

    set nombreComercial(value) {
        this._nombreComercial = value;
    }

    get activo() {
        return this._activo;
    }

    set activo(value) {
        this._activo = value;
    }

    get estado() {
        return this._estado;
    }

    set estado(value) {
        this._estado = value;
    }

}