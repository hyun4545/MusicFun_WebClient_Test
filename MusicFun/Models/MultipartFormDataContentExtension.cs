using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace MusicFun.Models
{
    public static class MultipartFormDataContentExtension
    {
        public static void AddFileAsync(this MultipartFormDataContent form, string fileName, Stream stream)
        {



            //MemoryStream memory = new MemoryStream();

            //stream.CopyTo(memory);
            //memory.Seek(0, SeekOrigin.Begin);
            stream.Seek(0, SeekOrigin.Begin);
            HttpContent streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            form.Add(streamContent, "file", fileName);
        }
        //new JavaScriptSerializer().Serialize(m);
        public static void AddTxt(this MultipartFormDataContent form, string name, string value)
        {
                HttpContent content = new StringContent(value);                
                form.Add(content, name);
           
        }

        private static async Task<MemoryStream> CopyStream(Stream stream)
        {

            var memory = new MemoryStream();
                
                    await stream.CopyToAsync(memory);
                    memory.Seek(0, SeekOrigin.Begin);
                    return memory;
               

           
           
           

        }
    }
    public static class StringContentExtension
    {
        public static void AddJson(this StringContent json, string name, object jsonObj)
        {
            string jsonString = new JavaScriptSerializer().Serialize(jsonObj);
            json = new StringContent(jsonString, Encoding.UTF8, "application/json");            

        }

    }
}