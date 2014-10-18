using UnityEngine;
using System.Collections;

public class GameSettings
{
	#region Player Settings
	public static float PLAYER_RATE_OF_FIRE = 0.2f; //In seconds.
	public static int PLAYER_BULLET_POOL_SIZE = 40;
	public static float PLAYER_SCALE_FACTOR = 1.0015f; //1.0025f
	public static float GUN_SEPARATION = 0.3f;
	public static float SHIELD_RECHARGE_TIME = 6.0f; //In seconds.
	public static int SUPER_SHOT_RATIO = 4; //Per multiples of scale.
	#endregion

	#region Enemy Settings
	public static float SHOTGUN_CHANCE = 0.5f;

	public static float SHOTGUN_SPREAD = 3.0f;
	public static int NUM_IN_PULSE = 9; //In degrees.
	public static float PULSE_SPREAD = 110.0f; //In degrees.

	public static int ENEMY_BULLET_POOL_SIZE = 100;
	public static float ENEMY_RATE_OF_SPAWN_MIN = 0.5f; //In seconds.
	public static float ENEMY_RATE_OF_SPAWN_MAX = 1.0f; //In seconds.
	public static float ENEMY_SPEED_MIN = 2.0f;
	public static float ENEMY_SPEED_MAX = 3.0f;
	#endregion
}
