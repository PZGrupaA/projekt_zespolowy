using UnityEngine;

using UnityEngine.SceneManagement;



namespace Zaliczenie

{

public class Hud : MonoBehaviour

{

int coins;

Texture coinIcon;

Health playerHealth;

Player playerComponent;

bool showContinuePrompt;

bool showWin;

float deathShownAt;

int selectedSlot = -1;

Font uiFont;



Texture2D pixel;

GUIStyle titleStyle;

GUIStyle subtitleStyle;

GUIStyle hintStyle;

GUIStyle retryStyle;

GUIStyle leaveStyle;

GUIStyle riddleTitleStyle;

GUIStyle riddleStyle;

GUIStyle letterStyle;

bool stylesReady;



void OnEnable()

{

if (Money.Instance != null)

{

coins = Money.Instance.Coins;

Money.Instance.CoinCollected += OnCoinCollected;

}



GameObject player = GameObject.FindGameObjectWithTag("Player");

if (player != null)

{

playerComponent = player.GetComponent<Player>();

playerHealth = player.GetComponent<Health>();

if (playerHealth != null)

playerHealth.Died += OnPlayerDied;

}



Sprite icon = SpriteBank.LoadSingle("monetka");

if (icon != null)

coinIcon = icon.texture;

}



void OnDisable()

{

if (Money.Instance != null)

Money.Instance.CoinCollected -= OnCoinCollected;



if (playerHealth != null)

playerHealth.Died -= OnPlayerDied;



Time.timeScale = 1f;

}



void OnCoinCollected(int amount)

{

coins = amount;

}



void Update()

{

if (showContinuePrompt)

{

if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))

ContinueGame();

else if (Input.GetKeyDown(KeyCode.Escape))

QuitOrEndGame();

return;

}



if (!showWin && LetterBag.Instance != null && LetterBag.Instance.Solved)

{

showWin = true;

selectedSlot = -1;

Time.timeScale = 0f;

}



if (!showWin)

return;



if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))

RestartLevel();

else if (Input.GetKeyDown(KeyCode.Escape))

QuitOrEndGame();

}



void OnPlayerDied()

{

showContinuePrompt = true;

deathShownAt = Time.unscaledTime;

Time.timeScale = 0f;

}



void OnGUI()

{

if (showContinuePrompt)

{

DrawDeathScreen();

return;

}



if (showWin)

{

DrawWinScreen();

return;

}



DrawPlayHud();

DrawBag();

}



void DrawPlayHud()

{

int screen = 1;

if (LevelLayout.ScreenWidth > 1f)

{

GameObject player = GameObject.FindGameObjectWithTag("Player");

if (player != null)

screen = Mathf.Clamp(Mathf.FloorToInt(player.transform.position.x / LevelLayout.ScreenWidth) + 1, 1, LevelLayout.ScreenCount);

}



GUI.Label(new Rect(24f, 16f, 320f, 24f), "Level 1 screen " + screen + " / " + LevelLayout.ScreenCount);

GUI.Label(new Rect(24f, 40f, 220f, 24f), "Coins: " + coins);

if (coinIcon != null)

GUI.DrawTexture(new Rect(160f, 36f, 24f, 24f), coinIcon, ScaleMode.ScaleToFit, true);

}



void DrawBag()

{

if (LetterBag.Instance == null)

return;



EnsureStyles();

PrepareCyrillic();



LetterBag bag = LetterBag.Instance;

float width = Mathf.Min(820f, Screen.width - 32f);

GUI.Label(new Rect((Screen.width - width) * 0.5f, 68f, width, 24f), "Riddle", riddleTitleStyle);

GUI.Label(new Rect((Screen.width - width) * 0.5f, 92f, width, 48f), LetterBag.Riddle, riddleStyle);



float slot = 62f;

float gap = 10f;

float total = slot * LetterBag.Size + gap * (LetterBag.Size - 1);

float x = (Screen.width - total) * 0.5f;

float y = Screen.height - slot - 46f;



GUI.Label(new Rect(x, y - 28f, total, 24f), "Bag " + bag.Count + " / " + LetterBag.Size, riddleTitleStyle);



Color previousBackground = GUI.backgroundColor;

for (int i = 0; i < LetterBag.Size; i++)

{

Rect rect = new Rect(x + i * (slot + gap), y, slot, slot);

bool chosen = i == selectedSlot;

GUI.backgroundColor = chosen

? new Color(0.83f, 0.62f, 0.22f, 1f)

: new Color(0.16f, 0.1f, 0.22f, 1f);



char glyph = bag.Get(i);

string caption = glyph == '\0' ? "" : glyph.ToString();

if (GUI.Button(rect, caption, letterStyle))

{

if (selectedSlot < 0 || selectedSlot == i)

selectedSlot = selectedSlot == i ? -1 : i;

else

{

bag.Swap(selectedSlot, i);

selectedSlot = -1;

}

}

}



GUI.backgroundColor = previousBackground;



string hint = bag.Count < LetterBag.Size

? "Collect 5 letters to pass the level "

: "Click 2 letters to swap them";

GUI.Label(new Rect(x - 40f, y + slot + 6f, total + 80f, 24f), hint, hintStyle);

}



void DrawDeathScreen()

{

EnsureStyles();



float fade = Mathf.Clamp01((Time.unscaledTime - deathShownAt) / 0.45f);

fade = fade * fade * (3f - 2f * fade);



Color previousColor = GUI.color;

Color previousBackground = GUI.backgroundColor;

Matrix4x4 previousMatrix = GUI.matrix;

GUI.backgroundColor = Color.white;



GUI.color = new Color(0.02f, 0.0f, 0.03f, 0.82f * fade);

GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), pixel);



float band = Screen.height * 0.22f;

GUI.color = new Color(0.45f, 0.02f, 0.05f, 0.45f * fade);

GUI.DrawTexture(new Rect(0f, 0f, Screen.width, band), pixel);

GUI.DrawTexture(new Rect(0f, Screen.height - band, Screen.width, band), pixel);



float centerX = Screen.width * 0.5f;

float centerY = Screen.height * 0.46f;

float scale = Mathf.Lerp(1.06f, 1f, fade);

GUIUtility.ScaleAroundPivot(new Vector2(scale, scale), new Vector2(centerX, centerY));

GUI.color = new Color(1f, 1f, 1f, fade);



titleStyle.fontSize = Mathf.Clamp(Mathf.RoundToInt(Screen.height * 0.085f), 32, 64);

subtitleStyle.fontSize = Mathf.Clamp(Mathf.RoundToInt(Screen.height * 0.03f), 14, 20);



float lineWidth = Mathf.Min(460f, Screen.width * 0.5f);

float titleWidth = Mathf.Min(720f, Screen.width - 48f);

DrawRule(centerX - lineWidth * 0.5f, centerY - 86f, lineWidth, new Color(0.72f, 0.18f, 0.16f, fade));

GUI.Label(new Rect(centerX - titleWidth * 0.5f, centerY - 78f, titleWidth, 72f), "YOU DIED", titleStyle);

GUI.Label(new Rect(centerX - titleWidth * 0.5f, centerY - 4f, titleWidth, 32f), "The fire went out.", subtitleStyle);

DrawRule(centerX - lineWidth * 0.5f, centerY + 40f, lineWidth, new Color(0.83f, 0.66f, 0.32f, fade));



float buttonWidth = 168f;

float buttonHeight = 48f;

float buttonY = centerY + 68f;

float gap = 18f;



if (GUI.Button(new Rect(centerX - buttonWidth - gap * 0.5f, buttonY, buttonWidth, buttonHeight), "RETRY", retryStyle))

ContinueGame();



if (GUI.Button(new Rect(centerX + gap * 0.5f, buttonY, buttonWidth, buttonHeight), "LEAVE", leaveStyle))

QuitOrEndGame();



GUI.Label(new Rect(centerX - 280f, buttonY + 64f, 560f, 24f), "Enter — try again Esc — leave", hintStyle);



GUI.matrix = previousMatrix;

GUI.color = previousColor;

GUI.backgroundColor = previousBackground;

}



void DrawWinScreen()

{

EnsureStyles();

PrepareCyrillic();



Color previousColor = GUI.color;

Color previousBackground = GUI.backgroundColor;

GUI.backgroundColor = Color.white;



GUI.color = new Color(0.02f, 0.03f, 0.02f, 0.84f);

GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), pixel);



float band = Screen.height * 0.2f;

GUI.color = new Color(0.55f, 0.38f, 0.08f, 0.35f);

GUI.DrawTexture(new Rect(0f, 0f, Screen.width, band), pixel);

GUI.DrawTexture(new Rect(0f, Screen.height - band, Screen.width, band), pixel);

GUI.color = Color.white;



float centerX = Screen.width * 0.5f;

float centerY = Screen.height * 0.44f;

titleStyle.fontSize = Mathf.Clamp(Mathf.RoundToInt(Screen.height * 0.07f), 28, 54);

titleStyle.normal.textColor = new Color(0.95f, 0.82f, 0.38f, 1f);

subtitleStyle.fontSize = Mathf.Clamp(Mathf.RoundToInt(Screen.height * 0.032f), 16, 22);



float lineWidth = Mathf.Min(520f, Screen.width * 0.55f);

float titleWidth = Mathf.Min(760f, Screen.width - 40f);

DrawRule(centerX - lineWidth * 0.5f, centerY - 78f, lineWidth, new Color(0.83f, 0.66f, 0.32f, 1f));

GUI.Label(new Rect(centerX - titleWidth * 0.5f, centerY - 70f, titleWidth, 64f), "LEVEL COMPLETED", titleStyle);

GUI.Label(new Rect(centerX - titleWidth * 0.5f, centerY + 2f, titleWidth, 36f), "Answer " + LetterBag.Answer, subtitleStyle);

DrawRule(centerX - lineWidth * 0.5f, centerY + 48f, lineWidth, new Color(0.83f, 0.66f, 0.32f, 1f));



float buttonWidth = 190f;

float buttonHeight = 48f;

float buttonY = centerY + 74f;

float gap = 18f;

GUI.backgroundColor = Color.white;

if (GUI.Button(new Rect(centerX - buttonWidth - gap * 0.5f, buttonY, buttonWidth, buttonHeight), "PLAY AGAIN", retryStyle))

RestartLevel();

if (GUI.Button(new Rect(centerX + gap * 0.5f, buttonY, buttonWidth, buttonHeight), "LEAVE", leaveStyle))

QuitOrEndGame();



GUI.Label(new Rect(centerX - 280f, buttonY + 60f, 560f, 24f), "Enter — play again          Esc — leave", hintStyle);



titleStyle.normal.textColor = new Color(0.86f, 0.16f, 0.14f, 1f);

GUI.color = previousColor;

GUI.backgroundColor = previousBackground;

}



void EnsureStyles()

{

if (pixel == null)

{

pixel = new Texture2D(1, 1, TextureFormat.RGBA32, false);

pixel.SetPixel(0, 0, Color.white);

pixel.Apply();

pixel.hideFlags = HideFlags.HideAndDontSave;

pixel.wrapMode = TextureWrapMode.Clamp;

}



if (stylesReady)

return;



titleStyle = new GUIStyle(GUI.skin.label);

titleStyle.fontSize = 48;

titleStyle.fontStyle = FontStyle.Bold;

titleStyle.alignment = TextAnchor.MiddleCenter;

titleStyle.normal.textColor = new Color(0.86f, 0.16f, 0.14f, 1f);



subtitleStyle = new GUIStyle(GUI.skin.label);

subtitleStyle.fontSize = 18;

subtitleStyle.alignment = TextAnchor.MiddleCenter;

subtitleStyle.normal.textColor = new Color(0.86f, 0.78f, 0.66f, 1f);



hintStyle = new GUIStyle(GUI.skin.label);

hintStyle.fontSize = 13;

hintStyle.alignment = TextAnchor.MiddleCenter;

hintStyle.normal.textColor = new Color(0.62f, 0.58f, 0.54f, 1f);



retryStyle = MakeButtonStyle(new Color(0.62f, 0.14f, 0.13f, 1f), new Color(0.82f, 0.22f, 0.16f, 1f));

leaveStyle = MakeButtonStyle(new Color(0.16f, 0.15f, 0.18f, 1f), new Color(0.30f, 0.28f, 0.32f, 1f));



uiFont = GameFont.Get();

riddleTitleStyle = new GUIStyle(GUI.skin.label);

riddleTitleStyle.font = uiFont;

riddleTitleStyle.fontSize = 16;

riddleTitleStyle.fontStyle = FontStyle.Bold;

riddleTitleStyle.alignment = TextAnchor.MiddleCenter;

riddleTitleStyle.normal.textColor = new Color(0.95f, 0.82f, 0.4f, 1f);



riddleStyle = new GUIStyle(GUI.skin.label);

riddleStyle.font = uiFont;

riddleStyle.fontSize = 20;

riddleStyle.wordWrap = true;

riddleStyle.alignment = TextAnchor.MiddleCenter;

riddleStyle.normal.textColor = new Color(0.96f, 0.93f, 0.86f, 1f);



letterStyle = new GUIStyle(GUI.skin.button);

letterStyle.font = uiFont;

letterStyle.fontSize = 28;

letterStyle.fontStyle = FontStyle.Bold;

letterStyle.alignment = TextAnchor.MiddleCenter;

letterStyle.normal.background = Solid(Color.white);

letterStyle.hover.background = Solid(Color.white);

letterStyle.active.background = Solid(Color.white);

letterStyle.normal.textColor = new Color(1f, 0.9f, 0.45f, 1f);

letterStyle.hover.textColor = Color.white;

letterStyle.active.textColor = Color.white;



if (uiFont != null)

{

titleStyle.font = uiFont;

subtitleStyle.font = uiFont;

hintStyle.font = uiFont;

retryStyle.font = uiFont;

leaveStyle.font = uiFont;

}



stylesReady = true;

}



void PrepareCyrillic()

{

if (uiFont == null)

return;



string alphabet = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNMĄĘÓŚĆęóąćś —.,!?0123456789/";

uiFont.RequestCharactersInTexture(alphabet, 64, FontStyle.Bold);

uiFont.RequestCharactersInTexture(alphabet, 28, FontStyle.Bold);

uiFont.RequestCharactersInTexture(alphabet, 20, FontStyle.Normal);

uiFont.RequestCharactersInTexture(alphabet, 16, FontStyle.Bold);

}



GUIStyle MakeButtonStyle(Color normal, Color hover)

{

GUIStyle style = new GUIStyle(GUI.skin.button);

style.fontSize = 16;

style.fontStyle = FontStyle.Bold;

style.alignment = TextAnchor.MiddleCenter;

style.normal.background = Solid(normal);

style.hover.background = Solid(hover);

style.active.background = Solid(hover);

style.normal.textColor = new Color(0.96f, 0.93f, 0.88f, 1f);

style.hover.textColor = Color.white;

style.active.textColor = Color.white;

return style;

}



Texture2D Solid(Color color)

{

Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);

texture.SetPixel(0, 0, color);

texture.Apply();

texture.hideFlags = HideFlags.HideAndDontSave;

texture.wrapMode = TextureWrapMode.Clamp;

return texture;

}



void DrawRule(float x, float y, float width, Color color)

{

Color previous = GUI.color;

GUI.color = color;

GUI.DrawTexture(new Rect(x, y, width, 2f), pixel);

GUI.color = previous;

}



void ContinueGame()

{

showContinuePrompt = false;

Time.timeScale = 1f;



if (playerComponent == null)

{

GameObject player = GameObject.FindGameObjectWithTag("Player");

if (player != null)

playerComponent = player.GetComponent<Player>();

}



if (playerComponent != null)

{

playerComponent.OnDied();

}

else if (playerHealth != null)

{

playerHealth.RestoreFull();

}

}



void RestartLevel()

{

Time.timeScale = 1f;

showWin = false;

selectedSlot = -1;

SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

}



void QuitOrEndGame()

{

showContinuePrompt = false;

Time.timeScale = 1f;



#if UNITY_EDITOR

UnityEditor.EditorApplication.isPlaying = false;

#else

Application.Quit();

#endif

}

}

}