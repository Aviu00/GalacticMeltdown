using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.LevelRelated;

public class EnemySpawner
{
    private const int NoSpawnRadius = 1;
    private const int DifficultyMultiplier = 25;
    private const int CurrencyIncreaseTime = 60;
    private const int CurrencyGainIncreaseAmount = 5;

    [JsonProperty] private readonly Level _level;
    [JsonProperty] private double _currency;
    [JsonProperty] private double _currencyGain = 1;
    [JsonProperty] private double _nextCurrencyAmount;

    [JsonIgnore] public List<Chunk> TargetChunks;
    [JsonProperty] private Counter _currencyCounter;

    [JsonConstructor]
    private EnemySpawner()
    {
    }
    public EnemySpawner(Level level)
    {
        CalculateNextCurrencyAmount();
        _level = level;
        Init();
    }

    private void Init()
    {
        _currencyCounter = new Counter(_level, CurrencyIncreaseTime, CurrencyIncreaseTime, counter =>
        {
            _currency += _currencyGain;
            _currencyGain += CurrencyGainIncreaseAmount;
            if (_currency > _nextCurrencyAmount) SpawnRandomEnemies();
            counter.ResetTimer();
        });
    }
    
    [OnDeserialized]
    private void OnDeserialized(StreamingContext _)
    {
        Init();
    }

    public void SpawnEnemiesInChunk(Chunk chunk)
    {
        Random rng = new Random(chunk.Seed);
        double currency = chunk.Difficulty * DifficultyMultiplier;
        currency *= rng.NextDouble() + 0.5;
        var points = chunk.GetFloorTileCoords();
        if(points is null || points.Count == 0)
            return;
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
        List<(int, int)> points = TargetChunks[Random.Shared.Next(0, TargetChunks.Count)].GetFloorTileCoords();
        if(points.Count == 0) return;
        var enemies= CalculateEnemies(ref _currency);
        foreach (var enemy in enemies)
        {
            (int x, int y)? point;
            do
            {
                point = points[Random.Shared.Next(0, points.Count)];
                points.Remove((point.Value.x, point.Value.y));
                if (_level.GetNonTileObject(point.Value.x, point.Value.y) is not null)
                    point = null;
            } while (points.Count > 0 && point is null);

            if (point is null)
            {
                _currency += enemy.Cost;
                continue;
            }
            SpawnEnemy(enemy, point.Value.x, point.Value.y);
        }
        CalculateNextCurrencyAmount();
    }

    private void CalculateNextCurrencyAmount()
    {
        _nextCurrencyAmount = _currencyGain * Random.Shared.Next(3, 16) + _currency;
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
            currency -= list[^1].Cost;
            curr = currency;
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