using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaManager : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private GameObject _lavaPool;
	[SerializeField] private GameObject _lavaSurface;

	[SerializeField] float _increaseRate;
	[SerializeField] float _decreaseRate;

	private Vector3 _surfaceInitalPosiiton;
	private Vector3 _poolInitalPosiiton;
	private Vector3 _poolInitalScale;

	private GameManager _gameManager;

	private bool isRising = true;
	private bool isLowering = false;

	public bool IsRising()
	{
		return isRising;
	}

	public bool IsLowering()
	{
		return isRising;
	}

	public void SetRaiseLavaSpeed(float speed)
	{
		_increaseRate = speed;
	}

	public void SetLowerLavaSpeed(float speed)
	{
		_decreaseRate = speed;
	}

	public void StartLavaFlow()
	{
		isRising = true;
		isLowering = false;
		gameObject.SetActive(true);
		Debug.Log(_gameManager);
		_gameManager.PlayRepeatSound(7);
		_increaseRate = Mathf.Abs(_increaseRate);
	}

	public void StopLavaFlow()
	{
		isRising = false;
		isLowering = false;
	}

	public void StartLavaDrain()
	{
		isRising = false;
		isLowering = true;
		_decreaseRate = -Mathf.Abs(_decreaseRate);
	}

	public void LavaReset()
	{
		_lavaPool.transform.position = _poolInitalPosiiton;
		_lavaPool.transform.localScale = _poolInitalScale;
		_lavaSurface.transform.position = _surfaceInitalPosiiton;
		StartLavaFlow();
	}

	private void Awake()
	{
		_gameManager = FindAnyObjectByType<GameManager>();
		_lavaSurface.GetComponent<SpriteRenderer>().material.EnableKeyword("_ISSURFACE");

		_poolInitalScale = _lavaPool.transform.localScale;
		_poolInitalPosiiton = _lavaPool.transform.position;
		_surfaceInitalPosiiton = _lavaSurface.transform.position;
	}

	private void FixedUpdate()
	{
		if (isRising)
		{
			_lavaSurface.transform.position += transform.up * _increaseRate;
			_lavaPool.transform.position += transform.up * _increaseRate / 2.0f;
			_lavaPool.transform.localScale += transform.up * _increaseRate;
			
		} 
		else if (isLowering && _lavaPool.transform.localScale.y >= 0.0f)
		{
			_lavaSurface.transform.position += transform.up * _decreaseRate;
			_lavaPool.transform.position += transform.up * _decreaseRate / 2.0f;
			_lavaPool.transform.localScale += transform.up * _decreaseRate;
		} 
		else if (isLowering)
		{
			_gameManager.StopSound();
			gameObject.SetActive(false);
		}
	}
}
