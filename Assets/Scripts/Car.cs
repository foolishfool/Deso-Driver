using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


  private void OnTriggerEnter(Collider other)
  {
      if (other.gameObject.CompareTag("Collision"))
      {
            for (int i = 0; i < GameController.Instance.Paths.Count; i++)
            {
                if (GameController.Instance.Paths[i].Target.gameObject == other.gameObject)
                {
                    GameController.Instance.Paths[i].Pause();
                }
            }


          Vector3 direction = (other.gameObject.transform.position - gameObject.transform.position).normalized;
          other.gameObject.GetComponent<Rigidbody>().AddForce(direction*3, ForceMode.Impulse);
      }
  }

  
}
