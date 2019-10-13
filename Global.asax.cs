using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

using System.Text;
using System.Net.Mail;
using UmbracoShipTac.Code;

namespace UmbracoShipTac
{
    public class Global : Umbraco.Web.UmbracoApplication
    {
        //DID not work the OR and IN search implementation
        //protected void Application_Start(object sender, EventArgs e)
        //{

        //    Lucene.Net.Analysis.StopAnalyzer.ENGLISH_STOP_WORDS_SET = new System.Collections.Hashtable();

  
        //}

        protected void Application_Error(object sender, EventArgs e)
        {

            // Get the error details
            var lastErrorWrapper = Server.GetLastError();

            Exception lastError = lastErrorWrapper;

            if (lastErrorWrapper.InnerException != null)
                lastError = lastErrorWrapper.InnerException;

            string lastErrorTypeName = lastError.GetType().ToString();
            string lastErrorMessage = lastError.Message;
            string lastErrorStackTrace = lastError.StackTrace;

  
            const string Subject = "An Error Has Occurred!";

            string htmlyellomsg = string.Empty;


                HttpUnhandledException httpUnhandledException =   new HttpUnhandledException(Server.GetLastError().Message, Server.GetLastError());
                htmlyellomsg = httpUnhandledException.GetHtmlErrorMessage();

             //If you do not call Server.ClearError or trap the error in the Page_Error or Application_Error event handler,
            //the error is handled based on the settings in the section of the Web.config file.

            //////Server.ClearError();

            //Prepare Mail Object
            ShiptalkMailMessage mm = new ShiptalkMailMessage(true, ShiptalkMailMessage.MailFrom.ShiptalkResourceCenter);


            mm.ToList.Add(ConfigUtil.CriticalErrorEmail);

            if (ConfigUtil.CriticalErrorEmailCC.Length>0)
                mm.ToList.Add(ConfigUtil.CriticalErrorEmailCC);

            mm.Subject = Subject;
        
            mm.Body = string.Format(@"
                                    <html>
                                    <body>
                                      <h1>An Error Has Occurred!</h1>
                                      <table cellpadding=""5"" cellspacing=""0"" border=""1"">
                                      <tr>
                                      <tdtext-align: right;font-weight: bold"">URL:</td>
                                      <td>{0}</td>
                                      </tr>
                                      <tr>
                                      <tdtext-align: right;font-weight: bold"">User:</td>
                                      <td>{1}</td>
                                      </tr>
                                      <tr>
                                      <tdtext-align: right;font-weight: bold"">Exception Type:</td>
                                      <td>{2}</td>
                                      </tr>
                                      <tr>
                                      <tdtext-align: right;font-weight: bold"">Message:</td>
                                      <td>{3}</td>
                                      </tr>
                                      <tr>
                                      <tdtext-align: right;font-weight: bold"">Stack Trace:</td>
                                      <td>{4}</td>
                                      </tr> 

                                      </table>
                                    </body>
                                    </html>
                                        {5}
                                        ",
                Request.RawUrl,
                User.Identity.Name,
                lastErrorTypeName,
                lastErrorMessage,
                lastErrorStackTrace.Replace(Environment.NewLine, "<br />"),
                htmlyellomsg
            )
            ;


            //// Attach the Yellow Screen of Death for this error   
            //string YSODmarkup = lastErrorWrapper.GetHtmlErrorMessage();
            //if (!string.IsNullOrEmpty(YSODmarkup))
            //{

            //}


            //Send Mail here
            if (ConfigUtil.WebEnvironment != "dev")
            {
            ShiptalkMail mail = new ShiptalkMail(mm);
            mail.SendMail();
            }



        }



    }
}