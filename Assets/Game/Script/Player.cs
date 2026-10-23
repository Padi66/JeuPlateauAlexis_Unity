using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Références")]
    public RoadList roadList;

    [Header("Déplacement")]
    public int currentIndex = 0;
    public float moveSpeed = 3f;

    private bool isMoving = false;

    // À appeler après le lancer de dé
    public void MoveByDiceResult(int diceResult)
    {
        StartCoroutine(MoveAlongRoad(diceResult));
    }

    private IEnumerator MoveAlongRoad(int diceResult)
    {
        isMoving = true;

        for (int i = 0; i < diceResult; i++)
        {
            int nextIndex = currentIndex + 1;

            GameObject nextCase = roadList.GetRoadCase(nextIndex);

            // Arrêter si la prochaine case n'existe pas
            if (nextCase == null)
                break;

            Vector3 targetPosition = new Vector3(
                nextCase.transform.position.x,
                transform.position.y,
                nextCase.transform.position.z
            );

            // Avancer jusqu'à la prochaine case
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );

                yield return null;
            }

            transform.position = targetPosition;
            currentIndex = nextIndex;
        }

        isMoving = false;
    }
}