using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSwim : MonoBehaviour
{
    public BoxCollider swimArea;
    public float moveTime;
    public float nextMoveDelay;
    public float rotateAmount;

    private bool isSwimming = false;
    private Quaternion initialRotation;

    // Start is called before the first frame update
    void Start()
    {
        initialRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(swim());
    }

    public static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    private IEnumerator swim()
    {
        if (isSwimming)
        {
            yield break;
        }

        isSwimming = true;

        float elapsed = 0;
        Vector3 initialPos = transform.position;
        Vector3 finalPos = RandomPointInBounds(swimArea.bounds);

        Quaternion initialRot = transform.rotation;
        Quaternion finalRot = initialRotation;
        if (finalPos.y > initialPos.y)
        {
            finalRot *=Quaternion.Euler(-rotateAmount, 0, 0);
        }
        else
        {
            finalRot *= Quaternion.Euler(rotateAmount, 0, 0);
        }

        if (finalPos.x > initialPos.x)
        {
            finalRot *= Quaternion.Euler(0, -rotateAmount, 0);
        }
        else
        {
            finalRot *= Quaternion.Euler(0, rotateAmount, 0);
        }

        finalRot *= Quaternion.Euler(0, 0, Random.Range(-rotateAmount, rotateAmount));

        while (elapsed <= moveTime)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPos, finalPos, elapsed / moveTime);
            transform.rotation = Quaternion.Lerp(initialRot, finalRot, elapsed / moveTime);
            yield return null;
        }
        transform.position = finalPos;

        yield return new WaitForSeconds(nextMoveDelay);
        isSwimming = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
