using es.itsduero.iecscyl;
using es.itsduero.iecscyl.data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

/// <summary>
/// Descripción breve de WebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
// [System.Web.Script.Services.ScriptService]
public class WebServiceUsuarios : System.Web.Services.WebService {

    private Usuario _usuario = new Usuario();
    private readonly BDUsuario _bdUsuario = new BDUsuario();
    private readonly Acceso _acceso = new Acceso();

    public WebServiceUsuarios () {

        //Elimine la marca de comentario de la línea siguiente si utiliza los componentes diseñados 
        //InitializeComponent(); 
    }

    public class RespuestaUsuarios
    {
        public int Estado { get; set; }
        public Usuario usuario { get; set; }
        public Usuario[] usuarios { get; set; } 
        public string Mensaje { get; set; }

    }

    [WebMethod(Description = "Comprobación de si existe un usuario con el mismo username (DNI)")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    [OperationContract]
    public RespuestaUsuarios UserNameRepeat(string userName)
    {
        RespuestaUsuarios respuesta = new RespuestaUsuarios();
        try
        {
            if (_bdUsuario.UserRepeat(userName))
            {
                respuesta.Estado = 1;
                respuesta.Mensaje = "Usuario repetido";
                Global.GenerarLog(_usuario, Global.GetUserIP(), "Creacion usuario DNI repetido");
            }
            else
            {
                respuesta.Estado = 0;
                respuesta.Mensaje = "Usuario no repetido";
            }
        }
        catch
        {
            respuesta.Estado = -1;
            respuesta.Mensaje = "Error comprobacion";
        }
        return respuesta;
    }

    [WebMethod(Description = "Creación de usuario")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    [OperationContract]
    public RespuestaUsuarios UserCreate(string dni, string nombre, string apellidos, int telefono, string email, string password)
    {
        Usuario user = new Usuario();
        user.activo = true;
        user.dni = dni;
        user.nombre = nombre;
        user.apellidos = apellidos;
        user.telefono = telefono;
        user.email = email;
        user.password = password;
        user.permisos = new Permisos { ID = 2 }; ;
        RespuestaUsuarios respuesta = new RespuestaUsuarios();
        try
        {
            _bdUsuario.Insert(ref user);
            respuesta.Estado = 1;
            respuesta.Mensaje = "Usuario creado correctamente";
            Global.GenerarLog(_usuario, Global.GetUserIP(), "Creacion de usuario");
        }                

        catch
        {
            respuesta.Estado = 0;
            respuesta.Mensaje = "Error creacion usuario";
        }
        return respuesta;
    }

    [WebMethod(Description = "Creación de usuario")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    [OperationContract]
    public RespuestaUsuarios GetUserById(int id, string token)
    {
        RespuestaUsuarios respuesta = new RespuestaUsuarios();
        try
        {
            _usuario = _acceso.identificarUsuario(short.Parse(id.ToString()), token);
            if (_usuario != null)
            {
                respuesta.usuario = _bdUsuario.GetByID(id);
                if (respuesta.usuario != null)
                {
                    respuesta.Estado = 1;
                    respuesta.Mensaje = "Usuario Encontrado";
                    Global.GenerarLog(_usuario, Global.GetUserIP(), "Obtencion de usuario");
                }
                else
                {
                    respuesta.Estado = 0;
                    respuesta.Mensaje = "Usuario no encontrado";
                }
            }
            else
            {
                respuesta.Estado = 0;
                respuesta.Mensaje = "Usuario no verificado";
            }
        }
        catch
        {
            respuesta.Estado = -1;
            respuesta.Mensaje = "Error al buscar usuario";
        }
        return respuesta;
    }

    [WebMethod(Description = "Actualizacion de usuario")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    [OperationContract]
    public RespuestaUsuarios UserUpdate(int ID, string dni, string nombre, string apellidos,
        int telefono, int ubicacion, int empresa, string email, string password, string token, string tipoTicket, string imgUser)
    {


        Usuario user = _bdUsuario.GetByID(ID);
        user.ID = ID;
        user.dni = dni;
        user.nombre = nombre;
        user.apellidos = apellidos;
        user.telefono = telefono;
        user.email = email;
        user.password = (password != "" ? password : user.password);
        user.token = token;
        user.activo = true;
        if (ubicacion != 0)
        {
            user.ubicacion = new Ubicacion { ID = ubicacion };
        }
        if (empresa != 0)
        {
            user.empresa = new Empresa { ID = empresa };
        }
        user.tipoTicket = tipoTicket[0];
        user.imgUser = (imgUser != "" ? user.ID + ".jpg" : user.imgUser);
        RespuestaUsuarios respuesta = new RespuestaUsuarios();
        try
        {
            _usuario = _acceso.identificarUsuario(short.Parse(ID.ToString()), token);
            if (_usuario != null)
            {
                imgUser = imgUser.Substring(imgUser.IndexOf(",") + 1);
                if (imgUser != "")
                {
                    respuesta.Mensaje = "Error insertando imagen usuario";
                    if (!Global.SaveData(user.ID + ".jpg", Convert.FromBase64String(imgUser)))
                    {
                        respuesta.Estado = -1;
                        return respuesta;
                    }
                    user.imgUser = user.ID + ".jpg";
                }
                respuesta.Mensaje = "Error actualizando usuario";
                _bdUsuario.Update(user);
                respuesta.Estado = 1;
                respuesta.Mensaje = "Usuario actualizado correctamente";
                Global.GenerarLog(_usuario, Global.GetUserIP(), "Usuario: " + _usuario.dni + " - Actualizacion de usuario");
            }
            else
            {
                respuesta.Estado = 0;
                respuesta.Mensaje = "Usuario no verificado";
            }
        }
        catch
        {
            respuesta.Estado = -1;
            respuesta.Mensaje = "Error: "+ID +", "+ dni +", "+ nombre +", "+ apellidos +", "+ telefono +", "+ ubicacion +", "+ empresa +", "+ email +", "+ password +", "+ token +", "+ tipoTicket +", "+imgUser;
        }
        return respuesta;
    }


    [WebMethod(Description = "Obtener imagen del usuario")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    [OperationContract]
    public RespuestaUsuarios GetImageId(int ID, string token)
    {
        RespuestaUsuarios respuesta = new RespuestaUsuarios();
        respuesta.usuario = new Usuario();
        try
        {
            _usuario = _acceso.identificarUsuario(short.Parse(ID.ToString()), token);
            if (_usuario != null)
            {
                respuesta.usuario.imgUser = Global.GetImageAsBase64(ID.ToString());
                if (respuesta.usuario.imgUser != null)
                {
                    respuesta.Estado = 1;
                    respuesta.Mensaje = "Imagen usuario Encontrado";
                    Global.GenerarLog(_usuario, Global.GetUserIP(), "Usuario: " + _usuario.dni + " - Obtencion de imagen de perfil");
                }
                else
                {
                    respuesta.Estado = 0;
                    respuesta.Mensaje = "Imagen usuario no encontrado";
                }
            }
            else
            {
                respuesta.Estado = 0;
                respuesta.Mensaje = "Usuario no verificado";
            }
        }
        catch
        {
            respuesta.Estado = -1;
            respuesta.Mensaje = "Error al buscar Imagen usuario";
        }
        return respuesta;
    }

    [WebMethod(Description = "Todos los pasajeros")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    [OperationContract]
    public RespuestaUsuarios GetAllPassengers(int id, string token)
    {
        RespuestaUsuarios respuesta = new RespuestaUsuarios();
        try
        {
            _usuario = _acceso.identificarUsuario(short.Parse(id.ToString()), token);
            if (_usuario != null)
            {
                ArrayList usuarios = _bdUsuario.GetAllPassengers();
                respuesta.usuarios = usuarios.Cast<Usuario>().ToArray();
                if (usuarios.Count == 0)
                {
                    respuesta.Estado = 0;
                    respuesta.Mensaje = "Viaje no encontrado";
                }
                else
                {
                    respuesta.Estado = 1;
                    respuesta.Mensaje = "Viaje creado correctamente";
                    Global.GenerarLog(_usuario, Global.GetUserIP(), "Usuario: " + _usuario.dni + " - Obtencion de todos los pasajeros");
                }
            }
            else
            {
                respuesta.Estado = 0;
                respuesta.Mensaje = "Usuario no verificado";
            }

        }
        catch
        {
            respuesta.Estado = -1;
            respuesta.Mensaje = "Error al buscar viaje";
        }
        return respuesta;
    }
}
