/// <summary>
/// 터렛 발사체 관리
/// </summary>
public interface IProjectile
{
    int Damage { get; set; }

    string AttackTargetTag { get; set; }
}
