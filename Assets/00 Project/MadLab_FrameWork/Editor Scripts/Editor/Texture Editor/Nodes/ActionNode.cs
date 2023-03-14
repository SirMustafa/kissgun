using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MadLab.Editor.TextureEditor
{
    public abstract class ActionNode : Node
    {
        [HideInInspector] public Node child;
    }
}
