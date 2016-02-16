﻿using UnityEngine;
using System.Collections;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class AdsBanner : MonoBehaviour {
	private BannerView bannerView;

	public void Request(){
		RequestBanner();
	}
	public void Show(){
		bannerView.Show();
	}
	public void Hide(){
		bannerView.Hide();
	}
	public void Destroy(){
		bannerView.Destroy();
	}
	
	private void RequestBanner()
	{
		#if UNITY_EDITOR
		string adUnitId = "unused";
		#elif UNITY_ANDROID
		string adUnitId = "ca-app-pub-8831771291359638/7193403904";
		#elif UNITY_IPHONE
		string adUnitId = "INSERT_IOS_BANNER_AD_UNIT_ID_HERE";
		#else
		string adUnitId = "unexpected_platform";
		#endif

		// Create a 320x50 banner at the top of the screen.
		bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the banner with the request.
		bannerView.LoadAd(request);
	}
}
