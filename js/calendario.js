class Calendario {
    constructor(id = null, usuario = null, fecha = null, turno = null) {
        this._id = id;
        this._usuario = usuario;
        this._fecha = fecha;
        this._turno = turno;
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
    
    get fecha() {
        return this._fecha;
    }
    
    set fecha(fecha) {
        this._fecha = fecha;
    }
    
    get turno() {
        return this._turno;
    }
    
    set turno(turno) {
        this._turno = turno;
    }
}