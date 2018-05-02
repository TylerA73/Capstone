using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

/*
 * Author: Tyler Arseneault
 * Date: March 20, 2018
 * Description: Used to initialize the state of a level using an XML file
 */
public class LevelInit : MonoBehaviour {

	public TextAsset xml;
	public string levelName { get; set; }
	
	void Start () {
		string data = xml.text;
		ParseXml(data); // Parse the data within the xml file
	}
	
	/*
	 *	ParseXml
	 *	Param: string data
	 *	Returns: N/A
	 *	Parses the xml file containing the statedata of the level
	 */
	void ParseXml(string data){
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(data); // Load the xml data into the xmldoc

		// Set the name of the level
		foreach(XmlElement element in xmlDoc.SelectNodes("Level")){
			levelName = element.GetAttribute("Name");
		}

		// Set the treasure boxes found throughout the level in their rightful places
		// If the box is already opened, do something to set it visually
		foreach(XmlElement element in xmlDoc.SelectNodes("Level/Treasure")){
			bool isOpen = bool.Parse(element.GetAttribute("Open"));
			float x = float.Parse(element.GetAttribute("lx"));
			float y = float.Parse(element.GetAttribute("ly"));
			float z = float.Parse(element.GetAttribute("lz"));

			// Place the treasure box to its x, y, and z coordinates
			// NOTE: In the XML file there will be a rotation parameter that needs to be parsed as well
			// when an actual treasure box asset is used
			// Cubes are a temporary placeholder for the treasure box
			// If the box is unopened, make it green
			// Else, if it is opened, make it red
			GameObject treasure = GameObject.CreatePrimitive(PrimitiveType.Cube);
			treasure.transform.position = new Vector3(x, y, z);
			if(isOpen){
				treasure.GetComponent<Renderer>().material.color = Color.red;
			}else{
				treasure.GetComponent<Renderer>().material.color = Color.green;
			}
		}
	}
}
