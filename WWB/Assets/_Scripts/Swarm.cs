using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Swarm Manager to keep track of all RandomAI's that are currently attacking the player
 *
 * Author: Daniel Brunelle
 */
public class Swarm : MonoBehaviour
{
	List<RandomAI> swarm;

	void Start()
	{
		swarm = new List<RandomAI>();
	}

	/* Update is called once per frame
	 *
	 * Author: Daniel Brunelle
	 */
	void Update()
	{
	}

	/*Add AI to mob
	 *
	 * Author: Daniel Brunelle
	 */
	public void AddToSwarm(RandomAI ai)
	{
		if (!swarm.Contains(ai))
		{
			swarm.Add(ai);
		}
	}

	/*Remove AI from mob
	 *
	 * Author: Daniel Brunelle
	 */
	public void RemoveFromSwarm(RandomAI ai)
	{
		if (swarm.Contains(ai))
		{
			swarm.Remove(ai);
		}
	}

	/*Make all AI's in swarm retreat
	 *
	 * Author: Daniel Brunelle
	 */
	public void Retreat()
	{

		foreach (RandomAI ai in swarm)
		{
			ai.Retreat();
		}
	}
}
