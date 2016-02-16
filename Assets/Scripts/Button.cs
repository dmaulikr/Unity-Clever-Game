using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class Button : MonoBehaviour {
	public GameObject _pause;
	public GameObject _rank;
	public GameObject _panelRank;

	public AdsBanner bannerView = new AdsBanner();
	public static Button instance;

	void Awake(){
		instance = this;
	}
	void Start(){
		bannerView.Request ();
		bannerView.Hide ();
	}
	public void LoadGame(){
		Application.LoadLevel("Gameplay");
	}
	public void LoadLevel(string name){
		bannerView.Destroy ();
		Application.LoadLevel(name);
	}
	public void Pause(){
		Time.timeScale = 0;
		bannerView.Show ();
		_pause.SetActive (true);
	}
	public void Resume(){
		Time.timeScale = 1;
		bannerView.Hide ();
		Debug.Log("Resume");
		_pause.SetActive (false);
	}
}
