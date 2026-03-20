using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace P_NutriTrack_Patricny_Reis.Services
{
    public class DataServices
    {
        private const string JSON_FILE_NAME = "P_NutriTrack_Patricny&Reis.ServerJson.db.json";

        HttpClient _client;
        JsonSerializerOptions serializerOptions;

        public List<Aliment> Items { get; private set; }

        public RestService()
        {
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }
    }
}
