using System;
using UnityEngine;
using UnityEngine.AI;

public static class CommonUtils {

    public class ComponentNotFoundException : SystemException
    {
        public ComponentNotFoundException(string message) : base(message) { }
        
        public ComponentNotFoundException(string message, Exception inner) : base (message, inner) { }

    }

    public class GameObjectNotFoundException : SystemException
    {
        public GameObjectNotFoundException(string message) : base(message) { }

        public GameObjectNotFoundException(string message, Exception inner) : base(message, inner) { }

    }

    public static bool IsInRange(int x, int y, int maxX, int maxY)
    {
        return x >= 0 && x < maxX && y >= 0 && y < maxY;
    }

    public static T GetComponentInGameControllerOrPanic<T>()
    {
        return GetComponentOrPanic<T>(FindGameObjectByTagOrPanic(CommonTags.GAME_CONTROLLER_TAG));
    }

    public static GameObject FindGameObjectByTagOrPanic(string tag)
    {
        GameObject go = GameObject.FindGameObjectWithTag(tag);

        if (go == null)
            throw new GameObjectNotFoundException("GameObject woth tag:" + tag + " not found");

        return go;
    }


    public static T GetComponentInGameObjectFoundWithTag<T>(string tag)
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

    public static void Move(Rigidbody rigidbody, Vector3 movement, float speed)
    {
        Vector3 newPos = rigidbody.transform.position + (movement.normalized * speed * Time.deltaTime);
        rigidbody.MovePosition(newPos);
    }

}
