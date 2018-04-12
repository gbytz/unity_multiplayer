using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAdd : MonoBehaviour {

	public HostSetup hostSetup;
	public string ID;

	public void addSelf(){
		hostSetup.AddSelf (ID, Vector3.zero, Quaternion.identity);
	}
}
