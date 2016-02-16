using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Facebook.MiniJSON;
using System;
using System.Data;

public class FBManager : MonoBehaviour {
	public static string errorMSG = "";
	public GameObject _isFBLoggedIn;
	public GameObject _notFBLoggeIn;
	public GameObject _avatarFB;
	public GameObject _usernameFB;
	public GameObject _rankFB;
	public GameObject _scoreMe;
	public Text _scoreDebug;
	public Text _dbDebug;
	private string name;
	private string get_data;
	
	private List<object> scoresList = null;

	public GameObject ScoreEntryPanel;
	public GameObject ScoreScrollList;
	public GameObject _rank;
	public GameObject _panelRank;

	private ConnectDB conDB = new ConnectDB();
	private PHP conPHP = new PHP();
	private Dictionary<string, string> profile = null;

	void Awake(){
		StartCoroutine (conPHP.GetDataList ());
		FB.Init (SetInit, OnHideUnity);
//		conDB.OpenConnect ();
	}

	void Start(){
		_panelRank.SetActive (false);
	}
	private void SetInit(){

		if (FB.IsLoggedIn) {
			Debug.Log("FB Logged In");
			DealWithFBMenu(true);
		} else {
			DealWithFBMenu(false);
		}
	}
	private void OnHideUnity(bool isGameShown){
		if (!isGameShown) {
			Time.timeScale = 0;
		} else {
			Time.timeScale = 1;
		}
	}
	public void FBlogin(){
		FB.Login ("public_profile,user_friends,email,publish_actions", AuthCallBack);
	}
	void AuthCallBack(FBResult result){
		if (FB.IsLoggedIn) {
			Debug.Log ("FB Login worked!");
			DealWithFBMenu(true);
//			SetDataPHP();       //<<<<<<<<<<<<<<<<<<<<<<<  Set Score PHP
/////			StartCoroutine(waitSetDataPHP(2));
		} else {
			Debug.Log("Fb Login fail");
			DealWithFBMenu(false);
		}
	}
	public void ShareWithFriends(){
		if (FB.IsLoggedIn) {
			FB.Feed (
			linkCaption: "I'm playing this awesome game!",
			linkName: "Check out this game"
			);
		} else {
			FBlogin();
		}
	}
	public void InviteFriends(){
		if (FB.IsLoggedIn) {
			FB.AppRequest (
			message: "This game is awesome, join me. Now!!",
			title: "Invite your friends to join you"
//			filters: new List<object> (){ "app_users" }
			);
		} else {
			FBlogin();
		}
	}
	void DealWithFBMenu(bool isLoggedIn){
		_scoreMe.GetComponent<Text>().text = "Score: " + Constants.getBestScore();
		if (FB.IsLoggedIn) {
			_isFBLoggedIn.SetActive (true);
			_notFBLoggeIn.SetActive(false);
//			_scoreMe.SetActive(true);           // <<< Score Me True
			FB.API(Util.GetPictureURL("me",128,128), Facebook.HttpMethod.GET, DealWithProfilePicture);
			FB.API("/me?fields=id,first_name", Facebook.HttpMethod.GET, DealWithUserName);
			FB.API("me?fields=name", Facebook.HttpMethod.GET, UserNameCallBack);
		} else {
			_notFBLoggeIn.SetActive (true);
			_isFBLoggedIn.SetActive (false);
//			_scoreMe.SetActive(false);			// Score Me False
		}
	}
	void DealWithProfilePicture(FBResult result){
		if (result.Error != null) {
			Debug.Log("problem with getting profile picture");
			FB.API(Util.GetPictureURL("me",128,128), Facebook.HttpMethod.GET, DealWithProfilePicture);
			return;
		}
		Image UserAvatar = _avatarFB.GetComponent<Image> ();
		UserAvatar.sprite = Sprite.Create (result.Texture, new Rect (0, 0, 128, 128), new Vector2 (0, 0));
	}
	void DealWithUserName(FBResult result){
		if (result.Error != null) {
			Debug.Log("problem with getting username");
			FB.API("/me?fields=id,first_name", Facebook.HttpMethod.GET, DealWithUserName);
			return;
		}
		profile = Util.DeserializeJSONProfile (result.Text);
		Text UserMsg = _usernameFB.GetComponent<Text> ();
		UserMsg.text = "Hello, " + profile ["first_name"];
	}
	void UserNameCallBack(FBResult result){
		if (result.Error != null) {
			get_data = result.Text;
		} else {
			get_data = result.Text;
		}
		var dict = Json.Deserialize (get_data) as IDictionary;
		name = dict ["name"].ToString ();
	}
	private void ScoreCallBack(FBResult result){
		Debug.Log ("Score callback :" + result.Text);

		scoresList = Util.DeserializeScores (result.Text);
		foreach (Transform child in ScoreScrollList.transform) {
			GameObject.Destroy(child.gameObject);
		}

		foreach (object score in scoresList) {
			var entry = (Dictionary<string, object>) score;
			var user = (Dictionary<string, object>) entry["user"];

			GameObject ScorePanel;
			ScorePanel = Instantiate(ScoreEntryPanel) as GameObject;
			ScorePanel.transform.parent = ScoreScrollList.transform;
			ScorePanel.transform.localScale = new Vector3(1,1,1);

			Transform ThisScoreName = ScorePanel.transform.Find("FriendName");
			Transform ThisScoreScore = ScorePanel.transform.Find("FriendScore");
			Text ScoreName = ThisScoreName.GetComponent<Text>();
			Text ScoreScore = ThisScoreScore.GetComponent<Text>();

			ScoreName.text = user["name"].ToString();
			ScoreScore.text = "Score : " + entry["score"].ToString();

			Transform TheUserAvatar = ScorePanel.transform.Find("FriendAvatar");
			Image UserAvatar = TheUserAvatar.GetComponent<Image>();

			FB.API(Util.GetPictureURL(user["id"].ToString(),128,128), Facebook.HttpMethod.GET, delegate(FBResult pictureResult) {
				if(pictureResult.Error != null){
					Debug.Log(pictureResult.Error);
				}else{
					UserAvatar.sprite = Sprite.Create(pictureResult.Texture, new Rect (0, 0, 128, 128), new Vector2 (0, 0));
				}
			});
		
		}
	}
//	public void Ranking(){
//		_rankFB.SetActive (true);
//		FB.API ("/796876857047705/scores?fields=score,user.limit(30)", Facebook.HttpMethod.GET, ScoreCallBack); // 796876857047705
//	}
//	public void SetScore(){
//		var scoreData = new Dictionary<string, string> ();
//		scoreData ["score"] = Constants.getBestScore().ToString ();
//		scoreData ["score"] = Random.Range(10, 200).ToString (); // set PlayerPref Best Score
//		FB.API ("/me/scores", Facebook.HttpMethod.POST, delegate(FBResult result) {
//			Debug.Log ("Score submit result: " + result.Text);
//			_scoreDebug.text = result.Text;
//		}, scoreData);
//	}
//	public void GetScore(){
//		FB.API ("/me?fields=id", Facebook.HttpMethod.GET, delegate(FBResult result) {
//		_scoreDebug.text = result.Text + " Or " + FB.UserId;
//		string sql = "select score from scores where id = '" + FB.UserId + "'";
//			_scoreDebug.text = conDB.ReadEntries (sql);
//		});
//	}
/*	public void InsertData(){
		string name = "";
		int score = Constants.getBestScore ();
		FB.API("/me?fields=first_name", Facebook.HttpMethod.GET, delegate(FBResult result) {
			name = result.Text;
		});
		string sql = "insert into scores(id, name, score) values('" + FB.UserId +"','"+ name +"'," + score + ")";
		string hasSql = "update scores set score ='" + score + "' where id ='" + FB.UserId + "'";
		string read = "select id from scores where id = '" + FB.UserId + "'";
		string temp = conDB.ReadEntries (read);
		if (Int64.Parse(temp) == -1) {
			conDB.InsertEntries (sql);
			Debug.Log("Insert Success!");
		} else {
			conDB.InsertEntries(hasSql);
			Debug.Log("Update Success!");
		}
	}
*/	public void LeaderBoard(){
//		string sql = "select id, name, score from scores order by score desc limit 30";
		if (FB.IsLoggedIn) {
			SetDataPHP ();
		}
		_panelRank.SetActive (true);
		_rankFB.SetActive (true);
		conPHP.GetItemData (ScoreScrollList, ScoreEntryPanel);
//		StartCoroutine(conPHP.GetItemData (ScoreScrollList, ScoreEntryPanel));

//		conDB.LogGameItem (ScoreScrollList, ScoreEntryPanel);
	}
	public void SetDataPHP(){
		string id = "";
		int score = 0;
		id = FB.UserId;
		score = Constants.getBestScore ();
//		StartCoroutine (conPHP.PostData("9453805654353", "Hikaru noKO", 70));     /// Test  ///
		StartCoroutine (conPHP.PostData(id, name, score));
	}
	IEnumerator waitSetDataPHP(float time){
		yield return new WaitForSeconds (time);
		SetDataPHP ();
	}
	public void Close(){
		_rank.SetActive (false);
		_panelRank.SetActive (false);
	}
	public void LoadGame(){
		if (FB.IsLoggedIn) {
			SetDataPHP ();
		}
		Application.LoadLevel("Gameplay");
	}
}
