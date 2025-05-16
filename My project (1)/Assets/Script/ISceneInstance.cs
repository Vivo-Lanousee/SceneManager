using UnityEngine;

/// <summary>
/// シーン読み込み時。
/// </summary>
public interface ISceneInstance
{
    public Scenes _Scenes { get; }
    public string _SceneName { get; }
    public void Init();
    /// <summary>
    /// シーン終了時
    /// </summary>
    void End();
}
/// <summary>
/// シーンをどれで使うのか
/// </summary>
public enum Scenes
{
    Main,
    Sub,
    Effective
}
