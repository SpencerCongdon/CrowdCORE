using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdAnim : MonoBehaviour {

	public MeshRenderer armsDownMesh;
	public MeshRenderer armsUpMesh;

	public Material[] ShirtMats;
	public Material[] SkinMats;


	void Awake () 
	{
		armsUpMesh.enabled = false;

		int shirtPick = Random.Range(0, ShirtMats.Length);
		int skinPick = Random.Range(0, SkinMats.Length);

		// skin = 0 shit = 1
		Material[] currentMats1 = armsDownMesh.materials;
		currentMats1[1] = ShirtMats[shirtPick];
		currentMats1[0] = SkinMats[skinPick];
		armsDownMesh.materials = currentMats1;

		Material[] currentMats2 = armsUpMesh.materials;
		currentMats2[1] = ShirtMats[shirtPick];
		currentMats2[0] = SkinMats[skinPick];
		armsUpMesh.materials = currentMats2;

	}

	void OnTriggerEnter(Collider coll)
	{
		if(coll.tag == "handsChecker")
		{
			armsUpMesh.enabled = true;
			armsDownMesh.enabled = false;
		}

	}

	void OnTriggerExit(Collider coll)
	{
		if(coll.tag == "handsChecker")
		{
			armsUpMesh.enabled = false;
			armsDownMesh.enabled = true;
		}

	}
}
