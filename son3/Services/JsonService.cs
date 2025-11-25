using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using son3.Models;
namespace son3.Services
{
    public class JsonService
    {
        private readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public async Task<List<Software>> LoadAsync(string path)
        {
            if (!File.Exists(path))
                return new List<Software>();

            using var stream = File.OpenRead(path);
            var items = await JsonSerializer.DeserializeAsync<List<Software>>(stream, _options);
            if (items == null)
                return new List<Software>();

            return items;
        }

        public async Task SaveAsync(string path, List<Software> items)
        {
            using var stream = File.Create(path);
            await JsonSerializer.SerializeAsync(stream, items, _options);
        }
    }
}
