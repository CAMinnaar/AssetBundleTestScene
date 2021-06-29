using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Config
{
    public int[] target_sizes;
    public int bit_depth;
    public string format;
}

[Serializable]
public class ConfigData
{
    public Config config;
}
