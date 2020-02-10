using System;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace ThreeD_ACW.Utility
{
    public class ShaderUtility
    {
        public int ShaderProgramID { get; private set; }
        public int VertexShaderID { get; private set; }
        public int FragmentShaderID { get; private set; }

        public ShaderUtility(string pVertexShaderFile, string pFragmentShaderFile)
        {
            StreamReader reader;
            VertexShaderID = GL.CreateShader(ShaderType.VertexShader); //Creates a new vertex shader
            reader = new StreamReader(pVertexShaderFile);
            GL.ShaderSource(VertexShaderID, reader.ReadToEnd());
            reader.Close();
            GL.CompileShader(VertexShaderID); //Compiles the loaded in vertex shader

            int result;
            GL.GetShader(VertexShaderID, ShaderParameter.CompileStatus, out result);
            if (result == 0)
            {
                throw new Exception("Failed to compile vertex shader!" + GL.GetShaderInfoLog(VertexShaderID));
            }

            FragmentShaderID = GL.CreateShader(ShaderType.FragmentShader); //Creates a new fragment shader
            reader = new StreamReader(pFragmentShaderFile);
            GL.ShaderSource(FragmentShaderID, reader.ReadToEnd());
            reader.Close();
            GL.CompileShader(FragmentShaderID); //Compiles the loaded in fragment shader

            GL.GetShader(FragmentShaderID, ShaderParameter.CompileStatus, out result);
            if (result == 0)
            {
                throw new Exception("Failed to compile fragment shader!" + GL.GetShaderInfoLog(FragmentShaderID));
            }

            ShaderProgramID = GL.CreateProgram();
            GL.AttachShader(ShaderProgramID, VertexShaderID);
            GL.AttachShader(ShaderProgramID, FragmentShaderID);
            GL.LinkProgram(ShaderProgramID);
        }

        public void Delete()
        {
            GL.DetachShader(ShaderProgramID, VertexShaderID); //Deletes the created shaders
            GL.DetachShader(ShaderProgramID, FragmentShaderID);
            GL.DeleteShader(VertexShaderID);
            GL.DeleteShader(FragmentShaderID);
            GL.DeleteProgram(ShaderProgramID);
        }
    }
}
