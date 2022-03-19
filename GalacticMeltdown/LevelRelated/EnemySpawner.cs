using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using Newtonsoft.Json;

namespace GalacticMeltdown.LevelRelated;

public class EnemySpawner
{
    private const int NoSpawnRadius = 1;

    [JsonProperty] private readonly Level _level;
    [JsonProperty] private double _currency;
    [JsonProperty] private double _currencyGain = 1;
    [JsonProperty] private double _nextCurrencyAmount;

    [JsonIgnore] public List<Chunk> TargetChunks;

    [JsonConstructor]
    private EnemySpawner()
    {
    }
    public EnemySpawner(Level level)
    {
        CalculateNextCurrencyAmount();
        _level = level;
        level.TurnFinished += NextTurn;
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext _)
    {
        _level.TurnFinished += NextTurn;
    }
    
    private void NextTurn(object _, EventArgs __)
    {
        _currency += _currencyGain;
        //_currencyGain += 1;
        if (_currency > _nextCurrencyAmount) SpawnRandomEnemies();
    }

    public void SpawnEnemiesInChunk(Chunk chunk)
    {
        Random rng = new Random(chunk.Seed);
        double currency = chunk.Difficulty * 20;
        currency *= (rng.NextDouble() + 0.5);
        var points = chunk.GetFloorTileCoords();
        foreach (var enemy in CalculateEnemies(ref currency, rng))
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