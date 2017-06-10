using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace MusicFun.Models
{
    public class Response
    {
        readonly HttpContent httpContent;
        readonly HttpResponseMessage response;
        public Response(HttpResponseMessage response) {
            this.httpContent = response.Content;
            this.response = response;
        }

        public async Task<Stream> GetStreamAsync()
        {
            using (Stream s = await httpContent.ReadAsStreamAsync()) {
                using (MemoryStream memory= await CopyStream(s)) {
                    return memory;
                } 
            }
           

        }

        public async Task<T> GetJsonObjAsync<T>()
        {

            JavaScriptSerializer js = new JavaScriptSerializer();
            var objText = await httpContent.ReadAsStringAsync();
            T obj = js.Deserialize<T>(objText);
            return obj;

            
        }

        public async Task<string> GetTxtAsync()
        {
            return await httpContent.ReadAsStringAsync();
        }

        public HttpStatusCode GetStatusCode() {
            return response.StatusCode;
        } 
        private static async Task<MemoryStream> CopyStream(Stream stream)
        {
            using (var memory = new MemoryStream())
            {
                await stream.CopyToAsync(memory);
                memory.Seek(0, SeekOrigin.Begin);
                return memory;
            }


        }
    }
}