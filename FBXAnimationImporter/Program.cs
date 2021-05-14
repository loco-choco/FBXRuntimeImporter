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

            FBXFileParser fileParser = new FBXFileParser(filePath);

            Console.WriteLine("Versao do Arquivo: " + fileParser.FileVersion);
            Console.WriteLine(fileParser. AllNodesToString());
            ////AnimationLayer and then AnimationStack indicates the start of a new animation, and AnimationNodes and AnimationCurves
            ////the curve inside the animation, where the curves of the bones are in order, with rotation first and then position curves		
            ////the AnimationNodes/AnimationCurves array ending means that there are no more curves for that animation

            Console.Read();
        }
    }
    
    
   
}
