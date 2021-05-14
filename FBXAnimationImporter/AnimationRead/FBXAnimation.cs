using System;
using System.Collections.Generic;
using System.Text;

namespace FBXRuntimeImporter.AnimationRead
{
    public class FBXAnimation
    {
        FBXBoneAnimation[] BonesAnimation;
        FBXAnimationNode FocalLenghAnimation;
        public FBXAnimation(FBXAnimationNodes animationNodes)
        {
            BonesAnimation = new FBXBoneAnimation[(animationNodes.AnimationNodes.Count-1)/3];
            for (int i = 0; i < BonesAnimation.Length; i++)
                BonesAnimation[i] = new FBXBoneAnimation(animationNodes.AnimationNodes[i * 3], animationNodes.AnimationNodes[i * 3 + 1]
                    , animationNodes.AnimationNodes[i * 3 + 2]);

            FocalLenghAnimation = animationNodes.AnimationNodes[animationNodes.AnimationNodes.Count - 1];
        }
    }
    public class FBXBoneAnimation
    {
        FBXAnimationCurve[] PositionCurves = new FBXAnimationCurve[3];
        FBXAnimationCurve[] RotationCurves = new FBXAnimationCurve[3]; //In euler angles :/
        FBXAnimationCurve[] ScaleCurves = new FBXAnimationCurve[3];


        /// <summary>
        /// 
        /// </summary>
        /// <param name="PositionAnimationCurveNode">The AnimationCurveNode node not the AnimationCurve node</param>
        /// <param name="RotationAnimationCurveNode">The AnimationCurveNode node not the AnimationCurve node</param>
        /// <param name="ScaleAnimationCurveNode">The AnimationCurveNode node not the AnimationCurve node</param>
        public FBXBoneAnimation(FBXAnimationNode PositionAnimationCurveNode, FBXAnimationNode RotationAnimationCurveNode, FBXAnimationNode ScaleAnimationCurveNode)
        {
            for (int i = 0; i < 3; i++)
                PositionCurves[i] = new FBXAnimationCurve(PositionAnimationCurveNode.AnimationCurves[i]);

            for (int i = 0; i < 3; i++)
                RotationCurves[i] = new FBXAnimationCurve(RotationAnimationCurveNode.AnimationCurves[i]);

            for (int i = 0; i < 3; i++)
                ScaleCurves[i] = new FBXAnimationCurve(ScaleAnimationCurveNode.AnimationCurves[i]);
        }
    }
}
