using System;
using System.Runtime.Serialization;

namespace SerializerHelpers
{
    public interface ISerializeHelpers
    {
        bool TrySave<T>(string dataPath, string saveName, T param) where T : class;
        bool TryLoad<T>(string dataPath, string saveName, out T param) where T : class;
        bool DeleteSaveFile(string dataPath, string saveName);
    }
}