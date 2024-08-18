interface IGunState 
{
    public void init(GunController _gunController, HandsOnGunControl _handOnGun);
    public void Enter();

    public virtual void TriggerStay(float value, TriggerControl triggerControl) { }
    public virtual void TriggerCancel(TriggerControl triggerControl) { }
    public virtual void ChangeTriggerMode(TriggerControl triggerControl) { }

    public virtual void Exit() { }
}