#nullable enable

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace alterNERDtive.edts
{

    public struct StarSystem
    {
        public string Name { get; set; }
        public Position Coords { get; set; }
    }
    public struct Position
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int Precision { get; set; }
    }

    public class EdtsApi
    {
        private static readonly string APIURL = "http://edts.thargoid.space/api/v1/";
        private static HttpClient ApiClient;

        static EdtsApi()
        {
            ApiClient = new HttpClient
            {
                BaseAddress = new Uri(APIURL)
            };
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static StarSystem GetCoordinates(string name)
        {
            HttpResponseMessage response = ApiClient.GetAsync($"system_position/{name}").Result;
            
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) // 400
            {
                throw new ArgumentException($"“{name}” is not a valid proc gen system name.", "~system");
            }
            
            response.EnsureSuccessStatusCode();
            dynamic json = response.Content.ReadAsAsync<dynamic>().Result["result"];

            int x = json["position"]["x"];
            int y = json["position"]["y"];
            int z = json["position"]["z"];
            int uncertainty = json["uncertainty"];

            return new StarSystem { Name=name, Coords=new Position { X=x, Y=y, Z=z, Precision=uncertainty } };
        }
    }
}