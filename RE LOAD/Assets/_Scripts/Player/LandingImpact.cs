using UnityEngine;
using MoreMountains.Feedbacks;

public class LandingImpact : MonoBehaviour
{
    public MMFeedbacks landingImpact;

    public void Activate()
    {
        landingImpact.PlayFeedbacks();
    }
    public void Deactivate()
    {
        landingImpact.StopFeedbacks();
    }
}
