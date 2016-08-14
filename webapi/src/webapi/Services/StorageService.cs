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

        public void Add(string key, object value, Stream stream)
        {
            _responseStorage.Add(key, value);
            _fileStorage.Add(key, stream);
        }

        public object[] Get(string key)
        {
            var response = new object[] { _responseStorage[key], _fileStorage[key] };
            return response;
        }

        public void Remove (string key)
        {
            _responseStorage.Remove(key);
            _fileStorage.Remove(key);
        }
    }
}
