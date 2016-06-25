using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

[System.Serializable]
public class Principal: MonoBehaviour {

    // Estas variáveis armazenam as dimensões da tela.
    public int screenWidth;
    public int screenHeight;

    // Estas variáveis armazenam valores relacionados ao desenho de botões na tela.
    public const int nButtons = 10;
    private float[] buttonRotation;
    private static float buttonWidthMultiplier = 1.7f;
    private static float buttonHeightMultiplier = 1.5f;
    private static float buttonHorizontalOffset = 0.35f;
    private static float buttonVerticalOffset = 6.0f;
    private static float buttonSeparationMultiplier = 0.1f;

    public GUIStyle buttonStyle;
    public GUIStyle disabledButtonStyle;
    private bool isStyleInit = false;

    // Esta variável armazena o nível base de Anti Aliasing do jogo.
    private static int antiAliasingLevel = 4;

    // Estas variáveis armazenam dados para leitura de uma base de palavras contextualizada.
    public DataStore dataStore;
    public TextAsset dataBank;
	public string Contexto;

    // Estas variáveis armazenam informações relacionadas a operação do jogo:
    // Botões, Palavras e Inputs.
    private char[] buttonSymbols;
    private bool[] buttonDisabled;

    public Word currWord;

    private string inputString;
    private Stack<int> inputButtonStack;

    // Estas variáveis estão relacionadas a operação do código e sua interação com o Unity.
    private GameObject stringDisplay;
    private GameObject imageDisplay;
    private GameObject undoButton;
	private GameObject btnConfirmar;

    private GameObject winParticle, winParticleDisplay;
    private GameObject validityDisplay;
    private Sprite wrongImage;
	private AudioSource audioPalavraCerta;
	private AudioSource audioPalavraErrada;
	private AudioSource audioFase;

	// Vida do jogador 
	private GameObject vidas;

	// Posiçao do personagem 
	public GameObject posicaoPersonagem;


	// Palavra e imagem atuais a serem descorbertas
	public string palavraAtual;
	public Sprite imagemAtual;

	// Numeros do array das imagens que ja foram escolhidas
	public Stack<int> numSairam;

	// Chefe
	public bool ativarBoss = false;
	private bool apareceuBoss= false;
	private AudioSource audioAparecerBoss;
	private GameObject aparicaoBoss;
	private GameObject boss;
	public Material bossMaterial;
	public Stack<int> numSairamBoss;
	private GameObject particlesBossPosition;
	private GameObject bossParticle;
	private GameObject posicaoParticulaBoss;
	private AudioSource audioPorradaChefe;



	// Recebe a dificuldade do jogo selecionada no menu de dificuldades
	public int dificuldadeJogo;

	// Prox fase
	public string proxFase;
    // Use this for initialization
    void Start()
    {
		//
		dificuldadeJogo = GerenciadorDoGame.Instancia.dificuldade;

		//Recupera elementos que estão na tela e que serão alterados durante a execuçao do codigo
        buttonSymbols = new char[nButtons];
        buttonRotation = new float[nButtons];
        buttonDisabled = new bool[nButtons];
        inputButtonStack = new Stack<int>();
        stringDisplay = GameObject.FindGameObjectWithTag("Display");
        imageDisplay = GameObject.FindGameObjectWithTag("ImageDisplay");
        undoButton = GameObject.FindGameObjectWithTag("UndoButton");
        validityDisplay = GameObject.FindGameObjectWithTag("ValidityDisplay");
        winParticleDisplay = GameObject.FindGameObjectWithTag("ParticleDisplay");
		vidas = GameObject.FindGameObjectWithTag("Life");
		posicaoPersonagem = GameObject.FindGameObjectWithTag("PosicaoPersonagem");
		boss = GameObject.FindGameObjectWithTag("Boss");
		particlesBossPosition = GameObject.FindGameObjectWithTag("ParticulasBoss");
		aparicaoBoss = GameObject.FindGameObjectWithTag("AparicaoBoss");
		btnConfirmar = GameObject.FindGameObjectWithTag("btnConfirmar");


        undoButton.GetComponent<Button>().enabled = false;
        wrongImage = Resources.Load("Images/Jogo/Elementos/mistake", typeof(Sprite)) as Sprite;
		winParticle = Resources.Load("Prefabs/EfeitoCorreto2", typeof(GameObject)) as GameObject;
		bossParticle = Resources.Load("Prefabs/boss", typeof(GameObject)) as GameObject;


        dataBank = Resources.Load("wordbank") as TextAsset;
        dataStore.Populate(dataBank);
        dataStore.CreateSymbolListFromArray();

		//Recebe as resoluções da tela em que o jogo está sendo rodado
        screenWidth = Screen.width;
        screenHeight = Screen.height;

		//Inicializa as pilhas que serão utilizadas para armazenar os numeros correspondetes as palavras que já foram utilizadas
		numSairam = new Stack<int>();
		numSairamBoss = new Stack<int>();

		//Atualiza o número de corações que serão exibidos de acordo com a quantidade de vida
		vidas.GetComponent<Image>().fillAmount= GerenciadorDoGame.Instancia.qtdVidaAtual / GerenciadorDoGame.Instancia.qtdVidaMax;
		print ("dificuldade do jogo e contexto  : " + dificuldadeJogo + Contexto);
		print ("Contexto inicial: " +Contexto);

		makeRandomWord(this.Contexto,dificuldadeJogo);
		// Descobrir o personagem selecionado e o instanciar na posiçao desejada
		GameObject character = GerenciadorDoGame.Instancia.personagens [GerenciadorDoGame.Instancia.personagem];
		//GameObject avatar = (GameObject)Instantiate (character, posicaoPersonagem.transform.position, transform.rotation);
		//Atualizar a imagem do personagem na cena do chefe
		posicaoPersonagem.GetComponent<SpriteRenderer> ().sprite = GerenciadorDoGame.Instancia.personagensAssustados[GerenciadorDoGame.Instancia.personagem];

		//Preparar a musica de quando aparece a imagem com o chefe
		audioAparecerBoss = gameObject.AddComponent<AudioSource>();
		audioAparecerBoss.clip = Resources.Load("Audio/Jogo/aparecer-boss") as AudioClip;
		//audioAparecerBoss.loop = true; // fazer com que o audio fique em loop

		//Preparar o audio que representa o ataque ao chefe
		audioPorradaChefe = gameObject.AddComponent<AudioSource>();
		audioPorradaChefe.clip = Resources.Load("Audio/Jogo/porrada-chefe") as AudioClip;
		audioPorradaChefe.volume = 0.15f;

		//Preparar o audio que representa o acerto da palavra
		audioPalavraCerta = gameObject.AddComponent<AudioSource>();
		audioPalavraCerta.clip = Resources.Load("Audio/Jogo/palavra-certa") as AudioClip;
		audioPalavraCerta.volume = 0.4f; // Setar o volume - varia de 0 a 1

		//Preparar o audio que representa o erro da palavra
		audioPalavraErrada = gameObject.AddComponent<AudioSource>();
		audioPalavraErrada.clip = Resources.Load("Audio/Jogo/palavra-errada") as AudioClip;
		audioPalavraErrada.volume = 0.7f;

		//Preparar a musica de fundo da fase
		audioFase = gameObject.AddComponent<AudioSource>();
		audioFase.clip = Resources.Load("Audio/Jogo/audio-fase") as AudioClip;
		audioFase.volume = 0.3f;
		audioFase.loop = true;
		audioFase.Play (); // Tocar audio
	}

    // Carrega uma palavra aleatoria no contexto fornecido.
	public void makeRandomWord(string context, int dificuldade)
	{
		print ("Contexto: " + context + " difuculdade: " + dificuldade);

		List<Word> tempList = dataStore.getWordListByDifficulty (context, dificuldade);
		//Buscar imagem aleatoria dentro das que ainda nao foram sorteadas
		int num = (int)UnityEngine.Random.Range (0, tempList.Count);
		if (numSairam.Count < 4) { // Se sairam menos do que 4 palavras normais é gerado um numero aleatório dentro do numero de elementos da 
			print ("Num: " + num);			//lista que ainda nao foi utilizado
			while (numSairam.Contains (num)) {
				num = (int)UnityEngine.Random.Range (0, tempList.Count);
				print ("Num: " + num);
			}
			numSairam.Push (num);
		} else {// Idem, mas agora somente para palavras utilizadas no Chefe
			while (numSairamBoss.Contains (num)) { 
				num = (int)UnityEngine.Random.Range (0, tempList.Count);
				print ("Num boss: " + numSairamBoss.Count);
			}
			print ("Num boss: " + numSairamBoss.Count);
			numSairamBoss.Push (num);
		}

		currWord = tempList [(int)(num)];
		currWord.load ();
		//O display da imagem no jogo recebe a imagem que foi carregada do banco
			imageDisplay.GetComponent<SpriteRenderer> ().enabled = true; 
			imageDisplay.GetComponent<SpriteRenderer> ().sprite = currWord._Image;

			//Define o tamanho da imagem que será exibido (a escala que a imagem será multiplicada)
			float localScaleValue;
			localScaleValue = redimensionarImagem ();
			imageDisplay.GetComponent<Transform> ().localScale = new Vector3 (localScaleValue, localScaleValue, 1f);


			//Obtem-se as letras que serão mostradas nos botões, além de outras letras aleatórias
			currWord.getSymbolArray ().CopyTo (buttonSymbols, 0);
			Debug.Log ("Copied word symbol array to game array.");
			for (int i = 0; i < nButtons; i++) {
				if (buttonSymbols [i] == '\0')
					buttonSymbols [i] = dataStore.getRandomSymbol ();
			}

			//As letras sao entao embaralhadas 
			Shuffle (buttonSymbols);
			//Gera a rotação dos botoes
			RefreshButton ();

			//Recebe a palavra atual, caso seja necessária a sua exibiçao no gameover
			palavraAtual = currWord._symbolSequence;
			imagemAtual = currWord._Image;
    }
    
    // Prepara o estilo dos botoes a serem renderizados pelo codigo.
    private void SetupStyles()
    {
        if (!isStyleInit)
        {
            buttonStyle.font = Resources.Load("Fonts/HEAVITAS") as Font;
            disabledButtonStyle.font = buttonStyle.font;
            buttonStyle.normal.background = Resources.Load("Images/Jogo/Elementos/note_button") as Texture2D;
			disabledButtonStyle.normal.background = Resources.Load("Images/Jogo/Elementos/note_button_disabled") as Texture2D;
			buttonStyle.active.background = Resources.Load("Images/Jogo/Elementos/note_button_pressed") as Texture2D;
            buttonStyle.active.textColor = Color.gray;
            disabledButtonStyle.active.textColor = Color.gray;
            buttonStyle.border = new RectOffset(2, 2, 2, 2);
            disabledButtonStyle.border = new RectOffset(0, 0, 0, 0);
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            disabledButtonStyle.alignment = buttonStyle.alignment;
            isStyleInit = true;
            RefreshButton();
        }
        buttonStyle.padding.top = (int)(((screenHeight / 10) * buttonHeightMultiplier) / 4);
        buttonStyle.padding.bottom = (int)(((screenHeight / 10) * buttonHeightMultiplier) / 10);
        disabledButtonStyle.padding = buttonStyle.padding;
        buttonStyle.fontSize = (int)((screenHeight / 10) * 1.0f);
        disabledButtonStyle.fontSize = buttonStyle.fontSize;
    }

    // Recalcula a rotacao randomizada dos botoes.
    private void RefreshButton()
    {
        for (int i = 0; i < nButtons; i++)
        {
            buttonRotation[i] = UnityEngine.Random.value * 5;
            if ((i % 2) == 0) buttonRotation[i] = -buttonRotation[i];
        }
        QualitySettings.antiAliasing = antiAliasingLevel;
    }

    // Habilita todos os botoes.
    private void EnableAllButtons()
    {
        for (int i = 0; i < nButtons; i++)
        {
            buttonDisabled[i] = false;
        }
    }

    // Executada pelo Unity sempre que ocorre alguma interacao entre o usuario e a interface.
    void OnGUI()
    {

        screenWidth = Screen.width;
        screenHeight = Screen.height;
		if (!ativarBoss) {
			aparicaoBoss.GetComponent<Image> ().enabled = false; 
			posicaoPersonagem.GetComponent<SpriteRenderer> ().enabled = false; 

			SetupStyles ();

			GUIStyle currStyle;
			for (int i = 0; i < (nButtons / 2); i++) {
				if (!buttonDisabled [i])
					currStyle = buttonStyle;
				else
					currStyle = disabledButtonStyle;
				Vector2 screenPivot = new Vector2 (((screenWidth / nButtons + 2) * buttonHorizontalOffset) + (i * (screenWidth / nButtons + 2) * (buttonWidthMultiplier + buttonSeparationMultiplier)) + ((screenWidth / 10) * buttonWidthMultiplier) / 2,
					                              ((screenHeight / 10) * buttonVerticalOffset) + ((screenHeight / 10) * buttonHeightMultiplier) / 2);
				GUIUtility.RotateAroundPivot (buttonRotation [i], screenPivot);
				if (
					GUI.Button (new Rect (((screenWidth / nButtons + 2) * buttonHorizontalOffset) + (i * (screenWidth / nButtons + 2) * (buttonWidthMultiplier + buttonSeparationMultiplier))
                , ((screenHeight / 10) * buttonVerticalOffset)
                , (screenWidth / 10) * buttonWidthMultiplier
                , (screenHeight / 10) * buttonHeightMultiplier)
                , buttonSymbols [i].ToString (), currStyle))
				if (!buttonDisabled [i])
					buttonPressed (i);
				GUIUtility.RotateAroundPivot (-(buttonRotation [i]), screenPivot);
			}
			for (int i = 0; i < (nButtons / 2); i++) {
				if (!buttonDisabled [i + nButtons / 2])
					currStyle = buttonStyle;
				else
					currStyle = disabledButtonStyle;
				Vector2 screenPivot = new Vector2 (((screenWidth / nButtons + 2) * buttonHorizontalOffset) + (i * (screenWidth / nButtons + 2) * (buttonWidthMultiplier + buttonSeparationMultiplier)) + ((screenWidth / 10) * buttonWidthMultiplier) / 2,
					                              ((screenHeight / 10) * buttonVerticalOffset) + ((screenHeight / 10) * buttonHeightMultiplier) + ((screenHeight / 10) * (buttonWidthMultiplier + buttonSeparationMultiplier)) / 2);
				GUIUtility.RotateAroundPivot (buttonRotation [5 + i], screenPivot);
				if (
					GUI.Button (new Rect (((screenWidth / nButtons + 2) * buttonHorizontalOffset) + (i * (screenWidth / nButtons + 2) * (buttonWidthMultiplier + buttonSeparationMultiplier))
                    , ((screenHeight / 10) * buttonVerticalOffset) + ((screenHeight / 10) * (buttonWidthMultiplier + buttonSeparationMultiplier))
                    , (screenWidth / 10) * buttonWidthMultiplier
                    , (screenHeight / 10) * buttonHeightMultiplier)
                    , buttonSymbols [i + (nButtons / 2)].ToString (), currStyle))
				if (!buttonDisabled [i + nButtons / 2])
					buttonPressed (i + nButtons / 2);
				GUIUtility.RotateAroundPivot (-(buttonRotation [5 + i]), screenPivot);
			}
		}
		//Faz aparecer a imagem de entrada do chefe
		if (ativarBoss == true) {
			aparicaoBoss.GetComponent<Image> ().enabled = true; 
			posicaoPersonagem.GetComponent<SpriteRenderer> ().enabled = true; 
			imageDisplay.GetComponent<SpriteRenderer> ().enabled = false; 
		} 
    }

    // Embaralha um array de char.
    private void Shuffle(char[] a)
    {
        // Loops through array
        for (int i = a.Length - 1; i > 0; i--)
        {
            // Randomize a number between 0 and i (so that the range decreases each time)
            int rnd = UnityEngine.Random.Range(0, i);

            // Save the value of the current i, otherwise it'll overright when we swap the values
            char temp = a[i];

            // Swap the new and old values
            a[i] = a[rnd];
            a[rnd] = temp;
        }

    }

    // Executada quando um botao do GUI e apertao.
    private void buttonPressed(int button)
    {
        Debug.Log("Pressed button [" + buttonSymbols[button] + "]");
        inputString = inputString + buttonSymbols[button];
        buttonDisabled[button] = true;
        undoButton.GetComponent<Button>().enabled = true;
        inputButtonStack.Push(button);
        UpdateDisplay();
    }

    // Desfaz a ultima operacao executada.
    public void Undo()
    {
        Debug.Log("Pressed Button [UNDO]");
        int buttonToUndo = inputButtonStack.Pop();
        buttonDisabled[buttonToUndo] = false;
        inputString = inputString.Substring(0, inputString.Length - 1);
        UpdateDisplay();
        if (inputButtonStack.Count == 0) undoButton.GetComponent<Button>().enabled = false;
    }

    // Atualiza os displays de palavra escrita e de vida.
    public void UpdateDisplay()
    {
		//A primeira letra de cada palavra será maiúscula
		if (inputString.Length == 1) {
			inputString = inputString.ToUpper ();
		}
        stringDisplay.GetComponent<InputField>().text = inputString;
    }

    // Valida a palavra escrita pelo usuario.
	public void Confirm(){

		if (currWord._symbolSequence == inputString.ToLower()){
			//Desabilita o botao confirmar
			btnConfirmar.GetComponent<Button>().interactable = false;

			//Toca o som referente a palavra quando o jogador acerta
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.clip = currWord._sound;
			audioSource.Play();

			//Toca o som de acerto de uma palavra
			audioPalavraCerta.Play();

			// instancia particula/efeito de sucesso/acerto da palavra 
            GameObject particulasAcerto = (GameObject)Instantiate(winParticle, winParticleDisplay.transform.position, winParticleDisplay.transform.rotation);
            ParticleSystem componenteParticulasAcerto = particulasAcerto.GetComponent<ParticleSystem>();
            Destroy(particulasAcerto, componenteParticulasAcerto.duration + componenteParticulasAcerto.startLifetime);

			//Verifica se o numero de palarvras da etapa normal são menores do que 4, caso sejam será gerada uma nova palavra
			if (numSairam.Count < 4) {
				StartCoroutine (ReadyNextWord (dificuldadeJogo));
			}else if(!apareceuBoss) {
				// Caso nao seja < 4 e se a imagem do chefe ainda nao tiver aparecido, a mesma será mostrada
				StartCoroutine (AparecerBoss());
			}else{// se a imagem do chefe já tiver aparecido
				if (numSairamBoss.Count < (GerenciadorDoGame.Instancia.numFase + 1 )){// o Chefe terá uma palavra a mais que o número da fase
					print ("Confirm numSairamBoss: " + numSairam.Count);
					//Instancia a particula de ataque ao chefe na posição que foi estabelecida na fase
					GameObject particulasChefe = (GameObject)Instantiate (bossParticle, particlesBossPosition.transform.position,transform.rotation);
					ParticleSystem componenteParticulasChefe = particulasChefe.GetComponent<ParticleSystem> ();
					audioPorradaChefe.Play (); // Toca o audio
					//Destrói a particula depois do seu tempo de duração estimado
					Destroy (particulasChefe, componenteParticulasChefe.duration + componenteParticulasChefe.startLifetime);

					//A cada palavra acertada pelo jogador, o chefe vai mudando a sua coloração
					if (numSairamBoss.Count == GerenciadorDoGame.Instancia.numFase) {
						boss.GetComponent<Image> ().color = Color.red;
					} else if (numSairamBoss.Count == GerenciadorDoGame.Instancia.numFase-1) {
						boss.GetComponent<Image> ().color = Color.green;
					} else if (numSairamBoss.Count == GerenciadorDoGame.Instancia.numFase-2) {
						boss.GetComponent<Image> ().color = Color.yellow;
					}
					StartCoroutine (ReadyNextWord (dificuldadeJogo+3));// A palara do chefe está registrada no banco de dados com o nivel da dificuldade desejada+3

				}else{// Se tiver acertado todas as palavras do chefe sera direcionado para a próxima fase e ganhará uma vida
					//Instancia a particula de ataque ao chefe na posição que foi estabelecida na fase
					GameObject particulasChefe = (GameObject)Instantiate (bossParticle, particlesBossPosition.transform.position,transform.rotation);
					ParticleSystem componenteParticulasChefe = particulasChefe.GetComponent<ParticleSystem> ();
					audioPorradaChefe.Play ();
					Destroy (particulasChefe, componenteParticulasChefe.duration + componenteParticulasChefe.startLifetime);
					// Após acertar a última palavra o chefe vai ficando transparente, diminuindo o valor do alpha 
					boss.GetComponent<Image> ().color = new Color(1F, 0F, 0F, 0.5F);
					StartCoroutine (PrepararNovaFase ());
					GerenciadorDoGame.Instancia.qtdVidaAtual += 1;
				}
			}
        }

        else{//Se errar a palavra
			// Diminui a quantidade de vida
			audioPalavraErrada.Play();
            GerenciadorDoGame.Instancia.qtdVidaAtual -= 1;
			Debug.Log ("Vida atual: " + GerenciadorDoGame.Instancia.qtdVidaAtual);
			vidas.GetComponent<Image>().fillAmount= GerenciadorDoGame.Instancia.qtdVidaAtual / GerenciadorDoGame.Instancia.qtdVidaMax;
            
			// Exibe o símbolo de errado
			validityDisplay.GetComponent<SpriteRenderer>().sprite = wrongImage;
			if (GerenciadorDoGame.Instancia.qtdVidaAtual == 0) {//Se a vida chegar a zero, redirecionar o jogador para a tela de Game Over
				Application.LoadLevel ("gameover");
			} 

        }
    }

	// TO DO: Troca a palavra mostrada pela palavra carregada no backbuffer.
	public void ChangeWord(int dificuldade)
	{
		CleanUp();
		makeRandomWord(Contexto,dificuldade);
	}


    // TO DO: Carrega uma nova palavra no backbuffer.
	public IEnumerator ReadyNextWord(int dificuldade)
    {
        yield return new WaitForSeconds(3);
        ChangeWord(dificuldade);
    }

    // Reverte todas as operacoes feitas pelo usuario na interface ao resolver uma palavra.
    private void CleanUp()
	{
		Debug.Log ("Cleaning up...");
		btnConfirmar.GetComponent<Button> ().interactable = true;
		validityDisplay.GetComponent<SpriteRenderer> ().sprite = null;
		buttonSymbols = new char[nButtons];
		buttonRotation = new float[nButtons];
		buttonDisabled = new bool[nButtons];
		inputButtonStack.Clear ();
		inputString = "";
		UpdateDisplay ();
	}

	// Aparecimento da imagem do chefe
	public IEnumerator AparecerBoss()
	{
		yield return new WaitForSeconds(3);
		CleanUp();
		StartCoroutine (PararMusica (audioFase));
		ativarBoss = true; // Exibe a imagem do chefe
		audioAparecerBoss.Play();
		yield return new WaitForSeconds(8);
		ChangeWord (dificuldadeJogo+3);
		ativarBoss = false; // Esconde a imagem do chefe
		audioFase.Play();
		audioFase.volume = 0.4f;
		audioAparecerBoss.Stop ();
		apareceuBoss = true;// 
		boss.GetComponent<Image> ().enabled = true; // Exibe o avatar do chefe
		Camera.main.GetComponent<Skybox>().material = bossMaterial; // Exibe um fundo diferente para o jogo, especifico para o chefe
	}

	//Prepara-se para a nova fase, aumentando o numero da fase no Gerenciador e exibindo a tela do mapa
	public IEnumerator PrepararNovaFase()
	{
		//Diminuindo o nivel do alpha de tempo em tempo 
		yield return new WaitForSeconds(1);
		boss.GetComponent<Image> ().color = new Color(1F, 0F, 0F, 0.2F);
		yield return new WaitForSeconds(0.6f);
		boss.GetComponent<Image> ().color = new Color(1F, 0F, 0F, 0.0F);
		yield return new WaitForSeconds(1);
		//Acrescenta um ao numero da da fase
		GerenciadorDoGame.Instancia.numFase += 1;
		StartCoroutine (PararMusica (audioFase)); // Para de tocar a musica da fase
		float fadeTime = GameObject.Find ("GameManager").GetComponent<Fading>().BeginFade(1); //Efeito de fade na transiçao entre a fase e o mapa
		yield return new WaitForSeconds(0.8f);
		Application.LoadLevel("mapa");
	}

	//Atribui um valor de escala às imagens de acordo com a sua resoluçao inicial
	private float redimensionarImagem(){
		float scaleValue;
		if(currWord._Image.rect.height > 450.0){
			scaleValue = (float)(0.7 * (450.0 / currWord._Image.rect.height));
		}else{
			scaleValue = (float)(0.7 * (currWord._Image.rect.height/450.0));
		}
		return scaleValue;
	}

	//Funçao para parar o audio, diminuindo gradualmente o som e depois pausá-lo
	public IEnumerator PararMusica(AudioSource musica){
		while (musica.volume != 0f) {
			musica.volume -= 0.1f;
			yield return new WaitForSeconds(0.2f);
		}
		musica.Pause ();

	}
		

}
