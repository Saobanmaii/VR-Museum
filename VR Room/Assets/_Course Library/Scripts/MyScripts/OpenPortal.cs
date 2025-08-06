using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPortal : MonoBehaviour
{
    public PortalRound_Controller portalController;
    public void OpenPortalToGoArt()
    {
        portalController.F_TogglePortalRound(true);
    }
}
