using UnityEngine;
using System.Collections;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.Data;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class ConnectDB : MonoBehaviour {
	private string connectString = "server=mysql13.000webhost.com;database=a1836709_game;uid=a1836709_game;pwd=clever1";
	// Unable to connect to any of the specified MySQL hosts.
	MySqlConnection con = null;
	MySqlCommand cmd = null;
	MySqlDataReader rdr = null;

	Int64 idFB = 0;
	Int64 id;
	string name;
	int score;
	public struct data
	{
		public Int64 id;
		public string name;
		public int score;
	}
	List<data> _GameItems;

	public void OpenConnect(){
		try{
			con = new MySqlConnection(connectString);
			con.Open();
			Debug.Log("Connection State: " + con.State);
		}
		catch(Exception e){
			Debug.Log(e.ToString());
		}
	}
	public void CloseConnect(){
		try{
			if(con != null)
				con.Close();
			con.Dispose();
			}
		catch(Exception e){
			Debug.Log(e.ToString());
		}
	}
	public string ReadEntries(string sql){
		try{
			cmd = new MySqlCommand(sql, con);
			rdr = cmd.ExecuteReader();

			if(rdr.HasRows){
				while(rdr.Read()){
					idFB = Int64.Parse(rdr.GetValue(0).ToString()); //GetValues Row index 0-i
				}
			}else{
				idFB = -1;
			}
			rdr.Close();
			return idFB.ToString();
		}
		catch(Exception e){
			FBManager.errorMSG = e.Message;
			return "[Read Fail]";
		}
	}
	public void InsertEntries(string sql){
		try{
			if(con != null)
				OpenConnect();
			using(cmd = new MySqlCommand(sql, con)){
				cmd.ExecuteNonQuery();}
//			CloseConnect();
		}
		catch(Exception e){
			FBManager.errorMSG = e.Message;
		}
	}
//	public void UpdateEntries(string sql){
//		try{
//			cmd = new MySqlCommand(sql, con);
//			cmd.ExecuteNonQuery();
//		}
//		catch(Exception e){
//			FBManager.errorMSG = e.Message;
//		}
//	}
	public void ReadTopScores(string sql){
		if(_GameItems == null)
			_GameItems = new List<data>();
		if(_GameItems.Count > 0)
			_GameItems.Clear();
		try{
			if(con != null)
			OpenConnect();
				using(cmd = new MySqlCommand(sql, con)){
					rdr = cmd.ExecuteReader();
					if(rdr.HasRows)
					while(rdr.Read()){
						data itm = new data();
						itm.id = Int64.Parse(rdr["id"].ToString());
						itm.name = rdr["name"].ToString();
						itm.score = int.Parse(rdr["score"].ToString());
						_GameItems.Add (itm);
					}
				con.Close();
				con.Dispose();
			}
		}
		catch(Exception e){
			FBManager.errorMSG = e.Message;
			Debug.Log("Read Top Score Fail");
		}
	}
	public void LogGameItem(string sql, GameObject ScrollList, GameObject EntryPanel){
		ReadTopScores (sql);
		if (_GameItems != null) {
			if(_GameItems.Count > 0){
//				foreach(data itm in _GameItems){
//					Debug.Log("ID: " + itm.id);
//					Debug.Log("Name: " + itm.name);
//					Debug.Log("Score: " + itm.score);


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
						Text ScoreName = ThisScoreName.GetComponent<Text>();
						Text ScoreScore = ThisScoreScore.GetComponent<Text>();
						
						ScoreName.text = itm.name.ToString();
						ScoreScore.text = "Score : " + itm.score.ToString();
						
						Transform TheUserAvatar = ScorePanel.transform.Find("FriendAvatar");
						Image UserAvatar = TheUserAvatar.GetComponent<Image>();
						
						FB.API(Util.GetPictureURL(itm.id.ToString(),128,128), Facebook.HttpMethod.GET, delegate(FBResult pictureResult) {
							if(pictureResult.Error != null){
								Debug.Log(pictureResult.Error);
							}else{
								UserAvatar.sprite = Sprite.Create(pictureResult.Texture, new Rect (0, 0, 128, 128), new Vector2 (0, 0));
							}
						});
						
					}
			}
		}
	}
}
