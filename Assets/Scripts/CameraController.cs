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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 dir = Car.transform.position - gameObject.transform.position;
        LayerMask mask = LayerMask.GetMask("IgnoreRayCast");
        //*** direction is important otherwsie some high budiling was not detected
        hitInfos = Physics.RaycastAll(Car.transform.position, -dir, 1000,~mask);
        Debug.DrawRay(gameObject.transform.position, dir, Color.green);
         foreach (var item in changedMaterials)
         {
             if (!InHitList(item.Key, hitInfos))
             {
                 //reset to inital material
                 Material newMaterial = new Material(OpaqueMaterial);
                newMaterial.SetTexture("_BaseMap", item.Value.GetTexture("_BaseMap"));

             
                GameController.Instance.IsCameraLevelingUp = false;
                SetWholeObjectMaterials(item.Key, newMaterial,false);        
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
                newMaterial.SetTexture("_BaseMap",initialMaterial.GetTexture("_BaseMap"));
                // Color transparentColor = new Color(Color.white.r, Color.white.g, Color.white.b, 0.1f);
                // newMaterial.color = transparentColor;
                GameController.Instance.IsCameraLevelingUp = true;
                SetWholeObjectMaterials(hitInfos[i].collider.gameObject, newMaterial,true);
 
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


    private void SetWholeObjectHide(GameObject smallPart)
    {

        if (smallPart.transform.parent.name != "Scene")
        {
            int index = smallPart.transform.parent.transform.childCount;
            smallPart.transform.parent.gameObject.SetActive(false);
            for (int i = 0; i < index; i++)
            {
                smallPart.transform.parent.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else
        {
            smallPart.SetActive(false);
        }
    }

    private void SetWholeObjectShow(GameObject smallPart)
    {

        if (smallPart.transform.parent.name != "Scene")
        {
            int index = smallPart.transform.parent.transform.childCount;
            smallPart.transform.parent.gameObject.SetActive(true);
            for (int i = 0; i < index; i++)
            {
                smallPart.transform.parent.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else
        {
            smallPart.SetActive(true);
        }
    }
}
