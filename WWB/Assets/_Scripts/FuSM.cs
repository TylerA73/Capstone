using System.Collections;
using System.Collections.Generic;

/*
 *	Author: Tyler Arseneault
 *	Description: Abstract class for calculating Fuzzy State Machine curves
 */

public abstract class FuSM{

	protected const float EPS = 1e-6f;
	public string name;

	public FuSM(string name){
		this.name = name;
	}

	public override string ToString(){
		return "FuSMFunction: " + name;
	}

	public abstract float Eval(float value);
	
}
