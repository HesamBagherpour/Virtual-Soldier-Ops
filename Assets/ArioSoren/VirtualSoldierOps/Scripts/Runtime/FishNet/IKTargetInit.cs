using RootMotion.FinalIK;
using UnityEngine;
namespace ArioSoren.VirtualSoldierOps
{
public class IKTargetInit : MonoBehaviour
{
	public VRPlayerRefrences playerRefrences;
    // Start is called before the first frame update
    public  void Start()
	{
		if(playerRefrences == null)
        	playerRefrences = FindAnyObjectByType<VRPlayerRefrences>();
        VRIK ikRefrences = GetComponent<VRIK>();
        if (ikRefrences.solver.spine.headTarget == null)
        {
            ikRefrences.solver.spine.headTarget = playerRefrences.headTargetIK;
        }
        if (ikRefrences.solver.rightArm.target == null)
        {
            ikRefrences.solver.rightArm.target = playerRefrences.handRTargetIK;
        }
        if (ikRefrences.solver.leftArm.target == null)
        {
            ikRefrences.solver.leftArm.target = playerRefrences.handLTargetIK;
        }

        transform.position = playerRefrences.transform.position;
    }
}
}
