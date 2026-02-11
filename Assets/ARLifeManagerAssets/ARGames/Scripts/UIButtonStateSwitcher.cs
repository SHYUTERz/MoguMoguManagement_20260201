using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonStateSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject homeRoom;
    [SerializeField] private GameObject seedCounter;
    [SerializeField] private GameObject mainCamera;

    [SerializeField] private Transform homeRoomCameraPos;
    [SerializeField] private Transform counterCameraPos;
    [SerializeField] private Transform itemShopCameraPos;
    [SerializeField] private Transform seedCoinCameraPos;

    public void OnClickEnterHoomRoom()
    {
        homeRoom.SetActive(true);
        seedCounter.SetActive(false);
        mainCamera.transform.position = homeRoomCameraPos.position;
        mainCamera.transform.rotation = homeRoomCameraPos.rotation;
    }

    public void EnterSeedCounter()
    {

    }
}
