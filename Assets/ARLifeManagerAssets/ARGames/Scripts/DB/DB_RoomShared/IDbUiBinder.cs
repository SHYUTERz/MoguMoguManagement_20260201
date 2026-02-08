/// <summary>
/// Implement this on room UI binder scripts.
/// They should read from GameDataCache and update UI, without opening DB.
/// </summary>
public interface IDbUiBinder
{
    void Apply(GameDataCache cache);
    void Clear();
}
