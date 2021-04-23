using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridWalker : MonoBehaviour
{
    public float gridSize;
    public float moveTime;
    public float nextMoveDelay;

    private bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDirection = Vector3.zero;
        float moveAmount = 0;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveAmount = gridSize;
            moveDirection = -transform.right;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            moveAmount = gridSize;
            moveDirection = transform.right;

        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            moveAmount = gridSize;
            moveDirection = transform.forward;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            moveAmount = gridSize;
            moveDirection = -transform.forward;
        }

        Vector3 moveDelta = moveDirection * moveAmount;
        if (moveDelta.magnitude != 0)
        {
            StartCoroutine(moveBy(moveDirection * moveAmount));
        }
    }

    private IEnumerator moveBy(Vector3 moveDelta)
    {
        if (isMoving)
        {
            yield break;
        }

        isMoving = true;

        float elapsed = 0;
        Vector3 initialPos = transform.position;
        Vector3 finalPos = initialPos + moveDelta;
        
        while (elapsed <= moveTime)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPos, finalPos, elapsed / moveTime);
            yield return null;
        }
        transform.position = finalPos;

        yield return new WaitForSeconds(nextMoveDelay);
        isMoving = false;
    }
}
