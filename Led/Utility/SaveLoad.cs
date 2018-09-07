using System.IO;
using System.Text;

namespace Led.Utility
{
    public static class SaveLoad
    {
        //public static string WriteFromObject(object data)
        //{
        //    //Create a stream to serialize the object to.  
        //    MemoryStream ms = new MemoryStream();

        //    // Serializer the object to the stream.
        //    DataContractJsonSerializer ser = new DataContractJsonSerializer(data.GetType());
        //    ser.WriteObject(ms, data);
        //    byte[] json = ms.ToArray();
        //    ms.Close();
        //    return Encoding.UTF8.GetString(json, 0, json.Length);
        //}

        //public static object ReadToObject(string json)
        //{
        //    object deserializedObject = new object();
        //    MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
        //    DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedObject.GetType());
        //    deserializedObject = ser.ReadObject(ms);
        //    ms.Close();
        //    return deserializedObject;
        //}
    }
}
