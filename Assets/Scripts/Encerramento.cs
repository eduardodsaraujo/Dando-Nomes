using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Encerramento : MonoBehaviour {

	private GameObject posicaoPersonagem;
	private AudioSource audioParebens;

	// Use this for initialization
	void Start () {
		// Insere a imagem do personagem alegre de acordo com o personagem escolhido no início do jogo
		posicaoPersonagem = GameObject.FindGameObjectWithTag("PosicaoPersonagem");
		posicaoPersonagem.GetComponent<Image> ().sprite = GerenciadorDoGame.Instancia.personagensFelizes[GerenciadorDoGame.Instancia.personagem];
		// Preparar o audio de parabéns
		audioParebens = gameObject.AddComponent<AudioSource>();
		audioParebens.clip = Resources.Load("Audio/Jogo/parabens") as AudioClip;

		StartCoroutine (AparecerFim ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	public IEnumerator AparecerFim (){
		audioParebens.Play (); // Toca o audio
		yield return new WaitForSeconds(6);
		Application.LoadLevel("fim");

	}
}
