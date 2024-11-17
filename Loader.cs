using UnityEngine;

namespace UnifromEngine
{
    public class Loader
    {
        public static GameObject LoaderObject;
        
        public static void Load()
        {
            LoaderObject = new GameObject("UnifromLoader");
            LoaderObject.AddComponent<Engine>();
            
            Object.DontDestroyOnLoad(LoaderObject);
        } 
    }
}