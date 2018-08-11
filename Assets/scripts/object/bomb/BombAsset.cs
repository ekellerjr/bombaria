using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Rendering;

[CreateAssetMenu(menuName = "GameAsset/Bomb")]
public class BombAsset : ScriptableObject
{
    public BombType Type;

    public float TimeBeforeIgnition;

    public float Energy;

    public MeshInstanceRenderer IdleLook;

    public MeshInstanceRenderer BlinkLook;

}
