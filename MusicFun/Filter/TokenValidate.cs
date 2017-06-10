using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace MusicFun.Filter
{
    public class TokenValidate: System.Web.Http.Filters.ActionFilterAttribute
    {//antiForgery
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (HttpContext.Current.Request.Cookies["antiForgery"] != null )
            {
                string token = HttpContext.Current.Request.Cookies["antiForgery"].Value;
                try
                {
                   
                        ValidateRequestHeader(token);
                   
                }
                catch (HttpAntiForgeryException e)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
                }
            }
            else {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
            }
       }
        private void ValidateRequestHeader(string token)
        {
            string cookieToken = String.Empty;
            string formToken = String.Empty;            
            if (!String.IsNullOrEmpty(token))
            {
                string[] tokens = token.Split(':');
                if (tokens.Length == 2)
                {
                    cookieToken = tokens[0].Trim();
                    formToken = tokens[1].Trim();
                }
            }
            AntiForgery.Validate(cookieToken, formToken);
        }

    }

}
