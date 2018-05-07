using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *	Author: Tyler Arseneault
 *	Dwscription: Class for calculating grade curve for FuSM
 */

public class Grade : FuSM{
	
	protected float x0, x1;

	//	Constructor for the Grade class
	public Grade (string name, float x0, float x1):base(name){
		this.x0 = x0;
		this.x1 = x1;
	}

	//	Evaluates the strength of the value
	public override float Eval(float value){
		if(value <= 0){
			return -1f;
		}else if(value <= x0){
			return 0f;
		}else if(value < x1){
			return (value-x0) / (x1-x0);
		}

		return 1f;
	}

	//	ToString for debugging purposes
	public override string ToString(){
		return "FuSM Function (Grade): " + name + " ~ [" + x0 + ":" + x1 + "]";
	}

}
