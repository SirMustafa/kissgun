using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MadLab.Editor.TextureEditor;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using Node = MadLab.Editor.TextureEditor.Node;

namespace MadLab.Editor
{
    public class ML_GraphView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<ML_GraphView, GraphView.UxmlTraits> { }

        public Action<NodeView> OnNodeSelected;
        
        private TextureBlueprint blueprint;

        
        public ML_GraphView(){

            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/00 Project/MadLab_FrameWork/Editor Scripts/Editor/Texture Editor/ML_TextureEditor.uss");
            styleSheets.Add(styleSheet);
        }

        NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.guid) as NodeView;
        }
        
        internal void PopulateView(TextureBlueprint blueprint)
        {
            this.blueprint = blueprint;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;
            
            if (blueprint.rootNode == null)
                blueprint.SetupRoot();
            
            // Create node views
            blueprint.nodes.ForEach(n => CreateNodeView(n));
            
            // Create edges
            blueprint.nodes.ForEach(n =>
            {
                var children = blueprint.GetChildren(n);
                children.ForEach(c =>
                {
                    NodeView parentView = FindNodeView(n);
                    NodeView childView = FindNodeView(c);

                    Edge edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);
                });
            });
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort => 
                endPort.direction != startPort.direction &&
                endPort.node != startPort.node).ToList();
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphviewchange)
        {
            if (graphviewchange.elementsToRemove != null)
            {
                graphviewchange.elementsToRemove.ForEach(elem =>
                {
                    NodeView nodeView = elem as NodeView;
                    if (nodeView != null)
                    {
                        blueprint.DeleteNode(nodeView.node);
                    }

                    Edge edge = elem as Edge;
                    if (edge != null)
                    {
                        NodeView parentView = edge.output.node as NodeView;
                        NodeView childView = edge.input.node as NodeView;
                        blueprint.RemoveChild(parentView.node, childView.node);
                    }
                });
            }

            if (graphviewchange.edgesToCreate != null)
            {
                graphviewchange.edgesToCreate.ForEach(edge =>
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    blueprint.AddChild(parentView.node, childView.node);
                });
            }
            return graphviewchange;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            {
                var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
                foreach (var type in types)
                {
                    evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
                }
            }
        }

        void CreateNode(System.Type type)
        {
            Node node = blueprint.CreateNode(type);
            CreateNodeView(node);
        }

        void CreateNodeView(Node node)
        {
            NodeView nodeView = new NodeView(node);
            nodeView.OnNodeSelected = OnNodeSelected;
            AddElement(nodeView);
        }
    }
}