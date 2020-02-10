using System;
using System.Collections.Generic;
using System.IO;
using OpenTK;

namespace ThreeD_ACW.Utility
{

    /// <summary>
    /// Model Utility reads a very simple, inefficient file format that uses 
    /// Triangles only. This is not good practice, but it does the job :)
    /// </summary>
    public class ModelUtility
    {
        public float[] Vertices { get; private set; }
        public int[] Indices { get; private set; }

        private ModelUtility() { }

        private static ModelUtility LoadFromBIN(string pModelFile)
        {
            ModelUtility model = new ModelUtility();
            BinaryReader reader = new BinaryReader(new FileStream(pModelFile, FileMode.Open));

            int numberOfVertices = reader.ReadInt32();
            int floatsPerVertex = 6;

            model.Vertices = new float[numberOfVertices * floatsPerVertex];

            byte[] byteArray = new byte[model.Vertices.Length * sizeof(float)];
            byteArray = reader.ReadBytes(byteArray.Length);

            Buffer.BlockCopy(byteArray, 0, model.Vertices, 0, byteArray.Length);

            int numberOfTriangles = reader.ReadInt32();

            model.Indices = new int[numberOfTriangles * 3];

            byteArray = new byte[model.Indices.Length * sizeof(int)];
            byteArray = reader.ReadBytes(model.Indices.Length * sizeof(int));
            Buffer.BlockCopy(byteArray, 0, model.Indices, 0, byteArray.Length);

            reader.Close();
            return model;
        }

        private static ModelUtility LoadFromSJG(string pModelFile)
        {
            ModelUtility model = new ModelUtility();
            StreamReader reader;
            reader = new StreamReader(pModelFile);
            string line = reader.ReadLine(); // vertex format
            int numberOfVertices = 0;
            int floatsPerVertex = 14;
            if (!int.TryParse(reader.ReadLine(), out numberOfVertices))
            {
                throw new Exception("Error when reading number of vertices in model file " + pModelFile);
            }

            model.Vertices = new float[numberOfVertices * floatsPerVertex];

            string[] values;
            for (int i = 0; i < model.Vertices.Length;) //Reads in the vertices
            {
                line = reader.ReadLine();
                values = line.Split(',');
                foreach (string s in values)
                {
                    if (!float.TryParse(s, out model.Vertices[i]))
                    {
                        throw new Exception("Error when reading vertices in model file " + pModelFile + " " + s + " is not a valid number");
                    }
                    ++i;
                }
            }

            reader.ReadLine();
            float numberOfTriangles = 0;
            line = reader.ReadLine();
            if (!float.TryParse(line, out numberOfTriangles))
            {
                throw new Exception("Error when reading number of triangles in model file " + pModelFile);
            }

            model.Indices = new int[(int)Math.Round(numberOfTriangles * 3)];

            for (int i = 0; i < numberOfTriangles * 3;) //Reads in the indices
            {
                line = reader.ReadLine();
                values = line.Split(',');
                foreach (string s in values)
                {
                    if (!int.TryParse(s, out model.Indices[i]))
                    {
                        throw new Exception("Error when reading indices in model file " + pModelFile + " " + s + " is not a valid index");
                    }
                    ++i;
                }
            }

            int[] timesCalled = new int[model.Vertices.Length];

            if (numberOfTriangles % 1 > 0) //Calculates the tangent and binormal for each of the points in the model
            {
                for (int i = 1; i < model.Indices.Length - 1; i++)
                {
                    ExtractData(ref model, model.Indices[0] * 14, model.Indices[i] * 14, model.Indices[i + 1] * 14, ref timesCalled);
                }
            }
            else //Allows the creation of triangle fan
            {
                for (int i = 0; i < model.Indices.Length; i = i + 3)
                {
                    ExtractData(ref model, model.Indices[i] * 14, model.Indices[i + 1] * 14, model.Indices[i + 2] * 14, ref timesCalled);
                }
            }

            reader.Close();
            return model;
        }

        private static ModelUtility LoadFromOBJ(string pModelFile)
        {
            string sjgFile = pModelFile.Replace(".obj", ".sjg"); //Checks if a .sjg of the model exists, if so use that one (makes the program quicker to load).
            if (File.Exists(sjgFile))
            {
                return LoadFromSJG(sjgFile);
            }

            ModelUtility model = new ModelUtility();
            StreamReader reader;
            reader = new StreamReader(pModelFile);
            List<string> lines = new List<string>();

            while (!reader.EndOfStream) //Read in the entire file
            {
                lines.Add(reader.ReadLine());
            }

            reader.Close();

            List<string> vertex = new List<string>();
            List<string> texCoord = new List<string>();
            List<string> normal = new List<string>();
            List<string> indices = new List<string>();

            for (int i = 0; i < lines.Count; i++) //Check what each line starts with and sorts into the correct type
            {
                if (lines[i].StartsWith("v "))
                {
                    vertex.Add(lines[i].Replace("v ", "").Trim());
                }
                else if (lines[i].StartsWith("vn "))
                {
                    normal.Add(lines[i].Replace("vn ", "").Trim());
                }
                else if (lines[i].StartsWith("vt "))
                {
                    texCoord.Add(lines[i].Replace("vt ", "").Trim());
                }
                else if (lines[i].StartsWith("f "))
                {
                    indices.AddRange(lines[i].Replace("f ", "").Trim().Split(' '));
                }
            }

            List<string> usedVert = new List<string>();

            for (int i = 0; i < indices.Count; i++) //Optimises the indices in the .obj, so that any repeated sets of indices only use one set of vertices.
            {
                bool used = false;

                for (int j = 0; j < usedVert.Count; j++)
                {
                    if(indices[i] == usedVert[j])
                    {
                        used = true;
                        indices[i] = j.ToString();
                        break;
                    }
                }

                if(!used)
                {
                    usedVert.Add(indices[i]);
                }
            }

            model.Vertices = new float[usedVert.Count * 14];
            model.Indices = new int[indices.Count];

            int[] timesCalled = new int[usedVert.Count];

            int vert = 0;
            int indi = 0;

            for (int i = 0; i < indices.Count; i++) //Places the vertices and indices into the model
            {
                string[] coords = indices[i].Split('/');

                if(coords.Length == 1)
                {
                    model.Indices[i] = int.Parse(indices[i]);
                }
                else
                { 
                    foreach (string v in vertex[int.Parse(coords[0]) - 1].Split(' ')) 
                    {
                        model.Vertices[vert] = float.Parse(v);
                        vert++;
                    }
                    foreach (string v in texCoord[int.Parse(coords[1]) - 1].Split(' '))
                    {
                        model.Vertices[vert] = float.Parse(v);
                        vert++;
                    }
                    foreach (string v in normal[int.Parse(coords[2]) - 1].Split(' '))
                    {
                        model.Vertices[vert] = float.Parse(v);
                        vert++;
                    }

                    model.Indices[i] = indi;
                    indi++;
                    vert += 6;
                }
            }

            StreamWriter writer = new StreamWriter(sjgFile); //Creates a sjg so that the next time it is loaded it is quicker.

            writer.WriteLine("Vertex Format x,y,z,r,g,b");
            writer.WriteLine((model.Vertices.Length / 14).ToString());

            for(int i = 0; i < model.Vertices.Length; i = i + 14)
            {
                string line = model.Vertices[i] + ","; //vertices
                line += model.Vertices[i + 1] + ",";
                line += model.Vertices[i + 2] + ",";
                line += model.Vertices[i + 3] + ","; // Texture Coordinates
                line += model.Vertices[i + 4] + ",";
                line += model.Vertices[i + 5] + ","; // Normals
                line += model.Vertices[i + 6] + ",";
                line += model.Vertices[i + 7] + ",0,0,0,0,0,0";
                writer.WriteLine(line);
            }

            writer.WriteLine("Index Format v1,v2,v3");
            writer.WriteLine((model.Indices.Length / 3).ToString());

            for(int i = 0; i < model.Indices.Length; i = i + 3)
            {
                string line = model.Indices[i] + ",";
                line += model.Indices[i + 1] + ",";
                line += model.Indices[i + 2];
                writer.WriteLine(line);
            }

            writer.Close();

            for (int i = 0; i < model.Indices.Length; i = i + 3) //Calculates the tangent and binormal for each of the points in the model
            {
                ExtractData(ref model, model.Indices[i] * 14, model.Indices[i + 1] * 14, model.Indices[i + 2] * 14, ref timesCalled);
            }

            return model;
        }

        //The following uses the method discribed at http://www.terathon.com/code/tangent.html

        private static void ExtractData(ref ModelUtility pModel, int pArrayPos1, int pArrayPos2, int pArrayPos3, ref int[] pTimesCalled)
        {
            Vector3 pos1 = new Vector3(pModel.Vertices[pArrayPos1 + 0], pModel.Vertices[pArrayPos1 + 1], pModel.Vertices[pArrayPos1 + 2]); //Gets the position and the texture coordinates for the three points
            Vector2 tex1 = new Vector2(pModel.Vertices[pArrayPos1 + 3], pModel.Vertices[pArrayPos1 + 4]);
            Vector3 pos2 = new Vector3(pModel.Vertices[pArrayPos2 + 0], pModel.Vertices[pArrayPos2 + 1], pModel.Vertices[pArrayPos2 + 2]);
            Vector2 tex2 = new Vector2(pModel.Vertices[pArrayPos2 + 3], pModel.Vertices[pArrayPos2 + 4]);
            Vector3 pos3 = new Vector3(pModel.Vertices[pArrayPos3 + 0], pModel.Vertices[pArrayPos3 + 1], pModel.Vertices[pArrayPos3 + 2]);
            Vector2 tex3 = new Vector2(pModel.Vertices[pArrayPos3 + 3], pModel.Vertices[pArrayPos3 + 4]);

            CreateTangentBinormal(ref pModel, pos1, pos2, pos3, tex1, tex2, tex3, pArrayPos1, ref pTimesCalled); //Makes the tangent and binormal for each point.
            CreateTangentBinormal(ref pModel, pos2, pos1, pos3, tex2, tex1, tex3, pArrayPos2, ref pTimesCalled);
            CreateTangentBinormal(ref pModel, pos3, pos2, pos1, tex3, tex2, tex1, pArrayPos3, ref pTimesCalled);
        }

        private static void CreateTangentBinormal(ref ModelUtility pModel, Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector2 tex1, Vector2 tex2, Vector2 tex3, int pos, ref int[] pTimesCalled)
        {
            Vector3 posDif1 = pos2 - pos1;
            Vector3 posDif2 = pos3 - pos1;

            Vector2 texDif1 = new Vector2(tex2.X - tex1.X, tex2.Y - tex1.Y);
            Vector2 texDif2 = new Vector2(tex3.X - tex1.X, tex3.Y - tex1.Y);

            Matrix2 inverseTex = new Matrix2(texDif2.Y, -texDif1.Y, -texDif2.X, texDif1.X); //Makes the tangent point in the positive texture x direction, and the binormal point in the positive texture y direction.
            Matrix2x3 positions = new Matrix2x3(posDif1, posDif2);
            float tex = 1 / ((texDif1.X * texDif2.Y) - (texDif2.X * texDif1.Y));

            Matrix2x3 tangentBinormal = tex * inverseTex * positions;

            pModel.Vertices[pos + 8] = ((pModel.Vertices[pos + 8] * pTimesCalled[pos / 14]) + tangentBinormal.M11) / (pTimesCalled[pos / 14] + 1); //Averages the tangent and binormal so far
            pModel.Vertices[pos + 9] = ((pModel.Vertices[pos + 9] * pTimesCalled[pos / 14]) + tangentBinormal.M12) / (pTimesCalled[pos / 14] + 1);
            pModel.Vertices[pos + 10] = ((pModel.Vertices[pos + 10] * pTimesCalled[pos / 14]) + tangentBinormal.M13) / (pTimesCalled[pos / 14] + 1);

            pModel.Vertices[pos + 11] = ((pModel.Vertices[pos + 11] * pTimesCalled[pos / 14]) + tangentBinormal.M21) / (pTimesCalled[pos / 14] + 1);
            pModel.Vertices[pos + 12] = ((pModel.Vertices[pos + 12] * pTimesCalled[pos / 14]) + tangentBinormal.M22) / (pTimesCalled[pos / 14] + 1);
            pModel.Vertices[pos + 13] = ((pModel.Vertices[pos + 13] * pTimesCalled[pos / 14]) + tangentBinormal.M23) / (pTimesCalled[pos / 14] + 1);

            pTimesCalled[pos / 14]++;
        }

        public static ModelUtility LoadModel(string pModelFile)
        {
            string extension = pModelFile.Substring(pModelFile.IndexOf('.'));

            if (extension == ".sjg")
            {
                return LoadFromSJG(pModelFile);
            }
            else if (extension == ".bin")
            {
                return LoadFromBIN(pModelFile);
            }
            else if (extension == ".obj")
            {
                return LoadFromOBJ(pModelFile);
            }
            else
            {
                throw new Exception("Unknown file extension " + extension);
            }
        }

    }
}
