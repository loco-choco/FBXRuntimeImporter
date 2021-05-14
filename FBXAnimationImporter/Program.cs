using System;
using System.IO;
using System.Collections.Generic;
using FBXRuntimeImporter.AnimationRead;
using System.Text;

namespace FBXRuntimeImporter
{
    //https://code.blender.org/2013/08/fbx-binary-file-format-specification/ reference to this craziness
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = "";

            while (!filePath.EndsWith(".fbx"))
            {
                Console.Write("Caminho do arquivo (.fbx): ");
                filePath = Console.ReadLine();
            }

            FBXFileParser fbxFile = new FBXFileParser(filePath);

            Console.WriteLine("Versao do Arquivo: " + fbxFile.FileVersion);
            Console.WriteLine(fbxFile. AllNodesToString());

            List<FBXAnimationNodes> animationsInNodes = new List<FBXAnimationNodes>();

            int animToGiveNodes = -1; //1- - none, 0 - animationOne, 1 - //Two
            int nodeToGiveCurve = -1;
            for (int i = 0; i < fbxFile.Objects.NestedRecords.Count; i++)
            {
                if (fbxFile.Objects.NestedRecords[i].Name == "AnimationStack" && fbxFile.Objects.NestedRecords[i + 1].Name == "AnimationLayer")
                {
                    animToGiveNodes++;
                    nodeToGiveCurve = -1;
                    animationsInNodes.Add(new FBXAnimationNodes(fbxFile.Objects.NestedRecords[i], fbxFile.Objects.NestedRecords[i + 1]));
                }
                else if (fbxFile.Objects.NestedRecords[i].Name == "AnimationCurveNode")
                {
                    nodeToGiveCurve++;
                    animationsInNodes[animToGiveNodes].AnimationNodes.Add(new FBXAnimationNode(fbxFile.Objects.NestedRecords[i]));
                }
                else if (animToGiveNodes > -1 && animToGiveNodes < animationsInNodes.Count && fbxFile.Objects.NestedRecords[i].Name == "AnimationCurve")
                    animationsInNodes[animToGiveNodes].AnimationNodes[nodeToGiveCurve].AnimationCurves.Add(fbxFile.Objects.NestedRecords[i]);
            }
            List<FBXAnimation> animations = new List<FBXAnimation>();
            foreach (var nodes in animationsInNodes)
                animations.Add(new FBXAnimation(nodes));
            ////AnimationLayer and then AnimationStack indicates the start of a new animation, and AnimationNodes and AnimationCurves
            ////the curve inside the animation, where the curves of the bones are in order, with rotation first and then position curves		
            ////the AnimationNodes/AnimationCurves array ending means that there are no more curves for that animation

            Console.Read();
        }
    }
    
    
   
}
