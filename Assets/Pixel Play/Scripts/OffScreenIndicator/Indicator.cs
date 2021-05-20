using Pathfinding;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Assign this script to the indicator prefabs.
/// </summary>
public class Indicator : MonoBehaviour
{
    [SerializeField] private IndicatorType indicatorType;
    public Image indicatorImage;
    private Text distanceText;
    public Image Arrow;
    [HideInInspector]
    public GameObject BelongedPickup;


    
    /// <summary>
    /// Gets if the game object is active in hierarchy.
    /// </summary>
    public bool Active
    {
        get
        {
            return transform.gameObject.activeInHierarchy;
        }
    }

    /// <summary>
    /// Gets the indicator type
    /// </summary>
    public IndicatorType Type
    {
        get
        {
            return indicatorType;
        }
    }

    void Awake()
    {
        distanceText = transform.GetComponentInChildren<Text>();

       GetComponent<Button>().onClick.AddListener(()=> SetTarget());

       
    }

 
    /// <summary>
    /// Sets the image color for the indicator.
    /// </summary>
    /// <param name="color"></param>
    public void SetImageColor(Color color)
    {
        indicatorImage.color = color;
    }
    public void SetImag(Sprite image)
    {
        indicatorImage.sprite = image;
    }
    /// <summary>
    /// Sets the distance text for the indicator.
    /// </summary>
    /// <param name="value"></param>
    public void SetDistanceText(float value)
    {
        distanceText.text = value >= 0 ? Mathf.Floor(value) + " m" : "";
    }

    /// <summary>
    /// Sets the distance text rotation of the indicator.
    /// </summary>
    /// <param name="rotation"></param>
    public void SetTextRotation(Quaternion rotation)
    {
        distanceText.rectTransform.rotation = rotation;
    }

    /// <summary>
    /// Sets the indicator as active or inactive.
    /// </summary>
    /// <param name="value"></param>
    public void Activate(bool value)
    {
        transform.gameObject.SetActive(value);
    }

    public void SetTarget()
    {

        GameController.Instance.Target.UIClicked = true;
        GameController.Instance.Target.UIPosiiton = BelongedPickup.gameObject.transform.position;
    }
}

public enum IndicatorType
{
    BOX,
    ARROW
}
