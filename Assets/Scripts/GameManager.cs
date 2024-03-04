using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[SerializeField] private GameObject _mainGame;
	[SerializeField] private GameObject _playerHUD;
	[SerializeField] private GameObject _mainMenu;
	[SerializeField] private GameObject _signupMenu;
	[SerializeField] private GameObject _leaderboardsMenu;
	[SerializeField] private GameObject _settingsMenu;
	[SerializeField] private GameObject _pauseMenu;
	[SerializeField] private TMP_InputField _usernameField; 
	[SerializeField] private Timer _timer;
	[SerializeField] private LeaderBoards _lb;
	[SerializeField] private Scrollbar _volumeDial;


	[SerializeField] private GameObject _overviewMenu;
	[SerializeField] private TMP_Text _overviewTime;
	[SerializeField] private TMP_Text _overviewBalloons;

	[SerializeField] private List<Level> _levels;

	[SerializeField] private List<AudioClip> _sounds;

	[SerializeField] private string url = "https://ddmo.fr.to/speed/";

	PlayerManager _player;

	AudioSource _audioPlayer;

	private int _currentLevel;
	private string _username;

	public void PlaySound(int id)
	{
		if (_sounds.Count > id)
		{
			_audioPlayer.PlayOneShot(_sounds[id]);
		}
	}

	public void PlayRepeatSound(int id)
	{
		if (_sounds.Count > id)
		{
			_audioPlayer.clip = _sounds[id];
			_audioPlayer.Play();
		}
	}
	
	public void OpenPauseMenu()
	{
        Cursor.lockState = CursorLockMode.Confined;
        _playerHUD.SetActive(false);
		_pauseMenu.SetActive(true);

        _player.gameObject.GetComponent<PlayerController>().SetPaused(true);
    }

	public void ClosePauseMenu()
	{
        Cursor.lockState = CursorLockMode.Locked;
        _playerHUD.SetActive(true);
		_pauseMenu.SetActive(false);

		_player.gameObject.GetComponent<PlayerController>().SetPaused(false);
    }

	public void StopSound()
	{
		_audioPlayer.Stop();
	}

	public void ChangeVolume()
	{
		_audioPlayer.volume = _volumeDial.value;
	}

	public Level GetCurrentLevel() => _levels[_currentLevel];

	IEnumerator SendScore()
	{
		UnityWebRequest www = UnityWebRequest.Get(url + $"set-score/?name={_username}&time={_timer.GetTimeInSeconds()}&balloons={_player.GetNumberOfBalloons()}");
		yield return www.SendWebRequest();

		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.Log(www.error);
		}
	}

	public void EndGame()
	{
		StartCoroutine(SendScore());
		
		OpenOverview();
		_overviewTime.text = _timer.GetTimeInSeconds().ToString("F2");
		_overviewBalloons.text = _player.GetNumberOfBalloons().ToString();
	}
	
	public void RestartCurrentLevel()
	{
        _player.IsDead = false;
        _player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
		_player.gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_DissolveAmount", 0.0f);
		_levels[_currentLevel].StartLevel(_player);
		if (_player.GotBalloon)
		{
			_player.DecreaseBallonCount();
			_player.GotBalloon = false;
		}
    }

	public void NextLevel()
	{
		_levels[_currentLevel].EndLevel();
		_currentLevel++;
		_player.GotBalloon = false;
		if (_levels.Count > _currentLevel) GetCurrentLevel().StartLevel(_player);
		else EndGame();
	}

	public void Play()
	{
        Cursor.lockState = CursorLockMode.Locked;
        _username = _usernameField.text;

		_mainMenu.SetActive(false);
		_settingsMenu.SetActive(false);
		_leaderboardsMenu.SetActive(false);
		_signupMenu.SetActive(false);
		_pauseMenu.SetActive(false);

		_mainGame.SetActive(true);
		_playerHUD.SetActive(true);

		_player.GotBalloon = false;
		_player.ResetNumberOfBalloons();
		_currentLevel = 0;

		_timer.ResetTimer();

		if (_levels.Count > 0) {
			Level l = GetCurrentLevel();
			l.gameObject.SetActive(true);
			l.StartLevel(_player);
		}
	}

	public void Signup()
	{
        Cursor.lockState = CursorLockMode.Confined;
        _mainGame.SetActive(false);
		_playerHUD.SetActive(false);

		_mainMenu.SetActive(false);
		_settingsMenu.SetActive(false);
		_leaderboardsMenu.SetActive(false);
		_signupMenu.SetActive(true);
		_pauseMenu.SetActive(false);

	}
	public void OpenOverview()
	{
        Cursor.lockState = CursorLockMode.Confined;
        _playerHUD.SetActive(false);

		_mainMenu.SetActive(false);
		_settingsMenu.SetActive(false);
		_leaderboardsMenu.SetActive(false);
		_signupMenu.SetActive(false);
		_overviewMenu.SetActive(true);
		_pauseMenu.SetActive(false);
	}

	public void OpenLeaderboards()
	{
        Cursor.lockState = CursorLockMode.Confined;
        _mainGame.SetActive(false);
		_playerHUD.SetActive(false);

		_mainMenu.SetActive(false);
		_settingsMenu.SetActive(false);
		_leaderboardsMenu.SetActive(true);
		_signupMenu.SetActive(false);
		_overviewMenu.SetActive(false);
		_pauseMenu.SetActive(false);

		_lb.Reload();
	}

	public void OpenMainMenu()
	{
		if (_player.IsDead) return;

        Cursor.lockState = CursorLockMode.Confined;
        _mainGame.SetActive(false);
		_playerHUD.SetActive(false);

		_mainMenu.SetActive(true);
		_settingsMenu.SetActive(false);
		_leaderboardsMenu.SetActive(false);
		_signupMenu.SetActive(false);
		_overviewMenu.SetActive(false);
		_pauseMenu.SetActive(false);
	}

	public void OpenSettings()
	{
		Cursor.lockState = CursorLockMode.Confined;
		_mainGame.SetActive(false);
		_playerHUD.SetActive(false);

		_mainMenu.SetActive(false);
		_settingsMenu.SetActive(true);
		_leaderboardsMenu.SetActive(false);
		_signupMenu.SetActive(false);
		_overviewMenu.SetActive(false);
		_pauseMenu.SetActive(false);

		_volumeDial.value = _audioPlayer.volume;
	}

	public void Quit()
	{
		Application.Quit();
	}

	public void Awake()
	{
        Cursor.lockState = CursorLockMode.Confined;

        _audioPlayer = GetComponent<AudioSource>();
		_player =FindAnyObjectByType<PlayerManager>();

		_mainMenu.SetActive(true);
		_mainGame.SetActive(false);
		_playerHUD.SetActive(false);

		foreach (Level level in _levels) level.gameObject.SetActive(false);
	}
}
