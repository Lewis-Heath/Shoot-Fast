
public interface IFireable
{
    void StartFiring();
    void StopFiring(string type);
    void Reload();
    GunType GetGunType();
    bool GetNeedToReload();
}
