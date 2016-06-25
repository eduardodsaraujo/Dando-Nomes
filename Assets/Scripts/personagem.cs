using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Personagem : MonoBehaviour {

	public Button[] personagens;
	public UnityEngine.UI.Text[] textosPersonagem;
	public UnityEngine.UI.Image[] bordasPersonagem;
	private AudioSource audioBotao;
	private AudioSource audioIupi;



	// Use this for initialization
	void Start () {
		//Preparar o audio do Botão
		audioBotao = gameObject.AddComponent<AudioSource>();
		audioBotao.clip = Resources.Load("Audio/Jogo/som-botao") as AudioClip;
		//Preparar o audio de comemoração do personagem
		audioIupi = gameObject.AddComponent<AudioSource>();
		audioIupi.clip = Resources.Load("Audio/Jogo/iupi") as AudioClip;

		Input.multiTouchEnabled = false;



	}


	public void clickButton(int pos){
		audioBotao.Play();// Tocar audio
		GerenciadorDoGame.Instancia.personagem = pos;
		MudarTextoPersonagem (textosPersonagem[pos], pos);
		StartCoroutine (EsperarMudarTexto ());
		for (int i = 0; i < personagens.Length; i++) {//Desativar os botoes para que não se possa selecionar um personagem após o outro
			if (pos != i) {
				personagens [i].interactable = false;
			} 
		}
	}

	IEnumerator EsperarMudarTexto(){
		
		yield return new WaitForSeconds (1f); 
		audioIupi.Play ();// Tocar audio
		yield return new WaitForSeconds (1f); 
		Application.LoadLevel("dificuldade");
	}


	public void MudarTextoPersonagem(UnityEngine.UI.Text textoPersonagem, int pos){
		bordasPersonagem [pos].GetComponent<Image> ().enabled = true;
		textoPersonagem.text = "EBA!";
	}


}
