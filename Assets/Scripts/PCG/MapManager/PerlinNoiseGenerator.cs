using UnityEngine;
using System;
using System.Collections.Generic;

public class PerlinNoiseGenerator
{
    [Header("噪声参数")]
    public float noiseScale = 0.1f;          // 噪声缩放（值越小越平滑）
    [Range(0f, 1f)] public float threshold = 0.2f; // 生成阈值
    public int octaves = 3;                   // 噪声八度
    public float persistence = 0.5f;          // 持久度
    public float lacunarity = 2f;             // 间隙度

    [Header("房屋设置")]
    [Range(0.1f, 2f)] public float placementDensity = 1f; // 放置密度

    private List<Vector3> noisePositions = new List<Vector3>();


    // 生成3D柏林噪声地图
    public List<Vector3> GenerateNoiseMap(Vector3Int mapSize, Vector3 startPosition)
    {
        // 计算采样步长（基于密度）
        int step = Mathf.Max(1, Mathf.RoundToInt(1f / placementDensity));
        
        for (int x = 0; x < mapSize.x; x += step)
        {
            for (int z = 0; z < mapSize.z; z += step)
            {
                // 计算世界坐标
                float worldX = startPosition.x + x;
                float worldZ = startPosition.z + z;
                
                // 获取噪声值（0-1范围）
                float noiseValue = CalculateNoise(worldX, worldZ);
                
                // 如果噪声值超过阈值，记录位置
                if (noiseValue > threshold)
                {
                    // 计算Y坐标（基于噪声值在最小最大高度之间）
                    float height = Mathf.Lerp(0, mapSize.y, noiseValue);
                    Vector3 position = new Vector3(worldX, startPosition.y + height, worldZ);
                    noisePositions.Add(position);
                }
            }
        }

        return noisePositions;
    }
    
    // 计算多层柏林噪声
    private float CalculateNoise(float x, float z)
    {
        float value = 0f;
        float frequency = noiseScale;
        float amplitude = 1f;
        float maxValue = 0f; // 用于归一化

        for (int i = 0; i < octaves; i++)
        {
            // 使用Unity的2D Perlin噪声（x和z作为平面坐标）
            float sampleX = x * frequency;
            float sampleZ = z * frequency;
            
            // 获取2D Perlin噪声值
            float noise = Mathf.PerlinNoise(sampleX, sampleZ);
            
            value += noise * amplitude;
            maxValue += amplitude;
            
            // 更新参数
            amplitude *= persistence;
            frequency *= lacunarity;
        }

        // 归一化到0-1范围
        return value / maxValue;
    }
}
