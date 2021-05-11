using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace FBXAnimationImporter
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
            
            foreach (var node in AllRecordNodes)
            {
                Console.Write("/\n/\n/\n"); //Parede de / / /
                Console.WriteLine(node);
            }
            Console.Read();
        }
    }
    
    
   
}
