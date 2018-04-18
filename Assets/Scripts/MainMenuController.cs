using System.Collections;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject carpet;

    public CanvasGroup[] groups;

    public Transform[] cameraTransforms;

    private PostProcessingProfile post;

    private Camera mainCamera;

    public Light mainCameraSpotlight;

    private int activeCanvasGroupIndex;

    void Start()
    {
        // Init Carpets
        for (int r = 0; r < 38; r++)
        {
            for (int c = 0; c < 12; c++)
            {
                Instantiate(carpet, new Vector3(-220 + (r * 50), 0, 220 - (c * 50)), Quaternion.identity);
            }
        }

        mainCamera = Camera.main;
        activeCanvasGroupIndex = 0;
        post = mainCamera.GetComponent<PostProcessingBehaviour>().profile;

        Animator loadingAnimator = GameObject.Find("MenuCanvas").transform.Find("BlackImage").GetComponent<Animator>();
        loadingAnimator.SetTrigger("isBlack");
    }

    public void StartGamePlay()
    {
        MapSizeController mapData = GameObject.FindObjectOfType<MapSizeController>();

        CharacterSelectionController characterData = GameObject.FindObjectOfType<CharacterSelectionController>();

        // Save To Global Variable Settings
        GameSettings.MAP_WIDTH = int.Parse(mapData.widthInput.text);
        GameSettings.MAP_HEIGHT = int.Parse(mapData.heightInput.text);
        
        Dropdown[] selectedPlayers = characterData.GetComponentsInChildren<Dropdown>();

        for (int i = 0; i < selectedPlayers.Length; i++)
        {
            int characterType = selectedPlayers[i].value;
            GameSettings.players.Add(characterType);
        }

        GameSettings.SCENE_HELPER = 2;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void OpenMenu(int menuIndex)
    {
        StartCoroutine(StartTransition(menuIndex));
    }

    IEnumerator StartTransition(int menuIndex)
    {
        yield return StartCoroutine(Fade(true, groups[activeCanvasGroupIndex]));
        groups[activeCanvasGroupIndex].gameObject.SetActive(false);
        
        DepthOfFieldModel.Settings dof = post.depthOfField.settings;
        if (menuIndex == 0) // Main Menu
        {
            mainCameraSpotlight.transform.localPosition = new Vector3(0, 0, -300);
            mainCameraSpotlight.intensity = 2.5f;
            mainCameraSpotlight.spotAngle = 30f;
            dof.focusDistance = 75f;
            dof.aperture = 1.2f;
        }
        else if (menuIndex == 1) // Play
        {
            mainCameraSpotlight.transform.localPosition = new Vector3(120, 0, -300);
            mainCameraSpotlight.intensity = 2.5f;
            mainCameraSpotlight.spotAngle = 45f;
            dof.focusDistance = 150f;
            dof.aperture = 1.2f;
        }
        else if (menuIndex == 2) // Settings
        {
            mainCameraSpotlight.transform.localPosition = new Vector3(60, 0, -400);
            mainCameraSpotlight.intensity = 3f;
            mainCameraSpotlight.spotAngle = 30f;
            dof.focusDistance = 87f;
            dof.aperture = 10f;
        }
        post.depthOfField.settings = dof;

        yield return StartCoroutine(MoveCamera(cameraTransforms[menuIndex]));

        activeCanvasGroupIndex = menuIndex;
        groups[menuIndex].gameObject.SetActive(true);
        yield return StartCoroutine(Fade(false, groups[menuIndex]));
    }

    IEnumerator MoveCamera(Transform nextCamera)
    {
        while (Vector3.Distance(mainCamera.transform.position, nextCamera.position) >= 0.01f &&
               mainCamera.transform.rotation != nextCamera.rotation)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, nextCamera.position, Time.deltaTime * 4);
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, nextCamera.rotation, Time.deltaTime * 4);

            yield return null;
        }
    }

    IEnumerator Fade(bool fadeaway, CanvasGroup canvasGroup)
    {
        if (fadeaway)
        {
            for (float i = 0; i < 1; i += Time.deltaTime)
            {
                canvasGroup.alpha -= Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            for (float i = 0; i < 1; i += Time.deltaTime)
            {
                canvasGroup.alpha += Time.deltaTime;
                yield return null;
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
