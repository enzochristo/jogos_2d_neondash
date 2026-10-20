# Neon Dash

link : https://enzochristo.itch.io/neodash-game
Jogo 2D individual feito na Unity, no estilo auto-runner de Geometry Dash. O cubo corre sozinho pela fase neon e o jogador só controla o pulo: desvie dos espinhos, serras e do perseguidor, colete os orbes (cada um dá pontos e **tempo extra**) e chegue ao portal antes que o relógio zere.

## Como jogar

- **Pular:** `Espaço`, `Seta para cima`, `W`, clique do mouse/toque ou **botão A do controle** (segure para pular continuamente).
- **Pausar:** `Esc`, `P` ou `Start` do controle. **Som:** `M`.
- **Menus:** mouse, setas/`Enter` ou direcional/A do controle.
- **Objetivo:** chegar ao portal amarelo no fim da fase.
- Bater de frente em um bloco, tocar em espinho/serra/perseguidor ou cair em um buraco custa **1 vida** (são 5) e você volta ao último **checkpoint** (bandeira).
- Você começa com **40 s**; cada orbe dá **+2 s**. Se o tempo acabar ou as vidas zerarem, a partida termina e dá para tentar de novo ou voltar ao menu.
- Pontuação: 100 por orbe + 10 por segundo restante + 50 por vida restante. O melhor tempo e o melhor score ficam salvos.

## Requisitos

- Unity **6000.3.26f1** (Unity 6.3)
- Render pipeline: Universal Render Pipeline (2D)
- Resolução do build WebGL: 600x480

## Como rodar

1. Clone o repositório.
2. No Unity Hub, clique em **Add** e selecione a pasta `NeonDash/`.
3. Abra o projeto com a versão da Unity indicada acima.
4. Abra a cena `Assets/Scenes/MainMenu.unity` e aperte **Play**.

As cenas também podem ser recriadas pelo menu **Tools > Neon Dash > Criar Cenas**, e o build web sai de **Tools > Neon Dash > Build WebGL**.

## Estrutura

```
NeonDash/
├── Assets/
│   ├── Scenes/
│   │   ├── MainMenu.unity      # cena 0: menu inicial
│   │   └── SampleScene.unity   # cena 1: o jogo
│   ├── Resources/
│   │   ├── Sprites/            # sprites e capa do menu (gerados por tools/gerar_assets.py)
│   │   └── Audio/              # musicas e efeitos sonoros (idem)
│   └── _Scripts/
│       ├── GameController.cs   # estado do jogo: score, vidas, tempo, fim de jogo
│       ├── MenuActions.cs      # botoes: iniciar jogo, voltar ao menu, sair
│       ├── MenuUI.cs           # monta o menu inicial
│       ├── UIManager.cs        # HUD, pausa e painel de fim de jogo
│       ├── UIFactory.cs        # helpers para criar Canvas, textos e botoes
│       ├── PlayerMovement.cs   # corrida automatica, pulo, morte e checkpoints
│       ├── LevelBuilder.cs     # monta a fase (chao, blocos, espinhos, inimigos...)
│       ├── EnemyPatrol.cs      # serra que vai e volta
│       ├── EnemySeeker.cs      # perseguidor que segue a altura do jogador
│       ├── CameraFollow.cs     # camera e tremor
│       ├── ParallaxBackground.cs
│       ├── AudioManager.cs     # musica e efeitos
│       ├── GameAssets.cs       # carrega sprites/audios de Resources
│       ├── Fragment.cs, Spin.cs, Bob.cs
│       └── Editor/             # importador de assets, criador de cenas/build e teste automatico
├── Packages/
└── ProjectSettings/
tools/gerar_assets.py           # gera sprites, capa e audio
capa_itch.png                   # imagem de capa do itch.io (630x500)
```

## Scripts

| Script | Função |
| --- | --- |
| `GameController` | Classe estática. `Init()` zera score, vidas (5) e tempo (40 s). `Tick()` desconta o tempo, `Collect()` soma pontos e tempo, `Hit()` tira uma vida e `Finish()` calcula bônus e recordes. `gameOver` fica `true` ao vencer, acabar o tempo ou as vidas. |
| `PlayerMovement` | Move o `Rigidbody2D` à velocidade constante, pula quando está no chão, gira o cubo no ar e trata as tags `Coletavel`, `Inimigo`, `Pad`, `Checkpoint` e `Meta`. |
| `LevelBuilder` | Constrói a fase inteira (230 tiles) com buracos, blocos, espinhos, serras, perseguidores, pads, orbes e checkpoints. |
| `EnemyPatrol` / `EnemySeeker` | Inimigos com comportamento de patrulha e de perseguição. |
| `UIManager` | HUD (score, tempo, vidas, progresso), pausa e painel de fim de jogo com tempo final e recorde. |

## Rubrica (autoavaliação)

- Tutorial: menu, cena de jogo, jogador, coletáveis e painel de fim de jogo.
- Mecânica de tempo: cronômetro regressivo, orbes dão tempo, tempo final e melhor tempo na tela final.
- Inimigos: espinhos (parados), serras (patrulha) e perseguidor.
- Visual: sprites neon coerentes, fundo com parallax e partículas de explosão.
- Áudio: uma música animada que toca no menu e no jogo sem reiniciar (para trocar, coloque `music_custom.mp3` em `Resources/Audio`) e efeitos de pulo, orbe, pad, checkpoint, morte, vitória e derrota.
- UI: score, tempo, vidas e barra de progresso.
- Controles: teclado, mouse/toque e controle Xbox (inclusive nos menus).
- Level design: tema neon/synthwave, arena nova com buracos, plataformas, pads e checkpoints.

## Créditos

Todos os sprites, a capa e o áudio foram gerados proceduralmente por `tools/gerar_assets.py` (Python + Pillow + NumPy), sem assets de terceiros. A fonte do jogo é a Liberation Sans, embutida na Unity. O estilo é inspirado em *Geometry Dash* (RobTop Games).
