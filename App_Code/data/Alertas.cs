using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

/// <summary>
/// Descripción breve de Alertas
/// </summary>
public class Alertas
{
    public static void MostrarModalError(Page page, string mensajeError)
    {
        string script = string.Format(@"<script type='text/javascript'>
                                $(document).ready(function() {{
                                    $('#modalBodyContent').text('{0}');
                                    $('#modalError').modal('show');
                                }});
                              </script>", mensajeError.Replace("'", "\\'"));
        page.ClientScript.RegisterStartupScript(page.GetType(), "ShowModalScript", script);
    }
}