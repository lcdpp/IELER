using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GamePictureInfo
{
    public Sprite Picture;
    public Color LineColor1;
    public Color LineColor2;
    public Color LineColor3;
    public Color LineColor4;
    public Color LineColor5;
}

public class GameMain : MonoBehaviour {

    public GamePictureInfo[] GamePictures;

    public GamePictureInfo CurGamePicture;

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
		
        if (EraserTexture != null && CurGamePicture != null)
        {
        }
	}

    void InitPictures()
    {
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
            _item.GetComponent<Image>().sprite = GamePictures[i].Picture;
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
        CurGamePicture = GamePictures[index];
        EraserTexture.image.texture = CurGamePicture.Picture.texture;
        EraserTexture.image.material.SetColor("_LineColor1", CurGamePicture.LineColor1);
        EraserTexture.image.material.SetColor("_LineColor2", CurGamePicture.LineColor2);
        EraserTexture.image.material.SetColor("_LineColor3", CurGamePicture.LineColor3);
        EraserTexture.image.material.SetColor("_LineColor4", CurGamePicture.LineColor4);
        EraserTexture.image.material.SetColor("_LineColor5", CurGamePicture.LineColor5);
        EraserTexture.Reset();
    }

    public void ShowList()
    {
        GamePanel.SetActive(false);
        GameList.SetActive(true);
    }
}
