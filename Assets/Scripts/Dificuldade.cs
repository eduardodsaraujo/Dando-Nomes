using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Dificuldade : MonoBehaviour {

	private AudioSource audioBotao;
	public GameObject[] btnsDificuldade;
	// Use this for initialization
	void Start () {
		//Preparar o audio do Botão
		audioBotao = gameObject.AddComponent<AudioSource>();
		audioBotao.clip = Resources.Load("Audio/Jogo/som-botao") as AudioClip;

		Input.multiTouchEnabled = false;

	}

	public void clickButton(int nivel){
		//Tocar o audio
		audioBotao.Play ();
		StartCoroutine (EscolherDificuldade (nivel));
		for (int i = 0; i < btnsDificuldade.Length; i++) { //Desativar os botoes diferentes do selecionado
			if (nivel != i + 1) {
				btnsDificuldade [i].GetComponent<Button> ().interactable = false;
			}
		}
	}

	private IEnumerator EscolherDificuldade(int dificuldade){
		yield return new WaitForSeconds (1f);
		GerenciadorDoGame.Instancia.dificuldade = dificuldade;
		Application.LoadLevel ("mapa");
	}

}
