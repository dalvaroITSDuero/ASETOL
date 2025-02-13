class Ubicacion {
    constructor(id = null, descripcion = null) {
        this._id = id;
        this._descripcion = descripcion;
    }
    get id() {
        return this._id;
    }

    set id(value) {
        this._id = value;
    }

    get descripcion() {
        return this._descripcion;
    }

    set descripcion(value) {
        this._descripcion = value;
    }

}