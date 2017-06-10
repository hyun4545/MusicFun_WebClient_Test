using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace MusicFun.Models
{
    public class Request
    {
        readonly string url;
        HttpClient httpClient;
        Uri uri {
            get { return new Uri(url); }
        }
        public Request(string url) {
            httpClient = new HttpClient();
            this.url = url;
        }
       public void SetToken(string name,string token)
        {
            httpClient.DefaultRequestHeaders.Add(name, token);
        }

       public async Task<Response> GetAsync()
        {
            
            HttpResponseMessage response;
                response = await httpClient.GetAsync(url);
                return new Response(response);
           
           
        }

       public async Task<Response> PostAsync(HttpContent httpContent)
        {
            HttpResponseMessage response;
            response = await httpClient.PostAsync(url, httpContent);
            return new Response(response);
        }

    }
}