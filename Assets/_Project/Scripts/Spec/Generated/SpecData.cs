using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpecData
{
    public List<SpecContentsOption> SpecContentsOption;
    public List<SpecBubble> SpecBubble;
    public List<SpecBubbleAttribute> SpecBubbleAttribute;
    public List<SpecBubbleGenerator> SpecBubbleGenerator;
    public List<SpecMap> SpecMap;
    public List<SpecMapConfig> SpecMapConfig;
    public List<SpecMapShootBubble> SpecMapShootBubble;
}
