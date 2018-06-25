using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hero : MonoBehaviour {
    static public Hero		S;

    [Header("Set in inspector")]
	public float	speed = 30;
	public float	rollMult = -45;
	public float  	pitchMult=30;
	public float gameRestartDelay = 2f;

    [Header("Set dynamically")]
	[SerializeField]
	private float	_shieldLevel=1;
	// This variable holds a reference to the last triggering GameObject
	private GameObject lastTriggerGo = null; // a

    private void Awake() {
        if (S == null) {
            S = this;
        } else {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }
    }

	// Update is called once per frame
	void Update () {
		float xAxis = Input.GetAxis("Horizontal");
		float yAxis = Input.GetAxis("Vertical");

		Vector3 pos = transform.position;
		pos.x += xAxis * speed * Time.deltaTime;
		pos.y += yAxis * speed * Time.deltaTime;
		transform.position = pos;
		
		// rotate the ship to make it feel more dynamic
		transform.rotation = Quaternion.Euler(yAxis*pitchMult, xAxis*rollMult, 0);
	}

	void OnTriggerEnter(Collider other) {
		Transform rootT = other.gameObject.transform.root;
		GameObject go = rootT.gameObject;
		//print("Triggered: "+go.name); // b

		// Make sure it's not the same triggering go as last time
		if (go == lastTriggerGo) { // c
			return;
		}
		lastTriggerGo = go; // d

		if (go.tag == "Enemy") { // If the shield was triggered by an enemy
			shieldLevel--; // Decrease the level of the shield by 1
			Destroy(go); // … and Destroy the enemy // e
		} else {
			print( "Triggered by non-Enemy: "+go.name); // f
		}
	}

	public float shieldLevel {
		get {
			return(_shieldLevel); // a
		}
		set {
			_shieldLevel = Mathf.Min (value, 4); // b
			// If the shield is going to be set to less than zero
			if (value < 0) { // c
				Destroy (this.gameObject);
				// Tell Main.S to restart the game after a delay
				Main.S.DelayedRestart( gameRestartDelay ); // a
			}
		}
	}
}