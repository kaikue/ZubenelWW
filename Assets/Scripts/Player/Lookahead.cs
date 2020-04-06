using UnityEngine;

public class Lookahead : MonoBehaviour
{
    public Rigidbody2D playerRB;
    public int lookaheadAmount;

    private float dx;
    private float lastPlayerX;
    
    private void FixedUpdate()
    {
        float playerX = playerRB.position.x;
        float playerDX = playerX - lastPlayerX; //use instead of playerRB.velocity.x in case player is running into a wall
        dx = Mathf.Lerp(dx, lookaheadAmount * playerDX, 0.1f);
        transform.position = playerRB.position + new Vector2(dx, 0);
        lastPlayerX = playerX;
    }
}
