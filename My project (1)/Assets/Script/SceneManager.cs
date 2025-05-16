using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Cysharp.Threading.Tasks;

/// <summary>
/// シーンマネージャーとして設計。
/// Addressableでの非同期読み込みとしてシーンを運用します。
/// </summary>
public class SceneManager : MonoBehaviour
{
    private AsyncOperationHandle<SceneInstance> Instance;

    /// <summary>
    /// メイン
    /// </summary>
    private AsyncOperationHandle<SceneInstance> _MainScene;
    /// <summary>
    /// サブ（上書きで呼び出すシーン⇒メニュー等）
    /// </summary>
    private AsyncOperationHandle<SceneInstance> _SubScene;
    /// <summary>
    /// Effectでシーンを使う時。（シーン切り替え時の繋ぎのシーン）
    /// </summary>
    private AsyncOperationHandle<SceneInstance> _EffectiveScene;

    /// <summary>
    /// 単一シーンロード⇒通常
    /// </summary>
    /// <param name="SceneName"></param>
    public void SceneLoad(string SceneName)
    {
        Addressables.LoadSceneAsync(SceneName);
    }


    public async UniTask SceneLoad_Effective(string SceneName)
    {
        if (_EffectiveScene.IsValid())
        {
            await Addressables.UnloadSceneAsync(_EffectiveScene);
        }
        _EffectiveScene =Addressables.LoadSceneAsync(SceneName,UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }
    /// <summary>
    /// シーンをアンロードする。
    /// </summary>
    public void SceneUnload()
    {
        //Instance.Release
    }

    private async void Start()
    {
        
        await SceneLoad_Effective("EffectScene_Sample");
        //await SceneLoad_Effective("SampleScene1");
    }
}
