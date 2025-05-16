using UnityEngine;

/// <summary>
/// �V�[���ǂݍ��ݎ��B
/// </summary>
public interface ISceneInstance
{
    public Scenes _Scenes { get; }
    public string _SceneName { get; }
    public void Init();
    /// <summary>
    /// �V�[���I����
    /// </summary>
    void End();
}
/// <summary>
/// �V�[�����ǂ�Ŏg���̂�
/// </summary>
public enum Scenes
{
    Main,
    Sub,
    Effective
}
