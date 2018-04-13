using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameLoader : MonoBehaviour
{
    public Text playerNameUI;

    public Slider playerHpUI;

    public GameObject playerStateUI, actionMenuUI;

    void Start()
    {
        playerStateUI = GameObject.FindGameObjectWithTag("GameStateCanvas");
        playerStateUI = playerStateUI.transform.Find("PlayerState").gameObject;

        // Init Action Menu
        actionMenuUI = GameObject.FindGameObjectWithTag("GameStateCanvas").transform.Find("ActionMenu").gameObject;

        // Init Name
        playerNameUI = playerStateUI.GetComponentInChildren<Text>();

        // Init HP Bar
        playerHpUI = playerStateUI.GetComponentInChildren<Slider>();
    }
}
