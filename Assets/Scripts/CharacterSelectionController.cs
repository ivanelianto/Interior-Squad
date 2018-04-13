using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionController : MonoBehaviour
{
    // Canvas Group
    public GameObject characterGroup;

    // Dropdown
    public GameObject playerOption;

    // Button
    public GameObject addButton;

    public List<GameObject> players = new List<GameObject>();

    private Vector3[] playerOptionLocalPosition = new Vector3[4];
    private Vector3[] playerOptionInstantiatePosition = new Vector3[4];

    void Start()
    {
        // Init Player
        for (int i = 0; i < 4; i++)
        {
            float x = playerOption.transform.position.x;
            float y = playerOption.transform.position.y - (i * 7.6f);
            float z = playerOption.transform.position.z;
            playerOptionInstantiatePosition[i] = new Vector3(x, y, z);

            GameObject player = InitializePlayer(playerOptionInstantiatePosition[i]);
            players.Add(player);

            // Sniper and Close Quarter
            Dropdown ui = player.GetComponentInChildren<Dropdown>();
            if (i == 2)
                ui.value = 1;
            else if (i == 3)
                ui.value = 2;
        }

        // Add Local Positions
        for (int i = 0; i < 4; i++)
            playerOptionLocalPosition[i] = players[i].transform.localPosition;

        // At Least 1 Player
        players[0].GetComponentInChildren<Button>().gameObject.SetActive(false);
    }

    public void AddCharacter()
    {
        GameObject player = InitializePlayer(playerOptionInstantiatePosition[players.Count]);
        players.Add(player);
        ToggleAddCharacterButtonVisibility();
    }

    private GameObject InitializePlayer(Vector3 position)
    {
        GameObject player = Instantiate(playerOption, position, Quaternion.identity);
        player.transform.SetParent(characterGroup.transform, false);
        player.GetComponentInChildren<Button>().onClick.AddListener(
            () =>
            {
                DestroyPlayer(player);
            });

        return player;
    }

    private void ToggleAddCharacterButtonVisibility()
    {
        if (players.Count < 4)
            addButton.SetActive(true);
        else
            addButton.SetActive(false);
    }

    public void DestroyPlayer(GameObject player)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetInstanceID() == player.GetInstanceID())
            {
                DestroyObject(players[i]);
                players.RemoveAt(i);

                break;
            }
        }

        SetCharacterOptionPosition();
    }

    private void SetCharacterOptionPosition()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].transform.localPosition = playerOptionLocalPosition[i];
        }

        ToggleAddCharacterButtonVisibility();
    }
}
