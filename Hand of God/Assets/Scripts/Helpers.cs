namespace Assets.Scripts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;

    using Random = UnityEngine.Random;

    internal class LogHelper
    {


        /// <summary>Writes text to Unity Console with a different color for the class.</summary>
        /// <param name="classname">USE: typeof(Class)</param>
        public static void Log(Type classname, string message)
        { Debug.Log("<color=teal>" + classname.FullName + ":</color> " + message); }

        /// <summary>
        /// Writes the error message in Debug.Log
        /// </summary>
        /// <param name="pClassName">USE: typeof(Class)</param>
        /// <param name="pMethod">USE: System.Reflection.MethodBase.GetCurrentMethod().Name unless in constructor</param>
        public static void WriteErrorMessage(Type pClassName, string pMethod, string pMessage)
        { Debug.Log("<color=red>ERROR in </color>" + pClassName.FullName + "." + pMethod + "() - Message: <color=red> " + pMessage + "</color>"); }

        /// <summary>
        /// Writes the error message in Debug.Log
        /// </summary>
        /// <param name="pClassName">USE: typeof(Class)</param>
        /// <param name="pMethod">USE: System.Reflection.MethodBase.GetCurrentMethod().Name unless in constructor</param>
        public static void WriteErrorMessage(Type pClassName, string pMethod, Exception pException)
        { Debug.Log("<color=red>ERROR in </color>" + pClassName.FullName + "." + pMethod + "() - Message: <color=red> " + pException.Message + "stacktrace:" + pException.StackTrace + "</color>"); }
    }

    public static class ExtensionMethods
    {
        // http://stackoverflow.com/questions/6976597/string-isnulloremptystring-vs-string-isnullorwhitespacestring
        public static bool IsNullEmptyOrWhitespace(this string s)
        { return s == null || s.All(char.IsWhiteSpace); }

        public static bool IsNullOrEmpty<T>(this IList<T> list)
        { return (list == null || list.Count == 0); }

        public static void Shuffle<T>(this IList<T> list)
        {
            if (list == null) return;
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
