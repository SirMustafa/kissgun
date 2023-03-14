using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace MadLab.Editor{
    public class ML_SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<ML_SplitView, TwoPaneSplitView.UxmlTraits> { }
    }
}
