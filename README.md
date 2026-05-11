# SWE-402 Worksheet 6 - Knockout Arena UI, Polish & Publishing

Open Assets/Scenes/KnockoutArena_UI.unity. The game starts idle on the title screen; choose Easy, Medium, or Hard to begin.

Implemented requirements:
- Canvas uses Screen Space - Overlay.
- TextMeshProUGUI score and wave text are anchored at the top-left and top-center.
- GameManager owns score, wave text, StartGame(int), UpdateScore(int), GameOver(), RestartGame(), and the OnGameOver event.
- Game over UI and restart button start inactive; GameOverUI subscribes/unsubscribes in OnEnable/OnDisable and fades in with a coroutine.
- Title Screen groups title text and three difficulty buttons. DifficultyButton passes an inspector-set difficulty to GameManager.StartGame(int).
- Spawning, player input, score, and enemy movement are gated by GameManager.IsGameActive.
- Serialized fields use [SerializeField], [Range], [Tooltip], and [HideInInspector] according to the worksheet.
- Coroutine UI polish includes game-over fade, title fade-out, score punch, and wave text slide.

Publishing note:
The scene is added to Build Settings and Player Settings are configured. Producing WebGL/desktop builds and uploading to Unity Play or itch.io still requires a signed-in local Unity account and the matching build module.
