using UnityEngine;	
using System.Collections;

public class EnemyAIController : AIController 
{
	public float sittingTime;
	protected Vector3 panickedNPCPosition;
	private bool sitting = false;
	private float leaveTime;
	private bool calledToPoint = false;

	private float nextInvestigateTime = 0;

	private static string axeManWalkingKey = "direction";

	private enum AxeManWalkingDirection
	{
		STILL = 0,
		WALK = 1,
	}

	new public void Start()
	{
		base.Start ();
		xScale = -xScale;
	}

	protected override void GameUpdate () 
	{
		if (updateNPC ())
			return;	
		
		// if lure is deleted
		if( nextPath == null ) return;

		if (sitting)
		{
			/*
			if (movement == pathPosition)
				setAnimatorInteger(axeManWalkingKey, (int)AxeManWalkingDirection.WALK);
			else
				setAnimatorInteger(axeManWalkingKey, (int)AxeManWalkingDirection.STILL);
			*/
			if (leaveTime <= Time.time)
			{
				sitting = false;
				killSelf = true;
				if (calledToPoint)
				{
					calledToPoint = false;
					Destroy(nextPath);
				}
				nextPath = getLeavingPath();
				this.GetComponent<BoxCollider2D>().isTrigger = false;
			}
			
			investigate();
		}

		Vector3 pathPosition = nextPath.transform.position;
		Vector3 positionNPC = transform.position;
		float step = speed * Time.deltaTime;

		Vector3 movement = Vector3.MoveTowards (positionNPC, pathPosition, step);

		animateCharacter(movement);
		
		transform.position = movement;

		if (movement == pathPosition && (nextPath.transform.position.Equals(panickedNPCPosition) || killSelf))
		{
			if (killSelf && !nextPath.transform.position.Equals(panickedNPCPosition))
				Destroy(gameObject);
			
			if (!sitting)
			{
				sitting = true;
				leaveTime = Time.time + sittingTime;
			}			
		}
	}

	private void animateCharacter(Vector3 movement)
	{
		determineDirectionChange (transform.position, movement);
		Vector3 biasPosition = new Vector3 (transform.position.x - movement.x, transform.position.y - movement.y);
		if (biasPosition.x == 0 && biasPosition.y == 0)
		{
			//no movement
			//flipXScale(!lastDirectionWasRight);
			setAnimatorInteger(axeManWalkingKey, (int)AxeManWalkingDirection.STILL);
		}
		else
			setAnimatorInteger(axeManWalkingKey, (int)AxeManWalkingDirection.WALK);
	}

	private void investigate()
	{
		if (nextInvestigateTime <= Time.time)
		{
			setAnimatorInteger(axeManWalkingKey, (int)AxeManWalkingDirection.WALK);
			nextInvestigateTime = Time.time + sittingTime/4;
			Vector2 position = Random.insideUnitCircle;
			nextPath.transform.position = new Vector3(position.x, position.y, 0.0f) + panickedNPCPosition;
		}
	}

	public void setStationaryPoint(GameObject panickedNPC)
	{
		panickedNPCPosition = panickedNPC.transform.position;
		panickedNPCPosition = new Vector3 (panickedNPCPosition.x, panickedNPCPosition.y);
		calledToPoint = true;
		nextPath = new GameObject ();
		nextPath.transform.position = panickedNPCPosition;
	}

	protected override void alert()
	{
		base.alert ();
		//setAnimatorInteger (axeManWalkingKey, (int)AxeManWalkingDirection.STILL);
	}
	
	new protected Vector3 getNextPath()
	{
		return panickedNPCPosition;
	}

	override protected void panic()
	{
		base.panic ();
		this.GetComponent<BoxCollider2D>().isTrigger = false;
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		// TODO: It's axe time
	}
}