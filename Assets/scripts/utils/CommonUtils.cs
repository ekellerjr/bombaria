using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommonUtils {

    public static bool IsInRange(int x, int y, int maxX, int maxY)
    {
        return x >= 0 && x < maxX && y >= 0 && y < maxY;
    }

}
