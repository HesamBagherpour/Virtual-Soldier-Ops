using UnityEngine;
using UnityEngine.Animations;

namespace ArioSoren.VirtualSoldierOps
{
    public class VrAssigns : MonoBehaviour
    {
        [SerializeField] Transform mainCamera;
        [SerializeField] Transform leftController;
        [SerializeField] Transform rightController;

        Transform head;
        Transform leftHand;
        Transform rightHand;

        void Start()
        {
            head = transform.GetChild(0).GetChild(0).GetChild(0);
            leftHand = transform.GetChild(0).GetChild(1).GetChild(0);
            rightHand = transform.GetChild(0).GetChild(2).GetChild(0);
            SetConstraints();
        }

        void SetConstraints()
        {
            ConstraintSource mainCameraSource = new ConstraintSource();
            ConstraintSource leftControllerSource = new ConstraintSource();
            ConstraintSource rightControllerSource = new ConstraintSource();

            Set(mainCameraSource, head, mainCamera);
            Set(leftControllerSource, leftHand, leftController);
            Set(rightControllerSource, rightHand, rightController);
        }

        void Set(ConstraintSource source, Transform baseTransform, Transform targetTransform)
        {
            source.weight = 1;
            source.sourceTransform = targetTransform;
            baseTransform.GetComponent<RotationConstraint>().AddSource(source);
            baseTransform.GetComponent<PositionConstraint>().AddSource(source);
        }
    }
}