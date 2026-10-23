using System.Collections.Generic;
using UnityEngine;

public class RoadList : MonoBehaviour
{
    public List<GameObject> roadCases = new List<GameObject>();

    public GameObject GetRoadCase(int index)
    {
        if (index < 0 || index >= roadCases.Count)
            return null;

        return roadCases[index];
    }
}