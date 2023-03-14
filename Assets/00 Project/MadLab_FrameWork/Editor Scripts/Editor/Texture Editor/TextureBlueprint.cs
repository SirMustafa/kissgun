using System.Collections.Generic;
using UnityEditor;
using UnityEngine;




namespace MadLab.Editor.TextureEditor
{
    [CreateAssetMenu] 
    public class TextureBlueprint : ScriptableObject
    {
        public Node rootNode;
        public Node.State blueprintState = Node.State.Running;

        public List<Node> nodes = new List<Node>();

        public Node.State Update()
        {
            if (rootNode.state == Node.State.Running)
            {
                blueprintState = rootNode.Update();
            }

            return blueprintState;
        }

        public void SetupRoot()
        {
            rootNode = CreateNode(typeof(RootNode)) as RootNode;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        public Node CreateNode(System.Type type)
        {
            Node node = ScriptableObject.CreateInstance(type) as Node;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();
            nodes.Add(node);

            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();
            
            return node;
        }

        public void DeleteNode(Node node)
        {
            if (node.GetType() == typeof(RootNode))
                rootNode = null;

            nodes.Remove(node);
    
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(Node parent, Node child)
        {
            RootNode rootNode = parent as RootNode;
            if (rootNode)
            {
                rootNode.child = child;
            }
            
            ActionNode actionNode = parent as ActionNode;
            if (actionNode)
            {
                actionNode.child = child;
            }
        }

        public void RemoveChild(Node parent, Node child)
        {
            RootNode rootNode = parent as RootNode;
            if (rootNode)
            {
                rootNode.child = null;
            }
            
            ActionNode actionNode = parent as ActionNode;
            if (actionNode)
            {
                actionNode.child = null;
            }
        }

        public List<Node> GetChildren(Node parent)
        {
            List<Node> children = new List<Node>();
            
            RootNode rootNode = parent as RootNode;
            if (rootNode && rootNode.child != null)
            {
                children.Add(rootNode.child);
            }

            ActionNode actionNode = parent as ActionNode;
            if (actionNode && actionNode.child != null)
            {
                children.Add(actionNode.child);
            }

            return children;
        }
    }
}
