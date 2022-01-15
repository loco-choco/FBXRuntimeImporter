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
                Console.Write("Absolute file path (.fbx): ");
                filePath = Console.ReadLine();
            }

            FBXFileParser fbxFile = new FBXFileParser(filePath);

            Console.WriteLine("Versao do Arquivo: " + fbxFile.FileVersion);
            //Console.WriteLine(fbxFile.AllNodesToString());
            FBXAnimation[] animations = fbxFile.ReadAnimations();
            Console.WriteLine(animations.Length);
            for(int i = 0; i < animations.Length; i++)
            {
                Console.WriteLine("Anim " + i);
                foreach (var anim in animations[i].BonesAnimation) 
                {
                    Console.WriteLine(anim.BoneName);
                    Console.WriteLine(anim.PositionCurves.Length);
                }
            }
            Console.WriteLine("Arquivo Interpretado :)");
            Console.Read();
        }
    }
    
    
   
}
