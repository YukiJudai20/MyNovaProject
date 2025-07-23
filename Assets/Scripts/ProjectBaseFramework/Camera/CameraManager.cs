using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public enum CameraType
{
    None,
    ThirdPerson,
    Battle,
}


public class CameraManager : MonoSingleton<CameraManager>
{
    public CinemachineVirtualCamera PlayerFollowCamera;
    public CinemachineVirtualCamera BattleCamera;
    public CinemachineVirtualCamera MiniMapCamera;

    private void Awake()
    {
        PlayerFollowCamera = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        BattleCamera = GameObject.Find("BattleCamera").GetComponent<CinemachineVirtualCamera>();
        MiniMapCamera = GameObject.Find("MiniMapCamera").GetComponent<CinemachineVirtualCamera>();
    }

    public void Initialize(GameObject playerRoot)
    {
        PlayerFollowCamera.Follow = playerRoot.transform;
        BattleCamera.Follow = playerRoot.transform;
        MiniMapCamera.Follow = playerRoot.transform;
        BattleCamera.gameObject.SetActive(false);
    }

    public void ChangeToNormalCamera()
    {
        PlayerFollowCamera.gameObject.SetActive(true);
        MiniMapCamera.gameObject.SetActive(true);
        BattleCamera.gameObject.SetActive(false);
    }

    public void ChangeToBattleCamera()
    {
        PlayerFollowCamera.gameObject.SetActive(false);
        MiniMapCamera.gameObject.SetActive(false);
        BattleCamera.gameObject.SetActive(true);
    }
}
