using UnityEngine;

public class RandomDe : MonoBehaviour
{
    public Player player;

    private int ResultatDe;

    public void LanceDe()
    {
        ResultatDe = Random.Range(1, 7);

        Debug.Log("Résultat du dé : " + ResultatDe);

        player.MoveByDiceResult(ResultatDe);
    }
}