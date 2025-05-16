using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Unity.VisualScripting.Antlr3.Runtime;

/// <summary>
/// シーンマネージャーとして設計。
/// Addressableでの非同期読み込みとしてシーンを運用します。
/// </summary>
public class SceneManager : MonoBehaviour
{
    /// <summary>
    /// メイン
    /// </summary>
    private AsyncOperationHandle<SceneInstance> _MainScene = default;
    private ISceneInfo _MainSceneInfo = null;

    /// <summary>
    /// サブ（上書きで呼び出すシーン⇒メニュー等）
    /// </summary>
    private AsyncOperationHandle<SceneInstance> _SubScene = default;
    private ISceneInfo _SubSceneInfo = null;

    /// <summary>
    /// Effectでシーンを使う時。（シーン切り替え時の繋ぎのシーン）
    /// </summary>
    private AsyncOperationHandle<SceneInstance> _EffectiveScene = default;
    private ISceneInfo _EffectiveSceneInfo = null;

    /// <summary>
    /// 単一シーンロード:メイン
    /// </summary>
    /// <param name="SceneName"></param>
    public async UniTask SceneLoad_Main(ISceneInfo MainSceneInfo,ISceneInfo EffectSceneInfo)
    {
        //終了処理
        await _MainSceneInfo.End();
        _MainSceneInfo.InputStop();

        //演出開始。(シーン処理をEffectのInit-Endで制御する)
        await SceneLoad_Effective(EffectSceneInfo);

        //こちらでメインのロードを行う。
        using (var _cts = new CancellationTokenSource())
        {
            //トークン発行
            var token = _cts.Token;
            try
            {
                //ロード
                _MainScene = Addressables.LoadSceneAsync(MainSceneInfo._SceneName,UnityEngine.SceneManagement.LoadSceneMode.Additive);
                await _MainScene.ToUniTask(cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("シーンロードキャンセル");
            }
            finally 
            {
                //メインシーンインスタンス更新
                _MainSceneInfo = MainSceneInfo;

                //初期化処理(ゲーム開始等の処理）
                _MainSceneInfo.InputStart();
                await _MainSceneInfo.Init();//先に演出を走らせておく。

                //EffectiveシーンのUnload処理（タイミング調整必須なので非同期で処理を走らせる。
                await _EffectiveSceneInfo.End();

            }
        }
    }
    /// <summary>
    /// シーンロード：サブ用。（メニュー画面等）
    /// </summary>
    /// <param name="SceneInfo"></param>
    /// <returns></returns>
    public async UniTask SceneLoad_Sub(ISceneInfo SceneInfo)
    {

        using (var _cts = new CancellationTokenSource())
        {
            var token = _cts.Token;
            
            try
            {
                _SubScene = Addressables.LoadSceneAsync(SceneInfo._SceneName,UnityEngine.SceneManagement.LoadSceneMode.Additive);
                await _SubScene.ToUniTask(cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("シーンロードキャンセル");
            }
        }
    }
    /// <summary>
    /// シーン切り替え時の演出用
    /// </summary>
    /// <param name="SceneName"></param>
    /// <returns></returns>
    
    /// <summary>
    /// サブシーンをアンロードする。
    /// </summary>
    public async UniTask SceneUnload_Sub()
    {
        if (_SubSceneInfo!=null)
        {
            //サブシーンのActionMapを終了
            _SubSceneInfo?.InputStop();
            //終了処理
            await _SubSceneInfo.End();

            using (var _cts = new CancellationTokenSource()) { 

                var token = _cts.Token;
                try
                {
                    //アンロード
                    _SubScene = Addressables.UnloadSceneAsync(_SubScene);
                    await _SubScene.ToUniTask(cancellationToken: token);
                }
                catch (OperationCanceledException)
                {
                    Debug.Log("シーンアンロードキャンセル");
                }
                finally
                {
                    _SubSceneInfo = null;
                    _SubScene=default;
                }
            }
        }
    }
    public async UniTask SceneUnload_Effective()
    {
        if (_SubSceneInfo != null)
        {
            using (var _cts = new CancellationTokenSource())
            {

                var token = _cts.Token;
                try
                {
                    //アンロード
                    _EffectiveScene = Addressables.UnloadSceneAsync(_EffectiveScene);
                    await _EffectiveScene.ToUniTask(cancellationToken: token);
                }
                catch (OperationCanceledException)
                {
                    Debug.Log("シーンアンロードキャンセル");
                }
                finally
                {
                    _EffectiveSceneInfo = null;
                    _EffectiveScene = default;
                }
            }
        }
    }

    /// <summary>
    /// エフェクトを生成。(publicにしているが基本直接呼ばないものと想定している）
    /// </summary>
    /// <param name="SceneName"></param>
    /// <returns></returns>
    public async UniTask SceneLoad_Effective(ISceneInfo SceneInfo)
    {
        using (var _cts = new CancellationTokenSource())
        {
            //トークン発行
            var token = _cts.Token;
            try
            {
                //ロード
                _EffectiveScene = Addressables.LoadSceneAsync(SceneInfo._SceneName);
                await _EffectiveScene.ToUniTask(cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("シーンロードキャンセル");
            }
            finally 
            { 
                _EffectiveSceneInfo = SceneInfo;
                //こちらでメインシーンロード時間を変更しておく。
                await _EffectiveSceneInfo.Init();
            }
        }
    }

    /// <summary>
    /// 初期に呼び出す処理(メインシーン初期設定をここで行う）
    /// </summary>
    [RuntimeInitializeOnLoadMethod]
    public static void Init()
    {

    }
}
