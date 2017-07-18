using System;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;

namespace SerializerHelpers
{
    class XMLSerializer : ISerializer
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

            if (typeof(T).IsSerializable)
            {
                string savePath = Path.Combine(dataPath, fileName);
                StreamWriter writer = new StreamWriter(savePath);
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(writer, param);
                    return true;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    writer.Close();
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
                StreamReader fileStream = new StreamReader(savePath);          
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    T data = (T)serializer.Deserialize(fileStream);
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
                    fileStream.Close();
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
