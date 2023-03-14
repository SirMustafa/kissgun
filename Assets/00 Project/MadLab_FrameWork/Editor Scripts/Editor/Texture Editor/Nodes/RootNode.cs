using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  MadLab.Editor.TextureEditor
{
    public class RootNode : Node
    {
        [HideInInspector] public Node child;
        public Texture texture;
        protected override void OnStart()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnStop()
        {
            throw new System.NotImplementedException();
        }

        protected override State OnUpdate()
        {
            return child.Update();
        }
    }
}
