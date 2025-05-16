using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// �V�[�����B
/// </summary>
public interface ISceneInfo
{
    public Use _Use { get; }
    public string _SceneName { get; }

    /// <summary>
    /// �f�t�H���g�V�[�����ǂ���
    /// </summary>
    public bool _IsDefault { get; }

    /// <summary>
    /// �Q�[���J�n�����̏����𑖂点��ꍇ�B
    /// </summary>
    /// <returns></returns>
    public UniTask Init();
    /// <summary>
    /// �V�[���I����
    /// </summary>
    public UniTask End();

    /// <summary>
    /// InputSystem�N��
    /// </summary>
    public void InputStart();
    /// <summary>
    /// InputSystem��~
    /// </summary>
    public void InputStop();
}
/// <summary>
/// �V�[������
/// </summary>
public enum Use
{
    Main,
    Sub,
    Effective
}
