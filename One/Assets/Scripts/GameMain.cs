using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMain : MonoBehaviour {

    public Sprite[] GamePictures;

    public GameObject ItemTemplate;
    
    public UIEraserTexture EraserTexture;

    public GameObject GamePanel;

    public GameObject GameList;

	// Use this for initialization
	void Start () {

        InitPictures();
        InitItems();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void InitPictures()
    {
        GamePictures = Resources.LoadAll<Sprite>("GamePictures");
    }

    void InitItems()
    {
        for (int i = 0; i < GamePictures.Length; ++i)
        {
            int _index = i;
            GameObject _item = GameObject.Instantiate(ItemTemplate);
            _item.transform.SetParent(ItemTemplate.transform.parent);
            _item.transform.localScale = Vector3.one;
            _item.SetActive(true);
            _item.GetComponent<Image>().sprite = GamePictures[i];
            _item.GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectItem(_index);
            });
        }
    }
   
    void SelectItem(int index)
    {
        GamePanel.SetActive(true);
        GameList.SetActive(false);
        EraserTexture.image.texture = GamePictures[index].texture;
        EraserTexture.Reset();
    }

    public void ShowList()
    {
        GamePanel.SetActive(false);
        GameList.SetActive(true);
    }
}
