using Pathfinding.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    RaycastHit[] hitInfos;
    private Material initialMaterial;
    public Material TransparentMaterial;
    public Material OpaqueMaterial;
    public GameObject Car;
    Dictionary<GameObject, Material> changedMaterials = new Dictionary<GameObject, Material>();
  //  private Vector3 distance;
   // public Vector3 InitialDistance;
   // public float X;
   // public float y;
   // public float Z;
    public float height;
    public GameObject Pivot;
    private bool isLevelUp;
    // Start is called before the first frame update
    void Start()
    {
      //  distance =   gameObject.transform.position - Car.transform.position ;
      //  InitialDistance = distance;
    }

    // Update is called once per frame
    void Update()
    {

        // Vector3 newPos = new Vector3(Car.transform.position.x + height + InitialDistance.x, Car.transform.position.y + InitialDistance.y + height, Car.transform.position.z + InitialDistance.z + height);
        Pivot.transform.position = Car.transform.position;
         gameObject.transform.LookAt(Car.transform);

        Vector3 dir = Car.transform.position - gameObject.transform.position;
        LayerMask mask = LayerMask.GetMask("IgnoreRayCast");
        //*** direction is important otherwsie some high budiling was not detected
        hitInfos = Physics.RaycastAll(Car.transform.position, -dir, 1000,~mask);
       // Debug.DrawRay(gameObject.transform.position, dir, Color.green);
         foreach (var item in changedMaterials)
         {
             if (!InHitList(item.Key, hitInfos))
             {
                 //reset to inital material
                Material newMaterial = new Material(OpaqueMaterial);
               if (item.Value.HasProperty("_BaseMap"))
                  newMaterial.SetTexture("_BaseMap", item.Value.GetTexture("_BaseMap"));

               SetWholeObjectMaterials(item.Key, newMaterial,false);

                isLevelUp = false;
             }        
         }
         changedMaterials.Clear();
        for (int i = 0; i < hitInfos.Length; i++)
        {
            if (!hitInfos[i].collider.gameObject.GetComponent<Renderer>())
            {
                return;
            }
            initialMaterial = hitInfos[i].collider.gameObject.GetComponent<Renderer>().material;

            if (!changedMaterials.ContainsKey(hitInfos[i].collider.gameObject))
            {
                changedMaterials.Add(hitInfos[i].collider.gameObject, initialMaterial);
                Material newMaterial = new Material(TransparentMaterial);
                if (initialMaterial.HasProperty("_BaseMap"))
                {
                    newMaterial.SetTexture("_BaseMap", initialMaterial.GetTexture("_BaseMap"));
                }

                 Color transparentColor = new Color(Color.white.r, Color.white.g, Color.white.b, 0.1f);
                 newMaterial.color = transparentColor;

                 SetWholeObjectMaterials(hitInfos[i].collider.
                 gameObject, newMaterial,true);
                isLevelUp = true;
            }
        }

        


    }

    private bool InHitList(GameObject obj, RaycastHit[] hitInfos)
    {
        for (int i = 0; i < hitInfos.Length; i++)
        {
            if (hitInfos[i].collider.gameObject == obj)
            {
               return true;
            }
           
        }
        return false;
    }


    private void SetWholeObjectMaterials(GameObject smallPart, Material newMaterial, bool setLayer)
    {

        if (smallPart.transform.parent.name != "Scene" && !smallPart.gameObject.CompareTag("Floor"))
        {
            int index = smallPart.transform.parent.transform.childCount;
            if (setLayer)
            {
                smallPart.transform.parent.gameObject.layer = 10;
            }
            if (smallPart.transform.parent.GetComponent<Renderer>())
            {
                smallPart.transform.parent.GetComponent<Renderer>().material = newMaterial;
            }
         
            for (int i = 0; i < index; i++)
            {
                smallPart.transform.parent.transform.GetChild(i).GetComponent<Renderer>().material = newMaterial;
                if (setLayer)
                {
                    smallPart.transform.parent.transform.GetChild(i).gameObject.layer = 10;
                }
            }
        }
        else
        {
            smallPart.GetComponent<Renderer>().material = newMaterial;
            if (setLayer)
            {
                smallPart.layer = 10;
            }
        }
    }




   
}
