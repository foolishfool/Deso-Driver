using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPosition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<Renderer>().isVisible)
        {
            if (!GameController.Instance.ResetPositions.Contains(gameObject))
            {
                GameController.Instance.ResetPositions.Add(gameObject);
            }
        }
        else
        {
            if (GameController.Instance.ResetPositions.Contains(gameObject))
                GameController.Instance.ResetPositions.Remove(gameObject);
        }


    }
}
