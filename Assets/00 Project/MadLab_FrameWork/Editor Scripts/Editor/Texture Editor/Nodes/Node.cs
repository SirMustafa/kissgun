using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MadLab.Editor.TextureEditor
{
    public abstract class Node : ScriptableObject
    {
        public enum State{
            Running,
            Failure,
            Success
        }

        [HideInInspector] public State state = State.Running;
        [HideInInspector] public bool started = false;
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;

        public State Update() {
            if (!started) {
                OnStart();
                started = true;
            }

            state = OnUpdate();
            
            if (state != State.Running) {
                OnStop();
                started = false;
            }

            return state;
        }


        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract State OnUpdate();
    }
}
