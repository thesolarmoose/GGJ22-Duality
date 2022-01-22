using DaysSystem;
using UnityEngine;
using UnityEngine.Playables;

public class SceneReseter : MonoBehaviour
{
    [SerializeField] private PlayableDirector cinematic;

    public void Reset()
    {
        cinematic.Play();
    }

    public void PassDay()
    {
        DayData.Instance.AddDayAndRestart();
    }
}