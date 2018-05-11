using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLabel : MonoBehaviour {

	public TextMesh label;

	public void labelText(string text){
		label.text = text;
	}
}
