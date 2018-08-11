using System;
using UnityEngine;
using UnityEngine.AI;

public static class CommonUtils
{
    public enum GetComponentPostCommand
    {
        None,
        DestroyGameObject
    }

    public enum CompareTagsMode
    {
        MatchOneTag,
        MatchAllTags
    }

    public class ComponentNotFoundException : SystemException
    {
        public ComponentNotFoundException(string message) : base(message) { }

        public ComponentNotFoundException(string message, Exception inner) : base(message, inner) { }

    }

    public class GameObjectNotFoundException : SystemException
    {
        public GameObjectNotFoundException(string message) : base(message) { }

        public GameObjectNotFoundException(string message, Exception inner) : base(message, inner) { }

    }

    public static int RandomHashKey(string keyPart = "null")
    {
        keyPart = keyPart == null ? "null" : keyPart;
        return (keyPart + "." + System.DateTime.Now.Ticks).GetHashCode();
    }

    public static bool IsInRange(int x, int y, int maxX, int maxY)
    {
        return x >= 0 && x < maxX && y >= 0 && y < maxY;
    }

    public static T GetComponentInGameControllerOrPanic<T>(GetComponentPostCommand command = GetComponentPostCommand.None)
    {
        return GetComponentOrPanic<T>(FindGameObjectWithTagOrPanic(CommonTags.GAME_CONTROLLER), command);
    }

    public static GameObject FindGameObjectWithTagOrPanic(string tag)
    {
        GameObject go = GameObject.FindGameObjectWithTag(tag);

        if (go == null)
            throw new GameObjectNotFoundException("GameObject with tag:" + tag + " not found");

        return go;
    }

    public static T GetComponentInGameObjectFoundWithTagOrPanic<T>(string tag, GetComponentPostCommand command = GetComponentPostCommand.None)
    {
        return GetComponentOrPanic<T>(FindGameObjectWithTagOrPanic(tag), command);
    }

    public static T GetComponentInGameObjectFoundWithTag<T>(string tag, GetComponentPostCommand command = GetComponentPostCommand.None)
    {       
        return GetComponent<T>(GameObject.FindGameObjectWithTag(tag), command);
    }

    public static T GetComponentOrPanic<T>(GameObject go, GetComponentPostCommand command = GetComponentPostCommand.None)
    {
        if (go == null)
            throw new ComponentNotFoundException("GameObject == null");

        T component = GetComponent<T>(go, command);

        if (component == null)
            throw new ComponentNotFoundException("Component: " + typeof(T).Name + " not found in GameObject: " + go);

        return component;
    }

    public static T GetComponent<T>(GameObject go, GetComponentPostCommand command = GetComponentPostCommand.None)
    {
        if (go == null)
            return default(T);

        T component = go.GetComponent<T>();

        if (component != null && command == GetComponentPostCommand.DestroyGameObject)
            UnityEngine.Object.Destroy(go);

        return component;
    }

    public static void Move(Rigidbody rigidbody, Vector3 movement, float speed /*, bool normalize = false*/)
    {
        /*
        if (normalize)
        {
            rigidbody.MovePosition(rigidbody.transform.position + (movement.normalized * speed * Time.deltaTime));
        }
        else
        {
            */
        rigidbody.MovePosition(rigidbody.transform.position + (movement * speed * Time.deltaTime));
        /*}*/
    }

    public static bool CompareTags(GameObject gameObject, CompareTagsMode mode, params string[] tags)
    {

        bool match = false;

        foreach (string tag in tags)
        {
            match = gameObject.CompareTag(tag);

            switch (mode)
            {
                case CompareTagsMode.MatchOneTag:
                    if (match) return true;
                    break;
                case CompareTagsMode.MatchAllTags:
                    if (!match) return false;
                    break;
                default:
                    throw new System.Exception("CompareTagsMode: " + mode + " unknown");
            }

        }

        return match;
    }

}
