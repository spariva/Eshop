using Newtonsoft.Json;

namespace Eshop.Extensions
{
    public static class SessionExtension
    {
        public static T GetObject<T>
            (this ISession session, string key)
        {
            string json = session.GetString(key);
            if (json == null)
            {
                return default(T);
            }
            T data = JsonConvert.DeserializeObject<T>(json);
            return data;
        }

        public static void SetObject
            (this ISession session, string key, Object value)
        {
            string data = JsonConvert.SerializeObject(value);
            session.SetString(key, data);
        }
    }
}
