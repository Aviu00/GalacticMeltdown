using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;

namespace GalacticMeltdown.LevelRelated;

public class EnemySpawner
{
    private const int NoSpawnRadius = 1;

    private readonly Level _level;
    
    private double _currency;
    private double _currencyGain = 1;
    private double _nextCurrencyAmount;

    public List<Chunk> TargetChunks;

    public EnemySpawner(Level level)
    {
        _level = level;
        CalculateNextCurrencyAmount();
        level.TurnFinished += NextTurn;
    }

    void NextTurn(object sender, EventArgs _)
    {
        _currency += _currencyGain;
        //_currencyGain += 1;
        if (_currency > _nextCurrencyAmount) SpawnRandomEnemies();
    }

    public void SpawnEnemiesInChunk(Chunk chunk)
    {
        Random rng = chunk.Rng;
        double currency = chunk.Difficulty * 20;
        currency *= (rng.NextDouble() + 0.5);
        var points = chunk.GetFloorTileCoords();
        foreach (var enemy in CalculateEnemies(ref currency, chunk.Rng))
        {
            var (x, y) = points[rng.Next(0, points.Count)];
            SpawnEnemy(enemy, x, y);
            points.Remove((x, y));
            if (points.Count == 0) break;
        }
    }

    private HashSet<Chunk> GetTargetChunks()
    {
        HashSet<Chunk> chunks = _level.GetChunksAroundControllable(DataHolder.ActiveChunkRadius, false);
        foreach (var chunk in _level.GetChunksAroundControllable(NoSpawnRadius, false))
        {
            chunks.Remove(chunk);
        }
        return chunks;
    }
    
    private void SpawnRandomEnemies()
    {
        TargetChunks ??= GetTargetChunks().ToList();
        if(TargetChunks.Count == 0) return;
        var enemies= CalculateEnemies(ref _currency);
        List<int> prevChunkIndices = new();
        List<(int, int)> points = new();
        for (int i = 0; i < enemies.Count; i++)
        {
            int index = Random.Shared.Next(0, TargetChunks.Count);
            if(prevChunkIndices.Contains(index)) continue;
            prevChunkIndices.Add(index);
            points.AddRange(TargetChunks[index].GetFloorTileCoords(false));
        }
        foreach (var enemy in enemies)
        {
            (int x, int y) = points[Random.Shared.Next(0, points.Count)];
            points.Remove((x, y));
            SpawnEnemy(enemy, x, y);
        }
        CalculateNextCurrencyAmount();
    }

    private void CalculateNextCurrencyAmount()
    {
        _nextCurrencyAmount = _currencyGain * Random.Shared.Next(5, 11);
    }

    private List<EnemyTypeData> CalculateEnemies(ref double currency, Random rng = null)
    {
        rng ??= Random.Shared;
        List<EnemyTypeData> list = new();
        List<EnemyTypeData> enemies = DataHolder.EnemyTypes.Values.ToList();
        var curr = currency;
        enemies = enemies.Where(enemy => enemy.Cost <= curr).ToList();
        while (enemies.Count > 0)
        {
            list.Add(enemies[rng.Next(0, enemies.Count)]);
            curr -= list[^1].Cost;
            currency = curr;
            enemies = enemies.Where(enemy => enemy.Cost <= curr).ToList();
        }
        return list;
    }

    private void SpawnEnemy(EnemyTypeData enemyData, int x, int y)
    {
        Enemy enemy = new Enemy(enemyData, x, y, _level);
        _level.AddNpc(enemy);
    }
}