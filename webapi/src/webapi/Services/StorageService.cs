using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace webapi.Services
{
    public class StorageService : IStorageService
    {
        private Dictionary<string, object> _responseStorage;
        private Dictionary<string, object> _fileStorage;

        public StorageService()
        {
            _responseStorage = new Dictionary<string, object>();
            _fileStorage = new Dictionary<string, object>();
        }

        public void Add(string key, object value, IFormFile file)
        {
            _responseStorage.Add(key, value);
            _fileStorage.Add(key, file);
        }

        public object[] Get(string key)
        {
            if (!_responseStorage.ContainsKey(key))
                return null;

            var response = new object[] { _responseStorage[key], _fileStorage[key] };
            return response;
        }

        public void Remove(string key)
        {
            if (!_responseStorage.ContainsKey(key))
                return;

            _responseStorage.Remove(key);
            _fileStorage.Remove(key);

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", key + ".jpg");
            File.Delete(filePath);
        }

        public void Clear()
        {
            _responseStorage.Clear();
            _fileStorage.Clear();

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "images");
            if (!Directory.Exists(filePath)) return;

            foreach (string file in Directory.GetFiles(filePath, "*.jpg"))
            {
                File.Delete(file);
            }
        }
    }
}
