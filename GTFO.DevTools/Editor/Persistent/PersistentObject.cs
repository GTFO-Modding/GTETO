using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Persistent
{
    public abstract class PersistentObject<T> : ScriptableObject
        where T : PersistentObject<T>
    {
        protected static string s_path;
        private static T s_instance;

        protected virtual void OnFirstCreate()
        { }

        public static T Instance
        {
            get
            {
                if (!s_instance)
                {
                    s_instance = AssetDatabase.LoadAssetAtPath<T>(s_path);
                    if (s_instance == null)
                    {
                        s_instance = CreateInstance<T>();
                        s_instance.OnFirstCreate();
                        AssetDatabase.CreateAsset(s_instance, s_path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }

                return s_instance;
            }
        }
    }
}
