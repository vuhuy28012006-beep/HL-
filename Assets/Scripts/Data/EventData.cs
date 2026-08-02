using UnityEngine;

[CreateAssetMenu(fileName = "New Event", menuName = "Timeline Sort/Event Data")]
public class EventData : ScriptableObject
{
    public int id;
    public string eventName;
    public long year;
    public Sprite image;
}