using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Text;
using System.Drawing;
using System.IO;
using es.itsduero.iecscyl;
using es.itsduero.iecscyl.data;

/// <summary>
/// Descripción breve de Global
/// </summary>
public static class Global
{

	/// <summary>
    /// Convierte la fecha en String
    /// </summary>
    /// <param name="_fecha">Fecha a convertir</param>
    /// <returns>Devuelve la fecha en formato yyyyMMdd</returns>
    /// <autor>dalvaro@itsduero.es</autor>
    public static String DateTimeToString(DateTime _fecha)
    {
        return _fecha.Year.ToString("0000") + _fecha.Month.ToString("00") + _fecha.Day.ToString("00");
    }


    /// <summary>
    /// Convierte la fecha en String
    /// </summary>
    /// <param name="_fecha">Fecha a convertir</param>
    /// <returns>Devuelve la fecha en formato yyyyMMddhhmm</returns>
    /// <autor>dalvaro@itsduero.es</autor>
    public static String DateTimeToStringConHora(DateTime _fecha)
    {
        return _fecha.Year.ToString("0000") + _fecha.Month.ToString("00") + _fecha.Day.ToString("00") + _fecha.Hour.ToString("00") + _fecha.Minute.ToString("00");
    }


    /// <summary>
    /// Convierte la fecha en DateTime
    /// </summary>
    /// <param name="_fecha">Fecha a convertir</param>
    /// <returns>Devuelve la fecha en formato DateTime</returns>
    /// <autor>dalvaro@itsduero.es</autor>
    public static DateTime? StringToDateTime(String _fecha)
    {
        if (_fecha.Trim().Length == 8)
        {
            int anyo = int.Parse(_fecha.Substring(0, 4));
            int mes = int.Parse(_fecha.Substring(4, 2));
            int dia = int.Parse(_fecha.Substring(6, 2));

            return new DateTime(anyo, mes, dia);
        }
        else
        {
            //return new DateTime(2000, 1, 1);
            return null;
        }
    }


    /// <summary>
    /// Convierte la fecha en DateTime
    /// </summary>
    /// <param name="_fecha">Fecha a convertir</param>
    /// <returns>Devuelve la fecha en formato DateTime</returns>
    /// <autor>dalvaro@itsduero.es</autor>
    public static DateTime? StringToDateTimeConHora(String _fecha)
    {
        if (_fecha.Trim().Length == 12)
        {
            int anyo = int.Parse(_fecha.Substring(0, 4));
            int mes = int.Parse(_fecha.Substring(4, 2));
            int dia = int.Parse(_fecha.Substring(6, 2));

            int hora = int.Parse(_fecha.Substring(8, 2));
            int minuto = int.Parse(_fecha.Substring(10, 2));

            return new DateTime(anyo, mes, dia, hora, minuto, 0);
        }
        else
        {
            //return new DateTime(2000, 1, 1);
            return null;
        }
    }


    /// <summary>
    /// Obtiene el nº de items a mostrar por cada página
    /// </summary>
    /// <param name="cookies">Colección de cookies del navegador del usuario</param>
    /// <returns>Número de items por página</returns>
    /// <autor>dalvaro@itsduero.es</autor>
    public static short obtenerValorNumeroItemsPorPagina(HttpCookieCollection cookies)
    {

        short numeroItemsPorPagina = short.Parse(ConfigurationManager.AppSettings["articulosPorPagina"]);

        // Obtenemos el valor de las cookies de nº de items por página
        HttpCookie cookieItemsPorPagina = cookies[ConfigurationManager.AppSettings["nameCookie"] + "_items_x_pag"];

        if (cookieItemsPorPagina != null && !string.IsNullOrEmpty(cookieItemsPorPagina.Value))
        {
            numeroItemsPorPagina = short.Parse(cookieItemsPorPagina.Value);
        }

        return numeroItemsPorPagina;

    }


    /// <summary>
    /// Almacena en una cookie de usuario el valor del nº de items a mostrar por cada página
    /// </summary>
    /// <param name="Response">Respuesta a enviar al navegador</param>
    /// <param name="numeroItemsPorPagina">Nuevo nº de items a mostrar por cada página</param>
    /// <autor>dalvaro@itsduero.es</autor>
    public static void guardarValorNumeroItemsPorPagina(HttpResponse Response, short numeroItemsPorPagina)
    {

        // Creamos la cookie
        HttpCookie cookieItemsPorPagina = new HttpCookie(ConfigurationManager.AppSettings["nameCookie"] + "_items_x_pag");

        cookieItemsPorPagina.Value = numeroItemsPorPagina.ToString();
        cookieItemsPorPagina.Expires = DateTime.Now.AddMonths(1); // 1 mes

        Response.Cookies.Add(cookieItemsPorPagina);

    }


    /// <summary>
    /// Deshabilita todos los controles existentes en una página
    /// </summary>
    /// <param name="ctrls">Colección con los controles existentes en una página</param>
    /// <autor>dalvaro@itsduero.es</autor>
    public static void DisableControls(ControlCollection ctrls)
    {
        foreach (Control ctrl in ctrls)
        {
            if (ctrl is TextBox)
                ((TextBox)ctrl).Enabled = false;
            if (ctrl is Button)
                ((Button)ctrl).Enabled = false;
            if (ctrl is LinkButton)
                ((LinkButton)ctrl).Enabled = false;
            else if (ctrl is DropDownList)
                ((DropDownList)ctrl).Enabled = false;
            else if (ctrl is CheckBox)
                ((CheckBox)ctrl).Enabled = false;
            else if (ctrl is RadioButton)
                ((RadioButton)ctrl).Enabled = false;
            else if (ctrl is FileUpload)
                ((FileUpload)ctrl).Enabled = false;
            else if (ctrl is HtmlInputButton)
                ((HtmlInputButton)ctrl).Disabled = true;
            else if (ctrl is HtmlInputText)
                ((HtmlInputText)ctrl).Disabled = true;
            else if (ctrl is HtmlTextArea)
                ((HtmlTextArea)ctrl).Disabled = true;
            else if (ctrl is HtmlSelect)
                ((HtmlSelect)ctrl).Disabled = true;
            else if (ctrl is HtmlInputCheckBox)
                ((HtmlInputCheckBox)ctrl).Disabled = true;
            else if (ctrl is HtmlInputRadioButton)
                ((HtmlInputRadioButton)ctrl).Disabled = true;

            DisableControls(ctrl.Controls);
        }
    }


    /// <summary>
    /// Devuelve un espacio en blanco
    /// </summary>
    /// <returns>Espacio en blanco</returns>
    /// <autor>dalvaro@itsduero.es</autor>
    public static char[] Espacio()
    {
        char[] espacio = new char[1];
        espacio[0] = ' ';
        return espacio;
    }


    /// <summary>
    /// Obtenemos la hora en punto
    /// </summary>
    /// <returns>Fecha / Hora</returns>
    /// <autor>dalvaro@itsduero.es</autor>
    public static DateTime getHoraEnPunto()
    {
        DateTime fechaAhora = DateTime.Now;

        // Fecha en punto
        DateTime fechaEnPunto = fechaAhora;
        if (fechaAhora.Minute >= 55)
        {
            // Sumamos 1 hora al día
            DateTime fechaAux = fechaAhora.AddHours(1);
            // Ponemos la hora en punto
            fechaEnPunto = new DateTime(fechaAux.Year, fechaAux.Month, fechaAux.Day, fechaAux.Hour, 0, 0);
        }
        else
        {
            // Ponemos la hora en punto
            fechaEnPunto = new DateTime(fechaAhora.Year, fechaAhora.Month, fechaAhora.Day, fechaAhora.Hour, 0, 0);
        }

        return fechaEnPunto;
    }


    /// <summary>
    /// Obtiene la fecha del turno
    /// </summary>
    /// <param name="_fecha">Fecha a convertir</param>
    /// <returns>Devuelve la fecha del turno</returns>
    /// <autor>dalvaro@itsduero.es</autor>
    public static DateTime FechaTurno(DateTime _fecha)
    {
        if (_fecha.Hour < 6)
        {
            DateTime diaAnterior = _fecha.AddDays(-1);
            return new DateTime(diaAnterior.Year, diaAnterior.Month, diaAnterior.Day, 0, 0, 0);
        }
        else
        {
            return new DateTime(_fecha.Year, _fecha.Month, _fecha.Day, 0, 0, 0);
        }
    }


    /// <summary>
    /// Vacía todos los controles existentes en una página
    /// </summary>
    /// <param name="ctrls">Colección con los controles existentes en una página</param>
    /// <autor>dalvaro@itsduero.es</autor>
    public static void EmptyControls(ControlCollection ctrls)
    {
        foreach (Control ctrl in ctrls)
        {
            if (ctrl is TextBox)
                ((TextBox)ctrl).Text = "";
            else if (ctrl is DropDownList)
                ((DropDownList)ctrl).SelectedIndex = 0;
            else if (ctrl is CheckBox)
                ((CheckBox)ctrl).Checked = false;
            else if (ctrl is RadioButton)
                ((RadioButton)ctrl).Checked = false;
            else if (ctrl is HtmlInputText)
                ((HtmlInputText)ctrl).Value = "";
            else if (ctrl is HtmlTextArea)
                ((HtmlTextArea)ctrl).Value = "";
            else if (ctrl is HtmlSelect)
                ((HtmlSelect)ctrl).Value = "";
            else if (ctrl is HtmlInputCheckBox)
                ((HtmlInputCheckBox)ctrl).Value = "";
            else if (ctrl is HtmlInputRadioButton)
                ((HtmlInputRadioButton)ctrl).Value = "";

            EmptyControls(ctrl.Controls);
        }
    }


    public static string generarTokenAleatorio()
    {
        // Creamos una matriz de caracteres
        ArrayList digitos = new ArrayList();
        digitos.Add("a");
        digitos.Add("b");
        digitos.Add("c");
        digitos.Add("d");
        digitos.Add("e");
        digitos.Add("f");
        digitos.Add("g");
        digitos.Add("h");
        digitos.Add("i");
        digitos.Add("j");
        digitos.Add("k");
        digitos.Add("l");
        digitos.Add("m");
        digitos.Add("n");
        digitos.Add("o");
        digitos.Add("p");
        digitos.Add("q");
        digitos.Add("r");
        digitos.Add("s");
        digitos.Add("t");
        digitos.Add("u");
        digitos.Add("v");
        digitos.Add("w");
        digitos.Add("x");
        digitos.Add("y");
        digitos.Add("z");
        digitos.Add("A");
        digitos.Add("B");
        digitos.Add("C");
        digitos.Add("D");
        digitos.Add("E");
        digitos.Add("F");
        digitos.Add("G");
        digitos.Add("H");
        digitos.Add("I");
        digitos.Add("J");
        digitos.Add("K");
        digitos.Add("L");
        digitos.Add("M");
        digitos.Add("N");
        digitos.Add("O");
        digitos.Add("P");
        digitos.Add("Q");
        digitos.Add("R");
        digitos.Add("S");
        digitos.Add("T");
        digitos.Add("U");
        digitos.Add("V");
        digitos.Add("W");
        digitos.Add("X");
        digitos.Add("Y");
        digitos.Add("Z");
        digitos.Add("0");
        digitos.Add("1");
        digitos.Add("2");
        digitos.Add("3");
        digitos.Add("4");
        digitos.Add("5");
        digitos.Add("6");
        digitos.Add("7");
        digitos.Add("8");
        digitos.Add("9");

        // Generamos un token aleatorio
        String token = "";

        Random digitoAleatorio = new Random();
        int caracter;

        // 1
        caracter = digitoAleatorio.Next(0, digitos.Count);
        token += digitos[caracter].ToString();
        // 2
        caracter = digitoAleatorio.Next(0, digitos.Count);
        token += digitos[caracter].ToString();
        // 3
        caracter = digitoAleatorio.Next(0, digitos.Count);
        token += digitos[caracter].ToString();
        // 4
        caracter = digitoAleatorio.Next(0, digitos.Count);
        token += digitos[caracter].ToString();
        // 5
        caracter = digitoAleatorio.Next(0, digitos.Count);
        token += digitos[caracter].ToString();
        // 6
        caracter = digitoAleatorio.Next(0, digitos.Count);
        token += digitos[caracter].ToString();
        // 7
        caracter = digitoAleatorio.Next(0, digitos.Count);
        token += digitos[caracter].ToString();
        // 8
        caracter = digitoAleatorio.Next(0, digitos.Count);
        token += digitos[caracter].ToString();
        // 9
        caracter = digitoAleatorio.Next(0, digitos.Count);
        token += digitos[caracter].ToString();
        // 10
        caracter = digitoAleatorio.Next(0, digitos.Count);
        token += digitos[caracter].ToString();

        return token;
    }

    public static byte[] ConvertBitMapToByteArray(Bitmap bitmap)
    {
        ImageConverter converter = new ImageConverter();
        return (byte[])converter.ConvertTo(bitmap, typeof(byte[]));
    }


    public static bool SaveData(string fileName, byte[] Data)
    {
        BinaryWriter Writer = null;
        //esto puede estar mal :)
        String carpetaImagenes = HttpContext.Current.Server.MapPath("~") + "/imgUser/";
        String fichero = carpetaImagenes + fileName;

        try
        {
            // Create a new stream to write to the file
            Writer = new BinaryWriter(File.OpenWrite(fichero));

            // Writer raw data                
            Writer.Write(Data);
            Writer.Flush();
            Writer.Close();
        }
        catch
        {
            return false;
        }

        return true;
    }

    public static string GetImageAsBase64(string fileName)
    {
        try
        { 
            // Construir la ruta de la carpeta donde están las imágenes
            string directoryPath = Path.Combine(HttpContext.Current.Server.MapPath("~"), "imgUser");

            // Buscar el archivo con cualquier extensión
            string[] files = Directory.GetFiles(directoryPath, fileName + ".*");

            // Validar si el archivo existe
            if (files.Length == 0 )
            {
                throw new FileNotFoundException("El archivo no fue encontrado.", fileName);
            }

            // Tomar el primer archivo encontrado
            string filePath = files.First();

            // Leer el archivo en bytes
            byte[] imageBytes = File.ReadAllBytes(filePath);

            // Convertir los bytes a Base64
            string base64String = Convert.ToBase64String(imageBytes);

            // Agregar el prefijo MIME si se requiere
            string mimeType = GetMimeType(filePath);
            return "data:" + mimeType + ";base64," + base64String;
        }
        catch (Exception ex)
        {
            // Manejo de errores
            Console.WriteLine("Error al recuperar la imagen: "+ ex.Message);
            return null;
        }
    }

    // Método auxiliar para obtener el tipo MIME basado en la extensión del archivo
    private static string GetMimeType(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLowerInvariant();

        // Usar múltiples casos en un switch
        switch (extension)
        {
            case ".jpg":
            case ".jpeg":
                return "image/jpeg";
            case ".png":
                return "image/png";
            case ".gif":
                return "image/gif";
            case ".bmp":
                return "image/bmp";
            case ".tiff":
                return "image/tiff";
            default:
                return "application/octet-stream"; // Por defecto
        }
    }

    public static void GenerarLog(Usuario _usuario, String IP, String accion)
    {
        Log _log = new Log();
        _log.usuario = _usuario;
        _log.fecha = DateTime.Now;
        _log.IP = IP;
        _log.accion = accion;
        new BDLog().Insert(_log);

    }

    public static string GetUserIP()
    {
        string ipAddress = HttpContext.Current.Request.Headers["X-Forwarded-For"];
        if (string.IsNullOrEmpty(ipAddress)){
            ipAddress = HttpContext.Current.Request.UserHostAddress;
        }
        return ipAddress;
    }
}
