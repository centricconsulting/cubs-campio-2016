using System.IO;

namespace webapi.Services
{
    public interface IStorageService
    {
        void Add(string key, object value, Stream stream);
        void Remove(string key);
        object[] Get(string key);
    }
}