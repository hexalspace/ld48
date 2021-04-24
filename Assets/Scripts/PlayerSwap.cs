using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwap : MonoBehaviour
{
    public PlayerLook playerLook;
    public PlayerMove playerMove;
    public Transform swapPoint;
    public Transform player;
    public float swapTime;
    public float swapDelay;

    private bool isSwapping = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(performSwap(swapTime));
        }
    }

    private IEnumerator performSwap(float swapTime)
    {
        if (isSwapping)
        {
            yield break;
        }

        isSwapping = true;
        // playerLook.enabled = false;
        playerMove.enabled = false;

        Vector3 playerStart = player.position;
        Vector3 swapStart = swapPoint.position;


        float elapsed = 0;
        while (elapsed <= swapTime)
        {
            elapsed += Time.deltaTime;
            player.transform.position = Vector3.Lerp(playerStart, swapStart, elapsed / swapTime);
            yield return null;
        }
        player.position = swapStart;
        swapPoint.position = playerStart;

        playerLook.enabled = true;
        playerMove.enabled = true;
        yield return new WaitForSeconds(swapDelay);
        isSwapping = false;
    }
}
