using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreenController : MonoBehaviour
{
    public GameObject loadingBar, whiteImage;

    public Text tipsText;

    AsyncOperation async;

    private static string[] tips =
    {
        "Use Sniper Attack Range Wisely.",
        "Remember Operator Can Attack Inifinitely.",
        "Becareful Close Quarter Can Throw Grenade."
    };

    private RectTransform loadingBarTransform;

    private float width = 0.10f,
                  height = 0.25f;

    void Start()
    {
        int randomIndex = Random.Range(0, 3);
        tipsText.text += tips[randomIndex];

        loadingBarTransform = loadingBar.GetComponent<RectTransform>();
        width = 10f;
        height = 25f;
        loadingBarTransform.sizeDelta = new Vector2(width, height);

        LoadScreenExample();
    }

    public void LoadScreenExample()
    {
        StartCoroutine(LoadingScreen());
    }

    IEnumerator LoadingScreen()
    {
        float percent = 0.1f;

        async = SceneManager.LoadSceneAsync(Assets.Scripts.GameSettings.SCENE_HELPER);
        async.allowSceneActivation = false;

        yield return new WaitForSeconds(2);

        while (!async.isDone && percent < 1)
        {
            //width += 5f;

            Vector2 newLoadingBarSize = Vector2.Lerp(loadingBarTransform.sizeDelta, new Vector2(150f, height), percent);

            loadingBarTransform.sizeDelta = newLoadingBarSize;

            percent += 0.000001f;

            yield return new WaitForSeconds(0.000001f);

            //yield return new WaitForSeconds(0.5f);

            if (async.progress == 0.9f && loadingBarTransform.GetComponent<RectTransform>().rect.width >= 149f)
            {
                loadingBarTransform.sizeDelta = new Vector2(150f, height);

                whiteImage.GetComponent<Animator>().SetBool("isWhite", true);
                //yield return new WaitUntil(() => whiteImage.GetComponent<Image>().color.a == 1);

                if (whiteImage.GetComponent<Image>().color.a >= 0.95f)
                    async.allowSceneActivation = true;
            }
        }
    }
}
