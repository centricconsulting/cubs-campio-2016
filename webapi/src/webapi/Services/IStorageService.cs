using Microsoft.AspNetCore.Http;
using System.IO;

namespace webapi.Services
{
    public interface IStorageService
    {
        void Add(string key, object value, IFormFile file);
        void Remove(string key);
        object[] Get(string key);
        void Clear();
    }
}