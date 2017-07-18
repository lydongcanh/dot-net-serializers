using System;
using System.Runtime.Serialization;

namespace SerializerHelpers
{
    public interface ISerializer
    {
        bool TrySave<T>(string dataPath, string saveName, T param) where T : class;
        bool TryLoad<T>(string dataPath, string saveName, out T param) where T : class;
        void DeleteSaveFile(string dataPath, string saveName);
    }
}
