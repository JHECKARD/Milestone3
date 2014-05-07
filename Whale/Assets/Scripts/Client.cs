using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

using UnityEngine;
using System.Collections;

public class Client : MonoBehaviour 
{
	public GameManager manager;
	public FakeServerInputs server;
	public string message;
	
	static TcpClient client;
	NetworkStream stream;
	const String serverIP = "2";
	
	private Thread clientThread;
	StreamReader playerReader;
	StreamWriter playerWriter;
	
	// Use this for initialization
	void Start () 
	{
		manager = GameObject.Find("GameManager").GetComponent<GameManager>();
		server = GameObject.Find ("FakeServer").GetComponent<FakeServerInputs>();
		message = "";
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	//sends move request to "server" from gameManager
	public void requestMove(string inputMove)
	{
		//sends the movement change command to server
		 // Translate the passed message into ASCII and store it as a Byte array.
    		Byte[] data = System.Text.Encoding.ASCII.GetBytes(inputMove);

    		// Send the message to the connected TcpServer. 
		    stream.Write(data, 0, data.Length);

    		Console.WriteLine("Sent: ", inputMove);
	}
	
	//gets move data from server and sends it to gameManager
	public void doMove(string newMove)
	{
		//sends velocity change comand to gameManager
			manager.serverCommand.Enqueue(newMove);
			manager.move = true;
			
			Byte[] data = new Byte[256];
    		
		// String to store the response ASCII representation.
    		String responseData = String.Empty;

    		// Read the first batch of the TcpServer response bytes.
    		Int32 bytes = stream.Read(data, 0, data.Length);
    		responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
    		Console.WriteLine("Received: ", responseData);
		manager.serverCommand.Enqueue(responseData);
	}
	
	public void Connect(String server, string userName, string pasword) 
	{
  		try 
  		{
    		// Create a TcpClient.
    		Int32 port = 8008;
    		client = new TcpClient(server, port);
			
			
			//sends pre-encrypted username and password to server.
			//if the Username and pasword do not match, the server will send a disconect command back
    		Byte[] data = System.Text.Encoding.ASCII.GetBytes("1$"+userName+"$"+
															  Encryptor.encryptString("elephant")+
															  "&"+pasword+"$");         

    		// Get a client stream for reading and writing. 
		    stream = client.GetStream();

    		// Send the message to the connected TcpServer. 
		    stream.Write(data, 0, data.Length);
  		} 
  		catch (ArgumentNullException e) 
  		{
    		Console.WriteLine("ArgumentNullException: {0}", e);
  		} 
  		catch (SocketException e) 
  		{
    		Console.WriteLine("SocketException: {0}", e);
  		}
	}
	
	public void Disconnect()
	{
		stream.Close();
		client.Close ();
	}
	
	
	
}
