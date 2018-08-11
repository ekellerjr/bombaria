
using System;
using System.Collections.Generic;
using UnityEngine;

public class BombManager {

    private const String BOMB_OBJECTS_PATH_PREFIX = "scriptable_objects/bomb/";
    private const String ASSET_FILE_TYPE = ".asset";

    private static BombManager instance;

    public static BombManager GetInstance()
    {
        if(instance == null)
        {
            instance = new BombManager();
            instance.Init();
        }

        return instance;
    }

    private readonly Dictionary<BombType, BombAsset> cache = new Dictionary<BombType, BombAsset>();

    private BombManager() { }

    private void Init()
    {
        
    }

    public BombAsset GetBombAsset(BombType type)
    {
        if (cache.ContainsKey(type))
            return cache[type];
        
        BombAsset bombAsset = LoadBombAssetOrPanic(type); ;

        cache.Add(type, bombAsset);
        
        return bombAsset;
    }

    private BombAsset LoadBombAssetOrPanic(BombType type)
    {
        string path = BOMB_OBJECTS_PATH_PREFIX + type.ToString() /*+ ASSET_FILE_TYPE*/;

        BombAsset bombAsset = Resources.Load<BombAsset>(path);

        if(bombAsset == null)
            throw new System.Exception("Load BombAsset of BombType: " + type + " from path: " + path + " failed");

        return bombAsset;
    }

}