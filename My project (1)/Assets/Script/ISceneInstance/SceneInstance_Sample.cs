using Cysharp.Threading.Tasks;
using UnityEngine;

public class SceneInstance_Sample : ISceneInfo
{
    Use ISceneInfo._Use => Use.Main;

    string ISceneInfo._SceneName => "Sample";

    bool ISceneInfo._IsDefault => false;

    async UniTask ISceneInfo.End()
    {
        Debug.Log("ƒeƒXƒg");
    }
    async UniTask ISceneInfo.Init()
    {
        Debug.Log("End");
    }
    void ISceneInfo.InputStart()
    {   
    }
    void ISceneInfo.InputStop()
    {
    }
}
