🛡️ Survive The Swarm

Instituto Politécnico do Cávado e do Ave (IPCA) Licenciatura em Engenharia de Desenvolvimento de Jogos Digitais (EDJD) Unidade Curricular: Técnica de Desenvolvimento de VideoJogos


📖 Sobre o Projeto
Survive The Swarm é um jogo top-down survival shooter 2D desenvolvido em C# com a framework MonoGame. O jogador assume o controlo de uma nave espacial em território hostil,
onde o objetivo principal é a sobrevivência extrema contra hordas intermináveis de inimigos, enquanto navega por campos de asteroides dinâmicos.

⚙️ Mecânicas de Jogo
O jogo baseia-se num ciclo infinito de sobrevivência com as seguintes características:

* Controlos de Movimento: O jogador controla uma nave espacial deslocando-se pelo ecrã utilizando as teclas W, A, S, D.
* Mira e Disparo: Sistema de rotação de 360° que acompanha o cursor do rato. O combate é efetuado através de lasers disparados com o Botão Esquerdo do Rato (LMB).
* Inimigos Dinâmicos: Sistema de spawn inteligente fora do campo de visão do jogador. Os inimigos possuem inteligência de perseguição, e a sua velocidade aumenta progressivamente conforme a pontuação do jogador sobe.
* Sistema de Meteoros (Obstáculos): O cenário gera meteoros que flutuam aleatoriamente (com direções e velocidades de rotação independentes). Estes funcionam como obstáculos físicos: bloqueiam o movimento da nave e destroem os lasers disparados.
* Sistema de Pontuação e Vidas:** O jogador inicia a partida com 3 vidas. A eliminação de cada inimigo concede 100 pontos. O jogo termina quando o jogador perde todas as vidas ao colidir com os inimigos.

🏛️Estrutura do Projeto
O projeto foi dividido em várias partes para facilitar a organização e manutenção do código.
SurviveTheSwarm/
│
├── Content/
│   └── Recursos gráficos e áudio
│
├── Classes/
│   └── Classes principais
│
├── GameObjects/
│   └── Jogador, inimigos e objetos
│
├── Managers/
│   └── Gestão de sistemas
│
├── States/
│   └── Estados do jogo
│
├── Game1.cs
│   └── Ciclo principal do jogo
│
└── Program.cs
    └── Inicialização


💻 Destaques Técnicos
Durante o desenvolvimento, foram implementados vários sistemas essenciais em MonoGame:

🔄Gestão de Estados (Game States)
Implementação de uma Máquina de Estados (FSM) que gere o fluxo do jogo através dos estados:

  Menu: Ecrã inicial e preparação.

  AJogar: Ciclo principal de atualização e renderização de física.

  FimDeJogo: Processamento de pontuação final e opção de reinício.

🎥 Câmara e Transformações de Matriz
Utilização de Matrix Mathematics (Matrix.CreateTranslation) para criar uma câmara virtual que mantém o jogador centralizado. O sistema inclui a conversão de coordenadas do ecrã para o mundo (Vector2.Transform),
permitindo que a mira do rato seja precisa mesmo com a câmara em movimento.

💥 Sistema de Colisões e Física
Uso de AABB (Axis-Aligned Bounding Boxes) através da estrutura Rectangle do MonoGame. O motor de jogo processa múltiplas camadas de colisão:

Nave <-> Inimigos (Dano)

Laser <-> Inimigos (Pontuação)

Laser <-> Meteoros (Obstrução)

Nave <-> Meteoros (Bloqueio de movimento)

🚀 Otimização e Memory Management
Para evitar o consumo excessivo de RAM e CPU, foi implementado um sistema de limpeza de entidades:

Objetos como meteoros e tiros são removidos das List<T> assim que ultrapassam um raio de 2000 unidades do jogador.

Gestão eficiente do ciclo de vida dos objetos para evitar memory leaks.

📂 Assets e Recursos
O projeto utiliza assets otimizados para garantir uma estética arcade coesa:

Sprites: Assets da coleção Kenney (Nave: playerShip2_blue, Inimigos: enemyBlack5, Lasers: laserBlue15, Meteoros: meteorBrown_big1).

Áudio: Implementação de som posicional e música ambiente utilizando as classes SoundEffect para disparos e MediaPlayer para a banda sonora em loop.

🚀 Como Executar
Pré-requisitos: Ter instalado o .NET SDK e o ambiente compatível com MonoGame (Visual Studio 2022 recomendado).

Clone o Projeto:

Bash
git clone https://github.com/TiagoSM1234/SurviveTheSwarm.git
Compilação: Abra o ficheiro SurviveTheSwarm.sln no Visual Studio.

Execução: Pressione F5 para compilar e iniciar o jogo.

👤 Autores
Tiago Martins 34986
Diogo Fernandes 34988
Vitor Ferreira 31488
