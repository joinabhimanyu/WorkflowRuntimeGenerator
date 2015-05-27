using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Microsoft.AspNet.FriendlyUrls;


namespace WorkflowRuntimeGenerator
{
    /// <summary>
    /// Summary description for DownloadFile
    /// </summary>
    public class DownloadFile : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string param = "";
            try
            {
                if (context.Request.QueryString["param"] != null)
                {
                    param = context.Request.QueryString["param"].ToString().Trim();

                }

                //if (context.Request.GetFriendlyUrlSegments().Count() > 0)
                //{
                //    IList<string> segments = context.Request.GetFriendlyUrlSegments();
                //    param = segments[0].ToString().Trim();
                //}
                
                string fileName = context.Server.MapPath("~/Downloaded/" + param.Trim());
                System.IO.FileInfo fileinfo = new System.IO.FileInfo(fileName.Trim());

                if (fileinfo.Exists)
                {
                    context.Response.Clear();
                    context.Response.ContentType = "text/xml";
                    context.Response.AddHeader("content-disposition", "attachment;fileName=" + fileinfo.Name);
                    context.Response.AddHeader("content-length", fileinfo.Length.ToString().Trim());
                    context.Response.TransmitFile(fileinfo.FullName);
                    //context.Response.Write("Hello World, you entered " + param.Trim());
                    context.Response.Flush();
                }

                else
	            {
                    throw new Exception ("File not found");
	            }
            }
            catch (Exception ex)
            {

                context.Response.ContentType = "text/plain";
                context.Response.Write(ex.Message);

            }
            finally
            {
                context.Response.End();
            }
            
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}