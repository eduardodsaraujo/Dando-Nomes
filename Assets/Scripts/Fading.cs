﻿using UnityEngine;
using System.Collections;

public class Fading : MonoBehaviour {
	//FadeIn and FadeOut effects
	public Texture2D fadeOutTexture;	// the texture that will overlay the screen. This can be a black image or a loading graphic
	public float fadeSpeed = 0.8f;		

	private int drawDepth = -1000;		// the texture's order in the draw hierarchy: a low number means it renders on top
	private float alpha = 1.0f;			
	private int fadeDir = -1;			// the direction to fade: in = -1 or out = 1

	void OnGUI()
	{
		// fade out/in the alpha value using a direction, a speed and Time.deltaTime to convert the operation to seconds
		alpha += fadeDir * fadeSpeed * Time.deltaTime;
		// force (clamp) the number to be between 0 and 1 because GUI.color uses Alpha values between 0 and 1
		alpha = Mathf.Clamp01(alpha);

		// set color of our GUI (in this case our texture). All color values remain the same & the Alpha is set to the alpha variable
		GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha);
		GUI.depth = drawDepth;																// make the black texture render on top (drawn last)
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);		// draw the texture to fit the entire screen area
	}

	// fade in if -1 and out if 1
	public float BeginFade (int direction)
	{
		fadeDir = direction;
		return (fadeSpeed);
	}
		
	void OnLevelWasLoaded()
	{
		// alpha = 1;		
		BeginFade(-1);		
	}
}
