using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;  
using System.Collections.Generic;
using System;

public class UIEraserTexture : MonoBehaviour ,IPointerDownHandler,IPointerUpHandler{

	public  RawImage image; 
	public  int brushScale = 4;

	Texture2D texRender; 
	RectTransform mRectTransform; 
	Canvas canvas;

	private Queue<Vector2> m_DrawPath = null;
	private Queue<Queue<Vector2>> m_WaitDrawPaths = new Queue<Queue<Vector2>>();
	private Queue<Vector2> m_ActivePath = null;

	private Vector2 m_LastPathPos = Vector2.zero;
	private Vector2 m_LastProcessPos = Vector2.zero;

    private Vector2 m_DrawPathStartPos = Vector2.zero;

	private bool isPointerDown = false;

	private float m_brushdRadius = 30f;

    private Color[] m_texPixels;

    private Rect m_drawArea;

	void Awake(){
		mRectTransform = GetComponent<RectTransform> (); 
		canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
	}

	void Start () {
        
        texRender = new Texture2D(image.texture.width, image.texture.height, TextureFormat.ARGB32, false, true);

        m_texPixels = texRender.GetPixels();

		Reset ();
	}

	public void OnPointerDown(PointerEventData data){
        Vector2 _uipos;
        if (ConvertSceneToUI(data.position, out _uipos))
		    AddToActivePath(_uipos);
		isPointerDown = true;
	}

	public void OnPointerUp(PointerEventData data){
        Vector2 _uipos;
        if (ConvertSceneToUI(data.position, out _uipos))
		    AddToActivePath(_uipos);
		EndActivePath();
		isPointerDown = false;
		m_LastPathPos = Vector2.zero;
	}

	private void OnPointerMove(){
        Vector2 _uipos;
        if (ConvertSceneToUI(Input.mousePosition, out _uipos))
		    AddToActivePath(_uipos);
	}

    bool ConvertSceneToUI(Vector2 screenPos, out Vector2 uiPos)
    {
        bool _ret = RectTransformUtility.ScreenPointToLocalPointInRectangle(mRectTransform, screenPos, null, out uiPos);
        if (_ret)
        {
            uiPos.x += mRectTransform.rect.width / 2;
            uiPos.y += mRectTransform.rect.height / 2;
        }

        return _ret;
    }

    /// <summary>
    /// 把当前坐标加入到未完成的路径当中
    /// </summary>
    /// <param name="pos">UI坐标</param>
    private void AddToActivePath(Vector2 pos)
	{
		if (m_LastPathPos == null
			|| Vector2.Distance(pos, m_LastPathPos) < 10)
			return;

		m_LastPathPos = pos;

		if (m_ActivePath == null)
			m_ActivePath = new Queue<Vector2>();
		
		m_ActivePath.Enqueue(pos);
	}

	/// <summary>
	/// 完成当路径, 并将当前路径加入到待处理路径当中
	/// </summary>
	private void EndActivePath()
	{
		if (m_ActivePath != null)
		{
			m_WaitDrawPaths.Enqueue(m_ActivePath);
			m_ActivePath = null;
		}
	}

	/// <summary>
	/// 处理绘制路径
	/// </summary>
	private void ProcessWaitDrawPath()
	{
		if (m_DrawPath == null
			|| m_DrawPath.Count == 0)
		{
			if (m_WaitDrawPaths.Count > 0)
			{
				m_DrawPath = m_WaitDrawPaths.Dequeue();
            }
			else
			{
				m_DrawPath = m_ActivePath;
			}
		}

		if (m_DrawPath != null && m_DrawPath.Count > 0)
		{
            m_drawArea = Rect.zero;

            float _starttime = Time.time;
			while(m_DrawPath.Count > 0)
			{
				Vector2 _newpos = m_DrawPath.Dequeue();
				RenderPath(m_LastProcessPos, _newpos);
				m_LastProcessPos = _newpos;
				if (Time.time - _starttime > 0.03)
					break;
			}

            RenderDrawArea();
            //texRender.SetPixels(m_texPixels);
            //texRender.Apply();
        }
	}

    private void FillDrawArea(Rect newArea)
    {
        if (m_drawArea == Rect.zero)
        {
            m_drawArea = newArea;
            return;
        }

        m_drawArea.xMin = Mathf.Min(m_drawArea.xMin, newArea.xMin);
        m_drawArea.xMax = Mathf.Max(m_drawArea.xMax, newArea.xMax);
        m_drawArea.yMin = Mathf.Min(m_drawArea.yMin, newArea.yMin);
        m_drawArea.yMax = Mathf.Max(m_drawArea.yMax, newArea.yMax);
    }

    Color[] m_drawPixelsBuffer;
    private void RenderDrawArea()
    {
        try
        {
            int xMin = (int)m_drawArea.xMin;
            int yMin = (int)m_drawArea.yMin;
            int width = (int)m_drawArea.width;
            int height = (int)m_drawArea.height;

            int needBufferSize = width * height;
            if (m_drawPixelsBuffer == null || m_drawPixelsBuffer.Length < needBufferSize)
            {
                m_drawPixelsBuffer = new Color[needBufferSize];
            }

            for (int y = 0; y < height; ++y)
            {
                Array.Copy(m_texPixels, (yMin + y) * texRender.width + xMin, m_drawPixelsBuffer, y * width, width);
            }

            texRender.SetPixels(xMin, yMin, width, height, m_drawPixelsBuffer);
            texRender.Apply();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

	private void RenderPath(Vector2 start, Vector2 end)
	{
        DrawSphere(end, m_brushdRadius);

// 		if (start.Equals (end)) {
// 			return;
// 		}
// 
// 		Rect disract = new Rect ((start+end).x/2+texRender.width/2, (start+end).y/2+texRender.height/2, Mathf.Abs (end.x-start.x), Mathf.Abs (end.y-start.y));
// 
// 		for (int x = (int)disract.xMin; x < (int)disract.xMax; x++) {
// 			for (int y = (int)disract.yMin; y < (int)disract.yMax; y++) {
// 				Draw (new Rect (x, y, brushScale, brushScale));
// 			}    
// 		}     
// 
// 		start = end;
	}

	void LateUpdate()
	{
		if (isPointerDown)
			OnPointerMove();

		ProcessWaitDrawPath();
	}

	Vector2 start = Vector2.zero;
	Vector2 end = Vector2.zero;


	public void Reset(){

        if (m_texPixels == null)
            return;

        for (int i = 0; i < m_texPixels.Length; ++i)
        {
            m_texPixels[i].a = 1f;
            m_texPixels[i].r = 0.8f;
            m_texPixels[i].g = 0.8f;
            m_texPixels[i].b = 0.8f;
        }

        texRender.SetPixels(m_texPixels);
        texRender.Apply();
        image.material.SetTexture("_RendTex", texRender);
	}

	void Draw(Rect rect){ 

		for (int x = (int)rect.xMin; x < (int)rect.xMax; x++) {
			for (int y = (int)rect.yMin; y < (int)rect.yMax; y++) {
				if (x < 0 || x > texRender.width || y < 0 || y > texRender.height) {
					return;
				}   
				Color color = texRender.GetPixel (x,y);
				color.a = 0;
				texRender.SetPixel (x,y,color);
			}    
		}     

		texRender.Apply(); 
		image.material.SetTexture ("_RendTex",texRender);
	}

	void DrawSphere(Vector2 center, float radius)
	{
        Rect _rc = new Rect();
        _rc.xMin = Mathf.Max(0, center.x - radius);
        _rc.xMax = Mathf.Min(texRender.width, center.x + radius);
        _rc.yMin = Mathf.Max(0, center.y - radius);
        _rc.yMax = Mathf.Min(texRender.height, center.y + radius);

        for (int x = (int)_rc.xMin; x < (int)_rc.xMax; x++)
        {
            for (int y = (int)_rc.yMin; y < (int)_rc.yMax; y++)
            {
                Vector2 _newPos = new Vector2(x, y);
                float _radius = Vector2.Distance(_newPos, center);
                if (_radius < radius)
                {
                    float alpha = 0;
//                     if (_radius > radius / 2)
//                         alpha = (_radius - radius / 2) / (radius / 2);
                    SetAlpha(x, y, alpha);
                }
            }
        }

        FillDrawArea(_rc);
    }

    void SetAlpha(int x, int y, float alpha)
    {
        int _index = y * texRender.width + x;
        m_texPixels[_index].a = Mathf.Min(m_texPixels[_index].a, alpha);
    }
}