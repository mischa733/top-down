using UnityEngine;

public class Item : MonoBehaviour
{
    public int ID;

    public virtual void useItem()
    {
        Debug.Log("using item");
    }
}
