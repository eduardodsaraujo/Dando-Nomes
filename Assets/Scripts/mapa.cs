using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Mapa : MonoBehaviour {
	public Image imgMapaPontos;
	public Image imgMapaContorno;
	private GameObject maisVida;
	private AudioSource audioMaisVida;
	// Use this for initialization
	void Start () {
		//Preparar o audio de ganho de vida
		audioMaisVida = gameObject.AddComponent<AudioSource>();
		audioMaisVida.clip = Resources.Load("Audio/Jogo/mais-vida") as AudioClip;

		// O componente da imagem recebe a porcentagem das imagens que ele deverá exibir
		imgMapaPontos.GetComponent<Image> ().fillAmount = GerenciadorDoGame.Instancia.percentMapaPontos;
		imgMapaContorno.GetComponent<Image> ().fillAmount = GerenciadorDoGame.Instancia.percentMapaContorno;
		maisVida = GameObject.FindGameObjectWithTag("MaisVida");
		// Recebe a fase atual
		int fase = GerenciadorDoGame.Instancia.numFase;

		// Se o jogador está iniciando o jogo
		if (fase == 1) {
			StartCoroutine (AparecerPontosFase0());
		} else if (fase == 2) { // Se o jogador passou da primeira fase
			StartCoroutine (AparecerPontosFase1 ());
		} else if (fase == 3) { // Se o jogador passou da segunda fase
			StartCoroutine (AparecerPontosFase2 ());
		} else {  // Se o jogador passou da terceira fase
			StartCoroutine (AparecerPontosFase3 ());
		}

	}
	
	// Os códigos abaixo se resumem em fazer aparecer um pedaço de uma determinada imagem a cada determinado periodo de tempo. Isso é feito para que os pontos e
	// os contornos na tela apareçam gradualmente e de acordo com a fase atual
	public IEnumerator AparecerPontosFase0()
	{
		// Guarda o valor atual do percentual da imagem que foi mostrado, que será utilizado caso ocorra Game Over e o jogador continue de onde parou
		GerenciadorDoGame.Instancia.percentMapaPontosAntes = GerenciadorDoGame.Instancia.percentMapaPontos;
		GerenciadorDoGame.Instancia.percentMapaContornoAntes = GerenciadorDoGame.Instancia.percentMapaContorno;
		yield return new WaitForSeconds(0.5f);
		imgMapaPontos.GetComponent<Image> ().fillAmount += 0.1f;
		yield return new WaitForSeconds(0.5f);
		imgMapaPontos.GetComponent<Image> ().fillAmount += 0.1f;// Pontos em 0.2
		yield return new WaitForSeconds(5);
		GerenciadorDoGame.Instancia.percentMapaPontos = imgMapaPontos.GetComponent<Image> ().fillAmount;
		GerenciadorDoGame.Instancia.percentMapaContorno = imgMapaContorno.GetComponent<Image> ().fillAmount;
		Application.LoadLevel("fase1");
	}

	public IEnumerator AparecerPontosFase1(){
		// Se a ultima fase exibida não for o game over acrescente uma vida, caso contrario o jogador vai voltar para a fase com o numero de vidas padrão
		if (GerenciadorDoGame.Instancia.ultimaFaseExibida != 3) {
			StartCoroutine (AcrescentarVida ());
		}
		GerenciadorDoGame.Instancia.percentMapaPontosAntes = GerenciadorDoGame.Instancia.percentMapaPontos;
		GerenciadorDoGame.Instancia.percentMapaContornoAntes = GerenciadorDoGame.Instancia.percentMapaContorno;
		while (imgMapaContorno.GetComponent<Image> ().fillAmount != 0.3f) {
			yield return new WaitForSeconds (0.2f);
			imgMapaContorno.GetComponent<Image> ().fillAmount += 0.1f;
		}
		yield return new WaitForSeconds(0.5f);
		//Precisamos mudar a orientação do modo de preencher a imagem por haver dois pontos numa mesma altura
		imgMapaPontos.GetComponent<Image> ().fillMethod = Image.FillMethod.Horizontal;
		imgMapaPontos.GetComponent<Image> ().fillAmount += 0.25f;
		yield return new WaitForSeconds(0.5f);
		imgMapaPontos.GetComponent<Image> ().fillAmount += 0.2f;// Pontos em 0.65
		yield return new WaitForSeconds(0.5f);
		imgMapaPontos.GetComponent<Image> ().fillMethod = Image.FillMethod.Vertical;
		imgMapaPontos.GetComponent<Image> ().fillAmount -= 0.23f;// Pontos em 0.42
		imgMapaPontos.GetComponent<Image> ().fillAmount += 0.1f;// Pontos em 0.52
		yield return new WaitForSeconds(0.5f);
		imgMapaPontos.GetComponent<Image> ().fillAmount += 0.18f;// Pontos 0.7
		yield return new WaitForSeconds(6);
		GerenciadorDoGame.Instancia.percentMapaPontos = imgMapaPontos.GetComponent<Image> ().fillAmount;
		GerenciadorDoGame.Instancia.percentMapaContorno = imgMapaContorno.GetComponent<Image> ().fillAmount;
		Application.LoadLevel("fase2");


	}

	public IEnumerator AparecerPontosFase2(){
		if (GerenciadorDoGame.Instancia.ultimaFaseExibida != 3) {
			StartCoroutine (AcrescentarVida ());
		}
		GerenciadorDoGame.Instancia.percentMapaPontosAntes = GerenciadorDoGame.Instancia.percentMapaPontos;
		GerenciadorDoGame.Instancia.percentMapaContornoAntes = GerenciadorDoGame.Instancia.percentMapaContorno;
		yield return new WaitForSeconds(0.2f);
		imgMapaContorno.GetComponent<Image> ().fillAmount += 0.2f;
		yield return new WaitForSeconds(0.2f);
		imgMapaContorno.GetComponent<Image> ().fillAmount += 0.1f;
		yield return new WaitForSeconds(0.2f);
		imgMapaContorno.GetComponent<Image> ().fillAmount += 0.1f;// Contorno 0.7
		yield return new WaitForSeconds(0.5f);
		imgMapaPontos.GetComponent<Image> ().fillAmount += 0.2f;
		yield return new WaitForSeconds(0.5f);
		imgMapaPontos.GetComponent<Image> ().fillAmount += 0.2f;// POntos 1
		yield return new WaitForSeconds(6);
		GerenciadorDoGame.Instancia.percentMapaPontos = imgMapaPontos.GetComponent<Image> ().fillAmount;
		GerenciadorDoGame.Instancia.percentMapaContorno = imgMapaContorno.GetComponent<Image> ().fillAmount;
		Application.LoadLevel("fase3");


	}

	public IEnumerator AparecerPontosFase3(){
		GerenciadorDoGame.Instancia.percentMapaPontosAntes = GerenciadorDoGame.Instancia.percentMapaPontos;
		GerenciadorDoGame.Instancia.percentMapaContornoAntes = GerenciadorDoGame.Instancia.percentMapaContorno;
		while (imgMapaContorno.GetComponent<Image> ().fillAmount != 1f) {
			yield return new WaitForSeconds (0.2f);
			imgMapaContorno.GetComponent<Image> ().fillAmount += 0.1f;
		}
		GerenciadorDoGame.Instancia.percentMapaPontos = imgMapaPontos.GetComponent<Image> ().fillAmount;
		GerenciadorDoGame.Instancia.percentMapaContorno = imgMapaContorno.GetComponent<Image> ().fillAmount;
		yield return new WaitForSeconds(3f);
		float fadeTime = GameObject.Find ("GameManager").GetComponent<Fading>().BeginFade(1); //Efeito de fade na transiçao entre o mapa e o encerramento
		yield return new WaitForSeconds(0.8f);
		Application.LoadLevel("encerramento");
	}

	public IEnumerator AcrescentarVida()
	{
		yield return new WaitForSeconds (0.5f);
		float alpha = 0.2f;
		while (alpha != 1.0f) {
			maisVida.GetComponent<Image> ().color = new Color (255F, 255F, 255F, alpha);
			yield return new WaitForSeconds (0.2f);
			alpha += 0.2f;
		}
		audioMaisVida.Play();
		yield return new WaitForSeconds (1.0f);
		alpha = 0.8f;
		while (alpha != 0.0f) {
			maisVida.GetComponent<Image> ().color = new Color (255F, 255F, 255F, alpha);
			yield return new WaitForSeconds (0.1f);
			alpha -= 0.2f;
		}

	}

		
}
