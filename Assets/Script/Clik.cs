using UnityEngine;
using UnityEngine.EventSystems;

public class Clik : MonoBehaviour,  IDragHandler // класс для хранения состояния обекта
{

    private bool ChengePos;
    private int Speed = 10;
    public bool Chenge
    {
        get { return ChengePos; }
        set { ChengePos = value; }
    }
    void Start()
    {
        Chenge = true;
    }

    public void OnDrag(PointerEventData eventData)

    {
        
        Chenge = true;
        transform.position += new Vector3(eventData.delta.x / Speed, eventData.delta.y / Speed, 0);
    }
    


}
