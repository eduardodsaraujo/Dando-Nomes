using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GerenciadorDoGame : MonoBehaviour {

	public float qtdVidaMax;
	public float qtdVidaAtual;

	//Variaveis utilizadas no mapa
	public int numFase;
	public float percentMapaPontos;
	public float percentMapaContorno;

	//Utilizados caso dê gameOver
	public float percentMapaPontosAntes;
	public float percentMapaContornoAntes;


	public int dificuldade;
	public int personagem;
	public GameObject[] personagens;
	public Sprite[] personagensAssustados;
	public Sprite[] personagensFelizes;
	public string palavraErrada;
	public Sprite imagemErrada;
	public GameObject img;
	public int ultimaFaseExibida;

	public static GerenciadorDoGame Instancia = null;


	void Awake(){

		//Estrutura para garantir que o gameobject com os atributos fundamentais do jogo não seja destruído ao trocar de cena
		if (Instancia == null) {
			Instancia = this;
		} else if (Instancia != this) {
			Destroy (gameObject);
		}
		DontDestroyOnLoad (gameObject);
	}

	// Use this for initialization
	void Start () {

	}

	//Funçao chamada depois de uma nova cena(level) ser carregada
	void OnLevelWasLoaded(){
		// Se a cena atual for a cena do mapa, dos personagens ou da escolha da dificuldade, começará a tocar o audio
		if (Application.loadedLevelName == "mapa" || Application.loadedLevelName == "selecao"
			|| Application.loadedLevelName == "dificuldade") {
			if (!(gameObject.GetComponent<AudioSource> ().isPlaying)) {//Se o audio já estiver tocando
				gameObject.GetComponent<AudioSource> ().volume = 0.8f;
				gameObject.GetComponent<AudioSource> ().Play (); //Tocar o audio 
			}
		}

		//Se a cena atual for uma dessas, para de tocar o audio
		if (Application.loadedLevelName == "fase1" || Application.loadedLevelName == "fase2" || Application.loadedLevelName == "fase3" || 
			Application.loadedLevelName == "gameover" || Application.loadedLevelName =="encerramento" ) { 
			AudioSource musicaMenu = gameObject.GetComponent<AudioSource>();
			StartCoroutine (PararMusica(musicaMenu));
		}

		if (Application.loadedLevelName != "mapa") {
			ultimaFaseExibida = Application.loadedLevel;
		}

	}
	// Update is called once per frame
	void Update () {
		// Se a cena atual for uma das fases
		if (Application.loadedLevelName == "fase1"
		    || Application.loadedLevelName == "fase2" || Application.loadedLevelName == "fase3") {

			img = GameObject.FindGameObjectWithTag ("GameManager");

			//Obtém a palavra e imagem atual, para que possam ser reproduzidas na tela de GameOver
			palavraErrada = img.GetComponent<Principal> ().palavraAtual;
			imagemErrada = img.GetComponent<Principal> ().imagemAtual;
		} 

	}

	//Funçao para parar o audio, diminuindo gradualmente o som e depois pausá-lo
	public IEnumerator PararMusica(AudioSource musica){
		while (musica.volume != 0f) {
			musica.volume -= 0.1f;
			yield return new WaitForSeconds(0.1f);
		}
		musica.Pause ();
	}

}
