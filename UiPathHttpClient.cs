using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Http;
using System.Globalization;
using System.IO;
using System.Activities;

namespace LGUPlus
{
    public class KMAResponse
    {
        public HttpStatusCode status { get; set; }
        public string body { get; set; }
    }
    public class UiPathHttpClient
    {

        public UiPathHttpClient() :
            this("https://ailab.synap.co.kr")
        {
        }
        public UiPathHttpClient( string endpoint)
        {
            this.url = endpoint;
            this.client = new HttpClient();
            this.content = new MultipartFormDataContent("clova----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
        }

        public void setEndpoint( string endpoint)
        {
            if (!string.IsNullOrEmpty(endpoint))
            {
                this.url = endpoint;
            }
        }
        public void setSecret( string secret, string headerName)
        {
            this.client.DefaultRequestHeaders.Add(headerName, secret);
        }
        public void setOCRSecret(string secret)
        {
            this.client.DefaultRequestHeaders.Add("X-OCR-SECRET", secret);
        }
        public void setSpeechSecret(string secret)
        {
            this.client.DefaultRequestHeaders.Add("X-CLOVASPEECH-API-KEY", secret);
        }

        public void AddFile(string fileName, string fieldName = "file")
        {
            var fstream = System.IO.File.OpenRead(fileName);
            byte[] buf = new byte[fstream.Length];
            int read_bytes = 0;
            int offset = 0;
            int remains = (int)fstream.Length;
            do
            {
                read_bytes += fstream.Read(buf, offset, remains);
                offset += read_bytes;
                remains -= read_bytes;
            } while (remains != 0);
            fstream.Close();

            this.content.Add(new StreamContent(new MemoryStream(buf)), fieldName, System.IO.Path.GetFileName(fileName));
        }

        public void AddField( string name, string value)
        {
            this.content.Add(new StringContent(value), name);
        }

        public void Clear()
        {
            this.content.Dispose();
            this.content = new MultipartFormDataContent("clova----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
        }

        public async Task<KMAResponse> Download()
        {
            using (var message = this.client.GetAsync(this.url))
            {
                KMAResponse resp = new KMAResponse();
                resp.status = message.Result.StatusCode;
                byte[] _bytebody = await message.Result.Content.ReadAsByteArrayAsync();
                resp.body = Encoding.UTF8.GetString(Encoding.Convert(Encoding.GetEncoding("euc-kr"), Encoding.UTF8, _bytebody));
                return resp;
            }
        }
        public async Task<KMAResponse> Upload()
        {
#if DEBUG
            Console.WriteLine("http content count :" + this.content.Count());
#endif
            using (var message = this.client.PostAsync(this.url, this.content))
            {
                KMAResponse resp = new KMAResponse();
                resp.status = message.Result.StatusCode;
                resp.body = await message.Result.Content.ReadAsStringAsync();
                return resp;
            }
        }


        private HttpClient client;
        private string url;
        private MultipartFormDataContent content;
    }
}
