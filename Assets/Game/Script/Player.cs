using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [Header("Références")]
    public RoadList roadList;

    [Header("Déplacement")]
    public int currentIndex = 0;
    public float moveSpeed = 3f;

    [Header("Saut")]
    public float hopHeight = 0.5f;

    [Header("Effets des cases")]
    public int bonusSteps = 2;
    public int malusSteps = 2;

    private bool isMoving = false;
    private Rigidbody rb;
    private float groundY;

    public bool IsMoving
    {
        get { return isMoving; }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.isKinematic = true;
        groundY = transform.position.y;
    }

    private void Start()
    {
        currentIndex = GameData.playerIndex;

        GameObject startCase = roadList.GetRoadCase(currentIndex);
        if (startCase != null)
        {
            Vector3 pos = startCase.transform.position;
            transform.position = new Vector3(pos.x, groundY, pos.z);
        }
    }

    public void MoveByDiceResult(int diceResult)
    {
        if (isMoving)
            return;

        StartCoroutine(TurnRoutine(diceResult));
    }

    private IEnumerator TurnRoutine(int diceResult)
    {
        isMoving = true;

        yield return StartCoroutine(Move(diceResult));
        yield return StartCoroutine(ApplyCaseEffect());

        GameData.playerIndex = currentIndex;

        isMoving = false;
    }

    // steps positif = avancer, steps négatif = reculer
    private IEnumerator Move(int steps)
    {
        int direction = 1;
        if (steps < 0)
            direction = -1;

        int count = Mathf.Abs(steps);

        for (int i = 0; i < count; i++)
        {
            int nextIndex = currentIndex + direction;
            GameObject nextCase = roadList.GetRoadCase(nextIndex);

            if (nextCase == null)
                break;

            // 1) Se placer au-dessus de la case
            rb.isKinematic = true;

            Vector3 target = new Vector3(
                nextCase.transform.position.x,
                groundY + hopHeight,
                nextCase.transform.position.z
            );

            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            transform.position = target;
            currentIndex = nextIndex;

            // 2) Activer la gravité : il tombe (limite de temps de sécurité)
            rb.isKinematic = false;

            float fallTimer = 0f;
            while (transform.position.y > groundY + 0.1f && fallTimer < 2f)
            {
                fallTimer += Time.deltaTime;
                yield return null;
            }

            // 3) Atterrissage
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
            transform.position = new Vector3(target.x, groundY, target.z);
        }
    }

    private IEnumerator ApplyCaseEffect()
    {
        GameObject caseObject = roadList.GetRoadCase(currentIndex);
        if (caseObject == null)
            yield break;

        Tile tile = caseObject.GetComponent<Tile>();
        if (tile == null)
            yield break;

        switch (tile.type)
        {
            case CaseType.Simple:
                break;

            case CaseType.Bonus:
                Debug.Log("Case bonus !");
                yield return StartCoroutine(Move(bonusSteps));
                break;

            case CaseType.Malus:
                Debug.Log("Case malus !");
                yield return StartCoroutine(Move(-malusSteps));
                break;
        }
    }
}