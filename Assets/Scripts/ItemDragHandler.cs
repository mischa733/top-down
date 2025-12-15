using UnityEngine;
using UnityEngine.EventSystems;
using Vector2 = System.Numerics.Vector2;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform originalParent;
    CanvasGroup canvasGroup;

    public float minDropDistance = 2f;
    public float maxDropDistance = 3f;
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        
        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>();
        Slot originalSlot = originalParent?.GetComponent<Slot>();

        if (dropSlot == null)
        {
            GameObject item = eventData.pointerEnter;
        }

        if (dropSlot != null)
        {
            if (dropSlot.currentItem != null)
            {
                dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                originalSlot.currentItem = dropSlot.currentItem;
                dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            }
            else
            {
                originalSlot.currentItem = null;
            }
            
            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject;
        }
        else
        {
            if (!isWithinInventory(eventData.position))
            {
                DropItem(originalSlot);
            }
            else
            {
                transform.SetParent(originalParent);
            }
            
        }
        GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
    }

    bool isWithinInventory(UnityEngine.Vector2 mousePosition)
    {
        RectTransform inventoryRect = originalParent.parent.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(inventoryRect, mousePosition);
    }

    void DropItem(Slot originalSlot)
    {
        originalSlot.currentItem = null;
        
        Transform playerTransfrom = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransfrom == null)
        {
           Debug.LogError("Missing 'Player' tag");
           return;
        }
        
        UnityEngine.Vector2 dropOffset = Random.insideUnitCircle.normalized * Random.Range(minDropDistance, maxDropDistance);
        UnityEngine.Vector2 dropPosition = (UnityEngine.Vector2) playerTransfrom.position + dropOffset;
        
        Instantiate(gameObject, dropPosition, Quaternion.identity);
        
        Destroy(gameObject);
    }
}
