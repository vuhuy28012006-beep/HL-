using UnityEngine;

public class EventCard : MonoBehaviour
{
    [Header("Card Data")]
    public EventData Data;

    [Header("Board")]
    public int CurrentIndex;

    public bool IsSelected;

    public void Initialize(EventData data, int index)
    {
        Data = data;
        CurrentIndex = index;
        IsSelected = false;
    }

    public void Select()
    {
        IsSelected = true;
    }

    public void Deselect()
    {
        IsSelected = false;
    }

    public void SetIndex(int newIndex)
    {
        CurrentIndex = newIndex;
    }
}