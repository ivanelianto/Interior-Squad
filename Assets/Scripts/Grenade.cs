using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grenade : MonoBehaviour
{
    public float delay,
                 radius,
                 force;

    public GameObject explosionEffectParticle;

    public AudioClip explodeSound;

    private GameObject gameManager;

    private Text anyText;

    private static List<Text> texts = new List<Text>();

    private bool hasExplode = false,
                 startExplodingTime = false;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("MainScript");
        //anyText = gameManager.GetComponent<Gameplay>().anyText;

        //texts = new List<Text>();

        //for (int i = 0; i < 4; i++)
        //    texts.Add(Instantiate(anyText));
    }

    // Update is called once per frame
    void Update()
    {
        if (startExplodingTime)
            delay -= Time.deltaTime;

        if (delay <= 0f && !hasExplode)
        {
            StartCoroutine(Wrapper());
            hasExplode = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (!hasExplode)
            StartExplodingTime();
    }

    public void StartExplodingTime()
    {
        startExplodingTime = true;
    }

    IEnumerator Wrapper()
    {
        yield return StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        GameObject particle = Instantiate(explosionEffectParticle, transform.position, transform.rotation);

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();

            if (rb != null)
            {
                int damage = CalculateDamage(collider.gameObject);

                Player character = collider.gameObject.GetComponent<Player>();

                if (character != null)
                {
                    character.playerHp -= damage;

                    if (character.playerHp <= 0)
                    {
                        character.GetComponent<Animator>().enabled = false;

                        character.currentStandingTile.swat = null;

                        character.GetComponentInChildren<Light>().enabled = false;

                        character.currentStandingTile.isFilled = false;

                        character.gameObject.layer = LayerMask.NameToLayer("Default");

                        if (character.isPlayer)
                            gameManager.GetComponent<Gameplay>().players.Remove(character.gameObject);
                        else
                            gameManager.GetComponent<Gameplay>().enemies.Remove(character.gameObject);

                        Rigidbody[] characterRigidbody = character.gameObject.GetComponentsInChildren<Rigidbody>();

                        foreach (Rigidbody crb in characterRigidbody)
                            crb.isKinematic = false;

                        rb.AddExplosionForce(force * 500f, transform.position, radius);
                    }
                }
            }
        }

        AudioSource.PlayClipAtPoint(explodeSound, transform.position);

        // Delay Before Destroying Object
        yield return new WaitForSeconds(2);

        Destroy(gameObject);

        Destroy(particle);
    }

    int CalculateDamage(GameObject explosionObject)
    {
        Vector3 positionAttacker = this.transform.position;
        Vector3 positionVictim = explosionObject.transform.position;
        Vector3 positionDelta = positionVictim - positionAttacker;

        float damage = Mathf.Lerp(70, 10, (positionDelta.magnitude / 10) / 5);

        return (int)System.Math.Round((double)damage);
    }

    IEnumerator ShowDamage(int damage, Transform parentObject)
    {
        yield return null;

        if (parentObject != null)
        {
            foreach (Text t in texts)
            {
                t.gameObject.SetActive(true);

                t.text = "-" + damage;

                float percent = 0.1f;

                while (percent < 1f)
                {
                    Vector3 start = parentObject.Find("PlayerNamePlaceholder").transform.position + (Vector3.up * 1);
                    Vector3 finish = parentObject.Find("PlayerNamePlaceholder").transform.position + (Vector3.up * 5);

                    Vector3 newPosition = Vector3.Lerp(start, finish, percent);

                    t.transform.position = Camera.main.WorldToScreenPoint(newPosition);

                    t.GetComponent<CanvasGroup>().alpha -= 0.02f;

                    percent += 0.02f;

                    yield return null;
                }

                t.GetComponent<CanvasGroup>().alpha = 1f;

                Destroy(t);
            }
        }
    }
}
