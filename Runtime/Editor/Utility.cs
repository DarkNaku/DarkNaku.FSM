using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class Utility {
    public static T GetField<T>(object o, string fieldName) {
        if (o == null) {
            Debug.LogError("[Utility] GetField : Target is null.");
            return default;
        }

        var fieldInfo = o.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (fieldInfo == null) {
            Debug.LogErrorFormat("[Utility] GetField : Can't found '{0}' field.", fieldName);
            return default;
        }

        return (T)(fieldInfo.GetValue(o));
    }

    public static void SetField<T>(object o, string fieldName, object v) {
        if (o == null) {
            Debug.LogError("[Utility] SetField : Target is null.");
            return;
        }

        var fieldInfo = o.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (fieldInfo == null) {
            Debug.LogErrorFormat("[Utility] SetField : Can't found '{0}' field.", fieldName);
            return;
        }

        fieldInfo.SetValue(o, v);
    }

    public static int GetLocalIdentifierInFile(UnityEngine.Object obj) {
#if UNITY_EDITOR

        PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);

        SerializedObject serializedObject = new SerializedObject(obj);

        inspectorModeInfo.SetValue(serializedObject, InspectorMode.Debug, null);

        SerializedProperty localIdProp = serializedObject.FindProperty("m_LocalIdentfierInFile");

        return localIdProp.intValue;
#else
	    return -1;
#endif
    }

    public static string ToMD5(this string s) {
        var builder = new StringBuilder();
        byte[] bytes = Encoding.ASCII.GetBytes(s);
        byte[] result = (new System.Security.Cryptography.MD5CryptoServiceProvider()).ComputeHash(bytes);

        for (int i = 0; i < result.Length; i++) {
            builder.Append(result[i].ToString("X2"));
        }

        return builder.ToString();
    }
}
