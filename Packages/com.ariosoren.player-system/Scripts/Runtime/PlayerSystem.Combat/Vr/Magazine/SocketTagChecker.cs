using UnityEngine.XR.Interaction.Toolkit;

public class SocketTagChecker : XRSocketInteractor
{
    public string Tag {get; set;}

    public override bool CanHover(IXRHoverInteractable interactable)
    {
        return base.CanHover(interactable) && interactable.transform.tag == Tag;
    }

    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        return base.CanSelect(interactable) && interactable.transform.tag == Tag;
    }
}