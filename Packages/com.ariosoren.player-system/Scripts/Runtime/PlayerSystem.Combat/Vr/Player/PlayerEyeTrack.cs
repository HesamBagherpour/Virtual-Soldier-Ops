using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class PlayerEyeTrack : MonoBehaviour
{
    [Header("Head Gaze Interactor")]
    [SerializeField] XRGazeInteractor gazeInteractor;

    [Header("Hand Controllers")]
    [SerializeField] PlayerHandController leftHandController;
    [SerializeField] PlayerHandController rightHandController;

    [Header("Ray Interactor")]
    [SerializeField] GameObject leftRayInteractor;
    [SerializeField] GameObject rightRayInteractor;

    void Start()
    {
        gazeInteractor.uiHoverEntered.AddListener(OnUiHoverEntered);
        gazeInteractor.uiHoverExited.AddListener(OnUiHoverExited);

        leftHandController.OnSelectChange += LeftControllerSelectionChange;
        rightHandController.OnSelectChange += RightControllerSelectionChange;
    }

    void OnDestroy()
    {
        gazeInteractor.uiHoverEntered.RemoveListener(OnUiHoverEntered);
        gazeInteractor.uiHoverExited.RemoveListener(OnUiHoverExited);

        leftHandController.OnSelectChange -= LeftControllerSelectionChange;
        rightHandController.OnSelectChange -= RightControllerSelectionChange;
    }

    void OnUiHoverEntered(UIHoverEventArgs hover)
    {
        UpdateInteractors();
    }

    void OnUiHoverExited(UIHoverEventArgs hover)
    {
        UpdateInteractors();
    }


    void LeftControllerSelectionChange()
    {
        RayActivation(leftHandController, leftRayInteractor);
    }
    void RightControllerSelectionChange()
    {
        RayActivation(rightHandController, rightRayInteractor);
    }

    public void OnSceneChange()
    {
        UpdateInteractors();
    }

    void UpdateInteractors()
    {
        RayActivation(leftHandController, leftRayInteractor);
        RayActivation(rightHandController, rightRayInteractor);
    }

    void RayActivation(PlayerHandController handController, GameObject rayInteractor)
    {
        if(IsAnyUiHovered() && handController.HasSelection() == false)
            ActiveRay(handController, rayInteractor);
        else
            DeActiveRay(rayInteractor);
    }

    bool IsAnyUiHovered()
    {
        return gazeInteractor.IsOverUIGameObject();
    }

    void ActiveRay(PlayerHandController handController, GameObject rayInteractor)
    {
        if (handController.HasSelection() == false)
            if(rayInteractor.activeSelf != true)
                rayInteractor.SetActive(true);
    }

    void DeActiveRay(GameObject rayInteractor)
    {
        if(rayInteractor.activeSelf != false)
            rayInteractor.SetActive(false);
    }
}