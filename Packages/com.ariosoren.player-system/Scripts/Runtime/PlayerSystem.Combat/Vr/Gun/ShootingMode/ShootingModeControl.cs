using System;
using UnityEngine;

public enum ShootingMode
{
    safety,
    semi,
    fullAuto,
}

public enum ModeType
{
    pistol = 1,
    rifle = 2,
}

public enum ChangeModeDirection
{
    up = 1,
    down = -1,
}

public class ShootingModeControl : MonoBehaviour
{
    [SerializeField] ModeType type;
    [SerializeField] AudioSource audioSource;

    Animator animator;
    ShootingMode mode;
    int allowedModes;

    public Action<ShootingMode> OnShootingModeChange;
    
    void Start()
    {
        animator = GetComponent<Animator>();   
        mode = ShootingMode.safety;
        allowedModes = (int)type;
    }

    public void ChangeMode(ChangeModeDirection direction)
    {
        if(type == ModeType.rifle)
        {
            var modeId = (int)mode + (int)direction;

            if(modeId <= allowedModes && modeId >= 0)
            {
                mode = (ShootingMode)modeId;
                PlayChangingModeAnimation(mode.ToString());
                OnShootingModeChange?.Invoke(mode);
                audioSource.Play();
            }
        }
        else if(type == ModeType.pistol)
        {
            mode = mode == ShootingMode.safety? ShootingMode.semi : ShootingMode.safety;
            PlayChangingModeAnimation(mode.ToString());
            OnShootingModeChange?.Invoke(mode);
            audioSource.Play();
        }
    }

    void PlayChangingModeAnimation(string animation)
    {
        animator.CrossFade(animation, 1f);
    }

    public ShootingMode GetMode()
    {
        return mode;
    }
}