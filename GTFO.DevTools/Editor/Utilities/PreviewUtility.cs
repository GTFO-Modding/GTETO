using System;
using UnityEngine;

namespace GTFO.DevTools.Utilities
{
    public static class PreviewUtility
    {
        public static event Action<GameObject> DoPreview;
        public static event Action<GameObject> DoClearPreview;

        public static void CreatePreview(GameObject obj)
        {
            DoPreview?.Invoke(obj);
        }

        public static void ClearPreview(GameObject obj)
        {
            DoClearPreview?.Invoke(obj);
        }

        public static void MarkAsEditorOnly<T>(T obj)
            where T : UnityEngine.Object
        {
            if (obj is GameObject gameObj)
            {
                gameObj.tag = "EditorOnly";
            }
        }

        public static T Instantiate<T>(T source)
            where T : UnityEngine.Object
        {
            var copy = GameObject.Instantiate(source);
            MarkAsEditorOnly(copy);
            return copy;
        }

        public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation)
            where T : UnityEngine.Object
        {
            var copy = GameObject.Instantiate(original, position, rotation);
            MarkAsEditorOnly(copy);
            return copy;
        }

        public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform parent)
            where T : UnityEngine.Object
        {
            var copy = GameObject.Instantiate(original, position, rotation, parent);
            MarkAsEditorOnly(copy);
            return copy;
        }

        public static T Instantiate<T>(T original, Transform parent)
            where T : UnityEngine.Object
        {
            var copy = GameObject.Instantiate(original, parent);
            MarkAsEditorOnly(copy);
            return copy;
        }

        public static T Instantiate<T>(T original, Transform parent, bool worldPositionStays)
            where T : UnityEngine.Object
        {
            var copy = GameObject.Instantiate(original, parent, worldPositionStays);
            MarkAsEditorOnly(copy);
            return copy;

        }
    }
}
