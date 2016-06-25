using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;


[System.Serializable]
public class Word
{
	public string _context { get; set; }
	public string _word { get; set; }
	public string _symbolSequence { get; set; }
	private string _imagePath;
	public Sprite _Image { get; set; }
	private string _altImagePath;
	public Sprite _altImage { get; set; }
	private string _soundPath { get; set; }
	public AudioClip _sound { get; set; }
	public int _difficulty { get; set; }
	public bool _isBoss { get; set; }
	public bool _hasAltImage {get; set;}

	public Word(string[] init)
	{
		_hasAltImage = false;
		Debug.Log("Initializing word " + init[1]);
		_context = init[0];
		_word = init[1];
		_symbolSequence = init[2];
		_imagePath = "Images/" + _context + "/" + init[3];
		_soundPath = "Audio/" + _context + "/" + init[3].ToUpper();
		if (init.Length > 4)
		{
			_difficulty = Int16.Parse(init[4]);
			if (init.Length > 5)
			{
				_hasAltImage = true;
				_altImagePath = "Images/" + _context + "/" + init[5];
			}
		}
		Debug.Log("Word successfully initialized.");
	}

	public bool load()
	{
		_Image = Resources.Load(_imagePath, typeof(Sprite)) as Sprite;
		Debug.Log("Loaded " + _Image + " from " + _imagePath);
		if(_hasAltImage) _altImage = Resources.Load(_altImagePath, typeof(Sprite)) as Sprite;
		_sound = Resources.Load(_soundPath, typeof(AudioClip)) as AudioClip;
		return true;
	}

	public void unload()
	{
		Resources.UnloadAsset(_Image);
		Resources.UnloadAsset(_altImage);
		Resources.UnloadAsset(_sound);
	}

	public char[] getSymbolArray()
	{
		return _symbolSequence.ToCharArray();
	}
}


[System.Serializable]
public class DataStore  {

	public List<string> ContextList;
	public List<List<List<Word>>> WordList = new List<List<List<Word>>>();
	public List<char> SymbolList = new List<char>();

	public string[] databankTranscript;

	public DataStore()
	{
	}

	public bool Populate(TextAsset databank)
	{
		try {
			Debug.Log("Started Populating Database");
			string line;

			int currentContextIndex = -1;
			string currentContextName = "";

			databankTranscript = databank.text.Split("\n"[0]);

			for (int i = 0; i < databankTranscript.Length; i++)
			{
				line = databankTranscript[i];

				if (line != null)
				{
					string[] entries = line.Split(' ');
					if (entries[0] != "\\\\")
					{
						if (currentContextName != entries[0])
						{
							Debug.Log("Context is: " + entries[0] + ". Current Context is: " + currentContextName + "[" + currentContextIndex + "]");
							if (ContextList.Contains(entries[0]))
							{
								Debug.Log("Found context in contextList");
								currentContextIndex = ContextList.IndexOf(entries[0]);
								currentContextName = entries[0];
							}
							else
							{
								Debug.Log("Could not find context in contextList. Creating...");
								currentContextName = entries[0];
								currentContextIndex = ContextList.Count;
								ContextList.Add(entries[0]);
								for(int j = 0; j < 10; j++)
								{
									WordList.Add(new List<List<Word>>());
									WordList[currentContextIndex].Add(new List<Word>());
									Debug.Log("Initialized list " + j + " at context " + currentContextName);
								}
								Debug.Log("Context Created. Current Context is: " + currentContextName + "[" + currentContextIndex + "]");
							}
						}
						Debug.Log("Context is the same as current context. Creating Word in context " + currentContextIndex);
						Word entryWord = new Word(entries);
						Debug.Log("Context: " + currentContextIndex + ", Difficulty: " + ((entryWord._difficulty)-1));
						WordList[currentContextIndex][(entryWord._difficulty)-1].Add(entryWord);
						Debug.Log("Loaded " + entries[0] + " " + entries[1]);
					}
				}
			}
			return true;
		}
		catch( Exception e )  { Debug.LogError(e); return false; }
	}

	public bool CreateSymbolListFromArray()
	{
		try
		{
			string[] HardSymbolList = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "á", "ã", "é", "ê", "í", "ó", "õ", "ú" };
			for (int i = 0; i < HardSymbolList.Length; i++)
			{
				SymbolList.Add((Convert.ToChar(HardSymbolList[i])));
			}
			return true;
		}
		catch (Exception e)
		{
			Debug.LogError(e);
			return false;
		}
	}

	public List<List<Word>> getWordListByContext(string context)
	{
		try
		{
			if (ContextList.Contains(context)) return WordList[ContextList.IndexOf(context)];
			else throw new KeyNotFoundException();
		}
		catch (KeyNotFoundException)
		{
			Debug.LogError("Could not find the requested context.");
			Debug.LogError ("Contexto: " + context);
			throw new KeyNotFoundException();

		}
	}

	public List<Word> getWordListByDifficulty(string context, int difficulty)
	{
		return getWordListByContext(context)[difficulty-1];
	}

	public List<Word> getWordListByDifficultyRange(string context, int diffRangeStart, int diffRangeEnd)
	{
		List<Word> returnList = new List<Word>();
		if (diffRangeEnd < diffRangeStart) { throw new InvalidOperationException(); }
		if (diffRangeEnd == diffRangeStart) { return getWordListByDifficulty(context, diffRangeStart); }
		for(int i = diffRangeStart; i < diffRangeEnd+1; i++)
		{
			returnList.AddRange(getWordListByDifficulty(context, i));
		}
		return returnList;
	}

	public char getRandomSymbol()
	{
		int randomSymbol = (int)(Math.Round((UnityEngine.Random.value) * (SymbolList.Count-1)));
		Debug.Log(randomSymbol);
		return SymbolList[randomSymbol];
	}
}

