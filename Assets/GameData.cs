using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game Data1")]
public class GameData : ScriptableObject
{
    public List<Vector3> playerPositions = new List<Vector3>();

    Dictionary<PlayerHealth, Vector3> playerStartPositions = new Dictionary<PlayerHealth, Vector3>();
    Dictionary<EnemyHealth, Vector3> enemies = new Dictionary<EnemyHealth, Vector3>();

    public void SetPlayerStartPos(PlayerHealth player, Vector3 startPos)
    {
        if (!playerStartPositions.ContainsKey(player))
            playerStartPositions.Add(player, startPos);
        else
            playerStartPositions[player] = startPos;
    }

    public Vector3 GetPlayerStartPos(PlayerHealth player) => playerStartPositions.ContainsKey(player) ? playerStartPositions[player] : Vector3.zero;

    public void SetEnemyPos(EnemyHealth enemy, Vector3 lastPosition)
    {
        if (!enemies.ContainsKey(enemy))
            enemies.Add(enemy, lastPosition);
        else
            enemies[enemy] = lastPosition;
    }

    public Vector3 GetEnemyPos(EnemyHealth enemy)
    {
        Vector3 pos = default;
        if (enemies.ContainsKey(enemy))
            return enemies[enemy];
        
        return pos;
    }

    public void ClearEnemyData()
    {
        enemies.Clear();
    }
}
