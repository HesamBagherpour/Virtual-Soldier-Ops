using ArioSoren.PlayerSystem.Combat.PlayerSystem.Combat.VR.Player;
using UnityEngine;

namespace ArioSoren.PlayerSystem.Combat.PlayerSystem.Combat.VR.Gun
{
    public class HandsOnGunControl : MonoBehaviour
    {
        [SerializeField] GameObject firstHand_Left;
        [SerializeField] GameObject firstHand_Right;
        [SerializeField] GameObject secondHand_Left;
        [SerializeField] GameObject secondHand_Right;
        [SerializeField] GameObject boltHand_Left;
        [SerializeField] GameObject boltHand_Right;

        GameObject[] firstHands = new GameObject[2];
        GameObject[] secondHands = new GameObject[2];
        GameObject[] boltHands = new GameObject[2];

        void Awake()
        {
            SetHands(firstHands, firstHand_Left, firstHand_Right);
            SetHands(secondHands, secondHand_Left, secondHand_Right);
            SetHands(boltHands, boltHand_Left, boltHand_Right);
        }

        public void SetHands(GameObject[] pairOfHands, GameObject left, GameObject right)
        {
            pairOfHands[(int)PlayerHand.Left] = left;
            pairOfHands[(int)PlayerHand.Right] = right;
        }

        public void SetSecondHandToNormal()
        {
            Debug.unityLogger.Log($"HandsOnGunControl | SetSecondHandToNormal | Started.");
            SetHands(secondHands, secondHand_Left, secondHand_Right);
        }
        public void SetSecondHandToBolt()
        {
            Debug.unityLogger.Log($"HandsOnGunControl | SetSecondHandToBolt | Started.");
            SetHands(secondHands, boltHand_Left, boltHand_Right);
        }

        public void SetToNoGrab()
        {
            Debug.unityLogger.Log($"HandsOnGunControl | SetToNoGrab | Started.");
            SetDeactive(firstHands);
            SetDeactive(secondHands);
            SetDeactive(boltHands);
        }

        public void SetToSingleGrab(PlayerHand grabHand)
        {
            Debug.unityLogger.Log($"HandsOnGunControl | SetToSingleGrab | Started.");
            SetActive(firstHands, grabHand);
            SetDeactive(secondHands);
            SetDeactive(boltHands);
        }

        public void SetToDoubleGrab(PlayerHand firstEnteredHand)
        {
            Debug.unityLogger.Log($"HandsOnGunControl | SetToDoubleGrab | Started.");
            PlayerHand secondEnteredHand;
            if(firstEnteredHand == PlayerHand.Left)
                secondEnteredHand = PlayerHand.Right;
            else
                secondEnteredHand = PlayerHand.Left;

            SetActive(firstHands, firstEnteredHand);
            SetDeactive(boltHands);
            SetActive(secondHands, secondEnteredHand);
        }

        public void ActivateBoltHand(PlayerHand playerHand)
        {
            boltHands[(int)playerHand].SetActive(true);
        }

        public void DeactivateBoltHand(PlayerHand playerHand)
        {
            boltHands[(int)playerHand].SetActive(false);
        }

        void SetActive(GameObject[] hands, PlayerHand grabHand)
        {
            foreach (var hand in hands)
            {
                if(hand == hands[(int)grabHand])
                    hand.SetActive(true);
                else
                    hand.SetActive(false);
            }
        }

        void SetDeactive(GameObject[] hands)
        {
            foreach (var hand in hands)
                if(hand.activeSelf != false)
                    hand.SetActive(false);
        }
    }
}