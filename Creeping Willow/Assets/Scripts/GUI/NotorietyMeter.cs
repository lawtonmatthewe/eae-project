using UnityEngine;
using System.Collections;

public class NotorietyMeter : MonoBehaviour
{
	public float notorietyMax = 75.0f;
	private float notoriety = 0.0f;

	private int axemanCount = 0;
	public Texture2D axemanHeadTexture;

	void Start()
	{
		RegisterListeners();
	}

	void OnDestroy()
	{
		UnregisterListeners();
	}

	protected void RegisterListeners()
	{
		//MessageCenter.Instance.RegisterListener( MessageType.NPCAlertLevel, HandleNPCAlertMessage );
		MessageCenter.Instance.RegisterListener (MessageType.NPCPanickedOffMap, HandleNPCPanickedMessage);
	}
	
	protected void UnregisterListeners()
	{
		//MessageCenter.Instance.UnregisterListener( MessageType.NPCAlertLevel, HandleNPCAlertMessage );
		MessageCenter.Instance.UnregisterListener( MessageType.NPCPanickedOffMap, HandleNPCPanickedMessage);
	}

	protected void HandleNPCPanickedMessage( Message message )
	{
		NPCPanickedOffMapMessage mess = message as NPCPanickedOffMapMessage;

		notoriety += 30.0f;
		
		if( notoriety > notorietyMax )
		{
			MessageCenter.Instance.Broadcast( new NotorietyMaxedMessage( mess.PanickedPosition ) );
			notoriety = notorietyMax;
			axemanCount++;
		}
	}

	/*
	protected void HandleNPCAlertMessage( Message message )
	{
		NPCAlertLevelMessage mess = message as NPCAlertLevelMessage;
		
		switch( mess.alertLevelType )
		{
		case AlertLevelType.Panic:
			notoriety += 30.0f;

			if( notoriety > notorietyMax )
			{
				MessageCenter.Instance.Broadcast( new NotorietyMaxedMessage( mess.NPC ) );
				notoriety = notorietyMax;
				axemanCount++;
			}
			break;
			
		default:
			break;
		}
	}
	*/
	
	void Update()
	{
	
	}

	void OnGUI ()
	{
		GUI.matrix = GlobalGameStateManager.PrepareMatrix();

		float width = 80;
		float height = 800;

		float top = GlobalGameStateManager.originalHeight / 2 - height / 2;
		float left = 80;

		float percentFull = notoriety / notorietyMax;

		GUI.Box( new Rect( left, top, width, height ), GUIContent.none );
		GUI.Box( new Rect( left, top + height * ( 1 - percentFull ), width, height * percentFull ), GUIContent.none );

		for( int i = 0; i < axemanCount; i++ )
		{
			GUI.DrawTexture( new Rect( left + width, top + height * i, 40, height / ( i + 1 ) ), axemanHeadTexture, ScaleMode.ScaleToFit );
		}

		GUI.matrix = Matrix4x4.identity;
	}
}
