using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using System.IO;
using PCLStorage;
using System.Collections;
using System.Threading.Tasks;

namespace Cards.Core
{
    public static class JSONSerialManager
    {

        public static void serialize(string path, object toSerialize)
        {
            using (StreamWriter sw = new StreamWriter(path,false))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.TypeNameHandling = TypeNameHandling.Auto;
                serializer.Serialize(writer, toSerialize);
            }
        }

        public static T deserialize<T>(string path)
        {
            T fileReadJSON;

            try
            {
                using (StreamReader sr = new StreamReader(path))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    JsonSerializer deserializer = new JsonSerializer();
                    deserializer.TypeNameHandling = TypeNameHandling.Auto;
                    fileReadJSON = deserializer.Deserialize<T>(reader);
                }
            }
            catch(Exception e)
            {
                fileReadJSON = default(T); 
            }

            return fileReadJSON; 
        }

        public async static Task<List<T>> deserializeDirAsync<T>(IFolder dir)
        {
            IList<IFile> files = await dir.GetFilesAsync(); 
            List<T> filesReadJSON = new List<T>(files.Count); 
            foreach(IFile file in files)
            {
                T deserialized = deserialize<T>(file.Path); 
                if( !Object.Equals(deserialized, default(T) ) )
                    filesReadJSON.Add(deserialized); 
            }
            return filesReadJSON;
        }

       
    }

}