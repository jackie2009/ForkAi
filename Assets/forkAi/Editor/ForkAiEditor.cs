
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Xml;



public enum LinkDirEnum
{
	down=0,up=1,right=2,left=3
}
public class NodeVo{
	public int id=-1;
	public int aiType;
	public int aiIndex;//xml index like type
	public int targertID=-1;
	public LinkDirEnum dir=LinkDirEnum.down;
	public LinkDirEnum targetDir=LinkDirEnum.down;

	public int targertID2=-1;
	public LinkDirEnum dir2=LinkDirEnum.down;
	public LinkDirEnum targetDir2=LinkDirEnum.down;
	public Rect windowRect = new Rect(50, 50, 200, 70);
	public string param="";
 
	  
	 
 
	private int getNextID(){
		int nextID = 0;
		while (true) {
			if(ForkAiEditor.dirModeMap.ContainsKey(nextID)==false){return nextID;}
			nextID++;
		}
		 
	}
	public NodeVo (bool autoID=true){
		if (autoID) {
			id = getNextID ();
 
			ForkAiEditor.dirModeMap [id] = this;
		}
	}
	public void calculateIndex(){
	XmlNodeList list=	ForkAiEditor.xmlRoot.SelectNodes ("item/id");
		for (int i = 0; i < list.Count; i++) {
			if(int.Parse(list[i].InnerText)==aiType){
				aiIndex=i;
				break;
			}
		}
	}
	public void calculateType(){
		aiType = int.Parse (ForkAiEditor.xmlRoot.SelectNodes ("item/id") [aiIndex].InnerText);
	}
}
 
public class ForkAiEditor : EditorWindow {
	 
	 
	//窗口的ID
public	static Dictionary<int,NodeVo> dirModeMap;
 	static Texture2D arrow;
 
	public static XmlNode xmlRoot;
	public static string filePath="";
	 [MenuItem("Window/ForkAiEditor")]
	static void ShowEditor() {
	  EditorWindow.GetWindow<ForkAiEditor>();
		 
		dirModeMap = new  Dictionary<int, NodeVo> ();
		XmlDocument xmlDoc = new XmlDocument();
	 
		 xmlDoc.LoadXml(Resources.Load<TextAsset>("configAi").text);
		xmlRoot= xmlDoc.FirstChild;
 
 		arrow = Resources.Load<Texture2D> ("arrow");
		XmlNodeList list=	xmlRoot.SelectNodes("item/label");
		aiLabels=new string[list.Count];
		for (int i = 0; i < list.Count; i++) {
			aiLabels[i]=list[i].InnerText;
		}
		filePath = "";
	
	}
	Vector2 rectAll;
	  Vector2 scrollPosition=new Vector2(0,0);
static	string [] aiLabels;
	NodeVo currentDrawing=new NodeVo(false);
	void OnGUI() {
		EditorGUILayout.BeginHorizontal ();
	 
		GUILayout.Label (filePath);
		if (GUILayout.Button ("新建节点")) {
			NodeVo vo=	 new NodeVo ();
			vo.windowRect.position=scrollPosition+new Vector2(this.position.width/2-100,this.position.height/2-35);
		}
		if (GUILayout.Button ("保存文件")) {
			saveFile();
		}
		if (GUILayout.Button ("文件另存为")) {
			filePath="";
			saveFile();
				}
		 
		if (GUILayout.Button ("打开文件")) {
			ShowEditor();
			string path = EditorUtility.OpenFilePanel("Load txt", "", "");

			StreamReader sr = new StreamReader(path);  
			while (!sr.EndOfStream) {
				NodeVo vo=new NodeVo(false);
				vo.aiType=int.Parse(sr.ReadLine());
				vo.dir=(LinkDirEnum)int.Parse(sr.ReadLine());
				vo.dir2=(LinkDirEnum)int.Parse(sr.ReadLine());
				vo.id=int.Parse(sr.ReadLine());
				vo.param=sr.ReadLine();
				vo.targertID=int.Parse(sr.ReadLine());
				vo.targertID2=int.Parse(sr.ReadLine());
				vo.targetDir=(LinkDirEnum)int.Parse(sr.ReadLine());
				vo.targetDir2=(LinkDirEnum)int.Parse(sr.ReadLine());
				vo.windowRect.x=int.Parse(sr.ReadLine());
				vo.windowRect.y=int.Parse(sr.ReadLine());

				dirModeMap[vo.id]=vo;
				vo.calculateIndex();
			}
			 

			sr.Close();
			filePath=path;
		}
		 
		EditorGUILayout.EndHorizontal ();
		scrollPosition = GUI.BeginScrollView(new Rect(0,0,this.position.width,this.position.height),  
		                                     scrollPosition, new Rect(0, 0, rectAll.x, rectAll.y));  
		//绘画窗口
		rectAll = Vector2.zero;
 		BeginWindows();
		if (dirModeMap == null)
						return;
 		foreach (var item in dirModeMap.Values) {
			var nodeType = xmlRoot.SelectNodes("item/type")[item.aiIndex].InnerText;
			if (nodeType == "1"){
				GUI.color=new Color(0.7f,0.7f,1);
			}else{
				GUI.color=Color.white;
			}
			if (nodeType == "2")
			{
				GUI.color = Color.yellow;
			}

			item.windowRect = GUI.Window(item.id, item.windowRect, DrawNodeWindow, item.id+"");
			rectAll.x=Mathf.Max(rectAll.x,item.windowRect.max.x);
			rectAll.y=Mathf.Max(rectAll.y,item.windowRect.max.y);
		}

 
 		EndWindows();
		//连接窗口
		foreach (var item in dirModeMap.Values) {
			if(item.targertID>=0){
			DrawNodeCurve(item,dirModeMap[item.targertID],1);
			}

			if(item.targertID2>=0){
				DrawNodeCurve(item,dirModeMap[item.targertID2],2);
			}
		 
		}
		if (Event.current.type == EventType.MouseDown) {
			if (Event.current.button == 0) {
				currentDrawing.id = -1;
			}
			if (Event.current.button == 2) {
				currentDrawing.aiType = 3 - currentDrawing.aiType;
				
			}

			if (Event.current.button == 1) {

				if(	currentDrawing.id >=0){
					if(currentDrawing.aiType==1)
						dirModeMap[currentDrawing.id].targertID=-1;
					else
						dirModeMap[currentDrawing.id].targertID2=-1;
					currentDrawing.id = -1;


				}
			}
		}
		 
	 
	 
		if (currentDrawing.id != -1) {
			DrawNodeCurve(currentDrawing,currentDrawing.aiType);
			 
		}
		GUI.EndScrollView ();
		 
	}
	void Update(){
		 
			Repaint();
	 
	}
	void upateCurrentLink(LinkDirEnum dir,NodeVo vo){
		if(currentDrawing.id!=-1&&currentDrawing.id!=vo.id){
			NodeVo src=dirModeMap[currentDrawing.id];
 
			if(currentDrawing.aiType==1){
			src.targertID=vo.id;
				src.dir=currentDrawing.dir;
			src.targetDir=dir;
			}else{
				src.targertID2=vo.id;
				src.dir2=currentDrawing.dir;
				src.targetDir2=dir;
			}
			currentDrawing.id=-1;
		}else{
			currentDrawing.aiType=1;
			currentDrawing.windowRect=vo.windowRect;
			currentDrawing.dir=dir;
			currentDrawing.id=vo.id;
		}
	}
	//绘画窗口函数
	void DrawNodeWindow(int id) {
 
	NodeVo vo=	dirModeMap[id];
		GUI.color = new Color (0, 0, 0, 0.2f);
		Rect up = new Rect (100 - 10, 0, 20, 20);
		Rect down = new Rect (100-10, 70-20,20, 20);
		Rect left = new Rect (0, 35-10,20, 20);
		Rect right = new Rect (200-20, 35-10,20, 20);
 
		if (GUI.Button (up, "")) {
			upateCurrentLink(LinkDirEnum.up,vo);
		}

	 
		if (GUI.Button (down, "")) {
			upateCurrentLink(LinkDirEnum.down,vo);
		}
		if (GUI.Button (right, "")) {
			upateCurrentLink(LinkDirEnum.right,vo);
		}
		if (GUI.Button (left, "")) {
			upateCurrentLink(LinkDirEnum.left,vo);
		}

	
		GUI.color = Color.white;
		GUILayout.BeginHorizontal();
		GUILayout.Space (20);
		vo.aiIndex=EditorGUILayout.Popup (vo.aiIndex,aiLabels) ;
		if(GUILayout.Button ("+")){
			NodeVo voNew=new NodeVo();
			voNew.windowRect=vo.windowRect;
			voNew.aiIndex=vo.aiIndex;
			voNew.aiType=vo.aiType;
			voNew.param=vo.param;
			voNew.windowRect.position+=new Vector2(0,80);
		}
		if(GUILayout.Button ("!")){
			EditorUtility.DisplayDialog(aiLabels[vo.aiIndex],	xmlRoot.SelectNodes("item/help")[vo.aiIndex].InnerText,"ok");
		}
		if(GUILayout.Button ("X")){
			dirModeMap.Remove(id);
			foreach (var item in dirModeMap.Values) {
				if(item.targertID==id){
					item.targertID=-1;
					 
				}
				if(item.targertID2==id){
					item.targertID2=-1;
					
				}
			}
		}
		GUILayout.Space (20);
		GUILayout.EndHorizontal();
	
		GUILayout.BeginHorizontal();
		GUILayout.Space (20);
 
		vo.param = EditorGUILayout.TextField (vo.param);
		GUILayout.Space (20);
		GUILayout.EndHorizontal();
 
		 GUI.DragWindow();
		 
	}
	void saveFile ()
	{
		string path = filePath;
		if(filePath.Length<3){
			path = EditorUtility.SaveFilePanel("save txt", "", "temp.txt","txt");
		}
		
		StreamWriter sw = new StreamWriter(path);
		foreach (var item in dirModeMap.Values) {
			item.calculateType();
			sw.WriteLine((int)item.aiType);
			sw.WriteLine((int)item.dir);//non
			sw.WriteLine((int)item.dir2);//non
			sw.WriteLine((int)item.id);
			sw.WriteLine(item.param);
		 
			sw.WriteLine((int)item.targertID);
			sw.WriteLine((int)item.targertID2);
			sw.WriteLine((int)item.targetDir);//non
			sw.WriteLine((int)item.targetDir2);//non
			sw.WriteLine((int)item.windowRect.x);//non
			sw.WriteLine((int)item.windowRect.y);//non
		}
		sw.Flush(); 
		sw.Close();
        filePath = path;
        Debug.Log(filePath);

       

	}
	void DrawNodeCurve(NodeVo self,int mode) {
		
		Rect start = self.windowRect;
		 
		LinkDirEnum targetDir =   self.targetDir ;
		LinkDirEnum dir =   self.dir ;
		Vector3 startPos = new Vector3(start.x + start.width/2, start.y + start.height / 2, 0)+ getDir(dir,start);
		Vector3 endPos =  Event.current.mousePosition;//new Vector3(Input.mousePosition.x,this.maxSize.y-Input.mousePosition.y,0);

		Vector3 startTan = startPos + getDir(dir,start).normalized*80;
		Vector3 endTan = endPos ;
		Handles.DrawBezier(startPos, endPos, startTan, endTan, mode==1?Color.green:Color.blue, null, 4);
		GUI.color = mode == 1 ? Color.green : Color.blue;
		int rot = 0;
		switch (targetDir) {
		case LinkDirEnum.down:
			rot= -90;
			break;
		case LinkDirEnum.up:
			rot= 90;
			break;
		case LinkDirEnum.right:
			rot= 180;
			break;
		case LinkDirEnum.left:
			rot= 0;
			break;
		}
		GUIUtility.RotateAroundPivot(rot,endPos);
		GUI.DrawTexture (new Rect(endPos.x-arrow.width/2,endPos.y-arrow.height/2,arrow.width,arrow.height), arrow);
		GUIUtility.RotateAroundPivot(-rot,endPos);
	}
	void DrawNodeCurve(NodeVo self,NodeVo next,int mode) {

		Rect start = self.windowRect;
		Rect end = next.windowRect;
		LinkDirEnum targetDir = mode == 1 ? self.targetDir : self.targetDir2;
		LinkDirEnum dir = mode == 1 ? self.dir : self.dir2;
		Vector3 startPos = new Vector3(start.x + start.width/2, start.y + start.height / 2, 0)+ getDir(dir,start);
		Vector3 endPos = new Vector3 (end.x + end.width / 2, end.y + end.height / 2, 0) + getDir (targetDir,end);
	
		Vector3 startTan = startPos + getDir(dir,start).normalized*80;
		Vector3 endTan = endPos + getDir(targetDir,end).normalized*80;
		Handles.DrawBezier(startPos, endPos, startTan, endTan, mode==1?Color.green:Color.blue, null, 4);
	GUI.color = mode == 1 ? Color.green : Color.blue;
		int rot = 0;
		switch (targetDir) {
		case LinkDirEnum.down:
			rot= -90;
			break;
		case LinkDirEnum.up:
			rot= 90;
			break;
		case LinkDirEnum.right:
			rot= 180;
			break;
		case LinkDirEnum.left:
			rot= 0;
			break;
		}
		GUIUtility.RotateAroundPivot(rot,endPos);
		GUI.DrawTexture (new Rect(endPos.x-arrow.width/2,endPos.y-arrow.height/2,arrow.width,arrow.height), arrow);
		GUIUtility.RotateAroundPivot(-rot,endPos);
	}

	Vector3 getDir (LinkDirEnum i,Rect rect)
	{
		switch (i) {
		case LinkDirEnum.down:
			return -Vector3.down*rect.height/2;
		case LinkDirEnum.up:
			return -Vector3.up*rect.height/2;
		case LinkDirEnum.right:
			return Vector3.right*rect.width/2;
		case LinkDirEnum.left:
			return Vector3.left*rect.width/2;
		}
		return Vector3.down;
	}

}