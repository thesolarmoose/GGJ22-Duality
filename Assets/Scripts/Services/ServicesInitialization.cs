using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

namespace Services
{
    public class ServicesInitialization : MonoBehaviour
    {
        async void Start()
        {
            try
            {
                await UnityServices.InitializeAsync();
                List<string> consentIdentifiers = await AnalyticsService.Instance.CheckForRequiredConsents();
            }
            catch (ConsentCheckException e)
            {
                // Something went wrong when checking the GeoIP, check the e.Reason and handle appropriately.
                Debug.LogWarning(e.Message);
            }
        }
    }
}