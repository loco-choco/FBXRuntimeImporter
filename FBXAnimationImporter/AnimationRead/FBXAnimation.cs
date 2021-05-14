using System;
using System.Collections.Generic;
using System.Text;

namespace FBXRuntimeImporter.AnimationRead
{
    public class FBXAnimationNodes
    {
        public FBXRecordNode AnimationStack;
        public FBXRecordNode AnimationLayer;
        public List<AnimationNode> AnimationNodes;

        public FBXAnimationNodes(FBXRecordNode AnimationStack, FBXRecordNode AnimationLayer)
        {
            this.AnimationStack = AnimationStack;
            this.AnimationLayer = AnimationLayer;
            AnimationNodes = new List<AnimationNode>();
        }
    }

    public class AnimationNode
    {
        public FBXRecordNode Node;
        public List<FBXRecordNode> AnimationCurves;

        public AnimationNode(FBXRecordNode AnimationNode)
        {
            AnimationNode = Node;
            AnimationCurves = new List<FBXRecordNode>();
        }
    }
}
