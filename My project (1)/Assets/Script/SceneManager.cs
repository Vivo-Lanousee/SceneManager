using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Cysharp.Threading.Tasks;

/// <summary>
/// �V�[���}�l�[�W���[�Ƃ��Đ݌v�B
/// Addressable�ł̔񓯊��ǂݍ��݂Ƃ��ăV�[�����^�p���܂��B
/// </summary>
public class SceneManager : MonoBehaviour
{
    private AsyncOperationHandle<SceneInstance> Instance;

    /// <summary>
    /// ���C��
    /// </summary>
    private AsyncOperationHandle<SceneInstance> _MainScene;
    /// <summary>
    /// �T�u�i�㏑���ŌĂяo���V�[���˃��j���[���j
    /// </summary>
    private AsyncOperationHandle<SceneInstance> _SubScene;
    /// <summary>
    /// Effect�ŃV�[�����g�����B�i�V�[���؂�ւ����̌q���̃V�[���j
    /// </summary>
    private AsyncOperationHandle<SceneInstance> _EffectiveScene;

    /// <summary>
    /// �P��V�[�����[�h�˒ʏ�
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
    /// �V�[�����A�����[�h����B
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
