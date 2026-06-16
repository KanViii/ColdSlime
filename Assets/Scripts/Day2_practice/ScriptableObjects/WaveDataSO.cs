using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewWaveData", menuName = "Scriptable Objects/Wave Data")]
public class WaveDataSO : ScriptableObject
{
    public List<UnitStatsSO> enemiesToSpawn;
    public float minspawnInterval = 2f;    
    public float maxspawInterval = 5f;    
}