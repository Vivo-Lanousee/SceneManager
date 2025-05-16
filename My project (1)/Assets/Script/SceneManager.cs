using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Unity.VisualScripting.Antlr3.Runtime;

/// <summary>
/// �V�[���}�l�[�W���[�Ƃ��Đ݌v�B
/// Addressable�ł̔񓯊��ǂݍ��݂Ƃ��ăV�[�����^�p���܂��B
/// </summary>
public class SceneManager : MonoBehaviour
{
    /// <summary>
    /// ���C��
    /// </summary>
    private AsyncOperationHandle<SceneInstance> _MainScene = default;
    private ISceneInfo _MainSceneInfo = null;

    /// <summary>
    /// �T�u�i�㏑���ŌĂяo���V�[���˃��j���[���j
    /// </summary>
    private AsyncOperationHandle<SceneInstance> _SubScene = default;
    private ISceneInfo _SubSceneInfo = null;

    /// <summary>
    /// Effect�ŃV�[�����g�����B�i�V�[���؂�ւ����̌q���̃V�[���j
    /// </summary>
    private AsyncOperationHandle<SceneInstance> _EffectiveScene = default;
    private ISceneInfo _EffectiveSceneInfo = null;

    /// <summary>
    /// �P��V�[�����[�h:���C��
    /// </summary>
    /// <param name="SceneName"></param>
    public async UniTask SceneLoad_Main(ISceneInfo MainSceneInfo,ISceneInfo EffectSceneInfo)
    {
        //�I������
        await _MainSceneInfo.End();
        _MainSceneInfo.InputStop();

        //���o�J�n�B(�V�[��������Effect��Init-End�Ő��䂷��)
        await SceneLoad_Effective(EffectSceneInfo);

        //������Ń��C���̃��[�h���s���B
        using (var _cts = new CancellationTokenSource())
        {
            //�g�[�N�����s
            var token = _cts.Token;
            try
            {
                //���[�h
                _MainScene = Addressables.LoadSceneAsync(MainSceneInfo._SceneName,UnityEngine.SceneManagement.LoadSceneMode.Additive);
                await _MainScene.ToUniTask(cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("�V�[�����[�h�L�����Z��");
            }
            finally 
            {
                //���C���V�[���C���X�^���X�X�V
                _MainSceneInfo = MainSceneInfo;

                //����������(�Q�[���J�n���̏����j
                _MainSceneInfo.InputStart();
                await _MainSceneInfo.Init();//��ɉ��o�𑖂点�Ă����B

                //Effective�V�[����Unload�����i�^�C�~���O�����K�{�Ȃ̂Ŕ񓯊��ŏ����𑖂点��B
                await _EffectiveSceneInfo.End();

            }
        }
    }
    /// <summary>
    /// �V�[�����[�h�F�T�u�p�B�i���j���[��ʓ��j
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
                Debug.Log("�V�[�����[�h�L�����Z��");
            }
        }
    }
    /// <summary>
    /// �V�[���؂�ւ����̉��o�p
    /// </summary>
    /// <param name="SceneName"></param>
    /// <returns></returns>
    
    /// <summary>
    /// �T�u�V�[�����A�����[�h����B
    /// </summary>
    public async UniTask SceneUnload_Sub()
    {
        if (_SubSceneInfo!=null)
        {
            //�T�u�V�[����ActionMap���I��
            _SubSceneInfo?.InputStop();
            //�I������
            await _SubSceneInfo.End();

            using (var _cts = new CancellationTokenSource()) { 

                var token = _cts.Token;
                try
                {
                    //�A�����[�h
                    _SubScene = Addressables.UnloadSceneAsync(_SubScene);
                    await _SubScene.ToUniTask(cancellationToken: token);
                }
                catch (OperationCanceledException)
                {
                    Debug.Log("�V�[���A�����[�h�L�����Z��");
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
                    //�A�����[�h
                    _EffectiveScene = Addressables.UnloadSceneAsync(_EffectiveScene);
                    await _EffectiveScene.ToUniTask(cancellationToken: token);
                }
                catch (OperationCanceledException)
                {
                    Debug.Log("�V�[���A�����[�h�L�����Z��");
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
    /// �G�t�F�N�g�𐶐��B(public�ɂ��Ă��邪��{���ڌĂ΂Ȃ����̂Ƒz�肵�Ă���j
    /// </summary>
    /// <param name="SceneName"></param>
    /// <returns></returns>
    public async UniTask SceneLoad_Effective(ISceneInfo SceneInfo)
    {
        using (var _cts = new CancellationTokenSource())
        {
            //�g�[�N�����s
            var token = _cts.Token;
            try
            {
                //���[�h
                _EffectiveScene = Addressables.LoadSceneAsync(SceneInfo._SceneName);
                await _EffectiveScene.ToUniTask(cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("�V�[�����[�h�L�����Z��");
            }
            finally 
            { 
                _EffectiveSceneInfo = SceneInfo;
                //������Ń��C���V�[�����[�h���Ԃ�ύX���Ă����B
                await _EffectiveSceneInfo.Init();
            }
        }
    }

    /// <summary>
    /// �����ɌĂяo������(���C���V�[�������ݒ�������ōs���j
    /// </summary>
    [RuntimeInitializeOnLoadMethod]
    public static void Init()
    {

    }
}
