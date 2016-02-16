using UnityEngine;
using System.Collections;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class AdsInterstitial : MonoBehaviour {
	private InterstitialAd interstitial;
	
	void Awake(){
		Request ();
	}
	void Start(){
//		StartCoroutine (waitAds ());
	}
	void Update(){
		Show ();
	}
	public void Request(){
		RequestInterstitial();
	}
	public void Show(){
		ShowInterstitial();
	}
	public void Destroy(){
		interstitial.Destroy();
	}
	
	private void RequestInterstitial()
	{
		#if UNITY_EDITOR
		string adUnitId = "unused";
		#elif UNITY_ANDROID
		string adUnitId = "ca-app-pub-8831771291359638/3960735902";
		#elif UNITY_IPHONE
		string adUnitId = "INSERT_IOS_INTERSTITIAL_AD_UNIT_ID_HERE";
		#else
		string adUnitId = "unexpected_platform";
		#endif

		// Initialize an InterstitialAd.
		interstitial = new InterstitialAd(adUnitId);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the interstitial with the request.
		interstitial.LoadAd(request);
	}

	private void ShowInterstitial()
	{
		if (interstitial.IsLoaded())
		{
			interstitial.Show();
		}
		else
		{
			print("Interstitial is not ready yet.");
		}
	}
	IEnumerator waitAds(){
		yield return new WaitForSeconds (1.0f);
		ShowInterstitial ();
		yield return new WaitForSeconds (6.0f);
		ShowInterstitial ();
	}
	public void LoadGame(){
		interstitial.Destroy ();
		Application.LoadLevel("Gameplay");
	}
}
