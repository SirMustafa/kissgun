using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class EventGraphView : GraphView
{   
    public EventGraphView()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        AddElement(CreateEntryNodePoint());
    }

    private Port GeneratePort(IntNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }

    private IntNode CreateEntryNodePoint()
    {
        var node = new IntNode
        {
            title = "Int",
        };

        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Out";
        node.outputContainer.Add(generatedPort);

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(100, 200, 100, 150));
        return node;
    }

    public IntNode CreateIntNode(string nodeName)
    {
        var node = new IntNode
        {
            title = nodeName,
        };

        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Out";
        node.outputContainer.Add(generatedPort);

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(100, 200, 100, 150));
        return node;
    }
}
