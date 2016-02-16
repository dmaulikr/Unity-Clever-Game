using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Data;
using System.Text.RegularExpressions;

public class PHP : MonoBehaviour {
	public Text debugPHP;
	public string addData = "http://clevertech.site11.com/add.php";
	public string display = "http://clevertech.site11.com/display.php";
	public Texture2D picUnknow;
	ArrayList dataList = new ArrayList ();

	Int64 id;
	string name;
	int score;
	public struct data
	{
		public string id;
		public string name;
		public int score;
	}
	List<data> _GameItems;
//	IEnumerator Start(){
/*		WWW www = new WWW("http://clevertech.site11.com/add.php?id=123456&name=minato&score=15");
		yield return www;
		Debug.Log(www.text);
*/
/*		WWWForm form = new WWWForm ();
		form.AddField ("id", "9999999999999999");
		form.AddField ("name", "BBB CCCC");
		form.AddField ("score", "99");
		WWW www = new WWW (addData , form);
		
		yield return www;
		if (www.error == null) {
			Debug.Log(www.text);
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}
*/
//	}

	IEnumerator GetScores(){
		WWW getScore = new WWW (display);
		yield return getScore;
		dataList.AddRange (Regex.Split (getScore.text, ";"));
		debugPHP.text = "";
		if (getScore.error != null) {
			Debug.Log ("There was an error getting the Leaderboard...");
		} else {
			for (int i=0; i <= (dataList.Count-1)/2; i+=3) {
				debugPHP.text += " ID: " + dataList[i];
				debugPHP.text += " Name: " + dataList[i+1];
				debugPHP.text += " Score: " + dataList[i+2];
				debugPHP.text += "\n";
			}
		}
	}

	public IEnumerator PostData(string id,string name, int score){
		WWWForm form = new WWWForm ();
		form.AddField ("id", id.ToString());
		form.AddField ("name", name.ToString());
		form.AddField ("score", score.ToString());

		WWW www = new WWW (addData,form);

		yield return www;
		if (www.error == null) {
			Debug.Log(www.text);
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}
	}
/*	IEnumerator WaitForRequest(WWW www){
		yield return www;
		if (www.error == null) {
			Debug.Log(www.text);
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}
	}
*/
	public IEnumerator GetDataList(){
		if (_GameItems == null) {
			_GameItems = new List<data> ();
		}
		if (_GameItems.Count > 0) {
			_GameItems.Clear ();
		}

		WWW getScore = new WWW (display);
		yield return getScore;
		dataList.AddRange (Regex.Split (getScore.text, ";"));
		if (getScore.error != null) {
			Debug.Log ("There was an error getting the Leaderboard...");
		} else {
			for (int i=0; i <= (dataList.Count-1)-3; i+=3) {
				data itm = new data();
				itm.id = dataList[i].ToString();
				itm.name = dataList[i+1].ToString();
				itm.score = int.Parse(dataList[i+2].ToString());
				_GameItems.Add (itm);
				Debug.Log(itm.id + "; " + itm.name + "; " + itm.score);
			}
		}
	}
	public void GetItemData(GameObject ScrollList, GameObject EntryPanel){
		int k = 1;
		if (_GameItems != null) {
			if(_GameItems.Count > 0){
				foreach (Transform child in ScrollList.transform) {
					GameObject.Destroy(child.gameObject);
				}
				foreach (data itm in _GameItems) {
					
					GameObject ScorePanel;
					ScorePanel = Instantiate(EntryPanel) as GameObject;
					ScorePanel.transform.parent = ScrollList.transform;
					ScorePanel.transform.localScale = new Vector3(1,1,1);
					
					Transform ThisScoreName = ScorePanel.transform.Find("FriendName");
					Transform ThisScoreScore = ScorePanel.transform.Find("FriendScore");
					Transform ThisRank = ScorePanel.transform.Find("FriendRank").FindChild("FriendRankText");
					Text ScoreName = ThisScoreName.GetComponent<Text>();
					Text ScoreScore = ThisScoreScore.GetComponent<Text>();
					Text LabelRank = ThisRank.GetComponent<Text>();
					
					ScoreName.text = itm.name.ToString();
					ScoreScore.text = "Score : " + itm.score.ToString();
					LabelRank.text = k.ToString();
					k++;
					
					Transform TheUserAvatar = ScorePanel.transform.Find("FriendAvatar");
					Image UserAvatar = TheUserAvatar.GetComponent<Image>();
					try{
						FB.API(Util.GetPictureURL(itm.id,128,128), Facebook.HttpMethod.GET, delegate(FBResult pictureResult) {
							if(pictureResult.Error != null){
								Debug.Log(pictureResult.Error);
							}else{
								UserAvatar.sprite = Sprite.Create(pictureResult.Texture, new Rect (0, 0, 128, 128), new Vector2 (0, 0));
							}
						});
					}
					catch(Exception e){
						Debug.Log(e.Message);
					}
					Debug.Log(itm.id + "; " + itm.name + "; " + itm.score);
				}
			}
		}
	}
}
