using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class DebugEvents : MonoBehaviour
{
    [SerializeField] private GameObject counterRoomChange, itemShopChange, seedCoinChange;
    [SerializeField] private GameObject counterRoomChangeButton, itemShopChangeButton, seedCoinChangeButton;

    public void LoadGameScene0()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadGameScene1()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadGameScene2()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadGameScene3()
    {
        SceneManager.LoadScene(3);
    }

    public void LoadGameCamera1()
    {
        counterRoomChangeButton.SetActive(false);
        itemShopChangeButton.SetActive(true);
        counterRoomChange.SetActive(false);
        itemShopChange.SetActive(true);
    }

    public void LoadGameCamera2()
    {
        itemShopChangeButton.SetActive(false);
        seedCoinChangeButton.SetActive(true);
        itemShopChange.SetActive(false);
        seedCoinChange.SetActive(true);
    }

    public void LoadGameScene4()
    {
        SceneManager.LoadScene(4);
    }
}