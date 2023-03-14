using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

public class EventGraph : EditorWindow
{
    private EventGraphView graphView;

    [MenuItem("ML Framework/Event Graph", false, 15)]
    public static void OpenWindow()
    {
        var window = GetWindow<EventGraph>("ML Event Graph"); 
    }

    private void OnEnable() {
        ConstructGraphView();
    }

    private void ConstructGraphView()
    {
        graphView = new EventGraphView{
            name = "Event Graph"
        };
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView); 
    }

    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();

        var nodeCreateButton = new Button(() => {
            graphView.CreateIntNode("Int");
        });

        nodeCreateButton.text = "Create Int";
        toolbar.Add(nodeCreateButton);

        rootVisualElement.Add(toolbar);
    }

    private void OnDisable() {
        rootVisualElement.Remove(graphView);
    }
}
