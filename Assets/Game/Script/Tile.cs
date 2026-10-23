using UnityEngine;

public enum CaseType
{
    Simple,
    Bonus,
    Malus
}

public class Tile : MonoBehaviour
{
    public CaseType type = CaseType.Simple;
}