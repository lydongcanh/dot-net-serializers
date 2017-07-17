﻿using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace SerializerHelpers
{
    public class BinaryHelper: ISerializer
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
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream fileStream = new FileStream(savePath, FileMode.OpenOrCreate);
                try
                {
                    binaryFormatter.Serialize(fileStream, param);
                    Debug.WriteLine(string.Format("Saved {0} successfully into {1}.", nameof(param), savePath));
                    return true;
                }
                catch (SerializationException sE)
                {
                    Debug.WriteLine(string.Format("Error when try to save {0} into {1}: {2}.", nameof(param), dataPath, sE));
                    return false;
                }
                finally
                {
                    fileStream.Close();
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
            Debug.Assert(dataPath != null, "TryLoad, null " + nameof(dataPath));
            Debug.Assert(fileName != null, "TryLoad, null " + nameof(fileName));

            string savePath = Path.Combine(dataPath, fileName);
            if (File.Exists(savePath))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream fileStream = new FileStream(savePath, FileMode.Open);
                try
                {
                    T data = (T)binaryFormatter.Deserialize(fileStream);
                    param = data;
                    Debug.WriteLine(string.Format("Loaded {0} successfully from {1} into {2}.", nameof(param), savePath, param));
                    return true;
                }
                catch (SerializationException sE)
                {
                    param = default(T);
                    Debug.WriteLine(string.Format("Error when try to load {0} from {1}: {2}.", nameof(param), dataPath, sE.Message));
                    return false;
                }
                finally
                {
                    fileStream.Close();
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
            Debug.Assert(dataPath != null, "DeleteSaveFile, null " + nameof(dataPath));
            Debug.Assert(fileName != null, "DeleteSaveFile, null " + nameof(fileName));

            string savePath = Path.Combine(dataPath, fileName);
            if (File.Exists(savePath))
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