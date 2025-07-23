using System;
using UnityEngine;
public class GameManager: MonoSingleton<GameManager>
{
    protected override void OnAwake()
    {
        base.OnAwake();
        EventCenter.Instance.AddEventListener<Vector3>("DungeonGenerateDone",CreatePlayer);
    }

    public void StartGame()
    {
        CreateGameWorld();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void CreateGameWorld()
    {
        UIManager.Instance.LoadingDungeonWorld();
        ScenesManager.Instance.LoadSceneAsync("GameLevel1", () =>
        {
            Cursor.lockState = CursorLockMode.Locked;
            UIManager.Instance.LoadingDungeonWorldDone();
        });
    }

    public void CreatePlayer(Vector3 pos)
    {
        Debug.Log("创建角色");
        ResManager.Instance.LoadGameObjectAsync("Prefab/Player",null,(go)=>
        {
            go.transform.position = new Vector3(pos.x, pos.y, pos.z);
            CameraManager.Instance.Initialize(go.transform.GetChild(0).gameObject);
        });
    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<Vector3>("DungeonGenerateDone",CreatePlayer);
    }
}
