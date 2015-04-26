﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using LitJson;

public class GameData{

	private static GameData instance;
	public bool sfx;
	public bool music;
	public bool extraBike;
	public int cash;
	public int currentBike;
	public int currentLvl;
	public int allowLvls;
	public List<int> allowBikes;
	public List<int> progressList;
	public List<List<int>> collectedItems;



	private string version = "save_00124";
	public static GameData Get()
	{
		if (instance == null)
		{
			instance = new GameData();
			instance = instance.Load();
			AppSoundManager.MuteSfx = !instance.sfx;
			AppSoundManager.MuteMusic = !instance.music;
		}
		return instance;
	}


	GameData Load ()
	{		
		string data = PlayerPrefs.GetString(version, null);
		Debug.Log("Load game data:" + data);
		if (data == null || data.Trim() == "")
		{
			reset();
			return this;
		}
		GameData gdata;
		try
		{
			gdata = JsonMapper.ToObject<GameData>(data);
		}
		catch (System.Exception e)
		{
			reset();
			return this;
		}
		return gdata;
	}
	
	void reset ()
	{
		sfx = true;
		music = true;
		extraBike = false;
		cash = 0;
		currentBike = 0;
		currentLvl = 1;
		allowLvls = 1;
		allowBikes = new List<int> ();
		allowBikes.Add (0);

		collectedItems = new List<List<int>> ();
		for(int i = 0; i < 7;i++)
			collectedItems.Add(new List<int>());

		progressList = new List<int> ();
		for (int i=0; i<GameSettings.countLevels; i++) {
			progressList.Add(0);
		}

		save();
	}

	public void save ()
	{
		string data = JsonMapper.ToJson(this);
		Debug.Log("Save gamedata as:" + data);
		PlayerPrefs.SetString(version, data);
	}

	public int GetFoundItemsCount()
	{
		return collectedItems [currentLvl].Count;
	}

	public void addFoundItem(int id)
	{
		collectedItems [currentLvl].Add (id);
	}

	public bool bikeIsUnlock(int num)
	{
		for(int i = 0; i < allowBikes.Count;i++)
		{
			if (num == allowBikes[i])
				return true;
		}
		return false;
	}

	public void setCurrentLevelProgress(int numLevel, int milliseconds){
		if ((milliseconds < progressList [numLevel-1]) || (progressList [numLevel - 1] == 0)) {
			progressList [numLevel-1] = milliseconds;
			//Debug.Log("Set time = "+milliseconds/1000+"in level №_"+numLevel);
			save();
		}
	}

	public int getLevelStars(int numLevel, int milliseconds){
		int stars = 0;
		if (milliseconds <= 1000 * GameSettings.getTime_3 (numLevel - 1))
			stars = 3;
		else if (milliseconds > 1000 * GameSettings.getTime_3 (numLevel - 1) && milliseconds <= 1000 * GameSettings.getTime_2 (numLevel - 1))
			stars = 2;
		else if (milliseconds > 1000 * GameSettings.getTime_2 (numLevel - 1) && milliseconds <= 1000 * GameSettings.getTime_1 (numLevel - 1))
			stars = 1;
		else
			stars = 0;

		return stars;
	}
}
