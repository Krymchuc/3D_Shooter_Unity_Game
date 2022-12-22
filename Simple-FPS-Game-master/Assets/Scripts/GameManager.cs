using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    PlayerMove player; //здоров'я гравця
    public static GameManager gm;
    public GameObject gameLabel;
    Text gameText;
    private void Awake()
    {
        if (gm == null) gm = this;
    }
    public enum GameState
    {
        Ready, Run, Pause, GameOver
    }
    public GameState gState;
    public GameObject gameOption; //Змінна об’єкта інтерфейсу користувача екрану параметрів
    // Start is called before the first frame update
    void Start()
    {
        // Встановити початковий стан гри у стан готовності.
        gState = GameState.Ready;
        // Отримати компонент Text з об'єкта UI стану гри.
        gameText = gameLabel.GetComponent<Text>();
        //Установлює текст статусу на Готово...
        gameText.text = "Ready...";
        //колір статусу
        gameText.color = new Color32(255, 185, 0, 255);
        //Підготовка до гри
        StartCoroutine(ReadyToStart());
        player = GameObject.Find("Player").GetComponent<PlayerMove>();
    }
    IEnumerator ReadyToStart()
    {
        yield return new WaitForSeconds(2f);
        gameText.text = "Go!";
        Score.score = 0;
        yield return new WaitForSeconds(0.5f);
        gameLabel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gState = GameState.Run;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenOptionWindow();
        }
        if (player.hp <= 0) // якщо гравець мертвий.
        {
            // зупинити анімацію гравця
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            player.GetComponentInChildren<Animator>().SetFloat("MoveMotion", 0); 
            gameLabel.SetActive(true); // текст статусу.
            gameText.text = "Game Over";//текстовий вміст статусу на Game Over..
            gameText.color = new Color32(255, 0, 0, 255);
            // Отримуємо компонент трансформації дочірнього об'єкта тексту стану.
            Transform buttons = gameText.transform.GetChild(0);
            buttons.gameObject.SetActive(true);// активувати об'єкт кнопки



            gState = GameState.GameOver;
        }
    }
    public void OpenOptionWindow()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        gameOption.SetActive(true); // Активація вікна параметрів.
        Time.timeScale = 0f; // швидкість гри на швидкість 0x.
        gState = GameState.Pause; //стан гри на стан паузи.
    }
    public void CloseOptionWindow()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gameOption.SetActive(false);// Вимкнути вікно параметрів.
        Time.timeScale = 1f; //Зміна швидкості гри на швидкість 1x.
        gState = GameState.Run; // стан гри на стан у грі.
    }
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Score.score = 0;
        Score.time = 0;
    }
    public void ResultGame()
    {
        Application.LoadLevel("Ending");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
