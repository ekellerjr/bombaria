using System;
using UnityEngine;

public static class CommonUtils {

    public class ComponentNotFoundException : SystemException
    {
        public ComponentNotFoundException(string message) : base(message) { }
        
        public ComponentNotFoundException(string message, Exception inner) : base (message, inner) { }

    }

    public static bool IsInRange(int x, int y, int maxX, int maxY)
    {
        return x >= 0 && x < maxX && y >= 0 && y < maxY;
    }

    public static T FindComponentInGameObjectByTag<T>(string tag)
    {
        GameObject go = GameObject.FindGameObjectWithTag(tag);

        if (go == null)
            return default(T);

        return go.GetComponent<T>();
    }

    public static T GetComponentOrPanic<T>(GameObject go)
    {
        if (go == null)
            throw new ComponentNotFoundException("GameObject == null" );

        T component = go.GetComponent<T>();

        if(component == null)
            throw new ComponentNotFoundException("Component: " + typeof(T).Name + " not found in GameObject: " + go);

        return component;
    }
}
