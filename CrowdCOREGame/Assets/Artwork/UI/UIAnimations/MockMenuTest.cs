using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockMenuTest : MonoBehaviour {

	public Animator splashAnim;
	public Animator textAnim;



	void Update()
	{
		if(Input.GetKeyDown("space"))
		{
			this.gameObject.GetComponent<Animator>().SetTrigger("Switch");
		}
	}

	public void SetOff()
	{
		splashAnim.SetTrigger("OffScreen");
		textAnim.SetBool("OffScreen", true);
	}

	public void SetTitleOn()
	{
		splashAnim.SetTrigger("Title");
		textAnim.SetBool("OffScreen", false);
	}

	public void SetControlsOn()
	{
		splashAnim.SetTrigger("Controls");
	}
}
