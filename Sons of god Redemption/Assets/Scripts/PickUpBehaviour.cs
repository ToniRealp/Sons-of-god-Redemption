using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PickUpBehaviour : MonoBehaviour {

    public bool picked, flag;
    public Animator animator;

    private GameObject player;

    protected TextMeshProUGUI pickText;
    protected GameObject canvas, textPos;
    public GameObject pickTextGO;
    public TMP_FontAsset font;

    // Use this for initialization
    void Start () {
        picked = flag = false;

        //Health Text
        canvas = GameObject.Find("Canvas");
        pickTextGO = new GameObject();
        pickTextGO.transform.SetParent(canvas.transform);
        pickText = pickTextGO.AddComponent<TextMeshProUGUI>();
        pickText.font = font;
        pickTextGO.name = "PickMe";
        pickText.alignment = TextAlignmentOptions.Center;
        textPos = this.gameObject.transform.Find("pickTextPos").gameObject;
        pickTextGO.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        if (picked&&!flag)
        {
            GameObject.Find("Leliel").GetComponent<PlayerController>().baseAttack += 2;
            GameObject.Find("Leliel").GetComponent<PlayerController>().stats.health += 10;
            GameObject.Find("Leliel").GetComponent<PlayerController>().health += 10;
            GameObject.Find("Leliel").GetComponent<PlayerController>().healthBar.value = GameObject.Find("Leliel").GetComponent<PlayerController>().health;
            flag = true;
            pickTextGO.SetActive(false);
        }
        //Show pick text
        pickTextGO.GetComponent<Transform>().position = Camera.main.WorldToScreenPoint(textPos.transform.position);
        pickText.text = "Press E";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag=="Player")
        {
            player = other.gameObject;
            pickTextGO.SetActive(true);
        }
    
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<PlayerController>().interact)
            {
                picked = true;
                animator.SetTrigger("open");
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Hide pick text
            pickTextGO.SetActive(false);
        }
    }

    public void DestroyPickUp()
    {
        Destroy(this.gameObject);
    }

}
