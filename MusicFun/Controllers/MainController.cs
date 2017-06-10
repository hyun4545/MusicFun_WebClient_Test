using Lib.Web.Mvc;
using MusicFun.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TagLib;

namespace MusicFun.Controllers
{
    [RoutePrefix("Main")]
    public class MainController : Controller
    {
        /*private bool isAntiToken {
            get {
                if (HttpContext.Items.Contains("isAnti"))
                {
                    return (bool)HttpContext.Items["isAnti"];
                }
                else {
                    return false;
                }
            }
        }*/
        private const string boundary = "----ferfefjeofjfjejf";
        private const string TOKEN_NAME = "access_token";
        private const string  MAIN_URL = "http://webapplication5020160822045658.azurewebsites.net/api/Account/";
   
        // GET: Main
        public async Task<ActionResult> Index()
        {

            if (Request.Cookies["access_token"] != null && Request.Cookies["access_token"].Value != null)
            {               
                    string token = Request.Cookies["access_token"].Value;
                    Request request = new Request(MAIN_URL + "getUser");
                    request.SetToken(TOKEN_NAME, token);
                    Response response = await request.GetAsync();
                    TempData["user"] = await response.GetTxtAsync();
                    return View();             
                                
                
            }
            else {
               return RedirectToAction("LoginView");
            }
            
           
        }

        public class MyResponse
        {
            
            public int status { get; set; }
            public string message { get; set; }
            public string name { get; set; }
        }

        public async Task<ContentResult> Register(member m)
        {
            var json = new JavaScriptSerializer().Serialize(m);
            Request request = new Request(MAIN_URL + "Register");         
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            Response response=await request.PostAsync(content);
            MyResponse resp=await response.GetJsonObjAsync<MyResponse>();          
            HttpCookie cookie = new HttpCookie("access_token", resp.message);
            cookie.HttpOnly = true;            
            Response.Cookies.Add(cookie);             
            return Content(resp.status.ToString());

        }

        public async Task<ContentResult> Login(member2 m)
        {
            var json = new JavaScriptSerializer().Serialize(m);
            Request request=
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response;
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            response = await httpClient.PostAsync(MAIN_URL + "Login",content);
            JavaScriptSerializer js = new JavaScriptSerializer();
            var objText = await response.Content.ReadAsStringAsync();
            MyResponse obj = js.Deserialize<MyResponse>(objText);
            //return obj;
            /*var json = new JavaScriptSerializer().Serialize(m);
            Request request = new Request(MAIN_URL + "Login");
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            Response response = await request.PostAsync(content);
            var resp = await response.GetJsonObjAsync<MyResponse>();*/
            if (obj.status == 1) {
                HttpCookie cookie = new HttpCookie("access_token", obj.message);
                cookie.HttpOnly = true;
                Response.Cookies.Add(cookie);
            }            
            return Content(obj.status.ToString());
        }

        public PartialViewResult LoginView() {
            HttpCookie cookie = new HttpCookie("antiForgery", GetAntiForgeryToken());
            cookie.HttpOnly = true;
            Response.Cookies.Add(cookie);
            return PartialView("Login");
        }
        public ActionResult UploadList()
        {
            return PartialView("UploadList");
        }
       [HttpPost]
        public async Task<int> ListSongsCount(string url)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://webapplication5020160822045658.azurewebsites.net/api/Account/" + url);
            string token = Request.Cookies["access_token"].Value;
            request.Headers.Add("access_token", token);
            request.Method = "GET";
            request.Accept = "application/json";
            IEnumerable<MusicList> myojb;
            using (var twitpicResponse = (HttpWebResponse)await request.GetResponseAsync())
            {

                using (var reader = new StreamReader(twitpicResponse.GetResponseStream()))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var objText = reader.ReadToEnd();
                    myojb = (IEnumerable<MusicList>)js.Deserialize(objText, typeof(IEnumerable<MusicList>));

                }


            }
            return myojb.Count();
           }
        [HttpPost]
        public async Task<int> ListSongsCount2(int list_id)
        {

            string url = "http://webapplication5020160822045658.azurewebsites.net/api/Account/GetListSong";
            HttpClient client = new HttpClient();
            string token = Request.Cookies["access_token"].Value;
            client.DefaultRequestHeaders.Add("access_token", token);
            MultipartFormDataContent mulContent = new MultipartFormDataContent("----ferfefjeofjfjejf");
            mulContent.Add(new StringContent(list_id.ToString()), "list_id");
            
            var response = await client.PostAsync(url, mulContent);            
            string result = await response.Content.ReadAsStringAsync();            
           
            IEnumerable<MusicList> myojb;
            var stream = await response.Content.ReadAsStreamAsync();
                using ( var reader = new StreamReader(stream))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var objText = reader.ReadToEnd();
                    myojb = (IEnumerable<MusicList>)js.Deserialize(objText, typeof(IEnumerable<MusicList>));

                }
            return myojb.Count();

        }


        public ActionResult ListSongs(string url)
        {
            System.Diagnostics.Debug.WriteLine(url);
            System.Diagnostics.Debug.WriteLine("EWFEWQFEW");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://webapplication5020160822045658.azurewebsites.net/api/Account/" + url);
            //eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjU4Zjg1ZjRjLTY4ZTAtNDQ2NC04ZjhlLWQyNTU5NTk0NDg0NyIsImVtYWlsIjoiZmpqIn0.5y4MRuONaoK1QsJ_BEzoiOG01bqKsW7__kaLs2_3cU4
            string token = Request.Cookies["access_token"].Value;
            request.Headers.Add("access_token", token);
            request.Method = "GET";
            request.Accept = "application/json";           
            IEnumerable<MusicList> myojb;
            using (var twitpicResponse = (HttpWebResponse)request.GetResponse())
            {

                using (var reader = new StreamReader(twitpicResponse.GetResponseStream()))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var objText = reader.ReadToEnd();
                    myojb = (IEnumerable<MusicList>)js.Deserialize(objText, typeof(IEnumerable<MusicList>));
                    
                }
               

            }
            
            return PartialView("MusicList", myojb);
        }
       
        public async Task<ActionResult> OtherListSongs(int list_id)
        {
            string url = "http://webapplication5020160822045658.azurewebsites.net/api/Account/GetListSong";
            HttpClient client = new HttpClient();
            string token = Request.Cookies["access_token"].Value;
            client.DefaultRequestHeaders.Add("access_token", token);
            IEnumerable<MusicList> myojb;
            MultipartFormDataContent mulContent = new MultipartFormDataContent("----ferfefjeofjfjejf");
            mulContent.Add(new StringContent(list_id.ToString()), "list_id");
            HttpResponseMessage response;
            response = await client.PostAsync(new Uri(url), mulContent);
            // response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            using (var reader = new StreamReader(stream))
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                var objText = reader.ReadToEnd();
                myojb = (IEnumerable<MusicList>)js.Deserialize(objText, typeof(IEnumerable<MusicList>));

            }
            return PartialView("MusicList", myojb);
        }
        [HttpPost]
        public async Task<ContentResult> AddList(string list_name)
        {
            string url = "http://webapplication5020160822045658.azurewebsites.net/api/Account/AddList";
            HttpClient client = new HttpClient();
            string token = Request.Cookies["access_token"].Value;
            client.DefaultRequestHeaders.Add("access_token", token);
         
            MultipartFormDataContent mulContent = new MultipartFormDataContent("----ferfefjeofjfjejf");            
            mulContent.Add(new StringContent(list_name), "list_name");
            HttpResponseMessage response;
            response = await client.PostAsync(new Uri(url), mulContent);
            // response.EnsureSuccessStatusCode();
            return Content((response.StatusCode==HttpStatusCode.OK).ToString());
        }
        public ActionResult MusicList(string keyword)           
        {
           
                
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://webapplication5020160822045658.azurewebsites.net/api/Account/ListMusic");
            //eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjU4Zjg1ZjRjLTY4ZTAtNDQ2NC04ZjhlLWQyNTU5NTk0NDg0NyIsImVtYWlsIjoiZmpqIn0.5y4MRuONaoK1QsJ_BEzoiOG01bqKsW7__kaLs2_3cU4
            string token = Request.Cookies["access_token"].Value;
            request.Headers.Add("access_token", token);
            request.Method = "GET";           
            request.Accept = "application/json";
            IEnumerable<MusicList> myojb;
            using (var twitpicResponse = (HttpWebResponse)request.GetResponse())
            {

                using (var reader = new StreamReader(twitpicResponse.GetResponseStream()))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var objText = reader.ReadToEnd();
                    myojb = (IEnumerable<MusicList>)js.Deserialize(objText, typeof(IEnumerable<MusicList>));
                   
                }
                if (!string.IsNullOrEmpty(keyword)) {
                    myojb = myojb.Where(c => c.song_title.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
                }
              
            }
            return PartialView("MusicList",myojb);
        }
        public  class member
        {
            //public System.Guid id { get; set; }
            public string email { get; set; }
            public string name { get; set; }
            public string password { get; set; }
        }
        public class member2
        {
            //public System.Guid id { get; set; }
            public string email { get; set; }
            
            public string password { get; set; }
        }
        // GET: Main
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            return PartialView("List");
        }
        public ContentResult Upload2()
        {
            var file = Request.Files["file"];
            TagLib.File myFile = TagLib.File.Create(new HttpPostedFileAbstraction(file));
            int duration = (myFile.Properties.Duration.Minutes * 60 + myFile.Properties.Duration.Minutes) * 1000;
            duration += myFile.Properties.Duration.Milliseconds;
            string name="";
                int i = myFile.Name.LastIndexOf(".");
            if (i > 0) {
                 name = myFile.Name.Remove(i);
            }
               
           
            return Content(duration.ToString()+"  author: "+myFile.Tag.Performers.FirstOrDefault()+"  title: "+name);
        }
        public class HttpPostedFileAbstraction : TagLib.File.IFileAbstraction
        {
            private HttpPostedFileBase file;

            public HttpPostedFileAbstraction(HttpPostedFileBase file)
            {
                this.file = file;
            }

            public string Name
            {
                get { return file.FileName; }
            }

            public System.IO.Stream ReadStream
            {
                get { return file.InputStream; }
            }

            public System.IO.Stream WriteStream
            {
                get { throw new Exception("Cannot write to HttpPostedFile"); }
            }

            public void CloseStream(System.IO.Stream stream) { }
        }
        public async Task<ContentResult> Upload()
        {
            
            string url = "http://webapplication5020160822045658.azurewebsites.net/api/Account/upload3";
            HttpClient client = new HttpClient();
            string token = Request.Cookies["access_token"].Value;
            client.DefaultRequestHeaders.Add("access_token", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjU4Zjg1ZjRjLTY4ZTAtNDQ2NC04ZjhlLWQyNTU5NTk0NDg0NyIsImVtYWlsIjoiZmpqIn0.5y4MRuONaoK1QsJ_BEzoiOG01bqKsW7__kaLs2_3cU4");
            var file = Request.Files["file"];
            var stream = file.InputStream;
            HttpContent fileContent = new StreamContent(stream);
            TagLib.File myFile = TagLib.File.Create(new HttpPostedFileAbstraction(file));
            MultipartFormDataContent mulContent = new MultipartFormDataContent("----ferfefjeofjfjejf");
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            mulContent.Add(fileContent, "file", file.FileName);
            int duration = (myFile.Properties.Duration.Minutes * 60 + myFile.Properties.Duration.Minutes) * 1000;
            duration += myFile.Properties.Duration.Milliseconds;
            mulContent.Add(new StringContent(duration.ToString()), "song_duration");
            mulContent.Add(new StringContent(myFile.Tag.Title), "song_name");
            mulContent.Add(new StringContent(myFile.Tag.Performers.FirstOrDefault()), "author");
            HttpResponseMessage response;
            response = await client.PostAsync(new Uri(url), mulContent);
           // response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            return Content(result);
        }
        public async Task<ContentResult> Modify()
        {
            string song_id = Request.Form["id"];
            string author = Request.Form["author"];
            string song_name = Request.Form["song_name"];
            string url = "http://webapplication5020160822045658.azurewebsites.net/api/Account/Modify/"+song_id;
            HttpClient client = new HttpClient();
            string token = Request.Cookies["access_token"].Value;
            client.DefaultRequestHeaders.Add("access_token", token);
            MultipartFormDataContent mulContent = new MultipartFormDataContent("----ferfefjeofjfjejf");
            mulContent.Add(new StringContent(author), "author");
            mulContent.Add(new StringContent(song_name), "song_name");
            HttpResponseMessage response;
            response = await client.PostAsync(url, mulContent);
            // response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();

            return Content(response.StatusCode.ToString());
        }
        public async Task<ContentResult> Delete()
        {
            string song_id = Request.Form["id"];            
            string url = "http://webapplication5020160822045658.azurewebsites.net/api/Account/Delete/" + song_id;
            HttpClient client = new HttpClient();
            string token = Request.Cookies["access_token"].Value;
            client.DefaultRequestHeaders.Add("access_token", token);
          
            HttpResponseMessage response;
            response = await client.GetAsync(url);
            // response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();

            return Content(response.StatusCode.ToString());
        }
         public async Task<ContentResult> Upload3()
         {

             //string url = "http://webapplication5020160822045658.azurewebsites.net/api/Account/upload3";
             // HttpClient client = new HttpClient();
             Request request = new Request(MAIN_URL + "upload3");
             string token = Request.Cookies["access_token"].Value;
             request.SetToken(TOKEN_NAME, token);
             //client.DefaultRequestHeaders.Add("access_token", token);
             var file = Request.Files["file"];

             var stream =  file.InputStream;
             //var buffer = new BufferedStream(stream,int.Parse(stream.Length.ToString()));
            // var memory = new MemoryStream();
             //await stream.CopyToAsync(memory);
             //memory.Seek(0, SeekOrigin.Begin);
             //HttpContent fileContent = new StreamContent(memory);            
             MultipartFormDataContent mulContent = new MultipartFormDataContent(boundary);
             mulContent.AddFileAsync(file.FileName, stream);
            // var name = HttpUtility.UrlDecode(file.FileName, System.Text.Encoding.UTF8);
            // name = HttpUtility.UrlEncode(name, System.Text.Encoding.UTF8);
             //mulContent.Add(fileContent, "file", file.FileName) ;
             TagLib.File myFile = TagLib.File.Create(new HttpPostedFileAbstraction(file));
             int duration = (myFile.Properties.Duration.Minutes * 60 + myFile.Properties.Duration.Seconds) * 1000;
             duration += myFile.Properties.Duration.Milliseconds;

             var author = Encoder(myFile.Tag.Performers.FirstOrDefault());
             var duration_s = Encoder(duration.ToString()); 
             var title = Encoder(myFile.Tag.Title);
             if (title == "") {

                 int i = myFile.Name.LastIndexOf(".");
                 if (i > 0)
                 {
                    title = myFile.Name.Remove(i);
                 }

             }
             mulContent.AddTxt("song_duration", duration_s);
             mulContent.AddTxt("song_name", title);
             mulContent.AddTxt("author", author);
             //mulContent.Add(new StringContent(duration_s), "song_duration");
             //mulContent.Add(new StringContent(title), "song_name");
             //mulContent.Add(new StringContent(author), "author");
                
                 Response response = await request.PostAsync(mulContent);

             //response = await client.PostAsync(url, mulContent);
             // response.EnsureSuccessStatusCode();
             //string result = await response.Content.ReadAsStringAsync();

             return Content(response.GetStatusCode().ToString());
         }
       /* public async Task<ContentResult> Upload3()
        {

            string url = "http://webapplication5020160822045658.azurewebsites.net/api/Account/upload3";
            HttpClient client = new HttpClient();

            string token = Request.Cookies["access_token"].Value;

            client.DefaultRequestHeaders.Add("access_token", token);
            var file = Request.Files["file"];

            var stream = file.InputStream;
            //var buffer = new BufferedStream(stream,int.Parse(stream.Length.ToString()));
            var memory = new MemoryStream();
            stream.CopyTo(memory);
            memory.Seek(0, SeekOrigin.Begin);
            HttpContent fileContent = new StreamContent(memory);
            MultipartFormDataContent mulContent = new MultipartFormDataContent(boundary);
            // mulContent.AddFile(file.FileName, stream);
            var name = HttpUtility.UrlDecode(file.FileName, System.Text.Encoding.UTF8);
            // name = HttpUtility.UrlEncode(name, System.Text.Encoding.UTF8);
            mulContent.Add(fileContent, "file", file.FileName);
            TagLib.File myFile = TagLib.File.Create(new HttpPostedFileAbstraction(file));
            int duration = (myFile.Properties.Duration.Minutes * 60 + myFile.Properties.Duration.Seconds) * 1000;
            duration += myFile.Properties.Duration.Milliseconds;

            var author = Encoder(myFile.Tag.Performers.FirstOrDefault());
            var duration_s = Encoder(duration.ToString());
            var title = Encoder(myFile.Tag.Title);
            if (title == "")
            {

                int i = myFile.Name.LastIndexOf(".");
                if (i > 0)
                {
                    title = HttpUtility.UrlEncode(myFile.Name.Remove(i), System.Text.Encoding.UTF8);
                }

            }

            mulContent.Add(new StringContent(duration_s), "song_duration");
            mulContent.Add(new StringContent(title), "song_name");
            mulContent.Add(new StringContent(author), "author");

            var response = await client.PostAsync(url, mulContent);
            // response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();

            return Content(response.StatusCode.ToString());
        }*/
        public async Task<ContentResult> addSongToList(int list_id, int song_id)
        {            
            Request request = new Request(MAIN_URL + "AddListSong");
            string token = Request.Cookies["access_token"].Value;
            request.SetToken(TOKEN_NAME, token);           
            MultipartFormDataContent mulContent = new MultipartFormDataContent(boundary);
            mulContent.AddTxt("list_id", list_id.ToString());
            mulContent.AddTxt("song_id", song_id.ToString());           
            Response response = await request.PostAsync(mulContent);
            return Content((response.GetStatusCode() == HttpStatusCode.OK).ToString());
        }

        public async Task<JsonResult> getSongsList()
        {           
            Request request = new Request(MAIN_URL + "MusicLists");            
            string token = Request.Cookies["access_token"].Value;
            request.SetToken("access_token", token);            
            Response response = await request.GetAsync();
            IEnumerable<SongLists> myojb=await response.GetJsonObjAsync<IEnumerable<SongLists>>();           
            return Json(myojb, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> getSongList()
        {            
            string token = Request.Cookies["access_token"].Value;           
            Request request = new Request("http://webapplication5020160822045658.azurewebsites.net/api/Account/MusicLists");
            request.SetToken("access_token",token);
            Dictionary <string,int> count=new Dictionary<string, int>();
            List<ListInfo> listInfo = new List<ListInfo>();
            IEnumerable<SongLists> myojb;
            Response response=await request.GetAsync();
            myojb=await response.GetJsonObjAsync<IEnumerable<SongLists>>();          
            foreach (var a in myojb)
            {
                listInfo.Add(new ListInfo() { songLists = a, list_count = await ListSongsCount2(a.Id) });
            }
            count.Add("CurrentList",await ListSongsCount("CurrentList"));
            count.Add("AlwaysList", await ListSongsCount("AlwaysList"));
            count.Add("LatestList", await ListSongsCount("LatestList"));
            var tupleModel = new Tuple<List<ListInfo>, Dictionary<string, int>>(listInfo,count);
            return View("List",tupleModel);

        }

        public static string Encoder(string s) {
            if (s != null)
            {
                return HttpUtility.UrlDecode(s, System.Text.Encoding.UTF8);

            }
            else {

                return "";
            }
            
        }
        public PartialViewResult Logout() {
            if (Request.Cookies["access_token"] != null)
            {
                HttpCookie myCookie = new HttpCookie("access_token");
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(myCookie);
            }
            return PartialView("Login");
        }
        [Route("Stream/{id}")]
        public RangeFileContentResult MusicStream(int id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://webapplication5020160822045658.azurewebsites.net/api/Account/GetStream/"+ id);
            //eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjU4Zjg1ZjRjLTY4ZTAtNDQ2NC04ZjhlLWQyNTU5NTk0NDg0NyIsImVtYWlsIjoiZmpqIn0.5y4MRuONaoK1QsJ_BEzoiOG01bqKsW7__kaLs2_3cU4
            string token = Request.Cookies["access_token"].Value;
            request.Headers.Add("access_token", token);
            request.Method = "GET";
            //FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTP_URL + "/music/32059e31-a667-4d0c-a7f0-1449bf49a856/2PM “우리집(My House)” Music Video.mp3" );
            // request.Method = WebRequestMethods.Ftp.DownloadFile;

            // request.Credentials = new NetworkCredential(FTP_USERNAME, FTP_PASSWORD);

            //FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            var response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                return new RangeFileContentResult(ReadFully(stream), "audio/mp3", "oo.mp3", DateTime.Now);
           
            }
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[10 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        public static Stream ReadFully2(Stream input)
        {
            byte[] buffer = new byte[10 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms;
            }
        }
        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
        [Route("Stream2/{id}")]
        public string Stream(int id)
        {

           
            return "stream"+id;
        }
        public ActionResult Play(bool? isPlay,int? idd)
        {
            if (isPlay != null) {
                TempData["isPlay"] = isPlay;
            }
                
            if (idd != null)
            {
                TempData["id"] = idd;
            }
            
            
            return Redirect("Index");
        }

        private static string GetAntiForgeryToken() {
            string cookieToken, formToken;
            AntiForgery.GetTokens(null, out cookieToken, out formToken);
            return String.Concat(cookieToken, ":", formToken);
        }
    }
}