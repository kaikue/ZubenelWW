using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBouncer : MonoBehaviour
{
	private const float EXPIRE_TIME = 2.0f;
	private const float SPEED = 15.0f;
	public const float bounceSpeed = 12.0f;

	private PlayerLand playerLand;
	private GameObject playerObj;
	private Rigidbody2D rb;

	private void Awake()
    {
		rb = GetComponent<Rigidbody2D>();
		
		StartCoroutine(Expire());
	}

	public void SetPlayer(PlayerLand playerLand, GameObject playerObj)
	{
		this.playerLand = playerLand;
		this.playerObj = playerObj;

		Vector2 playerVel = playerObj.GetComponent<Rigidbody2D>().velocity;
		float yVel = Mathf.Min(playerVel.y, 0) - SPEED;
		rb.velocity = new Vector2(0, yVel);
	}

	private void FixedUpdate()
	{
		float yVel = rb.velocity.y;
		float playerYVel = playerObj.GetComponent<Rigidbody2D>().velocity.y;
		if (yVel < 0 && playerYVel < yVel)
		{
			yVel = playerYVel;
			rb.velocity = new Vector2(rb.velocity.x, yVel);
		}
		Vector2 goalPos = new Vector2(playerObj.transform.position.x, rb.position.y + yVel * Time.fixedDeltaTime);
		rb.MovePosition(goalPos);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		GameObject collider = collision.collider.gameObject;
		if (collider.layer == LayerMask.NameToLayer("LevelGeometry"))
		{
			if (collision.contacts[0].normal.y < 0)
			{
				DestroySelf();
			}
			rb.velocity = new Vector2(rb.velocity.x, SPEED);
			gameObject.layer = LayerMask.NameToLayer("Default");
		}
	}

	private IEnumerator Expire()
	{
		yield return new WaitForSeconds(EXPIRE_TIME);
		DestroySelf();
	}

	private void DestroySelf()
	{
		playerLand.DestroySlime();
		Destroy(gameObject);
	}
}
