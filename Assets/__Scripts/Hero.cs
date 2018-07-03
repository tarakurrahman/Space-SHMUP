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
	public GameObject projectilePrefab;
	public float projectileSpeed = 40;
    public Weapon[] weapons; // a

    [Header("Set dynamically")]
	[SerializeField]
	private float	_shieldLevel=1;
	// This variable holds a reference to the last triggering GameObject
	private GameObject lastTriggerGo = null; // a

    // Declare a new delegate type WeaponFireDelegate
    public delegate void WeaponFireDelegate(); // a
    // Create a WeaponFireDelegate field named fireDelegate.
    public WeaponFireDelegate fireDelegate;

    void Start() {
            S = this;
        // fireDelegate += TempFire; // b

        // Reset the weapons to start _Hero with 1 blaster
        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);
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

        // Allow the ship to fire
        // if ( Input.GetKeyDown(KeyCode.Space) ) { // c
        // TempFire(); // c
        // } // c
        // Use the fireDelegate to fire Weapons
        // First, make sure the button is pressed: Axis("Jump")
        // Then ensure that fireDelegate isn't null to avoid an error
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null) { // d
            fireDelegate(); // e
        }
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

		if (go.tag == "Enemy") {
            // If the shield was triggered by an enemy
            // Decrease the level of the shield by 1
            shieldLevel--;
            // Destroy the enemy
            Destroy(go); 
		} else if (go.tag == "PowerUp") {
            // If the shield was triggered by a PowerUp
            AbsorbPowerUp(go);
        } else {
			print( "Triggered by non-Enemy: "+ go.name); // f
		}
	}

    public void AbsorbPowerUp(GameObject go) {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type) {
            case WeaponType.shield: // a
                shieldLevel++;
                break;

            default: // b
                if (pu.type == weapons[0].type) { // If it is the same type // c
                    Weapon w = GetEmptyWeaponSlot();
                    if (w != null) {
                        // Set it to pu.type
                        w.SetType(pu.type);
                    }
                } else { // If this is a different weapon type // d
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        }
        pu.AbsorbedBy(this.gameObject);
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
    Weapon GetEmptyWeaponSlot() {
        for (int i = 0; i < weapons.Length; i++) {
            if (weapons[i].type == WeaponType.none) {
                return (weapons[i]);
            }
        }
        return (null);
    }

    void ClearWeapons() {
        foreach (Weapon w in weapons) {
            w.SetType(WeaponType.none);
        }
    }
}