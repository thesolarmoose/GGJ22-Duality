using System;
using System.Collections.Generic;
using UnityEngine;

namespace DaysSystem
{
    public class DayDependant : MonoBehaviour
    {
        [SerializeField] private List<int> days;

        private void Start()
        {
            if (!days.Contains(DayData.Instance.Day))
            {
                gameObject.SetActive(false);
            }
        }
    }
}