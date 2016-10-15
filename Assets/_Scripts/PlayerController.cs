﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	// PRIVATE INSTANCE VARIABLES
	private Transform _transform;
	private Rigidbody2D _rigidbody;
	private Animator _animator;
	private float _move;
	private float _jump;
	private bool _isFacingRight;
	private bool _isGrounded;

	// PUBLIC INSTANCE VARIABLES (FOR TESTING)
	public float Velocity = 10f;
	public float JumpForce = 100f;
	public GameObject camera;
	public GameObject SpawnPoint;

	[Header("Sound Clips")]
	public AudioSource JumpSound;
	public AudioSource DeathSound;
	public AudioSource CoinSound;
	public AudioSource HurtSound;

	// Use this for initialization
	void Start () {
		this._initialize ();
	}
	
	// Update is called once per frame (Physics)
	void FixedUpdate () {

		if (this._isGrounded) {
			// check if input is present for movement
			this._move = Input.GetAxis ("Horizontal");
			if (this._move > 0f) {
				// when hero is moving set HeroState to 1 which is the Walk State
				this._animator.SetInteger ("HeroState", 1);
				this._move = 1;
				this._isFacingRight = true;
				this._flip ();
			} else if (this._move < 0f) {
				this._animator.SetInteger ("HeroState", 1);
				this._move = -1;
				this._isFacingRight = false;
				this._flip ();
			} else {
				// when hero is not moving set HeroState to 0 which is the Idle State
				this._animator.SetInteger ("HeroState", 0);
				this._move = 0f;
			}

			// check if input is present for jumping
			if (Input.GetKeyDown (KeyCode.Space)) {
				this._jump = 1f;
				this.JumpSound.Play ();
			}

			this._rigidbody.AddForce (new Vector2 (
				this._move * this.Velocity, 
				this._jump * this.JumpForce), 
				ForceMode2D.Force);

		} else {
			this._move = 0f;
			this._jump = 0f;
		}



		this.camera.transform.position = new Vector3 (
			this._transform.position.x, 
			this._transform.position.y, 
			-10f);

	}

	// PRIVATE METHODS
	/**
	 * This method initializes variables and object when called
	 */
	private void _initialize() {
		this._transform = GetComponent<Transform> ();
		this._rigidbody = GetComponent<Rigidbody2D> ();
		this._animator = GetComponent<Animator> ();
		this.camera = GameObject.FindWithTag ("MainCamera");
		this.SpawnPoint = GameObject.FindWithTag ("SpawnPoint");
		this._move = 0f;
		this._isFacingRight = true;
		this._isGrounded = false;
	}

	/**
	 * This method flips the character's bitmap across the x-axis
	 */
	private void _flip () {
		if (this._isFacingRight) {
			this._transform.localScale = new Vector2 (1f, 1f);
		} else {
			this._transform.localScale = new Vector2 (-1f, 1f);
		}
	}

	private void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.CompareTag ("DeathPlane")) {
			// move the player's position to the spawn point's position
			this._transform.position = this.SpawnPoint.transform.position;
			this.DeathSound.Play ();
		}
		if (other.gameObject.CompareTag ("Coin")) {
			//remove the coin
			Destroy(other.gameObject);
			this.CoinSound.Play ();
		}
		if (other.gameObject.CompareTag ("Enemy")) {
			//remove the coin
			Destroy(other.gameObject);
			this.HurtSound.Play ();
		}
	}

	private void OnCollisionStay2D(Collision2D other) {
		if (other.gameObject.CompareTag ("Platform")) {
			this._isGrounded = true;
		}
	}

	private void OnCollisionExit2D(Collision2D other) {
		this._animator.SetInteger ("HeroState", 2);
		this._isGrounded = false;
	}
}
