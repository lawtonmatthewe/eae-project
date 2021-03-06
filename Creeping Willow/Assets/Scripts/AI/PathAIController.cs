using UnityEngine;	
using System.Collections;

public class PathAIController : AIController 
{
	private string walkingKey = "direction";

	private enum WalkingDirection
	{
		STILL = 0,
		MOVING = 1,
	}

	new void Start()
	{
		base.Start ();

		// Get path for AI
		nextPath = movePath.getNextPath (null, gameObject);
	}

	public void setMovingPath(SubpathScript movePath)
	{
		this.movePath = movePath;
	}

	override protected GameObject getNextPath()
	{
		return movePath.getNextPath(null, gameObject);
	}
	
	// Update is called once per frame
	protected override void GameUpdate () 
	{
		if (updateNPC())
			return;

		// if lure is deleted
		if( nextPath == null ) return;

		Vector3 pathPosition = nextPath.transform.position;
		Vector3 position = transform.position;
		float step = speed * Time.deltaTime;
		Vector3 movement = Vector3.MoveTowards (position, pathPosition, step);
		Vector3 direction = Vector3.Normalize(movement - transform.position);

		Vector3 biasPosition = new Vector3 (transform.position.x - movement.x, transform.position.y - movement.y);
		
		if (biasPosition.x == 0)
		{
			setAnimatorInteger(walkingKey, (int)WalkingDirection.STILL);
		}
		else
		{
			setAnimatorInteger(walkingKey, (int)WalkingDirection.MOVING);
		}

		Vector3 changeMovement = avoid (direction);

		if( changeMovement != Vector3.zero )
		{	
			Vector3 newPos = Vector3.MoveTowards(transform.position,changeMovement,step);
			determineDirectionChange(transform.position, newPos);
			transform.position = newPos;
		}
		else
		{
			determineDirectionChange(transform.position, movement);
			transform.position = movement;
		}

		if (movement == pathPosition && !lured)
		{
			if (killSelf && nextPath.gameObject.tag.Equals("Respawn"))
				destroyNPC();
			
			int max = 10;
			int rand = Random.Range (0, max);
			if (rand < max - 1)
				nextPath = movePath.getNextPath(nextPath, gameObject);
			else
			{
				killSelf = true;
				nextPath = getLeavingPath();
			}
		}
	}
}
