using UnityEngine;
using System.Collections;

public class Fim : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (MostrarMenu ());
	}

	public IEnumerator MostrarMenu (){
		yield return new WaitForSeconds(5f);
		//Destrói o gerenciador do game, já que o mesmo poderá ser gerado novamente ao reiniciar o jogo
		Destroy (GameObject.Find("GerenciadorDoGame"));
		Application.LoadLevel("menu");

	}
}
