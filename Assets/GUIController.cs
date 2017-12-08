using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.XR.iOS;

public class FurnitureResult {
	public string name;
	public string objURL;
	public string ImgGIF;
	public string ImgPNG;
}

public class GUIController : MonoBehaviour {

	WebCamTexture webCamTexture;
	//start with TABLE
	private string serverURL = "http://holland.cs.washington.edu:8889";
	float m_furnitureScale = 1.7f;
	List<FurnitureResult> furnitureResultList;

	public GameObject parentObject;

	// Use this for initialization
	void Start () {
		webCamTexture = new WebCamTexture();
		webCamTexture.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width -150.0f, 0.0f, 150.0f, 100.0f), "Camera"))
		{
			Debug.Log ("Camera Enabled!");

		}
		
		if (GUI.Button(new Rect(Screen.width -150.0f, 110.0f, 150.0f, 100.0f), "Capture"))
		{
			Debug.Log ("ButtonPressed!");
			StartCoroutine(UploadPNG ());
		}

	}

	IEnumerator UploadPNG()
	{
		// NOTE - you almost certainly have to do this here:
		//webCamTexture.Play();
		yield return new WaitForEndOfFrame(); 

		// it's a rare case where the Unity doco is pretty clear,
		// http://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html
		// be sure to scroll down to the SECOND long example on that doco page 

		Debug.Log ("Setting Pixels!");


		//Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height);
		//photo.SetPixels(webCamTexture.GetPixels());
		//photo.Apply();

		////
		/// 
		Resolution currentResolution = Screen.currentResolution;
		Texture2D tex = new Texture2D(currentResolution.width, currentResolution.height, TextureFormat.RGB24, false);
		Texture2D cameraTexture = new Texture2D(currentResolution.width, currentResolution.height, TextureFormat.RGB24, false);

		GL.InvalidateState();

		RenderTexture currentRT = RenderTexture.active;

		RenderTexture renderTexture = new RenderTexture(cameraTexture.width, cameraTexture.height, 32);//Camera.main.activeTexture;
		Texture camtex = FindObjectOfType<UnityARVideo> ().m_ClearMaterial.mainTexture;

		Graphics.Blit(camtex, renderTexture);

		RenderTexture.active = renderTexture;

		tex.ReadPixels(new Rect (0, 0, currentResolution.width, currentResolution.height), 0, 0);
		//tex.SetPixels(croppedPixels);
		tex.Apply();

		RenderTexture.active = currentRT;

		//webCamTexture.Stop();

		//Encode to a PNG
		byte[] bytes = tex.EncodeToPNG();
		UnityEngine.Object.Destroy(tex);

		Debug.Log ("Uploading!");
		// Create a Web Form
		WWWForm form = new WWWForm();
		form.AddBinaryData("fileToUpload", bytes, "tangoimage.png");

		// Upload to a cgi script
		WWW w = new WWW(serverURL, form);

		StartCoroutine(WaitForRequest(w));
	}

	IEnumerator WaitForRequest(WWW www)
	{
//		// show Panel
//		GameObject.Find ("ScrollView").GetComponent<Animator> ().Play ("PanelAnimationShow");
//
//		// clear old panel gui
//		foreach(Transform childtransform in GameObject.Find("Panel").transform){
//			Destroy(childtransform.gameObject);
//		}
//
//		GameObject.Find ("Panel").GetComponent<RectTransform> ().sizeDelta = Vector2.zero;

		// download new stuff
		yield return www;

		// check for errors
		if (www.error == null)
		{
			Debug.Log("WWW Ok!: " + www.text);
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}    

		//parse xml

		Debug.Log ("Got Response at: " + Time.realtimeSinceStartup.ToString("N"));

		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(www.text);
		XmlNodeList resultList = xmlDoc.GetElementsByTagName("result");

		if (furnitureResultList == null)
			furnitureResultList = new List<FurnitureResult> ();
		else 
			furnitureResultList.Clear ();

		foreach (XmlNode resultInfo in resultList)
		{
			XmlNodeList resultContent = resultInfo.ChildNodes;

			FurnitureResult f = new FurnitureResult();

			foreach (XmlNode resultItem in resultContent)
			{
				if(resultItem.Name == "obj")
					f.objURL = resultItem.InnerText;
				else if (resultItem.Name == "name")
					f.name = resultItem.InnerText;
				else if (resultItem.Name == "gif")
					f.ImgGIF = resultItem.InnerText;
				else if (resultItem.Name == "png")
					f.ImgPNG = resultItem.InnerText;
			}

			furnitureResultList.Add(f);

			// ...check if the dictionary already has the word.
			//			if ( resultListDict.ContainsKey(f.name) )
			//			{
			//				// If we already have the word in the dictionary, increment the count of how many times it appears
			//				resultListDict[f.name]++;
			//			}
			//			else
			//			{
			//				// Otherwise, if it's a new word then add it to the dictionary with an initial count of 1
			//				resultListDict[f.name] = 1;
			//			}
		}

		// Create a dictionary sorted by value (i.e. how many times a word occurs)
		//		var sortedDict = (from entry in resultListDict orderby entry.Value descending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
		//
		//
		//		foreach (KeyValuePair<string, int> pair in sortedDict)
		//		{
		//			Debug.Log ("Count for "+pair.Key+" is "+pair.Value);
		//		}

		//objReader = new OBJ (parentObject);



		loadResult(furnitureResultList[0].objURL);

		//loadThumbnails ();

		//m_state = FurnitureCaptureState.PLACING;
	}

	public void loadResult(string objURL)
	{
		StartCoroutine(loadResultAsync(objURL));
	}

	public IEnumerator loadResultAsync(string objURL)
	{
		foreach(Transform childtransform in parentObject.transform){
			Destroy(childtransform.gameObject);
		}

		Debug.Log ("Child Count: " + parentObject.transform.childCount);

		//StartCoroutine(LoadAssetBundle(objURL));

		yield return LoadAssetBundle(objURL);

		Debug.Log ("Child Count 1: " + parentObject.transform.childCount);

//		foreach (MeshRenderer m in parentObject.GetComponentsInChildren<MeshRenderer> ()) {
//			m.enabled = false;
//		}
//
//		GameObject.Find ("GUIController").GetComponent<RoomModelingController> ().LoadFurnitureModels ();
//
//		GameObject.Find ("GUIController").GetComponent<RoomModelingController>().SampleFurnitureModels(m_furnitureScale);

	}

	IEnumerator LoadAssetBundle (string objFileName) {

		objFileName = "http://keech.cs.washington.edu:8000/04379243/798598965a343fc9b0957d845ac33749/modelios";

		float startTime = Time.realtimeSinceStartup;
		// Download the file from the URL. It will not be saved in the Cache
		using (WWW www = new WWW(objFileName)) {
			yield return www;
			if (www.error != null)
				Debug.LogError("WWW download had an error:" + www.error);
			AssetBundle bundle = www.assetBundle;
			var loadedObject = bundle.LoadAllAssets()[0];
			GameObject model = Instantiate(loadedObject) as GameObject;
			model.transform.parent = parentObject.transform;
			// Unload the AssetBundles compressed contents to conserve memory
			bundle.Unload(false);

		} // memory is freed from the web stream (www.Dispose() gets called implicitly)
		Debug.Log("Time for LoadAssetBundle: "+(Time.realtimeSinceStartup - startTime).ToString("f6"));
		Debug.Log ("Loaded AssetBundle at: " + Time.realtimeSinceStartup.ToString("N"));
	}
}
