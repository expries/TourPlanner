using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace TourPlanner.DAL
{
    /// <summary>
    /// Provides functionality to do HTTP requests combined with json (de)serialization
    /// </summary>
    public static class HttpConnection
    {
        /// <summary>
        /// Deserializes json response of HTTP GET request
        /// </summary>
        /// <param name="url"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>(string url)
        {
            string response = Get(url);
            var result = JsonConvert.DeserializeObject<T>(response);
            return result;
        }
        
        /// <summary>
        /// Returns response of GET request as string
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Get(string url)
        {
            var request = WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Get;
            return GetStringResponse(request);
        }


        /// <summary>
        /// Returns response of GET request as byte-array
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static byte[] GetBytes(string url)
        {
            var request = WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Get;
            return GetBinaryResponse(request);
        }

        /// <summary>
        /// Deserializes json response of POST request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Post<T>(string url, object data)
        {
            string response = Post(url, data);
            return JsonConvert.DeserializeObject<T>(response);
        }
        
        /// <summary>
        /// Returns response of POST request as string
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Post(string url, object data)
        {
            var request = WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/json";

            string content = JsonConvert.SerializeObject(data);
            byte[] contentBytes = System.Text.Encoding.UTF8.GetBytes(content);
            
            using var stream = request.GetRequestStream();
            stream.Write(contentBytes, 0, contentBytes.Length);
            return GetStringResponse(request);
        }

        /// <summary>
        /// Gets string response to WebRequest which has already sent its data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static string GetStringResponse(WebRequest request)
        {
            var response = request.GetResponse();
            var responseStream = response.GetResponseStream();

            if (responseStream is null)
            {
                return string.Empty;
            }
            
            var reader = new StreamReader(responseStream);
            var data = reader.ReadToEnd();
            return data;
        }

        /// <summary>
        /// Gets byte-array response to WebRequest which has already sent its data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static byte[] GetBinaryResponse(WebRequest request)
        {
            var response = request.GetResponse();
            var responseStream = response.GetResponseStream();

            if (responseStream is null)
            {
                return System.Array.Empty<byte>();
            }

            using var reader = new BinaryReader(responseStream);
            byte[] data = reader.ReadBytes(1024 * 1024 * 10);
            return data;
        }
    }
}