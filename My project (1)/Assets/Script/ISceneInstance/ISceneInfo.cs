using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// シーン情報。
/// </summary>
public interface ISceneInfo
{
    public Use _Use { get; }
    public string _SceneName { get; }

    /// <summary>
    /// デフォルトシーンかどうか
    /// </summary>
    public bool _IsDefault { get; }

    /// <summary>
    /// ゲーム開始時等の処理を走らせる場合。
    /// </summary>
    /// <returns></returns>
    public UniTask Init();
    /// <summary>
    /// シーン終了時
    /// </summary>
    public UniTask End();

    /// <summary>
    /// InputSystem起動
    /// </summary>
    public void InputStart();
    /// <summary>
    /// InputSystem停止
    /// </summary>
    public void InputStop();
}
/// <summary>
/// シーンをど
/// </summary>
public enum Use
{
    Main,
    Sub,
    Effective
}
