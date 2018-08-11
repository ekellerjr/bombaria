using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombariaSettings : MonoBehaviour {

    [Header("Player Settings")]
    public float playerMovementSpeed = 6.0f;

    [Header("Input Settings")]
    public KeyCode placeBombActionKey = KeyCode.Space;

    [Header("Action Cooldowns")]
    public float placeBombActionCooldown = 1.0f;
}