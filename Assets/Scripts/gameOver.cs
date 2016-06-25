using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {

	public  UnityEngine.UI.Text escritaCorreta;
	private Sprite imagemPalavraCorreta;
	public GameObject posicaoImagem;
	public AudioSource audioGameOver;
	// Use this for initialization
	void Start () {
		string palavraIncorreta = GerenciadorDoGame.Instancia.palavraErrada;
		print ("Palavra incorreta :" + palavraIncorreta);
		//Colocar a primeira letra da palavra em Maiúscula
		escritaCorreta.text = char.ToUpper (palavraIncorreta [0]) + palavraIncorreta.Substring (1);

		//Faz com que a apareça a imagem errada na tela
		posicaoImagem.GetComponent<SpriteRenderer>().sprite = GerenciadorDoGame.Instancia.imagemErrada;
		posicaoImagem.GetComponent<SpriteRenderer> ().enabled = enabled;

		//Preparar audio do GameOver
		audioGameOver = gameObject.AddComponent<AudioSource>();
		audioGameOver.clip = Resources.Load("Audio/Jogo/gameover") as AudioClip;
		//Tocar o audio
		audioGameOver.Play ();
	}

	public void Continuar(){
		//Atribui os valores anteriores aos valores atuais do GerenciadorDoGame e coloca o valor padrão de quantidade de vidas 
		GerenciadorDoGame.Instancia.qtdVidaAtual = 3;
		GerenciadorDoGame.Instancia.percentMapaPontos = GerenciadorDoGame.Instancia.percentMapaPontosAntes;
		GerenciadorDoGame.Instancia.percentMapaContorno = GerenciadorDoGame.Instancia.percentMapaContornoAntes;
		Application.LoadLevel ("mapa");
	}

	public void Sair(){
		Destroy (GameObject.Find("GerenciadorDoGame"));
		Application.LoadLevel ("menu");
	}
}
