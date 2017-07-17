﻿using System;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;

namespace SerializerHelpers
{
    class JSONHelper : ISerializer
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
                    Debug.WriteLine(string.Format("Saved {0} successfully into {1}.", nameof(param), savePath));
                    return true;
                }
                catch (Exception sE)
                {
                    Debug.WriteLine(string.Format("Error when try to save {0} into {1}: {2}.", nameof(param), dataPath, sE));
                    return false;
                }
                finally
                {
                    streamWriter.Close();
                    jsonReader.Close();
                }
            }
            else
            {
                Debug.WriteLine(nameof(param) + " should be marked as serilizable first.");
                return false;
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
                    Debug.WriteLine(string.Format("Loaded {0} successfully from {1} into {2}.", nameof(param), savePath, param));
                    return true;
                }
                catch (JsonException)
                {
                    Debug.WriteLine(string.Format("Error when try to load {0} from {1}: Wrong json format.", nameof(param), dataPath));
                    param = default(T);
                    return false;
                }
                catch (Exception sE)
                {
                    param = default(T);
                    Debug.WriteLine(string.Format("Error when try to load {0} from {1}: {2}.", nameof(param), dataPath, sE.Message));
                    return false;
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
                Debug.WriteLine(string.Format("Coundn't find {0} in {1}.", nameof(param), dataPath));
                return false;
            }
        }

        public bool DeleteSaveFile(string dataPath, string fileName)
        {
            Debug.Assert(dataPath != null, "TrySave, null " + nameof(dataPath));
            Debug.Assert(fileName != null, "TrySave, null " + nameof(fileName));

            string savePath = Path.Combine(dataPath, fileName); if (File.Exists(savePath))
            {
                File.Delete(savePath);
                Debug.WriteLine(string.Format("Deleted {0} successfully in {1}.", fileName, savePath));
                return true;
            }
            else
            {
                Debug.WriteLine(string.Format("Error when try to delete {0} in {1}.", fileName, dataPath));
                return false;
            }
        }
    }
}
