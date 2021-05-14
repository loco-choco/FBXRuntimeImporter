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

            List<byte> fbxFile;
            try{
                fbxFile = new List<byte>(File.ReadAllBytes(filePath));
            }
            catch{
                Console.WriteLine("Nao deu para ler o arquivo");
                Console.Read();
                return;
            }

            uint fileVersion = BitConverter.ToUInt32(fbxFile.GetRange(23,26).ToArray(),0);
            Console.WriteLine("Versao do Arquivo: " + fileVersion);
            Console.WriteLine("Tamanho do Arquivo: " + fbxFile.Count);

            const ulong firstByteAfterTheHeader = 27; //O Header vai de 0 - 26, então começamos no 27

            List<FBXRecordNode> AllRecordNodes = new List<FBXRecordNode>() {new FBXRecordNode(ref fbxFile,(int)firstByteAfterTheHeader) };

            for(int i =0; i<10; i++)
                    AllRecordNodes.Add(new FBXRecordNode(ref fbxFile, (int)AllRecordNodes[AllRecordNodes.Count - 1].EndOffset));
               

            //AllRecordNodes.RemoveAt(AllRecordNodes.Count - 1);
            
            foreach(var node in AllRecordNodes) { 
            Console.Write("/\n/\n/\n"); //Parede de / / /
            Console.WriteLine(node);//8th node is the object node, where animations, geometry and materials are stored
            }
            //Position & Angles are stored in 3D
            List<FBXAnimationNodes> animations = new List<FBXAnimationNodes>();
            int animToGiveNodes = -1; //1- - none, 0 - animationOne, 1 - //Two
            int nodeToGiveCurve = -1;
            for (int i = 0; i < AllRecordNodes[8].NestedRecords.Count; i++)
            {
                if (AllRecordNodes[8].NestedRecords[i].Name == "AnimationStack" && AllRecordNodes[8].NestedRecords[i + 1].Name == "AnimationLayer")
                {
                    animToGiveNodes++;
                    nodeToGiveCurve = -1;
                    animations.Add(new FBXAnimationNodes(AllRecordNodes[8].NestedRecords[i], AllRecordNodes[8].NestedRecords[i + 1]));
                }
                else if (AllRecordNodes[8].NestedRecords[i].Name == "AnimationCurveNode")
                {
                    nodeToGiveCurve++;
                    animations[animToGiveNodes].AnimationNodes.Add(new AnimationNode(AllRecordNodes[8].NestedRecords[i]));
                }
                else if (animToGiveNodes > -1 && animToGiveNodes < animations.Count && AllRecordNodes[8].NestedRecords[i].Name == "AnimationCurve")
                    animations[animToGiveNodes].AnimationNodes[nodeToGiveCurve].AnimationCurves.Add(AllRecordNodes[8].NestedRecords[i]);
            }
            ////AnimationLayer and then AnimationStack indicates the start of a new animation, and AnimationNodes and AnimationCurves
            ////the curve inside the animation, where the curves of the bones are in order, with rotation first and then position curves		
            ////the AnimationNodes/AnimationCurves array ending means that there are no more curves for that animation

            Console.Read();
        }
    }
    
    
   
}
