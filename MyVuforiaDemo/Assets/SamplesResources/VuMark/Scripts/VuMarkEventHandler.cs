/*===============================================================================
Copyright (c) 2016 PTC Inc. All Rights Reserved.

Confidential and Proprietary - Protected under copyright and other laws.
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/

using UnityEngine;
using Vuforia;

/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
/// </summary>
public class VuMarkEventHandler : MonoBehaviour, ITrackableEventHandler
                                                
{
    public GameObject platformPrefab;

    #region PRIVATE_MEMBER_VARIABLES
 
    private TrackableBehaviour mTrackableBehaviour;
    private GameObject platform;
    
    #endregion // PRIVATE_MEMBER_VARIABLES
    #region UNTIY_MONOBEHAVIOUR_METHODS
    
    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }

        //platform = Instantiate(platformPrefab, mTrackableBehaviour.transform.position, mTrackableBehaviour.transform.rotation);
    }

    #endregion // UNTIY_MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS

    /// <summary>
    /// Implementation of the ITrackableEventHandler function called when the
    /// tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
                                    TrackableBehaviour.Status previousStatus,
                                    TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            OnTrackingFound();
        }
        else
        {
            OnTrackingLost();
        }
    }

    public void Update()
    {
        Vector3 trackableBhvrPosition = mTrackableBehaviour.transform.position;
        Vector3 newPosition = new Vector3(trackableBhvrPosition.x, 0, trackableBhvrPosition.z) * 10;
        Vector3 trackableBhvrRotation = mTrackableBehaviour.transform.rotation.eulerAngles;
        Quaternion newRotation = Quaternion.Euler(0, trackableBhvrRotation.y, 0);

        platform.transform.position = newPosition;
        platform.transform.rotation = newRotation;
    }

    #endregion // PUBLIC_METHODS


    #region PRIVATE_METHODS

    private void OnTrackingFound()
    {
        if (platform == null)
        {
            platform = Instantiate(platformPrefab, mTrackableBehaviour.transform.position, mTrackableBehaviour.transform.rotation);
        }
        else
        {
            platform.SetActive(true);
        }

        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
    }


    private void OnTrackingLost()
    {
        platform.SetActive(false);

        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
    }

    #endregion // PRIVATE_METHODS
}
