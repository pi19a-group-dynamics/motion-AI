using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Data
{
    public int branchCount;
    public NeuroNet[][] branches;

    public Data(int count)
    {
        this.branchCount = count;
        this.branches = new NeuroNet[count][];
    }
}
