using System;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.XPath;


namespace Sitecore.Sharepoint.Web.Extensions
{
  using System.Net;
  using System.Text;

  public class SharepointExtension
  {
 

    /// <summary>
    /// Writes the authentication response in basic format.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="response">The response.</param>
    public static void WriteAuthenticationResponseBasic(HttpRequest request, HttpResponse response)
    {
      if (!(request.Headers.AllKeys.Contains("Authorization")) || (request.Headers["Authorization"].StartsWith("Basic")))
      {
        response.Clear();
        response.StatusCode = 401;
        response.StatusDescription = "Unauthorized";
        response.ContentType = "text/html";
        response.AppendHeader("WWW-Authenticate", "Basic");
        response.Write(
          "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\"><HTML><HEAD><TITLE>You are not authorized to view this page</TITLE><META HTTP-EQUIV=\"Content-Type\" Content=\"text/html; charset=Windows-1252\"><STYLE type=\"text/css\">  BODY { font: 8pt/12pt verdana }  H1 { font: 13pt/15pt verdana }  H2 { font: 8pt/12pt verdana }  A:link { color: red }  A:visited { color: maroon }</STYLE></HEAD><BODY><TABLE width=500 border=0 cellspacing=10><TR><TD><h1>You are not authorized to view this page</h1>You do not have permission to view this directory or page using the credentials that you supplied.<hr><p>Please try the following:</p><ul><li>Contact the Web site administrator if you believe you should be able to view this directory or page.</li><li>Click the <a href=\"javascript:location.reload()\">Refresh</a> button to try again with different credentials.</li><li>Try to login to Sitecore CMS with the same credentials.</li></ul><h2>HTTP Error 401.1 - Unauthorized: Access is denied due to invalid credentials.<br>Sitecore CMS</h2></TD></TR></TABLE></BODY></HTML>");
      }
    }

    /// <summary>
    /// Gets the credentials from header.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    public static NetworkCredential GetCredentialsFromHeader(HttpRequest request)
    {
      NetworkCredential credential = null;
      if (request.Headers.AllKeys.Contains("Authorization"))
      {
        string authorization = request.Headers["Authorization"];
        if (authorization.StartsWith("Basic "))
        {
          string encryptedCredentials = authorization.Substring("Basic ".Length);
          byte[] encryptedBytes = System.Convert.FromBase64String(encryptedCredentials);
          string decryptedCredentials = Encoding.UTF8.GetString(encryptedBytes);
          if (!string.IsNullOrEmpty(decryptedCredentials) && decryptedCredentials.Contains(':'))
          {
            string userName = StringUtil.GetPrefix(decryptedCredentials, ':');
            string password = StringUtil.GetPostfix(decryptedCredentials, ':');
            credential = new NetworkCredential(userName, password);
          }
        }
      }
      return credential;
    }


  }
}