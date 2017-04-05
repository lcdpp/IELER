using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;  
using System.Collections.Generic;

public class UIEraserTexture : MonoBehaviour ,IPointerDownHandler,IPointerUpHandler{

	public  RawImage image; 
	public  int brushScale = 4;

	Texture2D texRender; 
	RectTransform mRectTransform; 
	Canvas canvas;

	private Queue<Vector2> m_DrawPath = null;
	private Queue<Queue<Vector2>> m_WaitDrawPaths = new Queue<Queue<Vector2>>();
	private Queue<Vector2> m_ActivePath = null;

	private Vector2 m_LastPathPos = null;
	private Vector2 m_LastProcessPos = null;

	private bool isPointerDown = false;

	private float m_brushdRadius = 5f;

	void Awake(){
		mRectTransform = GetComponent<RectTransform> (); 
		canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
	}

	void Start () {

		texRender = new Texture2D(image.mainTexture.width, image.mainTexture.height,TextureFormat.ARGB32,true);

		Reset ();
	}

	public void OnPointerDown(PointerEventData data){
		Vector2 _uipos = ConvertSceneToUI(data.position);
		AddToActivePath(_uipos);
		isPointerDown = true;
	}

	public void OnPointerUp(PointerEventData data){
		Vector2 _uipos = ConvertSceneToUI(data.position);
		AddToActivePath(_uipos);
		EndActivePath();
		isPointerDown = false;
		m_LastPathPos = null;
	}

	private void OnPointerMove()
	{
		Vector2 _uipos = ConvertSceneToUI(Input.mousePosition);
		AddToActivePath(_uipos);
	}

	/// <summary>
	/// 把当前坐标加入到未完成的路径当中
	/// </summary>
	/// <param name="pos">UI坐标</param>
	private void AddToActivePath(Vector2 pos)
	{
		if (m_LastPathPos == null
			&& Vector2.Distance(pos, m_LastPathPos) < 1)
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
			float _starttime = Time.time;
			while(m_DrawPath.Count > 0)
			{
				Vector2 _newpos = m_DrawPath.Dequeue();
				RenderPath(m_LastProcessPos, _newpos);
				m_LastProcessPos = _newpos;
				if (Time.time - _starttime > 0.03)
					break;
			}
		}
	}

	private void RenderPath(Vector2 start, Vector2 end)
	{
		Draw (new Rect (end.x+texRender.width/2, end.y+texRender.height/2, brushScale, brushScale));

		if (start.Equals (Vector2.zero)) {
			return;
		}

		Rect disract = new Rect ((start+end).x/2+texRender.width/2, (start+end).y/2+texRender.height/2, Mathf.Abs (end.x-start.x), Mathf.Abs (end.y-start.y));

		for (int x = (int)disract.xMin; x < (int)disract.xMax; x++) {
			for (int y = (int)disract.yMin; y < (int)disract.yMax; y++) {
				Draw (new Rect (x, y, brushScale, brushScale));
			}    
		}     

		start = end;
	}

	void Update()
	{
		if (isPointerDown)
			OnPointerMove();

		ProcessWaitDrawPath();
	}

	Vector2 start = Vector2.zero;
	Vector2 end = Vector2.zero;

	Vector2 ConvertSceneToUI(Vector3 posi){
		Vector2 postion;
		if(RectTransformUtility.ScreenPointToLocalPointInRectangle(mRectTransform , posi, canvas.worldCamera, out postion)){
			return postion;
		} 
		return null;
	}

	void Reset(){

		for (int i = 0; i < texRender.width; i++) {

			for (int j = 0; j < texRender.height; j++) {

				Color color = texRender.GetPixel (i,j);
				color.a = 1;
				texRender.SetPixel (i,j,color); 
			}  
		}   

		texRender.Apply ();
		image.material.SetTexture ("_RendTex",texRender);

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
		
	}
}