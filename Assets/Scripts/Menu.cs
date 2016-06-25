using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	private AudioSource audioBotao;

	// Use this for initialization
	void Start () {
		//Preparar o audio do Botão
		audioBotao = gameObject.AddComponent<AudioSource>();
		audioBotao.clip = Resources.Load("Audio/Jogo/som-botao") as AudioClip;

		Input.multiTouchEnabled = false;

	}

	public void jogar () {
		//Tocar o audio
		audioBotao.Play();
		StartCoroutine (LoadLevel ());
	}

	IEnumerator LoadLevel(){
		yield return new WaitForSeconds (1f);
		Application.LoadLevel ("selecao");
	}

	public void exit(){
		audioBotao.Play();
		StartCoroutine (Sair ());
	}

	IEnumerator Sair(){
		yield return new WaitForSeconds (1f);
		Application.Quit();
	}

}
