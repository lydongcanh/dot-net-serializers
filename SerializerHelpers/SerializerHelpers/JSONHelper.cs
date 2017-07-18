using System;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;

namespace SerializerHelpers
{
    class JSONSerializer : ISerializer
    {
        /// <summary>
        /// Save all data into the data path.
        /// </summary>
        /// <typeparam name="T">Type of the data. (Have to be ref type for now.)</typeparam>
        /// <param name="dataPath">Path to the data folder.</param>
        /// <param name="fileName">Save file name.</param>
        /// <param name="param">Object that will be saved.</param>
        /// <returns>Check if saved successfully.</returns>
        public bool TrySave<T>(string dataPath, string fileName, T param) where T : class
        {
            Debug.Assert(dataPath != null, "TrySave, null " + nameof(dataPath));
            Debug.Assert(fileName != null, "TrySave, null " + nameof(fileName));
            Debug.Assert(param != default(T), "TrySave, invalid " + nameof(param));

            if (param.GetType().IsSerializable)
            {
                string savePath = Path.Combine(dataPath, fileName);
                JsonSerializer serializer = new JsonSerializer();
                StreamWriter streamWriter = new StreamWriter(savePath);
                JsonTextWriter jsonReader = new JsonTextWriter(streamWriter);
                try
                {
                    serializer.Serialize(jsonReader, param);
                    return true;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    streamWriter.Close();
                    jsonReader.Close();
                }
            }
            else
            {
                throw new InvalidDataException("Unserializable type.");
            }
        }

        /// <summary>
        /// Load saved data in the data path.
        /// </summary>
        /// <typeparam name="T">Type of the data. (Have to be ref type for now.)</typeparam>
        /// <param name="dataPath">Path to the data folder.</param>
        /// <param name="fileName">Save file name.</param>
        /// <param name="param">All the data will be loaded into this.</param>
        /// <returns>Check if the save file exist or not.</returns>
        public bool TryLoad<T>(string dataPath, string fileName, out T param) where T : class
        {
            Debug.Assert(dataPath != null, "TrySave, null " + nameof(dataPath));
            Debug.Assert(fileName != null, "TrySave, null " + nameof(fileName));

            string savePath = Path.Combine(dataPath, fileName);
            if (File.Exists(savePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                StreamReader streamReader = new StreamReader(savePath);
                JsonTextReader jsonReader = new JsonTextReader(streamReader);
                try
                {
                    object jsonData = serializer.Deserialize(jsonReader);                    
                    T data = JsonConvert.DeserializeObject<T>(jsonData.ToString());
                    param = data;
                    return true;
                }
                catch (Exception)
                {
                    param = default(T);
                    throw;
                }
                finally
                {
                    streamReader.Close();
                    jsonReader.Close();
                }
            }
            else
            {
                param = default(T);
                throw new FileNotFoundException("Couldnt find " + fileName + " in " + savePath);
            }
        }

        public void DeleteSaveFile(string dataPath, string fileName)
        {
            Debug.Assert(dataPath != null, "TrySave, null " + nameof(dataPath));
            Debug.Assert(fileName != null, "TrySave, null " + nameof(fileName));

            string savePath = Path.Combine(dataPath, fileName);
            if (File.Exists(savePath))
            {
                try
                {
                    File.Delete(savePath);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                throw new FileNotFoundException("Couldnt find " + fileName + " in " + savePath);
            }
        }
    }
}
